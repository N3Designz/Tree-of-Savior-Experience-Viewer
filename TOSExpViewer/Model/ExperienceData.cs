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
        private int gainedBaseExperience;
        private int experiencePerHour;
        private string timeToLevel;

        public int CurrentBaseExperience
        {
            get { return currentBaseExperience; }
            set
            {
                if (value == currentBaseExperience)
                {
                    return;
                }

                currentBaseExperience = value;
                NotifyOfPropertyChange(() => CurrentBaseExperience);
                NotifyOfPropertyChange(() => CurrentBaseExperiencePercent);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
            }
        }

        public int GainedBaseExperience
        {
            get { return gainedBaseExperience; }
            set
            {
                if (value == gainedBaseExperience)
                {
                    return;
                }

                gainedBaseExperience = value;
                NotifyOfPropertyChange(() => GainedBaseExperience);
            }
        }

        public float CurrentBaseExperiencePercent => (currentBaseExperience / (float)requiredBaseExperience) * 100;

        public int RequiredBaseExperience
        {
            get { return requiredBaseExperience; }
            set
            {
                if (value == requiredBaseExperience)
                {
                    return;
                }

                requiredBaseExperience = value;
                NotifyOfPropertyChange(() => RequiredBaseExperience);
                NotifyOfPropertyChange(() => CurrentBaseExperiencePercent);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
                NotifyOfPropertyChange(() => LastExperienceGainPercent);
            }
        }

        public double KillsTilNextLevel => Math.Ceiling((RequiredBaseExperience - CurrentBaseExperience) / (double)LastExperienceGain);

        /// <summary>  Experience gain is not differentiated between cards and monsters  </summary>
        public int LastExperienceGain
        {
            get { return lastExperienceGain; }
            set
            {
                if (value == lastExperienceGain)
                {
                    return;
                }

                lastExperienceGain = value;
                NotifyOfPropertyChange(() => LastExperienceGain);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
                NotifyOfPropertyChange(() => LastExperienceGainPercent);
            }
        }

        public float LastExperienceGainPercent => (lastExperienceGain / (float) requiredBaseExperience) * 100;

        public int PreviousRequiredBaseExperience
        {
            get { return previousRequiredBaseExperience; }
            set
            {
                if (value == previousRequiredBaseExperience)
                {
                    return;
                }

                previousRequiredBaseExperience = value;
                NotifyOfPropertyChange(() => PreviousRequiredBaseExperience);
            }
        }

        public DateTime StartTime { get; set; }

        public TimeSpan ElapsedTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        public int ExperiencePerHour
        {
            get { return experiencePerHour; }
            set
            {
                if (value == experiencePerHour)
                {
                    return;
                }

                experiencePerHour = value;
                NotifyOfPropertyChange(() => ExperiencePerHour);
            }
        }

        public string TimeToLevel
        {
            get { return timeToLevel; }
            set
            {
                if (value == timeToLevel)
                {
                    return;
                }

                timeToLevel = value;
                NotifyOfPropertyChange(() => TimeToLevel);
            }
        }

        public void Reset()
        {
            CurrentBaseExperience = 0;
            RequiredBaseExperience = 0;
            LastExperienceGain = 0;
            PreviousRequiredBaseExperience = 0;
            ExperiencePerHour = 0;
            StartTime = DateTime.Now;
        }
    }
}