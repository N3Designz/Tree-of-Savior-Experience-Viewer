using System;
using TOSExpViewer.Core;
using TOSExpViewer.Model;
using Caliburn.Micro;

namespace TOSExpViewer.Service
{
    public class ExperienceUpdateService : PropertyChangedBase
    {
        private readonly ExperienceDataToTextService experienceDataToTextService = new ExperienceDataToTextService();

        public void UpdateExperienceValues(ExperienceData experienceData, int currentExperience, int requiredExperience)
        {
            var newCurrentBaseExperience = currentExperience;
            var requiredBasedExp = requiredExperience;

            if (newCurrentBaseExperience == int.MinValue || requiredBasedExp == int.MinValue ||
                requiredBasedExp == 15) // for some reason required base exp returns as 15 when char not selected, no idea why
            {
                Reset(experienceData);
                experienceData.Reset();
                return;
            }

            experienceData.RequiredBaseExperience = requiredBasedExp;

            if (experienceData.FirstUpdate)
            {
                experienceData.PreviousRequiredBaseExperience = requiredBasedExp;
                experienceData.CurrentBaseExperience = newCurrentBaseExperience;
                experienceData.FirstUpdate = false;
            }
            else if (newCurrentBaseExperience != experienceData.CurrentBaseExperience) // exp hasn't changed, nothing else to do
            {
                if (experienceData.RequiredBaseExperience > experienceData.PreviousRequiredBaseExperience) // handle level up scenarios
                {
                    experienceData.LastExperienceGain = (experienceData.PreviousRequiredBaseExperience - experienceData.CurrentBaseExperience) + newCurrentBaseExperience;
                    experienceData.PreviousRequiredBaseExperience = requiredBasedExp;
                }
                else
                {
                    experienceData.LastExperienceGain = newCurrentBaseExperience - experienceData.CurrentBaseExperience;
                }

                experienceData.CurrentBaseExperience = newCurrentBaseExperience;
                experienceData.GainedBaseExperience += experienceData.LastExperienceGain;
            }

            experienceData.ExperiencePerHour = (int)(experienceData.GainedBaseExperience * (TimeSpan.FromHours(1).TotalMilliseconds / experienceData.ElapsedTime.TotalMilliseconds));

            experienceData.TimeToLevel = CalculateTimeToLevel(experienceData);
            // make sure the elapsed time is kept updated every tick, must be called from experience data class
            experienceData.NotifyOfPropertyChange(() => experienceData.ElapsedTime);

            experienceDataToTextService.writeToFile(experienceData);
        }

        private string CalculateTimeToLevel(ExperienceData experienceData)
        {
            if (experienceData.LastExperienceGain == 0)
            {
                return Constants.INFINITY;
            }

            var totalExperienceRequired = experienceData.RequiredBaseExperience - experienceData.CurrentBaseExperience;
            var experiencePerSecond = experienceData.GainedBaseExperience / experienceData.ElapsedTime.TotalSeconds;

            if (experiencePerSecond == 0 || double.IsNaN(experiencePerSecond))
            {
                return Constants.INFINITY;
            }

            var estimatedTimeToLevel = TimeSpan.FromSeconds(totalExperienceRequired / experiencePerSecond);

            return estimatedTimeToLevel.ToShortDisplayFormat();
        }

        public void Reset(ExperienceData experienceData)
        {
            // we don't want to reset all exp data values, just the current session values
            experienceData.GainedBaseExperience = 0;
            experienceData.StartTime = DateTime.Now;
            experienceData.TimeToLevel = CalculateTimeToLevel(experienceData);
            NotifyOfPropertyChange(() => experienceData.ExperiencePerHour);
        }
    }
}
