using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnVocabulary.Data;
using LearnVocabulary.Models;

namespace LearnVocabulary.Controllers
{
    public class VocabularyController : Controller
    {
        private readonly VocabularyContext _context;

        public VocabularyController(VocabularyContext context)
        {
            _context = context;
        }

        // GET: Vocabulary
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["SearchString"] = searchString;

            // Append search condition to the query
            var query = _context.Vocabulary.Select(v => v);
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(v => v.Words.ToLower().Contains(searchString.ToLower()));
            }

            // Get all vocabulary including their variations
            var vocabularies = await query.Include(v => v.Definition)
                .Include(v => v.VocabularyUsages)
                .ThenInclude(vu => vu.Usage)
                .AsNoTracking()
                .ToListAsync();

            // Convert to a list of ViewModel for the view
            var model = vocabularies.Where(voca => voca.VocabularyUsages.Count > 0)
                .Select(vocabulary => new VocabularyViewModel()
                {
                    Id = vocabulary.Id,
                    Words = vocabulary.Words,
                    Type = vocabulary.Type,
                    Definition = vocabulary.Definition,
                    Examples = vocabulary.VocabularyUsages.Select(vu => vu.Usage).ToList(),
                    Variations = vocabularies.Where(v => v.DefinitionId == vocabulary.DefinitionId && v.VocabularyUsages.Count == 0)
                                             .Select(v => new VariationViewModel()
                                             {
                                                 Words = v.Words,
                                                 Type = v.Type
                                             }).ToList()
                });

