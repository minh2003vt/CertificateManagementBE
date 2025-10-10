namespace Application.Dto.SpecialtyDto
{
    /// <summary>
    /// DTO for detailed specialty information
    /// </summary>
    public class SpecialtyDetailDto
    {
        public string SpecialtyId { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

