using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CM360.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public string? OwnerID { get; set; }

        [Required]
        public string? Name { get; set; }

        [Display(Name = "Employee ID")]
        [DataType(DataType.Text)]
        

        [Required] 
        public int? EmployeeNumber { get; set; }

        [Display(Name = "Expense Type")]
        public ExpenseCategory? ExpenseType { get; set; }

        [DataType(DataType.Currency)]
        [Required] 
        public decimal? Amount { get; set; }
    
        
        [DataType(DataType.EmailAddress)]

        [Required] 
        public string? Email { get; set; }
        public ExpenseStatus Status { get; set; }
    }
    public enum ExpenseStatus
    {
        Submitted,
        Approved,
        Rejected
    }

    public enum ExpenseCategory
    {
        InternetBill,
        Relocation,
        Travel,
        TeamLunch,
        Certification
    }

}

