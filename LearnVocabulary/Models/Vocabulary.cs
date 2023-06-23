using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Models
{
    public enum VocabularyType
    {
        Verb,
        Adjective,
        Noun,
        Adverb,
        Article,
        Phase,
        Idiom,
        Slang,
        [Display(Name = "Phrasal Verb")]
        PhrasalVerb
    }

    public class Vocabulary
    {
        public int Id { get; set; }

        [Required()]
        public string Words { get; set; }

        [DisplayName("Vocabulary Type")]
        public VocabularyType? Type { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int DefinitionId { get; set; }

        public Definition Definition { get; set; }

        public ICollection<VocabularyUsage> VocabularyUsages { get; set; }
    }
}
