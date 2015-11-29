using System;
using System.Collections.Generic;
using Caliburn.Micro;
using TOSExpViewer.Core;

namespace TOSExpViewer.Model
{
    public class ExperienceContainer : PropertyChangedBase
    {
        public ExperienceContainer(ExperienceData baseExperienceData, ExperienceData classExperienceData, IEnumerable<IExperienceControl> experienceControls)
        {
            if (baseExperienceData == null)
            {
                throw new ArgumentNullException(nameof(baseExperienceData));
            }
            if (classExperienceData == null)
            {
                throw new ArgumentNullException(nameof(classExperienceData));
            }
            if (experienceControls == null)
            {
                throw new ArgumentNullException(nameof(experienceControls));
            }

            BaseExperienceData = baseExperienceData;
            ClassExperienceData = classExperienceData;
            ExperienceControls = new BindableCollection<IExperienceControl>(experienceControls);
        }

        public ExperienceData BaseExperienceData { get; private set; }

        public ExperienceData ClassExperienceData { get; private set; }

        public BindableCollection<IExperienceControl> ExperienceControls { get; private set; }

        public void RestartSession()
        {
            RestartSession(BaseExperienceData);
            RestartSession(ClassExperienceData);
        }

        private void RestartSession(ExperienceData experienceData)
        {
            experienceData.GainedExperience = 0;
            experienceData.StartTime = DateTime.Now;
            experienceData.TimeToLevel = Constants.INFINITY;
            NotifyOfPropertyChange(() => experienceData.ExperiencePerHour);
        }
    }
}
