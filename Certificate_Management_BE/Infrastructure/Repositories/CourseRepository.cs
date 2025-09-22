using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(Context context) : base(context)
        {
        }
    }
}


