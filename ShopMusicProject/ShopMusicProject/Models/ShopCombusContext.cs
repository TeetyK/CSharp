using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopMusicProject.Models;

public partial class ShopCombusContext : DbContext
{
    public ShopCombusContext()
    {
    }

    public ShopCombusContext(DbContextOptions<ShopCombusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<BuyDtl> BuyDtls { get; set; }

    public virtual DbSet<Buying> Buyings { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDtl> CartDtls { get; set; }

    public virtual DbSet<Category> Categorys { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Duty> Dutys { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Work> Works { get; set; }

   /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=ShopCombus;User ID=webproject;Password=7484;Encrypt=True;Trust Server Certificate=True");
   */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.Property(e => e.BrandId)
                .ValueGeneratedNever()
                .HasColumnName("BrandID");
            entity.Property(e => e.BrandName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BuyDtl>(entity =>
        {
            entity.HasKey(e => new { e.BuyId, e.PdId });

            entity.Property(e => e.BuyId)
                .HasMaxLength(50)
                .HasColumnName("BuyID");
            entity.Property(e => e.PdId)
                .HasMaxLength(50)
                .HasColumnName("PdID");
            entity.Property(e => e.BdtlMoney).HasColumnName("BDtlMoney");
            entity.Property(e => e.BdtlPrice).HasColumnName("BDtlPrice");
            entity.Property(e => e.BdtlQty).HasColumnName("BDtlQty");
        });

        modelBuilder.Entity<Buying>(entity =>
        {
            entity.HasKey(e => e.BuyId);

            entity.ToTable("Buying");

            entity.Property(e => e.BuyId).HasMaxLength(50);
            entity.Property(e => e.BuyDocId)
                .HasMaxLength(50)
                .HasColumnName("BuyDocID");
            entity.Property(e => e.BuyRemark).HasColumnType("text");
            entity.Property(e => e.Saleman).HasMaxLength(50);
            entity.Property(e => e.StfId)
                .HasMaxLength(50)
                .HasColumnName("StfID");
            entity.Property(e => e.SupId)
                .HasMaxLength(50)
                .HasColumnName("SupID");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.CartId).HasMaxLength(50);
            entity.Property(e => e.CdateAt).HasColumnName("Cdate_at");
            entity.Property(e => e.Cf)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CF");
            entity.Property(e => e.Cpay)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CPay");
            entity.Property(e => e.Cqty).HasColumnName("CQty");
            entity.Property(e => e.Csend)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CSend");
            entity.Property(e => e.CusId).HasMaxLength(50);
            entity.Property(e => e.Cvoid)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CVoid");
        });

        modelBuilder.Entity<CartDtl>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.PdId });

            entity.Property(e => e.CartId)
                .HasMaxLength(50)
                .HasColumnName("CartID");
            entity.Property(e => e.PdId)
                .HasMaxLength(50)
                .HasColumnName("PdID");
            entity.Property(e => e.CdtlMoney).HasColumnName("CDtlMoney");
            entity.Property(e => e.CdtlPrice).HasColumnName("CDtlPrice");
            entity.Property(e => e.CdtlQty).HasColumnName("CDtlQty");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Category");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Ncategory).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CusId);

            entity.Property(e => e.CusId).HasMaxLength(50);
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginAt).HasColumnName("login_at");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Duty>(entity =>
        {
            entity.Property(e => e.DutyId)
                .HasMaxLength(50)
                .HasColumnName("DutyID");
            entity.Property(e => e.DutyName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmId);

            entity.Property(e => e.EmId).HasMaxLength(50);
            entity.Property(e => e.Fname).HasMaxLength(50);
            entity.Property(e => e.Lname).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.QuitAt).HasColumnName("quit_at");
            entity.Property(e => e.StartAt).HasColumnName("start_at");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Brandid).HasColumnName("brandid");
            entity.Property(e => e.Catid).HasColumnName("catid");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.LastbuyAt).HasColumnName("lastbuy_at");
            entity.Property(e => e.LastsaleAt).HasColumnName("lastsale_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.DateAt).HasColumnName("date_at");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Cid).HasColumnName("CId");
            entity.Property(e => e.DateAt).HasColumnName("date_at");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Amount)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("amount");
            entity.Property(e => e.Cid)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Payment)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("payment");
            entity.Property(e => e.PdId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Price)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("price");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Sid);

            entity.Property(e => e.Sid).HasMaxLength(50);
            entity.Property(e => e.Saddress).HasMaxLength(50);
            entity.Property(e => e.Semail).HasMaxLength(50);
            entity.Property(e => e.Sphone).HasMaxLength(50);
            entity.Property(e => e.Sremark).HasMaxLength(50);
            entity.Property(e => e.SupContact).HasMaxLength(50);
            entity.Property(e => e.SupName).HasMaxLength(50);
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(e=> new { e.EmId , e.WorkDate});

            entity.Property(e => e.EmId).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
