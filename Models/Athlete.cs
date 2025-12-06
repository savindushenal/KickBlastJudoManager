using System;
using System.Collections.Generic;

namespace KickBlastJudoManager.Models
{
    public class Athlete
    {
        public int AthleteId { get; set; }
        public string AthleteName { get; set; }
        public decimal CurrentWeight { get; set; }
        public int PlanId { get; set; }
        public int CompetitionCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties (for display)
        public string TrainingPlanName { get; set; }
        public string WeightCategoryName { get; set; }
        public decimal WeeklyFee { get; set; }
        public bool CanCompete { get; set; }
        
        // Calculated properties
        public int TotalCompetitions { get; set; }
        public decimal TotalCoachingHours { get; set; }
        public decimal MonthlyCost { get; set; }
    }
    
    public class Competition
    {
        public int CompetitionId { get; set; }
        public int AthleteId { get; set; }
        public DateTime CompetitionDate { get; set; }
        public string CompetitionName { get; set; }
        public decimal Fee { get; set; } = 220.00m;
        public string Result { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Competition Event (separate entity)
    public class CompetitionEvent
    {
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public DateTime CompetitionDate { get; set; }
        public int ParticipantCount { get; set; }
    }
    
    public class CoachingSession
    {
        public int SessionId { get; set; }
        public int AthleteId { get; set; }
        public DateTime SessionDate { get; set; }
        public decimal Hours { get; set; }
        public decimal HourlyRate { get; set; } = 90.50m;
        public decimal TotalFee { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class MonthlyBill
    {
        public int BillId { get; set; }
        public int AthleteId { get; set; }
        public DateTime BillingMonth { get; set; }
        public decimal TrainingFee { get; set; }
        public decimal CompetitionFees { get; set; }
        public decimal CoachingFees { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
