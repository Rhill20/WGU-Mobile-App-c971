using Microsoft.Maui.Controls;
using College_Tracker.Models;
using College_Tracker.Data;
using Plugin.LocalNotification;
using System;

namespace College_Tracker.Views
{
    public partial class CourseDetailsPage : ContentPage
    {
        private Course _course;
        private Term _term;
        private DatabaseHelper _databaseHelper;

        public CourseDetailsPage(Term term)
        {
            InitializeComponent();
            _term = term;
            _databaseHelper = new DatabaseHelper(App.DatabasePath);
        }

        public CourseDetailsPage(Course course)
        {
            InitializeComponent();
            _course = course;
            _databaseHelper = new DatabaseHelper(App.DatabasePath);

            if (_course != null)
            {
                TitleEntry.Text = _course.Title;
                StartDatePicker.Date = _course.StartDate;
                EndDatePicker.Date = _course.EndDate;
                StatusPicker.SelectedItem = _course.Status;
                InstructorNameEntry.Text = _course.InstructorName;
                InstructorPhoneEntry.Text = _course.InstructorPhone;
                InstructorEmailEntry.Text = _course.InstructorEmail;
                NotifyStartSwitch.IsToggled = _course.NotifyStart;
                NotifyEndSwitch.IsToggled = _course.NotifyEnd;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return; 
            }

            if (_course == null)
            {
                _course = new Course
                {
                    TermId = _term.TermId
                };
            }

            _course.Title = TitleEntry.Text;
            _course.StartDate = StartDatePicker.Date;
            _course.EndDate = EndDatePicker.Date;
            _course.Status = StatusPicker.SelectedItem?.ToString();
            _course.InstructorName = InstructorNameEntry.Text;
            _course.InstructorPhone = InstructorPhoneEntry.Text;
            _course.InstructorEmail = InstructorEmailEntry.Text;
            _course.NotifyStart = NotifyStartSwitch.IsToggled;
            _course.NotifyEnd = NotifyEndSwitch.IsToggled;

            await _databaseHelper.SaveCourseAsync(_course);

            // Set notifications
            SetNotifications(_course);

            // update the CoursesPage
            if (Navigation.NavigationStack.LastOrDefault() is CoursesPage coursesPage)
            {
                coursesPage.LoadCourses();
            }

            await DisplayAlert("Success", "Course saved successfully!", "OK");
            await Navigation.PopAsync();
        }

        private bool ValidateFields()
        {
            // title check
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                DisplayAlert("Validation Error", "Please enter a course title.", "OK");
                return false;
            }

            // status check
            if (string.IsNullOrWhiteSpace(StatusPicker.SelectedItem?.ToString()))
            {
                DisplayAlert("Validation Error", "Please select a course status.", "OK");
                return false;
            }

            // Name Check
            if (string.IsNullOrWhiteSpace(InstructorNameEntry.Text))
            {
                DisplayAlert("Validation Error", "Please enter the instructor's name.", "OK");
                return false;
            }

            // Phone Check
            if (string.IsNullOrWhiteSpace(InstructorPhoneEntry.Text))
            {
                DisplayAlert("Validation Error", "Please enter the instructor's phone number.", "OK");
                return false;
            }

            // Email Check
            if (string.IsNullOrWhiteSpace(InstructorEmailEntry.Text))
            {
                DisplayAlert("Validation Error", "Please enter the instructor's email address.", "OK");
                return false;
            }

            // Check if start date is before end date
            if (StartDatePicker.Date >= EndDatePicker.Date)
            {
                DisplayAlert("Validation Error", "The start date must be before the end date.", "OK");
                return false;
            }

            return true; // if met good to go
        }

        private void SetNotifications(Course course)
        {
            if (course.NotifyStart)
            {
                var startNotification = new NotificationRequest
                {
                    NotificationId = course.CourseId * 10 + 1,
                    Title = $"Course Start Alert: {course.Title}",
                    Description = $"The course '{course.Title}' starts today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = course.StartDate }
                };
                LocalNotificationCenter.Current.Show(startNotification);
            }

            if (course.NotifyEnd)
            {
                var endNotification = new NotificationRequest
                {
                    NotificationId = course.CourseId * 10 + 2,
                    Title = $"Course End Alert: {course.Title}",
                    Description = $"The course '{course.Title}' ends today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = course.EndDate }
                };
                LocalNotificationCenter.Current.Show(endNotification);
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_course != null)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this course?", "Yes", "No");
                if (confirmDelete)
                {
                    await _databaseHelper.DeleteCourseAsync(_course);

                    // update courses page
                    if (Navigation.NavigationStack.LastOrDefault() is CoursesPage coursesPage)
                    {
                        coursesPage.LoadCourses();
                    }

                    await DisplayAlert("Success", "Course deleted successfully!", "OK");
                    await Navigation.PopAsync();
                }
            }
        }
    }
}
