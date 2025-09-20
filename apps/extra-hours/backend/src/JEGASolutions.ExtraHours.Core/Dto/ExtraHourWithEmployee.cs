using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Dto
{
    public class ExtraHourWithEmployee
    {
        public ExtraHour ExtraHour { get; set; }
        public Employee Employee { get; set; }

        public ExtraHourWithEmployee(ExtraHour extraHour, Employee employee)
        {
            ExtraHour = extraHour;
            Employee = employee;
        }
    }
}
