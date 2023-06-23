using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Models
{
    public class VariationViewModel
    {
        public int Id { get; set; }

        public virtual string Words { get; set; }

        public VocabularyType? Type { get; set; }
    }
}
