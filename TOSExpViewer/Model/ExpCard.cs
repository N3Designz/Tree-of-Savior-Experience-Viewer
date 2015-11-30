using System;
using Caliburn.Micro;

namespace TOSExpViewer.Model
{
    public class ExpCard : PropertyChangedBase, IHaveDisplayName
    {
        private const int MAX_AMOUNT = 1000;

        private static readonly int[,] CARD_EXP = new[,]
        {
            { 500, 385 }, // base exp, class exp
            { 2686, 2068 },
            { 8442, 6500 },
            { 22860, 17602 },
            { 24571, 18919 },
            { 60312, 46440 },
            { 142150, 109455 },
            { 209334, 161187 },
            { 237943, 183216 },
            { 541023, 416587 },
            { 608155, 468279 },
            { 1344829, 1035518 }, // level 12 exp card (tosdatabase.net)
        };

        private int amount;
        private int classExperience;
        private int baseExperience;
        private string displayName;

        public static ExpCard CreateCardLevel(int cardLevel)
        {
            if (cardLevel < 1 || cardLevel > 12)
                throw new ArgumentOutOfRangeException(nameof(cardLevel), "Card level must be between 1 and 12");

            return new ExpCard()
            {
                DisplayName = $"Lv{cardLevel} EXP Card",
                BaseExperience = CARD_EXP[cardLevel - 1, 0],
                ClassExperience = CARD_EXP[cardLevel - 1, 1]
            };
        }

        public int BaseExperience
        {
            get { return baseExperience; }
            set
            {
                if (value == baseExperience) return;
                baseExperience = value;
                NotifyOfPropertyChange(() => BaseExperience);
            }
        }

        public int ClassExperience
        {
            get { return classExperience; }
            set
            {
                if (value == classExperience) return;
                classExperience = value;
                NotifyOfPropertyChange(() => ClassExperience);
            }
        }

        /// <summary>  How many of this type of card </summary>
        public int Amount
        {
            get { return amount; }
            set
            {
                if (value == amount) return;
                amount = value > MAX_AMOUNT ? MAX_AMOUNT : value;
                NotifyOfPropertyChange(() => Amount);
            }
        }

        public string DisplayName
        {
            get { return displayName; }
            set
            {
                if (value == displayName) return;
                displayName = value;
                NotifyOfPropertyChange(() => DisplayName);
            }
        }
    }
}