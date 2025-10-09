using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class StudyRecordRepository : GenericRepository<StudyRecord>, IStudyRecordRepository
    {
        public StudyRecordRepository(Context context) : base(context)
        {
        }
    }
}