            return View(model);
        }

        // GET: Vocabulary/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await GetVocabularyById(id.Value);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Vocabulary/Create
        public IActionResult Create()
        {
            var viewModel = new VocabularyViewModel()
            {
                Variations = new List<VariationViewModel>() { new VariationViewModel() },
                Examples = new List<Usage>() { new Usage() }
            };
            return View(viewModel);
        }

        // POST: Vocabulary/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VocabularyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var vocabulary = new Vocabulary()
                {
                    Words = viewModel.Words,
                    Type = viewModel.Type,
                    Definition = viewModel.Definition,
                    VocabularyUsages = viewModel.Examples.Select(e => new VocabularyUsage()
                    {
                        Usage = e
                    }).ToList()
                };
                await _context.AddAsync(vocabulary);

                var variations = viewModel.Variations.Where(v => !string.IsNullOrWhiteSpace(v.Words))
                    .Select(v => new Vocabulary()
                    {
                        Words = v.Words,
                        Type = v.Type,
                        Definition = vocabulary.Definition
                    });
                await _context.AddRangeAsync(variations);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Vocabulary/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await GetVocabularyById(id.Value);

            if (model == null)
            {
                return NotFound();
            }

            if (model.Variations.Count == 0)
            {
                model.Variations = new List<VariationViewModel>() { new VariationViewModel() };
            }

            if (model.Examples.Count == 0)
            {
                model.Examples = new List<Usage>() { new Usage() };
            }

            return View(model);
        }

        // POST: Vocabulary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VocabularyViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var vocabulary = await _context.Vocabulary
                            .Include(v => v.Definition)
                            .Include(v => v.VocabularyUsages)
                            .ThenInclude(vu => vu.Usage)
                            .SingleOrDefaultAsync(v => v.Id == id);
                    if (vocabulary == null)
                    {
                        return NotFound();
                    }

                    // Update vocabulary's information
                    vocabulary.Type = viewModel.Type;
                    vocabulary.Words = viewModel.Words;
                    vocabulary.Definition = viewModel.Definition;

                    // Delete or update examples of the vocabulary
                    foreach (var vu in vocabulary.VocabularyUsages)
                    {
                        var usage = viewModel.Examples.Find(u => u.Id == vu.UsageId);
                        if (usage == null)
                        {
                            _context.Entry(vu).State = EntityState.Deleted;
                        }
                        else
                        {
                            vu.Usage.Example = usage.Example;
                        }
                    }

                    // Add new examples for the vocabulary
                    foreach (var e in viewModel.Examples)
                    {
                        if (!vocabulary.VocabularyUsages.Any(vu => vu.UsageId == e.Id))
                        {
                            vocabulary.VocabularyUsages.Add(new VocabularyUsage()
                            {
                                Usage = new Usage()
                                {
                                    Example = e.Example
                                }
                            });
                        }
                    }

                    var variations = await _context.Vocabulary
                        .Where(v1 => v1.DefinitionId == vocabulary.DefinitionId && v1.Id != viewModel.Id)
                        .ToListAsync();

                    // Update or delete variations
                    foreach (var variation in variations)
                    {
                        var inputVar = viewModel.Variations.Find(v => v.Id == variation.Id);
                        if (inputVar == null)
                        {
                            _context.Entry(variation).State = EntityState.Deleted;
                        }
                        else
                        {
                            variation.Words = inputVar.Words;
                            variation.Type = inputVar.Type;
                        }
                    }

                    // Add new variations
                    foreach (var inputVar in viewModel.Variations)
                    {
                        if (inputVar.Id == default && !string.IsNullOrWhiteSpace(inputVar.Words))
                        {
                            _context.Vocabulary.Add(new Vocabulary()
                            {
                                Words = inputVar.Words,
                                Type = inputVar.Type,
                                DefinitionId = vocabulary.DefinitionId
                            });

                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VocabularyExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details));
            }

            return View(viewModel);
        }

        // GET: Vocabulary/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vocabulary = await GetVocabularyById(id.Value);

            if (vocabulary == null)
            {
                return NotFound();
            }

            return View(vocabulary);
        }

        // POST: Vocabulary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vocabulary = await _context.Vocabulary
                .Include(v => v.Definition)
                .Include(v => v.VocabularyUsages)
                .ThenInclude(vu => vu.Usage)
                .SingleOrDefaultAsync(v => v.Id == id);
            if (vocabulary == null)
            {
                return NotFound();
            }

            _context.Vocabulary.Remove(vocabulary);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Random()
        {
            return View(await GetRandomVocabulary());
        }

        public async Task<IActionResult> RecentVocabulary(int? id)
        {
            var recentWords = await _context.Vocabulary
                .OrderByDescending(v => v.UpdateDate)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            int selectedId;
            if (id != null)
            {
                selectedId = id.Value;
            }
            else
            {
                selectedId = recentWords.First().Id;
            }

            var model = new RecentVocabularyViewModel()
            {
                RecentVocabulary = recentWords,
                SelectedVocabulary = await GetVocabularyById(selectedId)
            };

            return View(model);
        }

        private async Task<VocabularyViewModel> GetVocabularyById(int id)
        {
            var vocabulary = await _context.Vocabulary
                .Include(v => v.Definition)
                .Include(v => v.VocabularyUsages)
                .ThenInclude(vu => vu.Usage)
                .AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == id);


            return await MakeViewModel(vocabulary);
        }

        private async Task<VocabularyViewModel> GetRandomVocabulary()
        {
            var count = _context.Vocabulary.Count();
            if (count == 0)
            {
                ModelState.AddModelError(string.Empty, "There is no vocabulary yet");
                return null;
            }

            Random random = new Random();
            int pos = random.Next(count);

            var vocabulary = await _context.Vocabulary
                .Include(v => v.Definition)
                .Include(v => v.VocabularyUsages)
                .ThenInclude(vu => vu.Usage)
                .AsNoTracking()
                .Skip(pos)
                .Take(1)
                .SingleAsync();
            return await MakeViewModel(vocabulary);
        }

        private async Task<VocabularyViewModel> MakeViewModel(Vocabulary vocabulary)
        {
            if (vocabulary == null)
            {
                return null;
            }

            var variations = await _context.Vocabulary
                .Where(v => v.DefinitionId == vocabulary.DefinitionId && v.Id != vocabulary.Id)
                .ToListAsync();

            var model = new VocabularyViewModel()
            {
                Id = vocabulary.Id,
                Type = vocabulary.Type,
                Words = vocabulary.Words,
                Definition = vocabulary.Definition,
                Variations = variations.Select(v => new VariationViewModel()
                {
                    Id = v.Id,
                    Type = v.Type,
                    Words = v.Words
                }).ToList(),
                Examples = vocabulary.VocabularyUsages.Select(vu => new Usage()
                {
                    Id = vu.Usage.Id,
                    Example = vu.Usage.Example
                }).ToList()
            };

            return model;
        }


        private bool VocabularyExists(int id)
        {
            return _context.Vocabulary.Any(e => e.Id == id);
        }
    }
}
