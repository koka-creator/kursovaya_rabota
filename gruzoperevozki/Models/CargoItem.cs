namespace Gruzoperevozki.Models
{
    public class CargoItem
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal InsuranceValue { get; set; }
    }
}


