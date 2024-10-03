using Microsoft.Maui.Controls;
using College_Tracker.Models;
using College_Tracker.Data;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace College_Tracker.Views
{
    public partial class CoursesPage : ContentPage
    {
        private ObservableCollection<Course> _courses;
        private DatabaseHelper _databaseHelper;
        private Term _term;
        private bool _isSwiping = false; 

        public CoursesPage(Term term)
        {
            InitializeComponent();
            _term = term;
            _databaseHelper = new DatabaseHelper(App.DatabasePath);
            _courses = new ObservableCollection<Course>();
            CoursesCollectionView.ItemsSource = _courses;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCourses();
        }
        private void OnCourseChanged(Course course)
        {
            LoadCourses();
        }

        public async void LoadCourses()
        {
            var courses = await _databaseHelper.GetCoursesByTermAsync(_term.TermId);
            _courses.Clear();
            foreach (var course in courses)
            {
                _courses.Add(course);
            }
        }
        //same logic as term page for swiping 
        private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_isSwiping)
            {
                
                _isSwiping = false; 
                CoursesCollectionView.SelectedItem = null; 
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is Course selectedCourse)
            {
                var courseOverviewPage = new CourseOverviewPage(selectedCourse);
                await Navigation.PushAsync(courseOverviewPage);
                CoursesCollectionView.SelectedItem = null; 
            }
        }

        private void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _isSwiping = true; 
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            _isSwiping = false; // Reset swiping
        }

        private async void OnEditCourseSwipe(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Course courseToEdit)
            {
                var courseDetailsPage = new CourseDetailsPage(courseToEdit);
                await Navigation.PushAsync(courseDetailsPage);
            }
        }

        private async void OnDeleteCourseSwipe(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Course courseToDelete)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete {courseToDelete.Title}?", "Yes", "No");
                if (confirmDelete)
                {
                    await _databaseHelper.DeleteCourseAsync(courseToDelete);
                    _courses.Remove(courseToDelete);
                    await DisplayAlert("Deleted", $"{courseToDelete.Title} has been deleted.", "OK");
                }
            }
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            if (_courses.Count >= 6)
            {
                await DisplayAlert("Limit Reached", "You cannot add more than 6 courses to a term.", "OK");
                return;
            }

            var courseDetailsPage = new CourseDetailsPage(_term);
            await Navigation.PushAsync(courseDetailsPage);
        }
    }
}
