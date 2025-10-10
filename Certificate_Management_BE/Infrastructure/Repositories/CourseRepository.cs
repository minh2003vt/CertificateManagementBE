using Application.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly Context _context;

        public CourseRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllSubjectsByCourseIdAsync(string courseId)
        {
            // Get all subjects connected to this course through CourseSubjectSpecialty
            var subjects = await _context.CourseSubjectSpecialties
                .Where(css => css.CourseId == courseId)
                .Select(css => css.Subject)
                .Distinct()
                .ToListAsync();

            return subjects;
        }
    }
}


