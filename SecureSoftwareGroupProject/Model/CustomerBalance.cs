namespace SecureSoftwareGroupProject.Models
{
    public class CustomerBalance
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string? Currency { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public decimal CreditLimit { get; set; }
        public string? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

}
