using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Models;
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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Marketing> Marketings { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderItemStatus> OrderItemStatuses { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableSession> TableSessions { get; set; }

    public virtual DbSet<TableStatus> TableStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-K56S2BSD\\SQLEXPRESS;Initial Catalog=TheSocialCebu;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6E0282C14");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4E460836B").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.PersonId).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Person).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__PersonI__412EB0B6");
        });

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.HasKey(e => e.BillingId).HasName("PK__Billing__F1656DF3D96419F2");

            entity.ToTable("Billing");

            entity.Property(e => e.BillingId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceCharge).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SessionId).HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VatAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Session).WithMany(p => p.Billings)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__Session__73BA3083");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B090EC681");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6D969CDF9560");

            entity.Property(e => e.DiscountId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.ApprovedBy).HasMaxLength(50);
            entity.Property(e => e.BillingId).HasMaxLength(50);
            entity.Property(e => e.DiscountTypeId).HasMaxLength(50);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Discounts__Appro__00200768");

            entity.HasOne(d => d.Billing).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discounts__Billi__7E37BEF6");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discounts__Disco__7F2BE32F");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountTypeId).HasName("PK__Discount__6CCE1DB65DC31C08");

            entity.ToTable("DiscountType");

            entity.Property(e => e.DiscountTypeId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.DiscountName).HasMaxLength(100);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD64906914E");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingId).HasMaxLength(50);

            entity.HasOne(d => d.Billing).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__Billin__04E4BC85");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA49751D28779");

            entity.ToTable("Location");

            entity.HasIndex(e => e.LocationName, "UQ__Location__F946BB8410B21986").IsUnique();

            entity.Property(e => e.LocationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.LocationName).HasMaxLength(50);
        });

        modelBuilder.Entity<Marketing>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK__Marketin__7ED91ACFDE1043C8");

            entity.ToTable("Marketing");

            entity.HasIndex(e => e.Email, "UQ__Marketin__A9D105342C0E4BF2").IsUnique();

            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Email).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF04DF5E23");

            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.SessionId).HasMaxLength(50);

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__OrderSta__68487DD7");

            entity.HasOne(d => d.Session).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__SessionI__693CA210");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED068156BA11C1");

            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Instructions).HasMaxLength(100);
            entity.Property(e => e.OrderId).HasMaxLength(50);
            entity.Property(e => e.ProdId).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__6D0D32F4");

            entity.HasOne(d => d.OrderItemStatus).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderItemStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__6E01572D");

            entity.HasOne(d => d.Prod).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__ProdI__6EF57B66");
        });

        modelBuilder.Entity<OrderItemStatus>(entity =>
        {
            entity.HasKey(e => e.OrderItemStatusId).HasName("PK__OrderIte__E0C3AD7210D99CC2");

            entity.ToTable("OrderItemStatus");

            entity.HasIndex(e => e.StatusName, "UQ__OrderIte__05E7698A085056F2").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__BC674CA1D14DA19D");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.StatusName, "UQ__OrderSta__05E7698A3FA881C1").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A38103888D2");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillingId).HasMaxLength(50);
            entity.Property(e => e.PaymentTime).HasColumnType("datetime");

            entity.HasOne(d => d.Billing).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Billing__778AC167");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__AA2FFBE505606B96");

            entity.ToTable("Person");

            entity.Property(e => e.PersonId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.People)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Person__RoleId__3C69FB99");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProdId).HasName("PK__Product__042785E599F30928");

            entity.ToTable("Product");

            entity.Property(e => e.ProdId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Availability).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProdName).HasMaxLength(150);
            entity.Property(e => e.SubcategoryId).HasMaxLength(50);

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Subcate__5EBF139D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A7D29B46D");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.RoleName).HasMaxLength(150);
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PK__SubCateg__9C4E705D3F5744E9");

            entity.ToTable("SubCategory");

            entity.Property(e => e.SubcategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CategoryId).HasMaxLength(50);
            entity.Property(e => e.SubcategoryName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubCatego__Categ__59FA5E80");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Table__7D5F01EEB39FA3AE");

            entity.ToTable("Table");

            entity.HasIndex(e => new { e.TableNumber, e.LocationId }, "UQ_Table_TableNumber_Location").IsUnique();

            entity.Property(e => e.TableId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.LocationId);
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.TableNumber).HasMaxLength(50);
            entity.Property(e => e.TableStatusId).HasDefaultValue(1);

            entity.HasOne(d => d.Location).WithMany(p => p.Tables)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__LocationI__4D94879B");

            entity.HasOne(d => d.TableStatus).WithMany(p => p.Tables)
                .HasForeignKey(d => d.TableStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__TableStat__4E88ABD4");
        });

        modelBuilder.Entity<TableSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__TableSes__C9F492908C19B4F5");

            entity.ToTable("TableSession");

            entity.Property(e => e.SessionId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.EndedAt).HasColumnType("datetime");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TableId).HasMaxLength(50);

            entity.HasOne(d => d.Table).WithMany(p => p.TableSessions)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableSess__Table__534D60F1");
        });

        modelBuilder.Entity<TableStatus>(entity =>
        {
            entity.HasKey(e => e.TableStatusId).HasName("PK__TableSta__2DE378128E5AEF09");

            entity.ToTable("TableStatus");

            entity.HasIndex(e => e.StatusName, "UQ__TableSta__05E7698A59F66328").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
