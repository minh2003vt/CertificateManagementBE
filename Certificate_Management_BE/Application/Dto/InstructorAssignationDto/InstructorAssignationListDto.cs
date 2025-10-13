namespace Application.Dto.InstructorAssignationDto
{
    /// <summary>
    /// DTO for listing instructor assignations
    /// </summary>
    public class InstructorAssignationListDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string AssignedByUserName { get; set; } = string.Empty;
        public DateTime AssignDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}

