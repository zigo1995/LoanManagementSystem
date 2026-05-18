using System;

namespace LoanManagementSystem.Application.DTOs
{
    public class CreateCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }

    public class UpdateCustomerDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public int? CreditScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}