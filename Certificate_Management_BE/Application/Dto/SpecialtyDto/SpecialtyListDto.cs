namespace Application.Dto.SpecialtyDto
{
    /// <summary>
    /// DTO for listing specialties
    /// </summary>
    public class SpecialtyListDto
    {
        public string SpecialtyId { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

