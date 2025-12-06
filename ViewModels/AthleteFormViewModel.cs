using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KickBlastJudoManager.Models;

namespace KickBlastJudoManager.ViewModels
{
    public class AthleteFormViewModel : INotifyPropertyChanged
    {
        private string _athleteName;
        private TrainingPlan _selectedPlan;
        private decimal _currentWeight;
        private WeightCategory _selectedCategory;
        private int _competitions;
        private decimal _privateHours;
        private decimal _monthlyCost;
        private bool _isEditMode;
        private int? _editingAthleteId;

        public string AthleteName
        {
            get => _athleteName;
            set { _athleteName = value; OnPropertyChanged(); }
        }

        public TrainingPlan SelectedPlan
        {
            get => _selectedPlan;
            set { _selectedPlan = value; OnPropertyChanged(); }
        }

        public decimal CurrentWeight
        {
            get => _currentWeight;
            set { _currentWeight = value; OnPropertyChanged(); }
        }

        public WeightCategory SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public int Competitions
        {
            get => _competitions;
            set { _competitions = value; OnPropertyChanged(); }
        }

        public decimal PrivateHours
        {
            get => _privateHours;
            set { _privateHours = value; OnPropertyChanged(); }
        }

        public decimal MonthlyCost
        {
            get => _monthlyCost;
            set { _monthlyCost = value; OnPropertyChanged(); }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set { _isEditMode = value; OnPropertyChanged(); }
        }

        public int? EditingAthleteId
        {
            get => _editingAthleteId;
            set { _editingAthleteId = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Clear()
        {
            AthleteName = string.Empty;
            SelectedPlan = null;
            CurrentWeight = 0;
            SelectedCategory = null;
            Competitions = 0;
            PrivateHours = 0;
            MonthlyCost = 0;
            IsEditMode = false;
            EditingAthleteId = null;
        }

        public Models.Athlete ToAthlete()
        {
            return new Models.Athlete
            {
                AthleteId = EditingAthleteId ?? 0,
                AthleteName = AthleteName,
                CurrentWeight = CurrentWeight,
                PlanId = SelectedPlan?.PlanId ?? 0,
                CompetitionCategoryId = SelectedCategory?.CategoryId ?? 0,
                TrainingPlanName = SelectedPlan?.Name,
                WeightCategoryName = SelectedCategory?.Name,
                MonthlyCost = MonthlyCost
            };
        }

        public void LoadFromAthlete(Models.Athlete athlete)
        {
            EditingAthleteId = athlete.AthleteId;
            AthleteName = athlete.AthleteName;
            CurrentWeight = athlete.CurrentWeight;
            Competitions = athlete.TotalCompetitions;
            PrivateHours = athlete.TotalCoachingHours;
            MonthlyCost = athlete.MonthlyCost;
            IsEditMode = true;
        }
    }
}
