using System;

namespace Gruzoperevozki.Models
{
    public class Driver
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public int BirthYear { get; set; }
        public int WorkExperience { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
    }
}




