using Application.Dto.TraineeAssignationDto;
using Application.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ITraineeAssignationService
    {
        Task<ServiceResponse<TraineeAssignationDetailDto>> CreateAsync(CreateTraineeAssignationDto dto, string assignedByUserId);
        Task<ServiceResponse<TraineeAssignationDetailDto>> GetByIdAsync(string traineeAssignationId);
        Task<ServiceResponse<List<TraineeAssignationListDto>>> GetByClassAsync(int classId);
        Task<ServiceResponse<TraineeAssignationDetailDto>> UpdateAsync(string traineeAssignationId, UpdateTraineeAssignationDto dto);
        Task<ServiceResponse<string>> DeleteAsync(string traineeAssignationId);
    }
}


