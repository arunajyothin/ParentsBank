using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAN_ParentsBank_Final.Models
{
    [CustomValidation(typeof(Account), "OwnerAndRecipientUniqueValidator")]
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Owner { get; set; }
        [Required]
        [EmailAddress]
        public string Recipient { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z \ \' \.]{3,50}?$", ErrorMessage = "Please enter a valid name")]
        public string Name { get; set; }
        [ReadOnly(true)]
        public DateTime OpenDate { get; set; }
        [Required]
        [Display(Name = "Interest Rate")]
        [CustomValidation(typeof(Account), "InterestRateValidator")]
        public decimal InterestRate { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishListItem> WishListItems { get; set; }

        // Get the current account balance without interest
        public decimal CurrentBalanceWithoutInterest()
        {
            // sum all the total of all of the transactions
            decimal total = Transactions.Sum(x => x.Amount);
            return total;
        }


        // this method calculates the YTD interest earned
        public decimal YearToDateInterestEarned()
        {
            decimal startBalance = 0;
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = DateTime.Now;
            List<CalculateInterest> calc = new List<CalculateInterest>();
            foreach(var t in Transactions)
            {
                calc.Add(new CalculateInterest(t.TransactionDate, t.Amount));
            }
            Calculator obj = new Calculator();
            decimal interest = obj.CalcInt(startDate, endDate, this.InterestRate, startBalance, calc);

            return Math.Round(interest, 2);

        }

        public static ValidationResult OwnerAndRecipientUniqueValidator(Account account, ValidationContext context)
        {
            if (account == null)
            {
                return ValidationResult.Success;
            }
            else if (account.Owner.ToUpper() == account.Recipient.ToUpper())
            {
                return new ValidationResult("Owner cannot be the same as recipient");
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public decimal getTotal()
        {
            return getAccountBalance() + YearToDateInterestEarned();
        }

        public decimal getAccountBalance()
        {
            decimal debits = 0;
            decimal credits = 0;
            decimal transactionBalance = 0;
            if (Transactions != null)
            {
                foreach (Transaction transaction in Transactions)
                {
                    if (transaction.TypeOfTransaction == "Debit")
                    {
                        debits += transaction.Amount;
                    }

                    if (transaction.TypeOfTransaction == "Credit")
                    {
                        credits += transaction.Amount;
                    }
                }
                transactionBalance = credits - debits;
            }

            return transactionBalance;
        }

        public bool IsOwner(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE OWNER
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return false;
            }

            return currentUser.ToUpper() == Owner.ToUpper();

        }

        public bool IsRecipient(string currentUser)
        {
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return false;
            }

            return currentUser.ToUpper() == Recipient.ToUpper();
        }

        public bool IsOwnerOrRecipient(string currentUser)
        {
            return IsOwner(currentUser) || IsRecipient(currentUser);
        }


        public string getDateOfLastDeposit()
        {
            List<DateTime> dateOfTransactions = new List<DateTime>();
            if (Transactions != null && Transactions.Count > 0)
            {
                foreach (Transaction transaction in Transactions)
                {
                    if (transaction.TypeOfTransaction == "Credit")
                    {
                        dateOfTransactions.Add(transaction.TransactionDate);
                    }
                }

                if (dateOfTransactions == null || !(dateOfTransactions.Count > 0))
                    return null;
                else
                    return String.Format("{0:MM/dd/yyyy}", dateOfTransactions.Max());
            }

            return "";

        }
        public static ValidationResult InterestRateValidator(decimal interestRate, ValidationContext context)
        {

            if (interestRate <= 0 || interestRate >= 100)
            {
                return new ValidationResult("Please enter an interest rate between 0 and 100%");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}

public class CalculateInterest
{
    public DateTime date;
    public decimal amount;

    public CalculateInterest(DateTime date, decimal amount)
    {
        this.date = date;
        this.amount = amount;
    }
}

class Calculator
{
    public decimal CalcInt(DateTime startDate, DateTime endDate, decimal intRate, decimal startingBalance, List<CalculateInterest> transactionsList)
    {      

        decimal total = 0,finalInterest=0;
        TimeSpan duration = endDate - startDate;
        int durationInDays = duration.Days;
        int totalDaysInAYear = (startDate.AddYears(1) - startDate).Days; 
        decimal timePeriod = 1 / (decimal)totalDaysInAYear;

        Dictionary<DateTime, decimal> transactions = transactionsList.Where(a => (a.date >= startDate && a.date <= endDate)).GroupBy(a => a.date).Select(a => new { Date = a.Key, Total = a.Select(t => t.amount).Sum() }).ToDictionary(a => a.Date, a => a.Total);

        foreach (DateTime day in EveryDaysTrans(startDate, endDate))
        {
            decimal amount = 0;

            if (transactions.ContainsKey(day))            
                amount = transactions[day];            

            total += amount;
            finalInterest =Convert.ToDecimal(CompoundInterestFormula(total, intRate, 12, timePeriod));
            
        }
        return (finalInterest- total);
    }

    public double CompoundInterestFormula(decimal principal,decimal interestRate,int compoundingTimes,decimal timePeriod)
    {
        double plain = (double)(1 + (interestRate / compoundingTimes));
        double exponent = (double)(compoundingTimes * (timePeriod));

        return (double)principal * Math.Pow(plain, exponent);
    }

    private IEnumerable<DateTime> EveryDaysTrans(DateTime from, DateTime to)
    {
        for (var days = from.Date; days.Date <= to.Date; days = days.AddDays(1))
            yield return days;
    }
}