using SQLite;
using College_Tracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace College_Tracker.Data
{
    public class DatabaseHelper
    {
        private SQLiteAsyncConnection _database;

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Term>().Wait();
            _database.CreateTableAsync<Course>().Wait();
            _database.CreateTableAsync<Assessment>().Wait();
        }

        public Task<List<Term>> GetTermsAsync()
        {
            return _database.Table<Term>().ToListAsync();
        }

        public Task<List<Course>> GetCoursesByTermAsync(int termId)
        {
            // Courses for termID
            return _database.Table<Course>()
                            .Where(c => c.TermId == termId)
                            .ToListAsync();
        }

        public async Task ClearAllDataAsync()
        {
            await _database.DeleteAllAsync<Term>();
            await _database.DeleteAllAsync<Course>();
            await _database.DeleteAllAsync<Assessment>();
        }

        public Task<List<Course>> GetCoursesAsync(int termId)
        {
            return _database.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        }

        public Task<List<Assessment>> GetAssessmentsAsync(int courseId)
        {
            return _database.Table<Assessment>().Where(a => a.CourseId == courseId).ToListAsync();
        }

        public async Task<int> SaveTermAsync(Term term)
        {
            if (term.TermId == 0)
            {             
                return await _database.InsertAsync(term);
            }
            else
            {
                return await _database.UpdateAsync(term);
            }
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            if (course.CourseId == 0)
            {
                return await _database.InsertAsync(course);
            }
            else
            {
                return await _database.UpdateAsync(course);
            }
        }

        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.AssessmentId == 0)
            {
                return await _database.InsertAsync(assessment);
            }
            else
            {
                return await _database.UpdateAsync(assessment);
            }
        }

        public Task<int> DeleteTermAsync(Term term)
        {
            return _database.DeleteAsync(term);
        }

        public Task<int> DeleteCourseAsync(Course course)
        {
            return _database.DeleteAsync(course);
        }

        public Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            return _database.DeleteAsync(assessment);
        }
    }
}
