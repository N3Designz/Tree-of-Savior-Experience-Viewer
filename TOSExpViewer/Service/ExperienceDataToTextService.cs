using System;
using System.Configuration;
using System.IO;
using TOSExpViewer.Model;

namespace TOSExpViewer.Service
{
    class ExperienceDataToTextService
    {
        private const string FolderName = "stream";

        private readonly bool isExperienceDataToTextServiceEnabled;

        public ExperienceDataToTextService()
        {
            isExperienceDataToTextServiceEnabled = Boolean.Parse(ConfigurationManager.AppSettings["StreamOutput"]);
        }

        public void WriteToFile(ExperienceData experienceData)
        {
            if(!isExperienceDataToTextServiceEnabled)
            {
                return;
            }
            
            var prefix = experienceData.DisplayName.Replace(" ", string.Empty).ToLower();
            WriteToFile($"{prefix}_currentBaseExperience.txt", experienceData.CurrentExperience.ToString("N0"));
            WriteToFile($"{prefix}_requiredBaseExperience.txt", experienceData.RequiredExperience.ToString("N0"));
            WriteToFile($"{prefix}_killsTilNextLevel.txt", experienceData.KillsTilNextLevel.ToString("N0"));
            WriteToFile($"{prefix}_currentBaseExperiencePercent.txt", experienceData.CurrentExperiencePercent.ToString("F2") + "%");
            WriteToFile($"{prefix}_lastExperienceGain.txt", experienceData.LastExperienceGain.ToString("N0"));
            WriteToFile($"{prefix}_timeToLevel.txt", experienceData.TimeToLevel);
            WriteToFile($"{prefix}_experiencePerHour.txt", experienceData.ExperiencePerHour.ToString("N0"));
        }

        private void WriteToFile(string filename, string data)
        {
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }

            File.WriteAllText(Path.Combine(FolderName, filename), data);
        }
    }
}
