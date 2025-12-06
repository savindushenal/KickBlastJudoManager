using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KickBlastJudoManager.Services;

namespace KickBlastJudoManager.Pages
{
    public partial class DashboardPage : Page
    {
        private DatabaseService _dbService;

        public DashboardPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            
            // Delay loading data until after page is loaded
            Loaded += DashboardPage_Loaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        public void RefreshData()
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                var athletes = _dbService.GetAllAthletes();
                
                // Update statistics
                txtTotalAthletes.Text = athletes.Count.ToString();
                txtBeginnerCount.Text = athletes.Count(a => a.TrainingPlanName == "Beginner").ToString();
                txtIntermediateCount.Text = athletes.Count(a => a.TrainingPlanName == "Intermediate").ToString();
                txtEliteCount.Text = athletes.Count(a => a.TrainingPlanName == "Elite").ToString();
                
                // Show recent 5 athletes
                dgRecentAthletes.ItemsSource = athletes.Take(5).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddAthlete_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/ManageAthletesPage.xaml", UriKind.Relative));
        }

        private void ViewRecords_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/ViewRecordsPage.xaml", UriKind.Relative));
        }

        private void CalculateCost_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/CalculateCostPage.xaml", UriKind.Relative));
        }
    }
}
