using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOSExpViewer.Model
{
    public class ExperienceCardData
    {
        public virtual int PotentialLevel { get; set; }
        public virtual long RemainingExperience { get; set; }
        public virtual long CurrentExperience { get; set; }
        public virtual long RequiredExperience { get; set; }
        public virtual float CurrentPercent { get; set; }
    }
}
