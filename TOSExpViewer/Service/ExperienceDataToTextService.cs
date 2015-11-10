using System;
using System.Configuration;
using System.IO;
using TOSExpViewer.Model;

namespace TOSExpViewer.Service
{
    class ExperienceDataToTextService
    {
        private const string path = "stream";

        private bool isExperienceDataToTextServiceEnabled;

        public ExperienceDataToTextService()
        {
            isExperienceDataToTextServiceEnabled = Boolean.Parse(ConfigurationManager.AppSettings["StreamOutput"]);
        }

        public void writeToFile(ExperienceData experienceData)
        {
            if(!isExperienceDataToTextServiceEnabled)
            {
                return;
            }

            Directory.CreateDirectory(path);
            
            writeToFile("currentBaseExperience.txt", experienceData.CurrentBaseExperience.ToString("N0"));
            writeToFile("requiredBaseExperience.txt", experienceData.RequiredBaseExperience.ToString("N0"));
            writeToFile("killsTilNextLevel.txt", experienceData.KillsTilNextLevel.ToString("N0"));
            writeToFile("currentBaseExperiencePercent.txt", experienceData.CurrentBaseExperiencePercent.ToString("F2") + "%");
            writeToFile("lastExperienceGain.txt", experienceData.LastExperienceGain.ToString("N0"));
            writeToFile("timeToLevel.txt", experienceData.TimeToLevel);
            writeToFile("experiencePerHour.txt", experienceData.ExperiencePerHour.ToString("N0"));
        }

        private void writeToFile(string filename, object data)
        {
            using (StreamWriter streamWriter = new StreamWriter(path + "/" + filename))
            {
                streamWriter.Write(data);
            }
        }
    }
}
