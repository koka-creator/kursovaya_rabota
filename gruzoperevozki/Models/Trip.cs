using System;
using System.Collections.Generic;

namespace Gruzoperevozki.Models
{
    public enum TripStatus
    {
        Запланирован,
        В_пути,
        На_погрузке,
        В_дороге,
        На_разгрузке,
        Завершен,
        Отменен
    }

    public class Trip
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrderId { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public List<string> DriverIds { get; set; } = new List<string>();
        public DateTime ArrivalDateTime { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Запланирован;
    }
}


