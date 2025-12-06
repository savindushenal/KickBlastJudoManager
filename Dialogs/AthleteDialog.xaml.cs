using System;
using System.Linq;
using System.Windows;
using KickBlastJudoManager.Models;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Dialogs
{
    public partial class AthleteDialog : Window
    {
        private DatabaseService _dbService;
        private int? _athleteId;
        public bool SaveClicked { get; private set; }

        public AthleteDialog(DatabaseService dbService, int? athleteId = null)
        {
            InitializeComponent();
            _dbService = dbService;
            _athleteId = athleteId;

            LoadData();

            if (_athleteId.HasValue)
            {
                Title = "Edit Athlete";
                btnSave.Content = "Update";
                LoadAthleteData();
            }
        }

        private void LoadData()
        {
            var plans = _dbService.GetAllTrainingPlans();
            var categories = _dbService.GetAllWeightCategories();

            cmbTrainingPlan.ItemsSource = plans;
            cmbWeightCategory.ItemsSource = categories;
        }

        private void LoadAthleteData()
        {
            if (!_athleteId.HasValue) return;

            var athlete = _dbService.GetAthleteById(_athleteId.Value);
            txtAthleteName.Text = athlete.AthleteName;
            txtCurrentWeight.Text = athlete.CurrentWeight.ToString();
            
            cmbTrainingPlan.SelectedItem = ((System.Collections.IEnumerable)cmbTrainingPlan.ItemsSource)
                .Cast<TrainingPlan>()
                .FirstOrDefault(p => p.PlanId == athlete.PlanId);
            
            cmbWeightCategory.SelectedItem = ((System.Collections.IEnumerable)cmbWeightCategory.ItemsSource)
                .Cast<WeightCategory>()
                .FirstOrDefault(c => c.CategoryId == athlete.CompetitionCategoryId);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SaveClicked = false;
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtAthleteName.Text))
                {
                    MessageBox.Show("Please enter athlete name.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtAthleteName.Focus();
                    return;
                }

                if (cmbTrainingPlan.SelectedItem == null)
                {
                    MessageBox.Show("Please select a training plan.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbTrainingPlan.Focus();
                    return;
                }

                if (!decimal.TryParse(txtCurrentWeight.Text, out decimal weight) || weight <= 0)
                {
                    MessageBox.Show("Please enter a valid weight.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCurrentWeight.Focus();
                    return;
                }

                if (cmbWeightCategory.SelectedItem == null)
                {
                    MessageBox.Show("Please select a weight category.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbWeightCategory.Focus();
                    return;
                }

                var selectedPlan = (TrainingPlan)cmbTrainingPlan.SelectedItem;
                var selectedCategory = (WeightCategory)cmbWeightCategory.SelectedItem;

                var athlete = new Athlete
                {
                    AthleteName = txtAthleteName.Text.Trim(),
                    CurrentWeight = weight,
                    PlanId = selectedPlan.PlanId,
                    CompetitionCategoryId = selectedCategory.CategoryId
                };

                if (_athleteId.HasValue)
                {
                    athlete.AthleteId = _athleteId.Value;
                    _dbService.UpdateAthlete(athlete);
                    MessageBox.Show("Athlete updated successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _dbService.InsertAthlete(athlete);
                    MessageBox.Show("Athlete added successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                SaveClicked = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving athlete: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
