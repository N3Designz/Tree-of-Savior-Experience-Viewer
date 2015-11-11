using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceComponents
{
    public class KillsTilNextLevelComponent : ExperienceComponent
    {
        public KillsTilNextLevelComponent()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 8.ToString("N0");
        }

        public KillsTilNextLevelComponent(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideKillsTilNextLevel;
            Value = experienceData.KillsTilNextLevel.ToString("N0");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.KillsTilNextLevel.ToString("N0");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideKillsTilNextLevel = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Kills TNL";

        public override string HideComponentText { get; set; } = "Hide Kills TNL";
    }
}