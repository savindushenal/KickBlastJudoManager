using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;
using KickBlastJudoManager.Dialogs;

namespace KickBlastJudoManager.Pages
{
    public partial class ManageAthletesPage : Page
    {
        private DatabaseService _dbService;

        public ManageAthletesPage()
        {
            InitializeComponent();
            InitializeData();
            LoadAthletes();
        }

        private void InitializeData()
        {
            try
            {
                _dbService = new DatabaseService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}\\n\\nPlease ensure database tables are created.",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAthletes()
        {
            if (_dbService == null) return;

            try
            {
                var athletes = _dbService.GetAllAthletes();
                dgAthletes.ItemsSource = athletes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading athletes: {ex.Message}",
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dbService == null) return;

            try
            {
                var athletes = _dbService.GetAllAthletes();
                var searchText = txtSearch.Text.ToLower();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    athletes = athletes.Where(a => 
                        a.AthleteName.ToLower().Contains(searchText) ||
                        a.TrainingPlanName.ToLower().Contains(searchText) ||
                        a.WeightCategoryName.ToLower().Contains(searchText)
                    ).ToList();
                }

                dgAthletes.ItemsSource = athletes;
            }
            catch { }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection handling if needed
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAthletes();
            txtSearch.Clear();
        }

        private void AddNewAthlete_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AthleteDialog(_dbService);
            dialog.Owner = Window.GetWindow(this);
            
            if (dialog.ShowDialog() == true && dialog.SaveClicked)
            {
                LoadAthletes();
            }
        }

        private void EditAthlete_Click(object sender, RoutedEventArgs e)
        {
            var athleteId = (int)((Button)sender).Tag;
            
            try
            {
                var athlete = _dbService.GetAthleteById(athleteId);
                
                var dialog = new AthleteDialog(_dbService, athleteId);
                dialog.Owner = Window.GetWindow(this);
                
                if (dialog.ShowDialog() == true && dialog.SaveClicked)
                {
                    LoadAthletes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading athlete: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteAthlete_Click(object sender, RoutedEventArgs e)
        {
            var athleteId = (int)((Button)sender).Tag;
            
            try
            {
                var athlete = _dbService.GetAthleteById(athleteId);
                
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {athlete.AthleteName}?\\n\\nThis will also delete all associated competitions and coaching sessions.", 
                    "Confirm Delete", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _dbService.DeleteAthlete(athleteId);
                    MessageBox.Show("Athlete deleted successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAthletes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting athlete: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
