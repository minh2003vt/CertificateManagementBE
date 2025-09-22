using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CourseSubjectSpecialtyRepository : GenericRepository<CourseSubjectSpecialty>, ICourseSubjectSpecialtyRepository
    {
        public CourseSubjectSpecialtyRepository(Context context) : base(context)
        {
        }
    }
}


