using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;
using KickBlastJudoManager.Dialogs;

namespace KickBlastJudoManager.Pages
{
    public partial class CompetitionManagementPage : Page
    {
        private DatabaseService _dbService;

        public CompetitionManagementPage()
        {
            InitializeComponent();
            InitializeData();
            LoadCompetitions();
        }

        private void InitializeData()
        {
            try
            {
                _dbService = new DatabaseService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}\n\nPlease ensure database tables are created.",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCompetitions()
        {
            if (_dbService == null) return;

            try
            {
                var allCompetitions = _dbService.GetAllCompetitionsGrouped();
                dgCompetitions.ItemsSource = allCompetitions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading competitions: {ex.Message}",
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dbService == null) return;

            try
            {
                var allCompetitions = _dbService.GetAllCompetitionsGrouped();
                var searchText = txtSearch.Text.ToLower();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    allCompetitions = allCompetitions.Where(c => 
                        c.CompetitionName.ToLower().Contains(searchText)
                    ).ToList();
                }

                dgCompetitions.ItemsSource = allCompetitions;
            }
            catch { }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCompetitions();
            txtSearch.Clear();
        }

        private void AddCompetition_Click(object sender, RoutedEventArgs e)
        {
            pnlAddForm.Visibility = Visibility.Visible;
            txtCompetitionName.Clear();
            dpCompetitionDate.SelectedDate = GetNext2ndSaturday();
            txtCompetitionName.Focus();
        }

        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddForm.Visibility = Visibility.Collapsed;
        }

        private DateTime GetNext2ndSaturday()
        {
            var now = DateTime.Now;
            var firstDay = new DateTime(now.Year, now.Month, 1);
            
            var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
            var firstSaturday = firstDay.AddDays(daysUntilSaturday);
            var secondSaturday = firstSaturday.AddDays(7);
            
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

        private void SaveCompetition_Click(object sender, RoutedEventArgs e)
        {
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

            if (!IsSecondSaturday(dpCompetitionDate.SelectedDate.Value))
            {
                MessageBox.Show("Competitions can only be held on the second Saturday of each month.\n\nPlease select a valid date.",
                    "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCompetitionDate.Focus();
                return;
            }

            try
            {
                _dbService.RegisterCompetitionEvent(txtCompetitionName.Text.Trim(), dpCompetitionDate.SelectedDate.Value);
                
                MessageBox.Show("Competition registered successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                pnlAddForm.Visibility = Visibility.Collapsed;
                LoadCompetitions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error registering competition: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageParticipants_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open participants dialog
            MessageBox.Show("Participants management coming soon!", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteCompetition_Click(object sender, RoutedEventArgs e)
        {
            var competitionId = (int)((Button)sender).Tag;
            
            var result = MessageBox.Show("Are you sure you want to delete this competition?\n\nAll participant registrations will also be removed.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteCompetition(competitionId);
                    
                    MessageBox.Show("Competition deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    LoadCompetitions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting competition: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Yes" : "No";
            }
            return "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
