namespace KickBlastJudoManager.Models
{
    public class TrainingPlan
    {
        public int PlanId { get; set; }
        
        private string _planName;
        public string PlanName 
        { 
            get => _planName; 
            set 
            { 
                _planName = value;
                Name = value; // Keep Name in sync
            }
        }
        
        public string Name { get; set; }
        public decimal WeeklyFee { get; set; }
        public bool CanEnterCompetitions { get; set; }
        public bool CanCompete { get; set; }

        public string CanCompeteDisplay => CanCompete ? "Yes" : "No";

        public static TrainingPlan[] GetAllPlans()
        {
            return new TrainingPlan[]
            {
                new TrainingPlan { PlanId = 1, Name = "Beginner", WeeklyFee = 250.00m, CanEnterCompetitions = false, CanCompete = false },
                new TrainingPlan { PlanId = 2, Name = "Intermediate", WeeklyFee = 300.00m, CanEnterCompetitions = true, CanCompete = true },
                new TrainingPlan { PlanId = 3, Name = "Elite", WeeklyFee = 350.00m, CanEnterCompetitions = true, CanCompete = true }
            };
        }

        public decimal GetMonthlyFee()
        {
            return WeeklyFee * 4;
        }

        public override string ToString()
        {
            return $"{Name} – Rs. {WeeklyFee:F2}/week";
        }
    }
}
