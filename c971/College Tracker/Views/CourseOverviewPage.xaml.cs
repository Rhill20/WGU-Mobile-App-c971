using Microsoft.Maui.Controls;
using College_Tracker.Models;
using College_Tracker.Data;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;
using System;

namespace College_Tracker.Views
{
    public partial class CourseOverviewPage : ContentPage
    {
        private Course _course;
        private DatabaseHelper _databaseHelper;
        private ObservableCollection<Assessment> _assessments;
        private bool _isSwiping = false;

        public CourseOverviewPage(Course course)
        {
            InitializeComponent();
            _course = course;
            _databaseHelper = new DatabaseHelper(App.DatabasePath);
            _assessments = new ObservableCollection<Assessment>();
            AssessmentsCollectionView.ItemsSource = _assessments;
            LoadAssessments();
            BindingContext = this;
        }

        public Course Course => _course;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAssessments(); // reload assessments
        }

        private async void LoadAssessments()
        {
            var assessments = await _databaseHelper.GetAssessmentsAsync(_course.CourseId);
            _assessments.Clear();
            foreach (var assessment in assessments)
            {
                _assessments.Add(assessment);
            }
        }

        private async void OnSaveNotesClicked(object sender, EventArgs e)
        {
            _course.Notes = NotesEditor.Text;
            await _databaseHelper.SaveCourseAsync(_course);
            await DisplayAlert("Success", "Notes saved successfully!", "OK");
        }

        private async void OnShareNotesClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NotesEditor.Text))
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = NotesEditor.Text,
                    Title = "Share Course Notes"
                });
            }
        }

        private async void OnAddAssessmentClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssessmentPage(_course));
        }

        private async void OnEditAssessmentSwipe(object sender, EventArgs e)
        {
            if (_isSwiping) return;

            if (((SwipeItem)sender).CommandParameter is Assessment assessmentToEdit)
            {
                await Navigation.PushAsync(new AssessmentPage(assessmentToEdit));
            }
        }

        private async void OnDeleteAssessmentSwipe(object sender, EventArgs e)
        {
            if (_isSwiping) return;

            if (((SwipeItem)sender).CommandParameter is Assessment assessmentToDelete)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete {assessmentToDelete.Name}?", "Yes", "No");
                if (confirmDelete)
                {
                    await _databaseHelper.DeleteAssessmentAsync(assessmentToDelete);
                    _assessments.Remove(assessmentToDelete);
                    await DisplayAlert("Deleted", $"{assessmentToDelete.Name} has been deleted.", "OK");
                }
            }
        }

        private void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _isSwiping = true; 
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            _isSwiping = false;
        }
    }
}
