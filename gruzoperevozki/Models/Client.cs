using System;

namespace Gruzoperevozki.Models
{
    public enum ClientType
    {
        Individual,
        LegalEntity
    }

    public class Client
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ClientType Type { get; set; }
        
        // Общие поля
        public string Phone { get; set; } = string.Empty;
        
        // Для физических лиц
        public string? FullName { get; set; }
        public string? PassportSeries { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public string? PassportIssuedBy { get; set; }
        
        // Для юридических лиц
        public string? CompanyName { get; set; }
        public string? DirectorName { get; set; }
        public string? LegalAddress { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? TaxId { get; set; }
    }
}


