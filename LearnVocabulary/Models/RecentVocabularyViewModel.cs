using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Models
{
    public class RecentVocabularyViewModel
    {
        public List<Vocabulary> RecentVocabulary { get; set; }

        public VocabularyViewModel SelectedVocabulary { get; set; }
    }

}
