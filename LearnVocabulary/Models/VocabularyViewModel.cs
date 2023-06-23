using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Models
{
    public class VocabularyViewModel : VariationViewModel
    {
        [Required]
        public override string Words { get => base.Words; set => base.Words = value; }

        //[Display(Name = "Definition")]
        //public string ContentEn { get; set; }

        //[Display(Name = "Definition in Vietnamese")]
        //public string ContentVi { get; set; }

        public Definition Definition { get; set; }

        public List<Usage> Examples { get; set; }

        public List<VariationViewModel> Variations { get; set; }

    }
}
