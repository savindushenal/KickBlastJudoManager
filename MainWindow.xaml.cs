using System;
using System.Windows;
using KickBlastJudoManager.Pages;

namespace KickBlastJudoManager
{
    public partial class MainWindow : Window
    {
        private DashboardPage _dashboardPage;

        public MainWindow()
        {
            InitializeComponent();
            
            // Navigate to Dashboard on startup
            _dashboardPage = new DashboardPage();
            MainFrame.Navigate(_dashboardPage);
        }

        private void NavigateToDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (_dashboardPage == null)
            {
                _dashboardPage = new DashboardPage();
            }
            else
            {
                _dashboardPage.RefreshData();
            }
            MainFrame.Navigate(_dashboardPage);
        }

        private void NavigateToManageAthletes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageAthletesPage());
        }

        private void NavigateToViewRecords_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ViewRecordsPage());
        }

        private void NavigateToTrainingPlans_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageTrainingPlansPage());
        }

        private void NavigateToCompetitionManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CompetitionManagementPage());
        }

        private void NavigateToCalculateCost_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CalculateCostPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", 
                "Exit Application", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
