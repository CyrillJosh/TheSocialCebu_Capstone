using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.Context;

public partial class MyDBContext : DbContext
{
    public MyDBContext()
    {
    }

    public MyDBContext(DbContextOptions<MyDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-K56S2BSD\\SQLEXPRESS;Initial Catalog=TheSocialCebu;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__6DB38D6EA29E2F2B");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Category_Id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("Category_Name");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__D2BA00E21F33D960");

            entity.ToTable("Location");

            entity.Property(e => e.LocationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Location_Id");
            entity.Property(e => e.LocationName)
                .HasMaxLength(100)
                .HasColumnName("Location_Name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProdId).HasName("PK__Product__C55BDF13781104AE");

            entity.ToTable("Product");

            entity.Property(e => e.ProdId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Prod_Id");
            entity.Property(e => e.Availability).HasDefaultValue(true);
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasColumnName("Category_Id");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProdImage).HasColumnName("Prod_Image");
            entity.Property(e => e.ProdName)
                .HasMaxLength(150)
                .HasColumnName("Prod_Name");
            entity.Property(e => e.SubcategoryId)
                .HasMaxLength(50)
                .HasColumnName("Subcategory_Id");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Categor__534D60F1");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Subcate__5441852A");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PK__SubCateg__B599509CFD304A5B");

            entity.ToTable("SubCategory");

            entity.Property(e => e.SubcategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Subcategory_Id");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasColumnName("Category_Id");
            entity.Property(e => e.SubcategoryName)
                .HasMaxLength(100)
                .HasColumnName("Subcategory_Name");

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubCatego__Categ__4222D4EF");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Table__3214EC072675F88F");

            entity.ToTable("Table");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.LocationId)
                .HasMaxLength(50)
                .HasColumnName("Location_Id");
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.TableNumber).HasMaxLength(50);

            entity.HasOne(d => d.Location).WithMany(p => p.Tables)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__Location___4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
