using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceComponents
{
    public class TimeToLevelComponent : ExperienceComponent
    {
        public TimeToLevelComponent()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = "~30m";
        }

        public TimeToLevelComponent(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideTimeToLevel;
            Value = experienceData.TimeToLevel;

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.TimeToLevel;
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideTimeToLevel = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Time TNL";

        public override string HideComponentText { get; set; } = "Hide Time TNL";
    }
}