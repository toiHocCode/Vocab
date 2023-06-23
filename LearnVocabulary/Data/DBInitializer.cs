using LearnVocabulary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Data
{
    public static class DBInitializer
    {

        private static void Register(VocabularyContext context, Vocabulary vocabulary, Definition definition, params Usage[] usages)
        {
            definition.Id = default;
            context.Definitions.Add(definition);
            context.SaveChanges();

            vocabulary.Id = default;
            vocabulary.DefinitionId = definition.Id;
            context.Vocabulary.Add(vocabulary);
            context.SaveChanges();

            foreach (var usage in usages)
            {
                usage.Id = default;
            }
            context.Usages.AddRange(usages);
            context.SaveChanges();

            foreach (var usage in usages)
            {
                context.VocabularyUsages.Add(new VocabularyUsage()
                {
                    VocabularyId = vocabulary.Id,
                    UsageId = usage.Id
                });
            }
            context.SaveChanges();

        }

        public static void Initialize(VocabularyContext context)
        {
            return; // TODO

            if (context.Vocabulary.Any())
            {
                return;
            }

            Register(
                context,
                new Vocabulary()
                {
                    Words = "Ghost",
                    Type = VocabularyType.Slang,
                },
                new Definition()
                {
                    ContentEn = "If you ghost someone, it means you suddenly stop all communication with them, with no explanation.",
                    ContentVi = "Không liên lạc và không thể liên lạc mà không biết lý do"
                }, new Models.Usage()
                {
                    Example = "Katia felt awful for weeks after being ghosted by a guy she really liked."
                }, new Models.Usage()
                {
                    Example = "Keiko wanted to break up with her boyfriend but didn’t want to upset him. So when he suddenly started ghosting her, she actually felt relieved."
                },
                new Models.Usage()
                {
                    Example = "Jorge and Rafa used to hang out all the time at school. But once they started university, Jorge just ghosted Rafa."
                });

            Register(context,
                new Vocabulary()
                {
                    Words = "Slow TV",
                    Type = Models.VocabularyType.Slang,
                },
                new Definition()
                {
                    ContentEn = "Actually, believe it or not, the entire film was just the view from the front of a train, all the way from one city to another. It was seven hours long.",
                    ContentVi = "Xem ti vi thời gian thực, rất lâu để xem hết 1 chương trình"
                },
                new Usage()
                {
                    Example = "Michala’s company specialises in making slow TV. She says business is great right now!"
                },
                new Usage()
                {
                    Example = "I recently discovered slow TV. My friends think it’s really dull, but I find it helps clear my head after a stressful day."
                },
                new Usage()
                {
                    Example = @"A: What are you watching? It looks just like a fireplace with a big piece of wood burning.
B: It is !Isn’t slow TV fantastic ? It takes about four hours for the wood to burn completely."
                });

        }
    }
}
