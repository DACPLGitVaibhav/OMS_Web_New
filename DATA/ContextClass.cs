using DATA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DATA
{
   public class ContextClass:DbContext
    {
        public ContextClass(DbContextOptions<ContextClass> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AutoManualConfg>(
                    eb =>
                    {
                        eb.HasNoKey();
                    });
        }
        public DbSet<Users> users { get; set; }
        public DbSet<LightBill> lightBills { get; set; }
        public DbSet<CustDetails> CustDetails { get; set; }
        public DbSet<ErpOrderDetails> Orders { get; set; }
        public DbSet<LineMaster> linemasters { get; set; }
        public DbSet<ProductionDetails> productions { get; set; }
        public DbSet<VariantCode> variantcode { get; set; }
        public DbSet<PreProductionDetails> PreOrders { get; set; }
        public DbSet<Baretail_Log> tbl_Baretail_Log { get; set; }
        public DbSet<LOT_details> lOT_Details { get; set; }
        public DbSet<AutoManualConfg> autoManualConfgs { get; set; }
        public DbSet<LineStatusMgmtNodes> lineStatusMgmtNodes { get; set;}
    }
}
