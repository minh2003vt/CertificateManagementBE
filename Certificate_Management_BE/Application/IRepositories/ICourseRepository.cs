using Domain.Entities;

namespace Application.IRepositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<List<Subject>> GetAllSubjectsByCourseIdAsync(string courseId);
    }
}


