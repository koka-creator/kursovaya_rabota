using System;

namespace Gruzoperevozki.Models
{
    public class Car
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StateNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal LoadCapacity { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public int ManufactureYear { get; set; }
        public int? OverhaulYear { get; set; }
        public decimal MileageAtYearStart { get; set; }
        public string? PhotoPath { get; set; }
    }
}



