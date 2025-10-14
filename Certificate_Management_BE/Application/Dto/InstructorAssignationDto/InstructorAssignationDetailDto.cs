namespace Application.Dto.InstructorAssignationDto
{
    /// <summary>
    /// DTO for detailed instructor assignation information
    /// </summary>
    public class InstructorAssignationDetailDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        
        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        
        public string AssignedByUserId { get; set; } = string.Empty;
        public string AssignedByUserName { get; set; } = string.Empty;
        
        public DateTime AssignDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}

