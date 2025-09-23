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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<BillingOrder> BillingOrders { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

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

    public virtual DbSet<TableStatus> TableStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-K56S2BSD\\SQLEXPRESS;Initial Catalog=TheSocialCebu;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6D98DE19D");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4D610957D").IsUnique();

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
            entity.HasKey(e => e.BillingId).HasName("PK__Billing__F1656DF39B2CC6E2");

            entity.ToTable("Billing");

            entity.Property(e => e.BillingId).HasMaxLength(50);
            entity.Property(e => e.BillingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceCharge).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TableId).HasMaxLength(50);
            entity.Property(e => e.VatAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DiscountId).HasMaxLength(50);

            entity.HasOne(d => d.Table).WithMany(p => p.Billings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__TableId__24285DB4");
            entity.HasOne(d => d.Discount).WithMany(p => p.Billings)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Billing__Discoun__6F7F8B4B");
        });

        modelBuilder.Entity<BillingOrder>(entity =>
        {
            entity.HasKey(e => e.BillingOrderId).HasName("PK__BillingO__48F8147A1352F71B");

            entity.ToTable("BillingOrder");

            entity.Property(e => e.BillingOrderId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingId).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasMaxLength(50);

            entity.HasOne(d => d.Billing).WithMany(p => p.BillingOrders)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillingOr__Billi__2EA5EC27");

            entity.HasOne(d => d.Order).WithMany(p => p.BillingOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillingOr__Order__2F9A1060");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B76D2CCE9");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountTypeId).HasName("PK__Discount__6CCE1DB62331B29B");

            entity.ToTable("DiscountType");

            entity.Property(e => e.DiscountTypeId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.DiscountName).HasMaxLength(100);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD62696CE5D");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingId).HasMaxLength(50);

            entity.HasOne(d => d.Billing).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__Billin__28ED12D1");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA497B76CBE94");

            entity.ToTable("Location");

            entity.HasIndex(e => e.LocationName, "UQ__Location__F946BB8495F126D8").IsUnique();

            entity.Property(e => e.LocationName).HasMaxLength(50);
        });

        modelBuilder.Entity<Marketing>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK__Marketin__7ED91ACFCA671819");

            entity.ToTable("Marketing");

            entity.HasIndex(e => e.Email, "UQ__Marketin__A9D105341E44969A").IsUnique();

            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Email).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF0ECE8C00");

            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.TableId).HasMaxLength(50);

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__OrderSta__367C1819");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__TableId__37703C52");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681DEB3E427");

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
                .HasConstraintName("FK__OrderItem__Order__3B40CD36");

            entity.HasOne(d => d.OrderItemStatus).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderItemStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__3C34F16F");

            entity.HasOne(d => d.Prod).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__ProdI__3D2915A8");
        });

        modelBuilder.Entity<OrderItemStatus>(entity =>
        {
            entity.HasKey(e => e.OrderItemStatusId).HasName("PK__OrderIte__E0C3AD724584C71F");

            entity.ToTable("OrderItemStatus");

            entity.HasIndex(e => e.StatusName, "UQ__OrderIte__05E7698AAE738593").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__BC674CA1A28F9DAB");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.StatusName, "UQ__OrderSta__05E7698A6E9DC324").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A3871161407");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasMaxLength(50);
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentTime).HasColumnType("datetime");

            entity.HasOne(d => d.PaymentNavigation).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Payment__50FB042B");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__AA2FFBE555E7EE81");

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
            entity.HasKey(e => e.ProdId).HasName("PK__Product__042785E5BB663E63");

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
                .HasConstraintName("FK__Product__Subcate__634EBE90");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AA8719A15");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.RoleName).HasMaxLength(150);
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PK__SubCateg__9C4E705DB40BC5A5");

            entity.ToTable("SubCategory");

            entity.Property(e => e.SubcategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CategoryId).HasMaxLength(50);
            entity.Property(e => e.SubcategoryName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubCatego__Categ__59063A47");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Table__7D5F01EEFC444ACF");

            entity.ToTable("Table");

            entity.HasIndex(e => new { e.TableNumber, e.LocationId }, "UQ_Table_TableNumber_Location").IsUnique();

            entity.Property(e => e.TableId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.TableNumber).HasMaxLength(50);
            entity.Property(e => e.TableStatusId).HasDefaultValue(1);

            entity.HasOne(d => d.Location).WithMany(p => p.Tables)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__LocationI__4CA06362");

            entity.HasOne(d => d.TableStatus).WithMany(p => p.Tables)
                .HasForeignKey(d => d.TableStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__TableStat__4D94879B");
        });

        modelBuilder.Entity<TableStatus>(entity =>
        {
            entity.HasKey(e => e.TableStatusId).HasName("PK__TableSta__2DE37812FCF52401");

            entity.ToTable("TableStatus");

            entity.HasIndex(e => e.StatusName, "UQ__TableSta__05E7698A2FD51644").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
