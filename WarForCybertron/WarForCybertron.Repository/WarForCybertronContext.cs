using Microsoft.EntityFrameworkCore;
using System;
using WarForCybertron.Model;

namespace WarForCybertron.Repository
{
    public class WarForCybertronContext : DbContext
    {
        public DbSet<Transformer> Transformers { get; set; }

        public WarForCybertronContext(DbContextOptions<WarForCybertronContext> options) : base(options)
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            ChangeTracker.AutoDetectChangesEnabled = false;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            var sqlPst = "SWITCHOFFSET(SYSDATETIMEOFFSET(), '-08:00')";
            var sqlGuid = "newsequentialid()";

            modelbuilder.Entity<Transformer>().ToTable("Transformers");
            modelbuilder.Entity<Transformer>().Property(x => x.DateCreated).HasDefaultValueSql(sqlPst);
            modelbuilder.Entity<Transformer>().Property(x => x.Id).HasDefaultValueSql(sqlGuid);
        }
    }
}
