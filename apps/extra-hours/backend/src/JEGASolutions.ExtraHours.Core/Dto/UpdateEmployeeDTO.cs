namespace JEGASolutions.ExtraHours.Core.Dto
{
    public class UpdateEmployeeDTO
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal? Salary { get; set; }
        public long? ManagerId { get; set; }
        public string? Role { get; set; }
    }
}