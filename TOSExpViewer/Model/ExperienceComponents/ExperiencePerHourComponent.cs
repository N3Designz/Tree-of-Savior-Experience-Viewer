using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceComponents
{
    public class ExperiencePerHourComponent : ExperienceComponent
    {
        public ExperiencePerHourComponent()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 1754.ToString("N0");
        }

        public ExperiencePerHourComponent(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideExperiencePerHour;
            Value = experienceData.ExperiencePerHour.ToString("N0");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.ExperiencePerHour.ToString("N0");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideExperiencePerHour = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Exp/Hr";

        public override string HideComponentText { get; set; } = "Hide Exp/Hr";
    }
}