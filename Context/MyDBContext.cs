using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Models.UserCLasses;
using TheSocialCebu_Capstone.Models.BillingClasses;
using TheSocialCebu_Capstone.Models.MenuClasses;
using TheSocialCebu_Capstone.Models.TableClasses;
using TheSocialCebu_Capstone.Models.OrderClasses;

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

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Marketing> Marketings { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-K56S2BSD\\SQLEXPRESS;Initial Catalog=TheSocialCebu;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6BE16A67F");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E484E9564B").IsUnique();

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
            entity.HasKey(e => e.BillingId).HasName("PK__Billing__F1656DF32888AA30");

            entity.ToTable("Billing");

            entity.Property(e => e.BillingId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceCharge).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TableId).HasMaxLength(50);
            entity.Property(e => e.VatAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Table).WithMany(p => p.Billings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__TableId__6477ECF3");
        });

        modelBuilder.Entity<BillingOrder>(entity =>
        {
            entity.HasKey(e => e.BillingOrderId).HasName("PK__BillingO__48F8147A61A8785D");

            entity.ToTable("BillingOrder");

            entity.Property(e => e.BillingOrderId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingId).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasMaxLength(50);

            entity.HasOne(d => d.Billing).WithMany(p => p.BillingOrders)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillingOr__Billi__68487DD7");

            entity.HasOne(d => d.Order).WithMany(p => p.BillingOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillingOr__Order__693CA210");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0BAEECD682");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6D96D37E7FA0");

            entity.Property(e => e.DiscountId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.ApprovedBy).HasMaxLength(50);
            entity.Property(e => e.BillingId).HasMaxLength(50);
            entity.Property(e => e.DiscountTypeId).HasMaxLength(50);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Discounts__Appro__75A278F5");

            entity.HasOne(d => d.Billing).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discounts__Billi__73BA3083");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discounts__Disco__74AE54BC");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountTypeId).HasName("PK__Discount__6CCE1DB6A7F5ED49");

            entity.ToTable("DiscountType");

            entity.Property(e => e.DiscountTypeId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.DiscountName).HasMaxLength(100);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD65BD7ABFE");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.BillingId).HasMaxLength(50);

            entity.HasOne(d => d.Billing).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BillingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__Billin__7A672E12");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA4978ECD8938");

            entity.ToTable("Location");

            entity.HasIndex(e => e.LocationName, "UQ__Location__F946BB84F5DD333E").IsUnique();

            entity.Property(e => e.LocationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.LocationName).HasMaxLength(50);
        });

        modelBuilder.Entity<Marketing>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK__Marketin__7ED91ACF5376E0C9");

            entity.ToTable("Marketing");

            entity.HasIndex(e => e.Email, "UQ__Marketin__A9D10534DD2F8720").IsUnique();

            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Email).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF930BD0E7");

            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.TableId).HasMaxLength(50);

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__TableId__5AEE82B9");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681768C439F");

            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.Instructions).HasMaxLength(100);
            entity.Property(e => e.OrderId).HasMaxLength(50);
            entity.Property(e => e.ProdId).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__5EBF139D");

            entity.HasOne(d => d.Prod).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__ProdI__5FB337D6");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A386D7FF1DF");

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
                .HasConstraintName("FK__Payment__Billing__6D0D32F4");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__AA2FFBE58F251683");

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
            entity.HasKey(e => e.ProdId).HasName("PK__Product__042785E55D23C41D");

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
                .HasConstraintName("FK__Product__Subcate__5629CD9C");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A820A67DE");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.RoleName).HasMaxLength(150);
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PK__SubCateg__9C4E705D687FCED8");

            entity.ToTable("SubCategory");

            entity.Property(e => e.SubcategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.CategoryId).HasMaxLength(50);
            entity.Property(e => e.SubcategoryName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubCatego__Categ__5165187F");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Table__7D5F01EE9861D905");

            entity.ToTable("Table");

            entity.HasIndex(e => new { e.TableNumber, e.LocationId }, "UQ_Table_TableNumber_Location").IsUnique();

            entity.Property(e => e.TableId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.LocationId).HasMaxLength(50);
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.TableNumber).HasMaxLength(50);

            entity.HasOne(d => d.Location).WithMany(p => p.Tables)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__LocationI__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
