namespace KickBlastJudoManager.Models
{
    public class WeightCategory
    {
        public int CategoryId { get; set; }
        
        private string _categoryName;
        public string CategoryName 
        { 
            get => _categoryName; 
            set 
            { 
                _categoryName = value;
                Name = value; // Keep Name in sync
            }
        }
        
        public string Name { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal? UpperWeightLimit { get; set; }

        public static WeightCategory[] GetAllCategories()
        {
            return new WeightCategory[]
            {
                new WeightCategory { CategoryId = 1, Name = "Heavyweight", MaxWeight = decimal.MaxValue, UpperWeightLimit = null },
                new WeightCategory { CategoryId = 2, Name = "Light-Heavyweight", MaxWeight = 100m, UpperWeightLimit = 100m },
                new WeightCategory { CategoryId = 3, Name = "Middleweight", MaxWeight = 90m, UpperWeightLimit = 90m },
                new WeightCategory { CategoryId = 4, Name = "Light-Middleweight", MaxWeight = 81m, UpperWeightLimit = 81m },
                new WeightCategory { CategoryId = 5, Name = "Lightweight", MaxWeight = 73m, UpperWeightLimit = 73m },
                new WeightCategory { CategoryId = 6, Name = "Flyweight", MaxWeight = 66m, UpperWeightLimit = 66m }
            };
        }

        public string GetWeightStatus(decimal currentWeight)
        {
            if (Name == "Heavyweight")
            {
                return "Within category";
            }

            if (currentWeight <= MaxWeight)
            {
                return "Within category";
            }
            else
            {
                decimal difference = currentWeight - MaxWeight;
                return $"Over category by {difference:F0}kg";
            }
        }

        public override string ToString()
        {
            if (Name == "Heavyweight")
            {
                return $"{Name} — Unlimited";
            }
            return $"{Name} — {MaxWeight:F0}kg";
        }
    }
}
