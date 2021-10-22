using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TelegramBot.Data
{
    public partial class KadrsContext : DbContext
    {
        public KadrsContext()
        {
        }

        public KadrsContext(DbContextOptions<KadrsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Raspisanie> Raspisanies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-4312I2K;Database=Kadrs;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Raspisanie>(entity =>
            {
                entity.ToTable("Raspisanie");

                entity.Property(e => e.OneGruppa).HasColumnName("One_gruppa");

                entity.Property(e => e.TwoGruppa).HasColumnName("Two_gruppa");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
