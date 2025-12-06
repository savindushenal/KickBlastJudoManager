using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Dialogs
{
    public partial class CoachingDialog : Window
    {
        private DatabaseService _dbService;
        private int _athleteId;

        public CoachingDialog(DatabaseService dbService, int athleteId, string athleteName)
        {
            InitializeComponent();
            _dbService = dbService;
            _athleteId = athleteId;

            txtHeader.Text = $"Coaching Sessions for {athleteName}";
            dpSessionDate.SelectedDate = DateTime.Now;

            LoadSessions();
        }

        private void LoadSessions()
        {
            var sessions = _dbService.GetAthleteCoachingSessions(_athleteId);
            dgSessions.ItemsSource = sessions;

            decimal totalHours = sessions.Sum(s => s.Hours);
            decimal totalCost = totalHours * 90.50m;

            txtTotalHours.Text = $"{totalHours:F1}";
            txtTotalCost.Text = $"Rs. {totalCost:F2}";
        }

        private void AddSession_Click(object sender, RoutedEventArgs e)
        {
            if (!dpSessionDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select session date.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpSessionDate.Focus();
                return;
            }

            if (!decimal.TryParse(txtHours.Text, out decimal hours) || hours <= 0)
            {
                MessageBox.Show("Please enter valid hours (greater than 0).", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtHours.Focus();
                return;
            }

            // Validate 5 hours per week maximum
            var sessionDate = dpSessionDate.SelectedDate.Value;
            var weekStart = sessionDate.AddDays(-(int)sessionDate.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);
            
            var sessions = _dbService.GetAthleteCoachingSessions(_athleteId);
            var weeklyHours = sessions
                .Where(s => s.SessionDate >= weekStart && s.SessionDate <= weekEnd && s.SessionId != 0)
                .Sum(s => s.Hours);
            
            if (weeklyHours + hours > 5)
            {
                var remaining = 5 - weeklyHours;
                MessageBox.Show($"Maximum 5 hours of private coaching per week allowed.\n\nThis week already has {weeklyHours:F1} hours.\nRemaining: {remaining:F1} hours.\n\nPlease enter {remaining:F1} hours or less.",
                    "Weekly Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtHours.Focus();
                return;
            }

            try
            {
                var session = new CoachingSession
                {
                    AthleteId = _athleteId,
                    SessionDate = dpSessionDate.SelectedDate.Value,
                    Hours = hours,
                    HourlyRate = 90.50m
                };
                
                _dbService.AddCoachingSession(session);
                
                txtHours.Clear();
                dpSessionDate.SelectedDate = DateTime.Now;
                
                LoadSessions();
                
                MessageBox.Show("Coaching session added successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding session: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSession_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            var sessionId = (int)button.Tag;

            var result = MessageBox.Show("Are you sure you want to delete this coaching session?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteCoachingSession(sessionId);
                    LoadSessions();
                    
                    MessageBox.Show("Coaching session deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting session: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class HoursToCostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal hours)
            {
                return hours * 90.50m;
            }
            return 0m;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
