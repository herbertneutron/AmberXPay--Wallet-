using System;
using Microsoft.EntityFrameworkCore;

namespace AmberXPay.Models
{
    public class AmberXPayContext : DbContext
    {
        public AmberXPayContext(DbContextOptions<AmberXPayContext> options)
            : base(options)
        {

        }

        public virtual DbSet<AmberXPayModel> Axp_User_Details { get; set; }
        public virtual DbSet<WalletModel> Axp_Wallet { get; set; }





    }
}
