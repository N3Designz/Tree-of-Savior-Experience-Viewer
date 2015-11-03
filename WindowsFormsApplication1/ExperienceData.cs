using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeOfSaviorExperienceViewer
{
    public class ExperienceData
    {
        public int currentBaseExperience { get; set; }
        public int requiredBaseExperience { get; set; }
        public int previousBaseExperience { get; set; }
        public float baseKillsTilNextLevel { get; set; }
        public int lastKillExperience { get; set; }
    }
}
