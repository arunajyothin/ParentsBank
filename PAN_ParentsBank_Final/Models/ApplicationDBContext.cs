using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PAN_ParentsBank_Final.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<PAN_ParentsBank_Final.Models.Account> Accounts { get; set; }

        public System.Data.Entity.DbSet<PAN_ParentsBank_Final.Models.WishListItem> WishListItems { get; set; }

        public System.Data.Entity.DbSet<PAN_ParentsBank_Final.Models.Transaction> Transactions { get; set; }
    }
}