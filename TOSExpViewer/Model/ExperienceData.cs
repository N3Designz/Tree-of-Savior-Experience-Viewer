using System;
using Caliburn.Micro;
using TOSExpViewer.Core;

namespace TOSExpViewer.Model
{
    public class ExperienceData : PropertyChangedBase
    {
        private int currentBaseExperience;
        private int requiredBaseExperience;
        private int lastExperienceGain;
        private int previousRequiredBaseExperience;
        private int gainedBaseExperience { get; set; }
        private DateTime startTime;
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
                NotifyOfPropertyChange(() => ExperiencePerHour);
                NotifyOfPropertyChange(() => TimeToLevel);
            }
        }

        public int GainedBaseExperience
        {
            get { return gainedBaseExperience; }
            set
            {
                if(value == gainedBaseExperience)
                {
                    return;
                }

                gainedBaseExperience = value;
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
                NotifyOfPropertyChange(() => ExperiencePerHour);
                NotifyOfPropertyChange(() => TimeToLevel);
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
            }
        }

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

        public DateTime StartTime
        {
            set
            {
                startTime = DateTime.Now;
            }
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                return DateTime.Now - startTime;
            }
        }

        public int ExperiencePerHour
        {
            get { return experiencePerHour; }
            set
            {
                experiencePerHour = value;
                NotifyOfPropertyChange(() => ExperiencePerHour);
            }
        }

        public string TimeToLevel
        {
            get { return timeToLevel; }
            set
            {
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
            startTime = DateTime.Now;
        }
    }
}