using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Pages
{
    public partial class ViewRecordsPage : Page
    {
        private DatabaseService _dbService;
        private List<Athlete> _allAthletes;

        public ViewRecordsPage()
        {
            InitializeComponent();
            try
            {
                _dbService = new DatabaseService();
                LoadAllAthletes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing page: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllAthletes()
        {
            try
            {
                _allAthletes = _dbService.GetAllAthletes();
                dgAllAthletes.ItemsSource = _allAthletes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading athletes: {ex.Message}\n\nPlease ensure database tables exist.", 
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAthletes();
        }

        private void FilterPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAthletes();
        }

        private void FilterAthletes()
        {
            if (_allAthletes == null) return;

            var filtered = _allAthletes.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filtered = filtered.Where(a => 
                    a.AthleteName.ToLower().Contains(searchTerm) ||
                    a.TrainingPlanName.ToLower().Contains(searchTerm) ||
                    a.WeightCategoryName.ToLower().Contains(searchTerm));
            }

            // Plan filter
            if (cmbFilterPlan.SelectedItem is ComboBoxItem item && item.Content.ToString() != "All Plans")
            {
                string planName = item.Content.ToString();
                filtered = filtered.Where(a => a.TrainingPlanName == planName);
            }

            dgAllAthletes.ItemsSource = filtered.ToList();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAllAthletes();
            txtSearch.Clear();
            cmbFilterPlan.SelectedIndex = 0;
        }
    }
}
