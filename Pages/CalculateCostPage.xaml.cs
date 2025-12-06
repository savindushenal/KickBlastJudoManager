using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Pages
{
    public partial class CalculateCostPage : Page
    {
        private DatabaseService _dbService;

        public CalculateCostPage()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                _dbService = new DatabaseService();
                LoadAthletes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}\n\nPlease ensure database tables are created.",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAthletes()
        {
            if (_dbService == null) return;

            try
            {
                var athletes = _dbService.GetAllAthletes();
                cmbAthlete.ItemsSource = athletes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading athletes: {ex.Message}",
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Athlete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAthlete.SelectedItem == null)
            {
                pnlAthleteInfo.Visibility = Visibility.Collapsed;
                return;
            }

            var athlete = (Athlete)cmbAthlete.SelectedItem;
            
            txtAthleteInfo.Text = $"Training Plan: {athlete.TrainingPlanName}\n" +
                                 $"Weekly Fee: Rs. {athlete.WeeklyFee:F2}\n" +
                                 $"Monthly Training Fee: Rs. {(athlete.WeeklyFee * 4):F2} (4 weeks)\n" +
                                 $"Can Enter Competitions: {(athlete.CanCompete ? "Yes" : "No (Beginner plan)")}";
            
            pnlAthleteInfo.Visibility = Visibility.Visible;
            pnlResult.Visibility = Visibility.Collapsed;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (cmbAthlete.SelectedItem == null)
            {
                MessageBox.Show("Please select an athlete.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbAthlete.Focus();
                return;
            }

            if (!int.TryParse(txtCompetitions.Text, out int competitions) || competitions < 0)
            {
                MessageBox.Show("Please enter a valid number of competitions (0 or greater).", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCompetitions.Focus();
                return;
            }

            if (!decimal.TryParse(txtCoachingHours.Text, out decimal coachingHours) || coachingHours < 0)
            {
                MessageBox.Show("Please enter valid coaching hours (0 or greater).", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCoachingHours.Focus();
                return;
            }

            var athlete = (Athlete)cmbAthlete.SelectedItem;

            // Business rule validations
            if (competitions > 0 && !athlete.CanCompete)
            {
                MessageBox.Show($"{athlete.AthleteName} cannot enter competitions.\n\nOnly Intermediate and Elite athletes can compete.\n\nPlease set competitions to 0.",
                    "Competition Restriction", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCompetitions.Focus();
                return;
            }

            // Max 5 hours per week = 20 hours per month (4 weeks)
            if (coachingHours > 20)
            {
                MessageBox.Show("Maximum coaching hours per month is 20 hours (5 hours per week × 4 weeks).\n\nPlease enter 20 or less.",
                    "Coaching Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCoachingHours.Focus();
                return;
            }

            // Calculate costs
            var monthlyTrainingFee = athlete.WeeklyFee * 4;
            var competitionFees = competitions * 220m;
            var coachingFees = coachingHours * 90.50m;
            var totalCost = monthlyTrainingFee + competitionFees + coachingFees;

            // Display result
            txtResult.Text = $"Athlete: {athlete.AthleteName}\n" +
                           $"Training Plan: {athlete.TrainingPlanName}\n\n" +
                           $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                           $"Training Fee:\n" +
                           $"  Rs. {athlete.WeeklyFee:F2} × 4 weeks = Rs. {monthlyTrainingFee:F2}\n\n" +
                           $"Competition Fees:\n" +
                           $"  {competitions} × Rs. 220.00 = Rs. {competitionFees:F2}\n\n" +
                           $"Private Coaching:\n" +
                           $"  {coachingHours:F1} hours × Rs. 90.50 = Rs. {coachingFees:F2}\n\n" +
                           $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                           $"TOTAL MONTHLY COST: Rs. {totalCost:F2}";

            pnlResult.Visibility = Visibility.Visible;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            cmbAthlete.SelectedIndex = -1;
            txtCompetitions.Text = "0";
            txtCoachingHours.Text = "0";
            pnlAthleteInfo.Visibility = Visibility.Collapsed;
            pnlResult.Visibility = Visibility.Collapsed;
        }
    }
}
