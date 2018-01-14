using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindInterfaces
{
    public class Game
    {
        public int Id { get; set; }
        public int ColorSequenceCount { get; set; }
        public int RemainingGuesses { get; set; }
        public List<string> AvailableColors { get; set; }
        public bool AllowDuplicates { get; set; }

        public bool IsSolved { get; set; }
        public List<string> ColorSequence { get; set; }
    }
}
