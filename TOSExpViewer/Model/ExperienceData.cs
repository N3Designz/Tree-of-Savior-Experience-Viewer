using System;
using Caliburn.Micro;

namespace TOSExpViewer.Model
{
    public class ExperienceData : PropertyChangedBase
    {
        private int currentBaseExperience;
        private int requiredBaseExperience;
        private int lastExperienceGain;
        private int previousRequiredBaseExperience;

        public int CurrentBaseExperience
        {
            get { return currentBaseExperience; }
            set
            {
                if (value == currentBaseExperience) return;
                currentBaseExperience = value;
                NotifyOfPropertyChange(() => CurrentBaseExperience);
                NotifyOfPropertyChange(() => CurrentBaseExperiencePercent);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
            }
        }

        public float CurrentBaseExperiencePercent => (currentBaseExperience / (float)requiredBaseExperience) * 100;

        public int RequiredBaseExperience
        {
            get { return requiredBaseExperience; }
            set
            {
                if (value == requiredBaseExperience) return;
                requiredBaseExperience = value;
                NotifyOfPropertyChange(() => RequiredBaseExperience);
                NotifyOfPropertyChange(() => CurrentBaseExperiencePercent);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
            }
        }
        
        public double KillsTilNextLevel => Math.Ceiling((RequiredBaseExperience - CurrentBaseExperience) / (double)LastExperienceGain);

        /// <summary>  Experience gain is not differentiated between cards and monsters  </summary>
        public int LastExperienceGain
        {
            get { return lastExperienceGain; }
            set
            {
                if (value == lastExperienceGain) return;
                lastExperienceGain = value;
                NotifyOfPropertyChange(() => LastExperienceGain);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
            }
        }

        public int PreviousRequiredBaseExperience
        {
            get { return previousRequiredBaseExperience; }
            set
            {
                if (value == previousRequiredBaseExperience) return;
                previousRequiredBaseExperience = value;
                NotifyOfPropertyChange(() => PreviousRequiredBaseExperience);
            }
        }

        public void Reset()
        {
            CurrentBaseExperience = 0;
            RequiredBaseExperience = 0;
            LastExperienceGain = 0;
            PreviousRequiredBaseExperience = 0;
        }
    }
}