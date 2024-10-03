using Microsoft.Maui.Controls;
using College_Tracker.Models;
using College_Tracker.Data;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;

namespace College_Tracker.Views
{
    public partial class TermsPage : ContentPage
    {
        private ObservableCollection<Term> _terms;
        private DatabaseHelper _databaseHelper;
        private bool _isSwiping = false; // Track swiping

        public TermsPage()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper(App.DatabasePath);
            _terms = new ObservableCollection<Term>();
            TermsCollectionView.ItemsSource = _terms;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTerms();
        }

        private async void LoadTerms()
        {
            var terms = await _databaseHelper.GetTermsAsync();

            _terms.Clear();
            foreach (var term in terms)
            {
                _terms.Add(term);
            }
        }

        private async void OnAddTermClicked(object sender, EventArgs e)
        {
            var termDetailsPage = new TermDetailsPage();
            await Navigation.PushAsync(termDetailsPage);
        }

        private async void OnTermSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_isSwiping)
            {
                //EDIT swiping now works. Ironed out bugs 
                _isSwiping = false; // Reset swiping
                TermsCollectionView.SelectedItem = null; // Deselect
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is Term selectedTerm)
            {
                var coursesPage = new CoursesPage(selectedTerm);
                await Navigation.PushAsync(coursesPage);
                TermsCollectionView.SelectedItem = null; 
            }
        }

        private void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _isSwiping = true; // User started swiping
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            _isSwiping = false;
        }

        private async void OnEditTermClicked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Term selectedTerm)
            {
                var termDetailsPage = new TermDetailsPage(selectedTerm);
                await Navigation.PushAsync(termDetailsPage);
            }
        }

        private async void OnDeleteTermClicked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Term selectedTerm)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the term '{selectedTerm.Title}'?", "Yes", "No");
                if (confirmDelete)
                {
                    await _databaseHelper.DeleteTermAsync(selectedTerm);
                    LoadTerms();
                }
            }
        }
    }
}
