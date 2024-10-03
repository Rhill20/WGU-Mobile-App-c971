using SQLite;

namespace College_Tracker.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentId { get; set; }
        public int CourseId { get; set; } // Foreign Key to Course
        public string Name { get; set; }
        public DateTime StartDate { get; set; } // New start date
        public DateTime EndDate { get; set; }   // New end date
        public string Type { get; set; } // PA or OA

        // notifications
        public bool NotifyStart { get; set; } 
        public bool NotifyEnd { get; set; }
    }
}
