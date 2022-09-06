using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LuckyDraw.Models
{
    public partial class LuckyDrawContext : DbContext
    {
        public LuckyDrawContext(DbContextOptions<LuckyDrawContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Campaign> Campaigns { get; set; } = null!;
        public virtual DbSet<Cm> Cms { get; set; } = null!;
        public virtual DbSet<Code> Codes { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Gift> Gifts { get; set; } = null!;
        public virtual DbSet<GiftCode> GiftCodes { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.ToTable("Campaign");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("endTime");

                entity.Property(e => e.Limit).HasColumnName("limit");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("startTime");
            });

            modelBuilder.Entity<Cm>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.ToTable("CMS");

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.Property(e => e.AcceptanceTime)
                    .HasColumnType("datetime")
                    .HasColumnName("acceptanceTime");

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<Code>(entity =>
            {
                entity.ToTable("Code");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code1)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.IdCampaign).HasColumnName("idCampaign");

                entity.Property(e => e.IdCustomer).HasColumnName("idCustomer");

                entity.HasOne(d => d.IdCampaignNavigation)
                    .WithMany(p => p.Codes)
                    .HasForeignKey(d => d.IdCampaign)
                    .HasConstraintName("FK__Code__idCampaign__2B3F6F97");

                entity.HasOne(d => d.IdCustomerNavigation)
                    .WithMany(p => p.Codes)
                    .HasForeignKey(d => d.IdCustomer)
                    .HasConstraintName("FK__Code__idCustomer__2C3393D0");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.Phone, "UQ__Customer__B43B145FD29EC43A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcceptanceTime)
                    .HasColumnType("datetime")
                    .HasColumnName("acceptanceTime");

                entity.Property(e => e.Birth)
                    .HasColumnType("date")
                    .HasColumnName("birth");

                entity.Property(e => e.IsBlock)
                    .HasColumnName("isBlock")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(32)
                    .HasColumnName("password")
                    .IsFixedLength();

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Position)
                    .HasMaxLength(10)
                    .HasColumnName("position");

                entity.Property(e => e.TypeBusiness)
                    .HasMaxLength(20)
                    .HasColumnName("type_business");
            });

            modelBuilder.Entity<Gift>(entity =>
            {
                entity.ToTable("Gift");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.IdCampaign).HasColumnName("idCampaign");

                entity.Property(e => e.IdProduct).HasColumnName("idProduct");

                entity.HasOne(d => d.IdCampaignNavigation)
                    .WithMany(p => p.Gifts)
                    .HasForeignKey(d => d.IdCampaign)
                    .HasConstraintName("FK__Gift__idCampaign__31EC6D26");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.Gifts)
                    .HasForeignKey(d => d.IdProduct)
                    .HasConstraintName("FK__Gift__idProduct__32E0915F");
            });

            modelBuilder.Entity<GiftCode>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GiftCode");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.IdGift).HasColumnName("idGift");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdGiftNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdGift)
                    .HasConstraintName("FK__GiftCode__idGift__35BCFE0A");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasIndex(e => e.Name, "UQ__Product__72E12F1B66115CF4")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
