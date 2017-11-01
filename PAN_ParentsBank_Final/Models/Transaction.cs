using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAN_ParentsBank_Final.Models
{
    public class Transaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Recipient Account")]
        public virtual int AccountId { get; set; }
        public virtual Account Account { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Transaction), "TransactionDateValidator")]
        [Display(Name = "Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [CustomValidation(typeof(Transaction), "TransactionAmountValidator")]
        public decimal Amount { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        [Display(Name = "Transaction Type")]
        public string TypeOfTransaction { get; set; }

        public static ValidationResult TransactionDateValidator(DateTime TransactionDate, ValidationContext context)
        {
            if (TransactionDate == null)
            {
                return ValidationResult.Success;
            }
            else if (TransactionDate > DateTime.Now)
            {
                return new ValidationResult("Transaction Date cannot be in the future");
            }
            else if (TransactionDate.Year < DateTime.Now.Year)
            {
                return new ValidationResult("Transaction Entries must be made for the current year");
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public static ValidationResult TransactionAmountValidator(decimal amount, ValidationContext context)
        {
            if (amount <= 0)
            {
                return new ValidationResult("Please enter a transaction amount greater than 0");
            }

            else
            {
                return ValidationResult.Success;
            }
        }
    }
}