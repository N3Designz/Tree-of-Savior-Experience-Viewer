using System;
using Caliburn.Micro;

namespace TOSExpViewer.Model
{
    public class ExperienceData : PropertyChangedBase, IHaveDisplayName
    {
        private int currentExperience;
        private int requiredExperience;
        private int lastExperienceGain;
        private int previousRequiredExperience;
        private int gainedExperience;
        private int experiencePerHour;
        private string timeToLevel;
        private DateTime startTime;

        public string DisplayName { get; set; }

        public bool FirstUpdate { get; set; }

        public int CurrentExperience
        {
            get { return currentExperience; }
            set
            {
                if (value == currentExperience)
                {
                    return;
                }

                currentExperience = value;
                NotifyOfPropertyChange(() => CurrentExperience);
                NotifyOfPropertyChange(() => CurrentExperiencePercent);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
            }
        }

        public int GainedExperience
        {
            get { return gainedExperience; }
            set
            {
                if (value == gainedExperience)
                {
                    return;
                }

                gainedExperience = value;
                NotifyOfPropertyChange(() => GainedExperience);
            }
        }

        public float CurrentExperiencePercent => (currentExperience / (float)requiredExperience) * 100;

        public int RequiredExperience
        {
            get { return requiredExperience; }
            set
            {
                if (value == requiredExperience)
                {
                    return;
                }

                requiredExperience = value;
                NotifyOfPropertyChange(() => RequiredExperience);
                NotifyOfPropertyChange(() => CurrentExperiencePercent);
                NotifyOfPropertyChange(() => KillsTilNextLevel);
                NotifyOfPropertyChange(() => LastExperienceGainPercent);
            }
        }

        public double KillsTilNextLevel => Math.Ceiling((RequiredExperience - CurrentExperience) / (double)LastExperienceGain);

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

        public float LastExperienceGainPercent => (lastExperienceGain / (float) requiredExperience) * 100;

        public int PreviousRequiredExperience
        {
            get { return previousRequiredExperience; }
            set
            {
                if (value == previousRequiredExperience)
                {
                    return;
                }

                previousRequiredExperience = value;
                NotifyOfPropertyChange(() => PreviousRequiredExperience);
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (value.Equals(startTime)) return;
                startTime = value;
                NotifyOfPropertyChange(() => StartTime);
                NotifyOfPropertyChange(() => ElapsedTime);
            }
        }

        public TimeSpan ElapsedTime => DateTime.Now - StartTime;

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
            FirstUpdate = true;
            CurrentExperience = 0;
            RequiredExperience = 0;
            LastExperienceGain = 0;
            PreviousRequiredExperience = 0;
            ExperiencePerHour = 0;
            StartTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{DisplayName} - First Update:{FirstUpdate}, Current:{CurrentExperience}, Required:{RequiredExperience}";
        }
    }
}