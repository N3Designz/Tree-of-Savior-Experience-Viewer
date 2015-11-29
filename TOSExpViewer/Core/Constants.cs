using System;

namespace TOSExpViewer.Core
{
    public class Constants
    {
        public const string INFINITY = "\u221E";

        internal static int MAX_CLASS_LEVEL = 14;

        internal static int MAX_CLASS_RANK = 10;

        private static readonly int[,] REQUIRED_CLASS_EXPERIENCE = new int[,] {
            { 58, 267, 653, 1231, 2011, 3004, 4217, 5657, 7330, 9243, 11399, 13804, 16462, 19377 },
            { 2724, 12519, 30548, 57524, 93983, 140362, 197030, 264311, 342492, 431834, 532575, 644935, 769117, 905313 },
            { 16004, 73536, 179432, 337883, 552038, 824456, 1157312, 1552505, 2011725, 2536501, 3128232, 3788208, 4517628, 5317613 },
            { 57976, 266388, 650005, 1224002, 1999789, 2986641, 4192431, 5624037, 7287587, 9188619, 11332198, 13722998, 16365363, 19263356 },
            { 245833, 1129555, 2756185, 5190075, 8479606, 12664103, 17776956, 23847324, 30901190, 38962040, 48051352, 58188936, 69393224, 81681440 },
            { 535630, 2461111, 6005263, 11308299, 18475632, 27592944, 38732988, 51959300, 67328480, 84891712, 104695792, 126783888, 151196128, 177970080 },
            { 924709, 4248849, 10367454, 19522586, 31896232, 47636312, 66868420, 89702256, 116235528, 146556592, 180746240, 218879008, 261024192, 307246592},
            { 1413534, 6494899, 15847954, 29842720, 48757392, 72818072, 102216768, 137121136, 177680576, 224030144, 276293312, 334584000, 399008192, 469664928 },
            { 2001893, 9198286, 22444384, 42264224, 69051792, 103127304, 144762672, 194195376, 251636960, 317278688, 391295488, 473848672, 565088256, 665154624 },
            { 2689951, 12359771, 30158602, 56790596, 92785144, 138572544, 194518144, 260941056, 338125536, 426328576, 525785184, 636712192, 759311232, 893770752 }
        };

        public static int GetRequiredClassExperience(int rank, int classLevel)
        {
            GuardRankAndClass(rank, classLevel);

            if (classLevel > 1)
            {
                return REQUIRED_CLASS_EXPERIENCE[rank - 1, classLevel - 1] - REQUIRED_CLASS_EXPERIENCE[rank - 1, classLevel - 2];
            }

            return REQUIRED_CLASS_EXPERIENCE[rank - 1, classLevel - 1];
        }

        public static int GetCurrentClassExperienceForLevelOnly(int currentTotalClassExperience, int rank, int classLevel)
        {
            GuardRankAndClass(rank, classLevel);

            if (classLevel > 1)
            {
                // subtract current experience from previous level required experience to get current experience
                return currentTotalClassExperience - REQUIRED_CLASS_EXPERIENCE[rank - 1, classLevel - 2];
            }
            
            return currentTotalClassExperience;
        }

        private static void GuardRankAndClass(int rank, int classLevel)
        {
            if (rank < 1 || rank > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(rank), "You must use a rank between 1 and 10");
            }
            if (classLevel < 1 || classLevel > 14)
            {
                throw new ArgumentOutOfRangeException(nameof(classLevel), "You must use a class level between 1 and 14");
            }
        }
    }
}
