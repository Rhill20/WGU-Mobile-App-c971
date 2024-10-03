using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace College_Tracker
{
    public partial class App : Application
    {
        public static string DatabasePath;

        public App(string dbPath)
        {
            InitializeComponent();
            DatabasePath = dbPath;

            var dbHelper = new Data.DatabaseHelper(DatabasePath);
            Task.Run(() => LoadSampleDataAsync(dbHelper)).Wait();

            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;

            // Set main page
            MainPage = new NavigationPage(new Views.TermsPage());
            
            // Request notification permission
            LocalNotificationCenter.Current.RequestNotificationPermission();
           
        }
        

        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsDismissed) return;

            if (e.IsTapped)
            {
                return;
            }
        }

        private async Task LoadSampleDataAsync(Data.DatabaseHelper dbHelper)
        {
            // Clear existing data
            await dbHelper.ClearAllDataAsync();

            var existingTerms = await dbHelper.GetTermsAsync();
            if (existingTerms.Any()) return;

            // Sample data
            var sampleTerm = new Models.Term
            {
                Title = "Spring 2024",
                StartDate = DateTime.Now.AddDays(+5),
                EndDate = DateTime.Now.AddMonths(+3)
            };
            await dbHelper.SaveTermAsync(sampleTerm);

            var sampleCourse = new Models.Course
            {
                TermId = sampleTerm.TermId,
                Title = "Advanced C# Programming",
                StartDate = sampleTerm.StartDate,
                EndDate = sampleTerm.EndDate,
                Status = "In Progress",
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Notes = "This course covers advanced topics in C#.",
                NotifyStart = false,
                NotifyEnd = true

            };
            await dbHelper.SaveCourseAsync(sampleCourse);

            var sampleAssessment1 = new Models.Assessment
            {
                CourseId = sampleCourse.CourseId,
                Name = "Midterm Exam",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(+2),
                Type = "Performance",
                NotifyStart = true,
                NotifyEnd = true
            };
            await dbHelper.SaveAssessmentAsync(sampleAssessment1);

            var sampleAssessment2 = new Models.Assessment
            {
                CourseId = sampleCourse.CourseId,
                Name = "Final Exam",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddMonths(1),
                Type = "Objective",
                NotifyStart = true,
                NotifyEnd = true
            };
            await dbHelper.SaveAssessmentAsync(sampleAssessment2);
        }
    }
}
