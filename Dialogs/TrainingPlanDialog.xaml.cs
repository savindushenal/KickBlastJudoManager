using System;
using System.Windows;
using KickBlastJudoManager.Models;

namespace KickBlastJudoManager.Dialogs
{
    public partial class TrainingPlanDialog : Window
    {
        public TrainingPlan TrainingPlan { get; private set; }
        private bool _isEditMode;

        public TrainingPlanDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            Title = "Add Training Plan";
        }

        public TrainingPlanDialog(TrainingPlan plan)
        {
            InitializeComponent();
            _isEditMode = true;
            Title = "Edit Training Plan";
            
            TrainingPlan = plan;
            txtPlanName.Text = plan.PlanName;
            txtWeeklyFee.Text = plan.WeeklyFee.ToString("F2");
            chkCanCompete.IsChecked = plan.CanCompete;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtPlanName.Text))
            {
                MessageBox.Show("Please enter a plan name.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPlanName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWeeklyFee.Text) || 
                !decimal.TryParse(txtWeeklyFee.Text, out decimal weeklyFee) || 
                weeklyFee <= 0)
            {
                MessageBox.Show("Please enter a valid weekly fee greater than 0.", 
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtWeeklyFee.Focus();
                return;
            }

            // Create or update training plan
            if (TrainingPlan == null)
            {
                TrainingPlan = new TrainingPlan();
            }

            TrainingPlan.PlanName = txtPlanName.Text.Trim();
            TrainingPlan.WeeklyFee = weeklyFee;
            TrainingPlan.CanCompete = chkCanCompete.IsChecked ?? false;
            TrainingPlan.CanEnterCompetitions = TrainingPlan.CanCompete;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
