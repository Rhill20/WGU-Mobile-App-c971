using Microsoft.Maui.Controls;
using College_Tracker.Models;
using College_Tracker.Data;
using Plugin.LocalNotification;

namespace College_Tracker.Views

{
    public partial class TermDetailsPage : ContentPage
    {
        private Term _term;
        private DatabaseHelper _databaseHelper;

        public TermDetailsPage()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper(App.DatabasePath);
        }

        public TermDetailsPage(Term term)
        {
            InitializeComponent();
            _term = term;
            _databaseHelper = new DatabaseHelper(App.DatabasePath);

            if (_term != null)
            {
                TitleEntry.Text = _term.Title;
                StartDatePicker.Date = _term.StartDate;
                EndDatePicker.Date = _term.EndDate;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Check if term title is empty
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter a term title.", "OK");
                return;
            }

            // Check if start date is after end date
            if (StartDatePicker.Date > EndDatePicker.Date)
            {
                await DisplayAlert("Validation Error", "The start date cannot be after the end date.", "OK");
                return;
            }

            if (_term == null)
            {
                _term = new Term();
            }

            // Set term values
            _term.Title = TitleEntry.Text;
            _term.StartDate = StartDatePicker.Date;
            _term.EndDate = EndDatePicker.Date;

            // Save the term to the database
            await _databaseHelper.SaveTermAsync(_term);

            await DisplayAlert("Success", "Term saved successfully!", "OK");
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_term != null)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this term?", "Yes", "No");
                if (confirmDelete)
                {
                    await _databaseHelper.DeleteTermAsync(_term);
                    await DisplayAlert("Success", "Term deleted successfully!", "OK");
                    await Navigation.PopAsync();
                }
            }
        }
    }
}
