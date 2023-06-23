using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Models
{
    public class VocabularyUsage
    {
        public int VocabularyId { get; set; }

        public int UsageId { get; set; }

        public Vocabulary Vocabulary { get; set; }

        public Usage Usage { get; set; }
    }
}
