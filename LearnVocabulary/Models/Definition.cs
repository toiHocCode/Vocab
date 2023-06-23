using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Models
{
    public class Definition
    {
        public int Id { get; set; }

        [DisplayName("Definition")]
        public string ContentEn { get; set; }

        [DisplayName("Definition in Vietnamese")]
        public string ContentVi { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
