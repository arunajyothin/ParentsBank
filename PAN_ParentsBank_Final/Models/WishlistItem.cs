using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAN_ParentsBank_Final.Models
{
    public class WishListItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //[Display(Name ="Recipient Account")]
        public virtual int AccountId { get; set; }
        public virtual Account Account { get; set; }
        [Display(Name ="Date Added")]
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }
        [Required]
        public string Description { get; set; }
        [Url]
        public string Link { get; set; }
        public bool Purchased { get; set; }
    }
}