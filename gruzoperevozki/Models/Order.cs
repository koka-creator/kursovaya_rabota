using System;
using System.Collections.Generic;

namespace Gruzoperevozki.Models
{
    public enum OrderStatus
    {
        Новый,
        В_обработке,
        Подтвержден,
        В_пути,
        Доставлен,
        Отменен
    }

    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime OrderDate { get; set; }
        public string SenderClientId { get; set; } = string.Empty;
        public string LoadingAddress { get; set; } = string.Empty;
        public string ReceiverClientId { get; set; } = string.Empty;
        public string UnloadingAddress { get; set; } = string.Empty;
        public decimal RouteLength { get; set; }
        public decimal Cost { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Новый;
        public List<CargoItem> CargoItems { get; set; } = new List<CargoItem>();
    }
}


