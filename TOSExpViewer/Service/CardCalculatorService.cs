using TOSExpViewer.Model;

namespace TOSExpViewer.Service
{
    public class CardCalculatorService
    {
        public static void calculateLevel(int currentLevel, int rank, long currentExperience, long totalExperienceFromCards, ExperienceType experienceType, ExperienceCardData experienceCardData)
        {
            long totalExperienceGainedFromLevels = ExperienceUtilityService.GetRequiredExperienceForLevel(experienceType, currentLevel, rank) + currentExperience;
            long finalExperienceGained = totalExperienceGainedFromLevels + totalExperienceFromCards;

            experienceCardData.PotentialLevel = calculatePotentialLevel(experienceType, currentLevel, rank, finalExperienceGained);

            experienceCardData.RemainingExperience = ExperienceUtilityService.GetRequiredExperienceForLevel(experienceType, experienceCardData.PotentialLevel + 1, rank) - finalExperienceGained;
            experienceCardData.RequiredExperience = ExperienceUtilityService.GetRequiredExperienceForLevel(experienceType, experienceCardData.PotentialLevel + 1, rank) - ExperienceUtilityService.GetRequiredExperienceForLevel(experienceType, experienceCardData.PotentialLevel, rank);
            experienceCardData.CurrentExperience = finalExperienceGained - ExperienceUtilityService.GetRequiredExperienceForLevel(experienceType, experienceCardData.PotentialLevel - 1, rank);
            experienceCardData.CurrentPercent = (experienceCardData.CurrentExperience / (float)experienceCardData.RequiredExperience) * 100;
        }

        private static int calculatePotentialLevel(ExperienceType experienceType, int currentLevel, int rank, long finalExperienceGained)
        {
            for (int potentialLevel = currentLevel; potentialLevel < ExperienceUtilityService.GetMaxBaseLevel(experienceType); potentialLevel++)
            {
                long requiredExperience = ExperienceUtilityService.GetRequiredExperienceForLevel(experienceType, potentialLevel, rank);

                if (requiredExperience > finalExperienceGained)
                {
                    return potentialLevel;
                }
            }

            return currentLevel;
        }
    }
}
