using Microsoft.EntityFrameworkCore;
using REST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Data
{
    public class DbConnection : DbContext
    {
        public DbConnection(DbContextOptions<DbConnection> options) : base(options)
        {
            Database.SetCommandTimeout(1500000);
        }

        // ***Common CD
        public DbSet<CD_Branch> CD_Branch { get; set; }
        public DbSet<CD_Dish> CD_Dish { get; set; }
        public DbSet<CD_Food> CD_Food { get; set; }
        public DbSet<CD_FoodSub> CD_FoodSub { get; set; }
        public DbSet<CD_GroupFood> CD_GroupFood { get; set; }
        public DbSet<CD_Running> CD_Running { get; set; }
        public DbSet<CD_Staple> CD_Staple { get; set; }
        public DbSet<CD_Zone> CD_Zone { get; set; }
        public DbSet<CD_Table> CD_Table { get; set; }
        public DbSet<CD_Title> CD_Title { get; set; }
        public DbSet<CD_Unit> CD_Unit { get; set; }
        public DbSet<CD_UserLogin> CD_UserLogin { get; set; }

        // ***StoreFront SF
        public DbSet<SF_Order> SF_Order { get; set; }
        public DbSet<SF_OrderSub> SF_OrderSub { get; set; }

        // ***Stock ST
        public DbSet<ST_Trans> ST_Trans { get; set; }
        public DbSet<ST_TranSub> ST_TranSub { get; set; }
        public DbSet<ST_TranManual> ST_TranManual { get; set; }

        // ***Member MB
        public DbSet<MB_Member> MB_Member { get; set; }

        // ***Setting pramary key
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ***ทะเบียน CD
            modelBuilder.Entity<CD_Branch>().HasKey(x => new { x.BranchId });
            modelBuilder.Entity<CD_Dish>().HasKey(x => new { x.DishId, x.BranchId });
            modelBuilder.Entity<CD_Food>().HasKey(x => new { x.FoodId, x.BranchId });
            modelBuilder.Entity<CD_FoodSub>().HasKey(x => new { x.FoodId, x.StapleId, x.BranchId });
            modelBuilder.Entity<CD_GroupFood>().HasKey(x => new { x.GroupFoodId, x.BranchId });
            modelBuilder.Entity<CD_Running>().HasKey(x => new { x.Name, x.BranchId });
            modelBuilder.Entity<CD_Staple>().HasKey(x => new { x.StapleId, x.BranchId });
            modelBuilder.Entity<CD_Zone>().HasKey(x => new { x.ZoneId, x.BranchId });
            modelBuilder.Entity<CD_Table>().HasKey(x => new { x.TableId, x.BranchId });
            modelBuilder.Entity<CD_Title>().HasKey(x => new { x.TitleId, x.BranchId });
            modelBuilder.Entity<CD_Unit>().HasKey(x => new { x.UnitId, x.BranchId });
            modelBuilder.Entity<CD_UserLogin>().HasKey(x => new { x.UserId, x.BranchId });

            // ***StoreFront SF
            modelBuilder.Entity<SF_Order>().HasKey(x => new { x.OrderId, x.BranchId });
            modelBuilder.Entity<SF_OrderSub>().HasKey(x => new { x.OrderId, x.TableId, x.FoodId, x.BranchId });

            // ***Stock ST
            modelBuilder.Entity<ST_Trans>().HasKey(x => new { x.Documents, x.BranchId });
            modelBuilder.Entity<ST_TranSub>().HasKey(x => new { x.Documents, x.StapleId, x.BranchId });
            modelBuilder.Entity<ST_TranManual>().HasKey(x => new { x.i, x.StapleId, x.BranchId });

            // ***Member MB
            modelBuilder.Entity<MB_Member>().HasKey(x => new { x.MemberId, x.BranchId });
        }
    }
}
