using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project_PRN211.Models;

public partial class WishContext : DbContext
{
    public WishContext()
    {
    }


    public WishContext(DbContextOptions<WishContext> options)
        : base(options)
    {
    }
    public virtual DbSet<ProductSold> ProductSold { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Product { get; set; }
    public virtual DbSet<Receiving_Address> Receiving_Address { get; set; }
    public object Products { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=127.0.0.1,1433;Initial Catalog=Wish;User ID=sa;Password=anhhuy512@;TrustServerCertificate=True;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UId).HasName("PK__Account__DD771E3C986492DF");

            entity.ToTable("Account");

            entity.Property(e => e.UId).HasColumnName("uID");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.IsSell).HasColumnName("isSell");
            entity.Property(e => e.Pass)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("pass");
            entity.Property(e => e.User)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            // Thêm khóa chính mới là CartId
            entity.HasKey(e => e.CartId);

            entity.ToTable("Cart");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Receiving_AddressId).HasColumnName("Receiving_AddressId");
        });
        modelBuilder.Entity<ProductSold>(entity =>
        {
            // Thêm khóa chính mới là Id
            entity.HasKey(e => e.Id);

            entity.ToTable("ProductSold");

            entity.Property(e => e.CartId).HasColumnName("CartId");

        });
        modelBuilder.Entity<Receiving_Address>(entity =>
        {
            // Thêm khóa chính mới là Id
            entity.HasKey(e => e.AddressId);

            entity.ToTable("Receiving_Address");

            entity.Property(e => e.AddressId).HasColumnName("AddressId");

        });


        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Cid);

            entity.ToTable("Category");

            entity.Property(e => e.Cid)
                .HasColumnName("cid");
            entity.Property(e => e.Cname)
                .HasMaxLength(50)
                .HasColumnName("cname");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");
            entity.HasKey(t => t.Id);

            entity.Property(e => e.CateId).HasColumnName("cateID");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.SellId).HasColumnName("sell_ID");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
