using System;
using KickBlastJudoManager.Models;

namespace KickBlastJudoManager.Services
{
    public class CostCalculator
    {
        private const decimal PRIVATE_TUITION_RATE = 90.50m;
        private const decimal COMPETITION_FEE = 220.00m;
        private const int WEEKS_PER_MONTH = 4;
        private const int MAX_PRIVATE_HOURS_PER_WEEK = 5;
        private const int MAX_PRIVATE_HOURS_PER_MONTH = MAX_PRIVATE_HOURS_PER_WEEK * WEEKS_PER_MONTH; // 5 hours/week × 4 weeks = 20 hours/month

        public static string ValidateInput(string athleteName, TrainingPlan plan, decimal weight,
            WeightCategory category, int competitions, int privateHours)
        {
            if (string.IsNullOrWhiteSpace(athleteName))
            {
                return "Athlete name is required.";
            }

            if (plan == null)
            {
                return "Please select a training plan.";
            }

            if (weight <= 0)
            {
                return "Current weight must be greater than zero.";
            }

            if (category == null)
            {
                return "Please select a weight category.";
            }

            if (competitions < 0)
            {
                return "Competitions entered cannot be negative.";
            }

            if (competitions > 0 && !plan.CanEnterCompetitions)
            {
                return "Only Intermediate and Elite athletes can enter competitions.";
            }

            if (privateHours < 0 || privateHours > MAX_PRIVATE_HOURS_PER_MONTH)
            {
                return $"Private coaching hours must be between 0 and {MAX_PRIVATE_HOURS_PER_MONTH} (maximum {MAX_PRIVATE_HOURS_PER_WEEK} hours per week).";
            }

            return null;
        }

        public static decimal CalculateMonthlyCost(TrainingPlan plan, int competitions, int privateHours)
        {
            decimal trainingCost = plan.GetMonthlyFee();
            decimal competitionCost = competitions * COMPETITION_FEE;
            decimal coachingCost = privateHours * PRIVATE_TUITION_RATE;

            return trainingCost + competitionCost + coachingCost;
        }

        public static string GenerateCostBreakdown(string athleteName, TrainingPlan plan,
            int competitions, int privateHours, WeightCategory category, decimal currentWeight)
        {
            decimal trainingCost = plan.GetMonthlyFee();
            decimal competitionCost = competitions * COMPETITION_FEE;
            decimal coachingCost = privateHours * PRIVATE_TUITION_RATE;
            decimal totalCost = trainingCost + competitionCost + coachingCost;

            string breakdown = $"Athlete: {athleteName}\n";
            breakdown += $"Training Plan ({plan.Name}): {plan.WeeklyFee:F2} × {WEEKS_PER_MONTH} = Rs. {trainingCost:F2}\n";

            if (competitions > 0)
            {
                breakdown += $"Competitions ({competitions} × {COMPETITION_FEE:F2}): Rs. {competitionCost:F2}\n";
            }

            if (privateHours > 0)
            {
                breakdown += $"Private Coaching ({privateHours} × {PRIVATE_TUITION_RATE:F2}): Rs. {coachingCost:F2}\n";
            }

            breakdown += "---------------------------------------\n";
            breakdown += $"Total Monthly Cost: Rs. {totalCost:F2}\n";
            breakdown += $"Weight Status: {category.GetWeightStatus(currentWeight)}";

            return breakdown;
        }
    }
}
