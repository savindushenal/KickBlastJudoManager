using System;
using System.Linq;
using System.Windows;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Dialogs
{
    public partial class CompetitionsDialog : Window
    {
        private DatabaseService _dbService;
        private int _athleteId;
        private bool _canCompete;

        public CompetitionsDialog(DatabaseService dbService, int athleteId, string athleteName, bool canCompete)
        {
            InitializeComponent();
            _dbService = dbService;
            _athleteId = athleteId;
            _canCompete = canCompete;

            txtHeader.Text = $"Competitions for {athleteName}";
            
            // Set to next 2nd Saturday by default
            dpCompetitionDate.SelectedDate = GetNext2ndSaturday();

            // Disable competition adding if athlete cannot compete
            if (!_canCompete)
            {
                txtCompetitionName.IsEnabled = false;
                dpCompetitionDate.IsEnabled = false;
                var btn = (System.Windows.Controls.Button)FindName("btnAddCompetition");
                if (btn != null) btn.IsEnabled = false;
                
                MessageBox.Show("This athlete's training plan (Beginner) does not allow competition entry.\n\nOnly Intermediate and Elite athletes can enter competitions.",
                    "Competition Restriction", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadCompetitions();
        }

        private DateTime GetNext2ndSaturday()
        {
            var now = DateTime.Now;
            var firstDay = new DateTime(now.Year, now.Month, 1);
            
            // Find first Saturday
            var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
            var firstSaturday = firstDay.AddDays(daysUntilSaturday);
            var secondSaturday = firstSaturday.AddDays(7);
            
            // If 2nd Saturday has passed, get next month's
            if (secondSaturday < now)
            {
                var nextMonth = firstDay.AddMonths(1);
                daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)nextMonth.DayOfWeek + 7) % 7;
                firstSaturday = nextMonth.AddDays(daysUntilSaturday);
                secondSaturday = firstSaturday.AddDays(7);
            }
            
            return secondSaturday;
        }

        private bool IsSecondSaturday(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday)
                return false;
            
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
            var firstSaturday = firstDay.AddDays(daysUntilSaturday);
            var secondSaturday = firstSaturday.AddDays(7);
            
            return date.Date == secondSaturday.Date;
        }

        private void LoadCompetitions()
        {
            var competitions = _dbService.GetAthleteCompetitions(_athleteId);
            dgCompetitions.ItemsSource = competitions;

            int count = competitions.Count;
            decimal total = count * 220m;

            txtTotalCount.Text = count.ToString();
            txtTotalCost.Text = $"Rs. {total:F2}";
        }

        private void AddCompetition_Click(object sender, RoutedEventArgs e)
        {
            // Check if athlete can compete
            if (!_canCompete)
            {
                MessageBox.Show("This athlete cannot enter competitions.\n\nOnly Intermediate and Elite training plans allow competition entry.",
                    "Competition Restriction", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCompetitionName.Text))
            {
                MessageBox.Show("Please enter competition name.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCompetitionName.Focus();
                return;
            }

            if (!dpCompetitionDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select competition date.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCompetitionDate.Focus();
                return;
            }

            // Validate competition date is 2nd Saturday
            if (!IsSecondSaturday(dpCompetitionDate.SelectedDate.Value))
            {
                MessageBox.Show("Competitions can only be held on the second Saturday of each month.\n\nPlease select a valid competition date.",
                    "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCompetitionDate.Focus();
                return;
            }

            try
            {
                var competition = new Competition
                {
                    AthleteId = _athleteId,
                    CompetitionName = txtCompetitionName.Text.Trim(),
                    CompetitionDate = dpCompetitionDate.SelectedDate.Value,
                    Fee = 220m
                };
                
                _dbService.AddCompetition(competition);
                
                txtCompetitionName.Clear();
                dpCompetitionDate.SelectedDate = DateTime.Now;
                
                LoadCompetitions();
                
                MessageBox.Show("Competition added successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding competition: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCompetition_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            var competitionId = (int)button.Tag;

            var result = MessageBox.Show("Are you sure you want to delete this competition?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteCompetition(competitionId);
                    LoadCompetitions();
                    
                    MessageBox.Show("Competition deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting competition: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
