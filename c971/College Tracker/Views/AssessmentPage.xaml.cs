using Microsoft.Maui.Controls;
using College_Tracker.Models;
using College_Tracker.Data;
using Plugin.LocalNotification;
using System;
using System.Linq;

namespace College_Tracker.Views
{
    public partial class AssessmentPage : ContentPage
    {
        private Assessment _assessment;
        private Course _course;
        private DatabaseHelper _databaseHelper;

        public AssessmentPage(Course course)
        {
            InitializeComponent();
            _course = course;
            _databaseHelper = new DatabaseHelper(App.DatabasePath);
        }

        public AssessmentPage(Assessment assessment)
        {
            InitializeComponent();
            _assessment = assessment;
            _course = new Course { CourseId = assessment.CourseId };
            _databaseHelper = new DatabaseHelper(App.DatabasePath);

            if (_assessment != null)
            {
                AssessmentNameEntry.Text = _assessment.Name;
                AssessmentStartDatePicker.Date = _assessment.StartDate;
                AssessmentEndDatePicker.Date = _assessment.EndDate;
                AssessmentTypePicker.SelectedItem = _assessment.Type;
                NotifyStartSwitch.IsToggled = _assessment.NotifyStart;
                NotifyEndSwitch.IsToggled = _assessment.NotifyEnd;
            }
        }

        private async void OnSaveAssessmentClicked(object sender, EventArgs e)
        {
            // Validate fields
            if (!ValidateFields())
            {
                return;
            }

            if (_assessment == null)
            {
                _assessment = new Assessment
                {
                    CourseId = _course.CourseId
                };
            }

            _assessment.Name = AssessmentNameEntry.Text;
            _assessment.StartDate = AssessmentStartDatePicker.Date;
            _assessment.EndDate = AssessmentEndDatePicker.Date;
            _assessment.Type = AssessmentTypePicker.SelectedItem?.ToString();
            _assessment.NotifyStart = NotifyStartSwitch.IsToggled;
            _assessment.NotifyEnd = NotifyEndSwitch.IsToggled;

            // Check for duplicate assessment types
            var assessments = await _databaseHelper.GetAssessmentsAsync(_course.CourseId);
            if (assessments.Any(a => a.Type == _assessment.Type && a.AssessmentId != _assessment.AssessmentId))
            {
                await DisplayAlert("Validation Error", $"A {_assessment.Type} assessment is already assigned to this course.", "OK");
                return;
            }

            await _databaseHelper.SaveAssessmentAsync(_assessment);

            // Set notifications for the assessment
            SetNotifications(_assessment);

            await DisplayAlert("Success", "Assessment saved successfully!", "OK");
            await Navigation.PopAsync();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(AssessmentNameEntry.Text))
            {
                DisplayAlert("Validation Error", "Please enter an assessment name.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(AssessmentTypePicker.SelectedItem?.ToString()))
            {
                DisplayAlert("Validation Error", "Please select an assessment type.", "OK");
                return false;
            }

            if (AssessmentStartDatePicker.Date == default || AssessmentEndDatePicker.Date == default)
            {
                DisplayAlert("Validation Error", "Please select a valid start and end date.", "OK");
                return false;
            }

            if (AssessmentStartDatePicker.Date >= AssessmentEndDatePicker.Date)
            {
                DisplayAlert("Validation Error", "The start date must be before the end date.", "OK");
                return false;
            }

            return true;
        }

        private void SetNotifications(Assessment assessment)
        {
            // Check if notifications are enabled for start date
            if (assessment.NotifyStart)
            {
                var startNotification = new NotificationRequest
                {
                    NotificationId = assessment.AssessmentId * 10 + 1,
                    Title = $"Assessment Start Alert: {assessment.Name}",
                    Description = $"The assessment '{assessment.Name}' starts on {assessment.StartDate:MM/dd/yyyy}.",
                    Schedule = new NotificationRequestSchedule { NotifyTime = assessment.StartDate }
                };
                LocalNotificationCenter.Current.Show(startNotification);
            }

            // Check if notifications are enabled for end date
            if (assessment.NotifyEnd)
            {
                var endNotification = new NotificationRequest
                {
                    NotificationId = assessment.AssessmentId * 10 + 2,
                    Title = $"Assessment End Alert: {assessment.Name}",
                    Description = $"The assessment '{assessment.Name}' ends on {assessment.EndDate:MM/dd/yyyy}.",
                    Schedule = new NotificationRequestSchedule { NotifyTime = assessment.EndDate }
                };
                LocalNotificationCenter.Current.Show(endNotification);
            }
        }

        private async void OnDeleteAssessmentClicked(object sender, EventArgs e)
        {
            if (_assessment != null)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this assessment?", "Yes", "No");
                if (confirmDelete)
                {
                    await _databaseHelper.DeleteAssessmentAsync(_assessment);
                    await DisplayAlert("Success", "Assessment deleted successfully!", "OK");
                    await Navigation.PopAsync();
                }
            }
        }
    }
}
