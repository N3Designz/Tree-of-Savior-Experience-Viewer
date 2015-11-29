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
                experienceData.Reset();
                return;
            }

            experienceData.RequiredExperience = requiredBasedExp;

            if (experienceData.FirstUpdate)
            {
                experienceData.PreviousRequiredExperience = requiredBasedExp;
                experienceData.CurrentExperience = newCurrentBaseExperience;
                experienceData.FirstUpdate = false;
            }
            else if (newCurrentBaseExperience != experienceData.CurrentExperience) // exp hasn't changed, nothing else to do
            {
                if (experienceData.RequiredExperience > experienceData.PreviousRequiredExperience) // handle level up scenarios
                {
                    experienceData.LastExperienceGain = (experienceData.PreviousRequiredExperience - experienceData.CurrentExperience) + newCurrentBaseExperience;
                    experienceData.PreviousRequiredExperience = requiredBasedExp;
                }
                else
                {
                    experienceData.LastExperienceGain = newCurrentBaseExperience - experienceData.CurrentExperience;
                }

                experienceData.CurrentExperience = newCurrentBaseExperience;
                experienceData.GainedExperience += experienceData.LastExperienceGain;
            }

            experienceData.ExperiencePerHour = (int)(experienceData.GainedExperience * (TimeSpan.FromHours(1).TotalMilliseconds / experienceData.ElapsedTime.TotalMilliseconds));

            experienceData.TimeToLevel = CalculateTimeToLevel(experienceData);
            // make sure the elapsed time is kept updated every tick, must be called from experience data class
            experienceData.NotifyOfPropertyChange(() => experienceData.ElapsedTime);

            experienceDataToTextService.WriteToFile(experienceData);
        }

        private string CalculateTimeToLevel(ExperienceData experienceData)
        {
            if (experienceData.LastExperienceGain == 0)
            {
                return Constants.INFINITY;
            }

            var totalExperienceRequired = experienceData.RequiredExperience - experienceData.CurrentExperience;
            var experiencePerSecond = experienceData.GainedExperience / experienceData.ElapsedTime.TotalSeconds;

            if (experiencePerSecond == 0 || double.IsNaN(experiencePerSecond))
            {
                return Constants.INFINITY;
            }

            var estimatedTimeToLevel = TimeSpan.FromSeconds(totalExperienceRequired / experiencePerSecond);

            return estimatedTimeToLevel.ToShortDisplayFormat();
        }
    }
}
