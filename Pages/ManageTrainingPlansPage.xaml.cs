using System;
using System.Windows;
using System.Windows.Controls;
using KickBlastJudoManager.Dialogs;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Pages
{
    public partial class ManageTrainingPlansPage : Page
    {
        private readonly DatabaseService _dbService;

        public ManageTrainingPlansPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            LoadTrainingPlans();
        }

        private void LoadTrainingPlans()
        {
            try
            {
                dgTrainingPlans.ItemsSource = _dbService.GetAllTrainingPlans();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading training plans: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTrainingPlan_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TrainingPlanDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _dbService.InsertTrainingPlan(dialog.TrainingPlan);
                    MessageBox.Show("Training plan added successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadTrainingPlans();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding training plan: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditPlan_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var plan = button?.Tag as Models.TrainingPlan;

            if (plan != null)
            {
                var dialog = new TrainingPlanDialog(plan);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        _dbService.UpdateTrainingPlan(dialog.TrainingPlan);
                        MessageBox.Show("Training plan updated successfully!", 
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadTrainingPlans();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating training plan: {ex.Message}", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DeletePlan_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var plan = button?.Tag as Models.TrainingPlan;

            if (plan != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the training plan '{plan.PlanName}'?\\n\\nNote: This will fail if any athletes are currently using this plan.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.DeleteTrainingPlan(plan.PlanId);
                        MessageBox.Show("Training plan deleted successfully!", 
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadTrainingPlans();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
