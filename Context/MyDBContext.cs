using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Models.MenuClasses;
using TheSocialCebu_Capstone.Models.OrderClasses;
using TheSocialCebu_Capstone.Models.TableClasses;
using TheSocialCebu_Capstone.Models.UserClasses;

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

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-K56S2BSD\\SQLEXPRESS;Initial Catalog=TheSocialCebu;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Billing>(entity =>
        {
            entity.HasKey(e => e.BillingId).HasName("PK__Billing__F1656D1348357234");

            entity.ToTable("Billing");

            entity.Property(e => e.BillingId)
                .HasMaxLength(50)
                .HasColumnName("BillingID");
            entity.Property(e => e.DiscountId)
                .HasMaxLength(50)
                .HasColumnName("DiscountID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");

            entity.HasOne(d => d.Discount).WithMany(p => p.Billings)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__Discoun__5D95E53A");

            entity.HasOne(d => d.Order).WithMany(p => p.Billings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__OrderID__5E8A0973");
        });

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

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF629FAE5F5");

            entity.ToTable("Discount");

            entity.Property(e => e.DiscountId)
                .HasMaxLength(50)
                .HasColumnName("DiscountID");
            entity.Property(e => e.Name)
                .HasMaxLength(1)
                .IsUnicode(false);
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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFB38BCE70");

            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");
            entity.Property(e => e.TableId)
                .HasMaxLength(50)
                .HasColumnName("TableID");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__TableID__59C55456");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A1B673F630");

            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("OrderItemID");
            entity.Property(e => e.Instructions).HasMaxLength(255);
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");
            entity.Property(e => e.ProdId)
                .HasMaxLength(50)
                .HasColumnName("Prod_Id");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__625A9A57");

            entity.HasOne(d => d.Prod).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Prod___634EBE90");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC03118505");

            entity.ToTable("Person");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProdId).HasName("PK__Product__C55BDF13B19F2A3B");

            entity.ToTable("Product");

            entity.Property(e => e.ProdId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Prod_Id");
            entity.Property(e => e.Availability).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProdImage).HasColumnName("Prod_Image");
            entity.Property(e => e.ProdName)
                .HasMaxLength(150)
                .HasColumnName("Prod_Name");
            entity.Property(e => e.SubcategoryId)
                .HasMaxLength(50)
                .HasColumnName("Subcategory_Id");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Subcate__7E37BEF6");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A726E8A15");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B616020045A9B").IsUnique();

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasColumnName("RoleID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
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
            entity.HasKey(e => e.Id).HasName("PK__Table__3214EC0746A59926");

            entity.ToTable("Table");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.LocationId)
                .HasMaxLength(50)
                .HasColumnName("Location_Id");
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.TableNumber).HasMaxLength(50);

            entity.HasOne(d => d.Location).WithMany(p => p.Tables)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__Location___5EBF139D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA586A8F57A4C");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E434A7EB9F").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .HasColumnName("AccountID");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasColumnName("RoleID");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__RoleID__787EE5A0");

            entity.HasOne(d => d.Person).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__UserID__797309D9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
