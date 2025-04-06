using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace backend.Data;

public partial class NhahangContext : IdentityDbContext<User, Role, Guid>
{
    public NhahangContext()
    {
    }

    public NhahangContext(DbContextOptions<NhahangContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cartitem> Cartitems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Foodorder> Foodorders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       base.OnModelCreating(modelBuilder);
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cartitem>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProductId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("cartitem");

            entity.HasIndex(e => e.ProductId, "cartItem_ibfk_2");

            entity.Property(e => e.UserId)
                .HasColumnName("userId")
                .HasColumnType("char(36)")
                .UseCollation("ascii_general_ci");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("productId")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.Cartitems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cartItem_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Cartitems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cartItem_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("category");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Foodorder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("foodorder");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UserId)
                .HasColumnName("userId")
                .HasColumnType("char(36)")
                .UseCollation("ascii_general_ci");

            entity.HasOne(d => d.User).WithMany(p => p.Foodorders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("foodorder_ibfk_1");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orderdetail");

            entity.HasIndex(e => e.OrderId, "orderId").IsUnique();

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("orderId")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("productId")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithOne(p => p.Orderdetail)
                .HasForeignKey<Orderdetail>(d => d.OrderId)
                .HasConstraintName("orderdetail_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("orderdetail_ibfk_2");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("payment");

            entity.HasIndex(e => e.OrderId, "orderId").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("orderId")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("paymentDate");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(50)
                .HasColumnName("transactionCode")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.Order).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .HasConstraintName("payment_ibfk_1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.CategoryId, "categoryId");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasColumnName("categoryId")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Description)
                .HasMaxLength(3000)
                .HasColumnName("description")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .HasColumnName("image")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("product_ibfk_1");
        });

        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PRIMARY");

        //    entity.ToTable("user");

        //    entity.Property(e => e.Id)
        //        .HasMaxLength(50)
        //        .HasColumnName("id")
        //        .UseCollation("utf8mb3_general_ci")
        //        .HasCharSet("utf8mb3");
        //    entity.Property(e => e.AccessToken)
        //        .HasMaxLength(50)
        //        .HasColumnName("accessToken")
        //        .UseCollation("utf8mb3_general_ci")
        //        .HasCharSet("utf8mb3");
        //    entity.Property(e => e.Name)
        //        .HasMaxLength(50)
        //        .HasColumnName("name")
        //        .UseCollation("utf8mb3_general_ci")
        //        .HasCharSet("utf8mb3");
        //    entity.Property(e => e.Password)
        //        .HasMaxLength(50)
        //        .HasColumnName("password")
        //        .UseCollation("utf8mb3_general_ci")
        //        .HasCharSet("utf8mb3");
        //    entity.Property(e => e.Role)
        //        .HasMaxLength(50)
        //        .HasColumnName("role")
        //        .UseCollation("utf8mb3_general_ci")
        //        .HasCharSet("utf8mb3");
        //    entity.Property(e => e.Email)
        //        .HasMaxLength(50)
        //        .HasColumnName("email")
        //        .UseCollation("utf8mb3_general_ci")
        //        .HasCharSet("utf8mb3");

        //});
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<Role>().HasKey(x => x.Id);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
