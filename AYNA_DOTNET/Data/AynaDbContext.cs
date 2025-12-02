using System;
using System.Collections.Generic;
using Ayna.Models;
using Microsoft.EntityFrameworkCore;

namespace Ayna.Data;

public partial class AynaDbContext : DbContext
{
    public AynaDbContext()
    {
    }

    public AynaDbContext(DbContextOptions<AynaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketCrop> BasketCrops { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Charity> Charities { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<DonatReq> DonatReqs { get; set; }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<DonationCrop> DonationCrops { get; set; }

    public virtual DbSet<Donor> Donors { get; set; }

    public virtual DbSet<Farmer> Farmers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PickUp> PickUps { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.BasId).HasName("PK__Basket__152482EF2D965958");

            entity.ToTable("Basket");

            entity.Property(e => e.BasId).HasColumnName("Bas_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.BasContent)
                .HasMaxLength(255)
                .HasColumnName("Bas_Content");
            entity.Property(e => e.BasPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Bas_Price");
            entity.Property(e => e.BasQty).HasColumnName("Bas_Qty");
            entity.Property(e => e.FarId).HasColumnName("Far_ID");

            entity.HasOne(d => d.Far).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.FarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Basket__Far_ID__66603565");
        });

        modelBuilder.Entity<BasketCrop>(entity =>
        {
            entity.HasKey(e => e.BcId).HasName("PK__BasketCr__29E89A0E899971B9");

            entity.Property(e => e.BcId).HasColumnName("BC_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.BasId).HasColumnName("Bas_ID");
            entity.Property(e => e.BcQty).HasColumnName("BC_Qty");
            entity.Property(e => e.CroId).HasColumnName("Cro_ID");

            entity.HasOne(d => d.Bas).WithMany(p => p.BasketCrops)
                .HasForeignKey(d => d.BasId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BasketCro__Bas_I__6754599E");

            entity.HasOne(d => d.Cro).WithMany(p => p.BasketCrops)
                .HasForeignKey(d => d.CroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BasketCro__Cro_I__68487DD7");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__D6AB58B9EFC63442");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId).HasColumnName("Cart_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.BasId).HasColumnName("Bas_ID");
            entity.Property(e => e.CartPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Cart_Price");
            entity.Property(e => e.CartQty).HasColumnName("Cart_Qty");
            entity.Property(e => e.OrdId).HasColumnName("Ord_ID");

            entity.HasOne(d => d.Bas).WithMany(p => p.Carts)
                .HasForeignKey(d => d.BasId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart__Bas_ID__693CA210");

            entity.HasOne(d => d.Ord).WithMany(p => p.Carts)
                .HasForeignKey(d => d.OrdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart__Ord_ID__6A30C649");
        });

        modelBuilder.Entity<Charity>(entity =>
        {
            entity.HasKey(e => e.CharId).HasName("PK__Charity__E5078F48ECFF3424");

            entity.ToTable("Charity");

            entity.Property(e => e.CharId).HasColumnName("Char_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.CharContact)
                .HasMaxLength(100)
                .HasColumnName("Char_Contact");
            entity.Property(e => e.CharCr)
                .HasMaxLength(100)
                .HasColumnName("Char_CR");
            entity.Property(e => e.CharLocation)
                .HasMaxLength(255)
                .HasColumnName("Char_Location");
            entity.Property(e => e.CharName)
                .HasMaxLength(150)
                .HasColumnName("Char_Name");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Charities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Charity__User_ID__6B24EA82");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(e => e.CroId).HasName("PK__Crops__90FF6DCCFCF6752F");

            entity.Property(e => e.CroId).HasColumnName("Cro_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.CroName)
                .HasMaxLength(100)
                .HasColumnName("Cro_Name");
            entity.Property(e => e.CroQuantity).HasColumnName("Cro_Quantity");
            entity.Property(e => e.CroShelfLife)
                .HasMaxLength(24)
                .HasComputedColumnSql("(CONVERT([nvarchar](20),datediff(day,[Added_At],[Expired_At]))+N' يوم')", false)
                .HasColumnName("Cro_ShelfLife");
            entity.Property(e => e.CroType)
                .HasMaxLength(100)
                .HasColumnName("Cro_Type");
            entity.Property(e => e.CroUnit)
                .HasMaxLength(50)
                .HasColumnName("Cro_Unit");
            entity.Property(e => e.CroWeight)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Cro_Weight");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("Expired_At");
            entity.Property(e => e.FarId).HasColumnName("Far_ID");

            entity.HasOne(d => d.Far).WithMany(p => p.Crops)
                .HasForeignKey(d => d.FarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Crops__Far_ID__6C190EBB");
        });

        modelBuilder.Entity<DonatReq>(entity =>
        {
            entity.HasKey(e => e.ReqId).HasName("PK__Donat_Re__E36A2768FF2BDA07");

            entity.ToTable("Donat_Req");

            entity.Property(e => e.ReqId).HasColumnName("Req_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.CharId).HasColumnName("Char_ID");
            entity.Property(e => e.DonId).HasColumnName("Don_ID");
            entity.Property(e => e.DonationRequestPickupDuration)
                .HasMaxLength(100)
                .HasColumnName("DonationRequest_PickupDuration");
            entity.Property(e => e.ReqDonation)
                .HasMaxLength(255)
                .HasColumnName("Req_Donation");
            entity.Property(e => e.ReqStatus)
                .HasMaxLength(50)
                .HasColumnName("Req_Status");

            entity.HasOne(d => d.Char).WithMany(p => p.DonatReqs)
                .HasForeignKey(d => d.CharId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Donat_Req__Char___6D0D32F4");

            entity.HasOne(d => d.Don).WithMany(p => p.DonatReqs)
                .HasForeignKey(d => d.DonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Donat_Req__Don_I__6E01572D");
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonId).HasName("PK__Donation__F71E08EB84800C73");

            entity.ToTable("Donation");

            entity.Property(e => e.DonId).HasColumnName("Don_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.DonDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Don_Date");
            entity.Property(e => e.DonDescription)
                .HasMaxLength(255)
                .HasColumnName("Don_Description");
            entity.Property(e => e.DonStatus)
                .HasMaxLength(50)
                .HasColumnName("Don_Status");
            entity.Property(e => e.FarId).HasColumnName("Far_ID");

            entity.HasOne(d => d.Far).WithMany(p => p.Donations)
                .HasForeignKey(d => d.FarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Donation__Far_ID__6EF57B66");
        });

        modelBuilder.Entity<DonationCrop>(entity =>
        {
            entity.HasKey(e => e.DcId).HasName("PK__Donation__46564CF903CE0DCD");

            entity.Property(e => e.DcId).HasColumnName("DC_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.CroId).HasColumnName("Cro_ID");
            entity.Property(e => e.DcQuantity).HasColumnName("DC_Quantity");
            entity.Property(e => e.DonId).HasColumnName("Don_ID");

            entity.HasOne(d => d.Cro).WithMany(p => p.DonationCrops)
                .HasForeignKey(d => d.CroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationC__Cro_I__6FE99F9F");

            entity.HasOne(d => d.Don).WithMany(p => p.DonationCrops)
                .HasForeignKey(d => d.DonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationC__Don_I__70DDC3D8");
        });

        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.DonorId).HasName("PK__Donor__2E4F2165306CBC03");

            entity.ToTable("Donor");

            entity.Property(e => e.DonorId).HasColumnName("Donor_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.DonorFirstName)
                .HasMaxLength(100)
                .HasColumnName("Donor_FirstName");
            entity.Property(e => e.DonorLastName)
                .HasMaxLength(100)
                .HasColumnName("Donor_LastName");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Donors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Donor__User_ID__71D1E811");
        });

        modelBuilder.Entity<Farmer>(entity =>
        {
            entity.HasKey(e => e.FarId).HasName("PK__Farmer__11E66CCF8A9796BD");

            entity.ToTable("Farmer");

            entity.Property(e => e.FarId).HasColumnName("Far_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.FarFirstName)
                .HasMaxLength(100)
                .HasColumnName("Far_FirstName");
            entity.Property(e => e.FarLastName)
                .HasMaxLength(100)
                .HasColumnName("Far_LastName");
            entity.Property(e => e.FarLocation)
                .HasMaxLength(255)
                .HasColumnName("Far_Location");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Farmers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Farmer__User_ID__72C60C4A");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__0DE604943496F7B9");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("Invoice_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.InvoiceAmountPaid)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Invoice_AmountPaid");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Invoice_Date");
            entity.Property(e => e.InvoiceTime).HasColumnName("Invoice_Time");
            entity.Property(e => e.PayId).HasColumnName("Pay_ID");

            entity.HasOne(d => d.Pay).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoice__Pay_ID__73BA3083");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrdId).HasName("PK__Order__1712A21EA4BADA33");

            entity.ToTable("Order");

            entity.Property(e => e.OrdId).HasColumnName("Ord_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.CharId).HasColumnName("Char_ID");
            entity.Property(e => e.DonorId).HasColumnName("Donor_ID");
            entity.Property(e => e.OrdDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Ord_Date");
            entity.Property(e => e.OrdPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Ord_Price");
            entity.Property(e => e.OrdStatus)
                .HasMaxLength(50)
                .HasColumnName("Ord_Status");
            entity.Property(e => e.OrdTime).HasColumnName("Ord_Time");

            entity.HasOne(d => d.Char).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CharId)
                .HasConstraintName("FK__Order__Char_ID__74AE54BC");

            entity.HasOne(d => d.Donor).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__Donor_ID__75A278F5");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PayId).HasName("PK__Payment__6F137505B907F9A6");

            entity.ToTable("Payment");

            entity.Property(e => e.PayId).HasColumnName("Pay_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.OrdId).HasColumnName("Ord_ID");
            entity.Property(e => e.PayAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Pay_Amount");
            entity.Property(e => e.PayDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Pay_Date");
            entity.Property(e => e.PayMethod)
                .HasMaxLength(50)
                .HasColumnName("Pay_Method");
            entity.Property(e => e.PayStatus)
                .HasMaxLength(50)
                .HasColumnName("Pay_Status");
            entity.Property(e => e.PayTime).HasColumnName("Pay_Time");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("Transaction_ID");

            entity.HasOne(d => d.Ord).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Ord_ID__76969D2E");
        });

        modelBuilder.Entity<PickUp>(entity =>
        {
            entity.HasKey(e => e.PickId).HasName("PK__PickUp__7A30A4584F70DD93");

            entity.ToTable("PickUp");

            entity.Property(e => e.PickId).HasColumnName("Pick_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.DonId).HasColumnName("Don_ID");
            entity.Property(e => e.PickLocation)
                .HasMaxLength(255)
                .HasColumnName("Pick_Location");
            entity.Property(e => e.PickTime)
                .HasColumnType("datetime")
                .HasColumnName("Pick_Time");

            entity.HasOne(d => d.Don).WithMany(p => p.PickUps)
                .HasForeignKey(d => d.DonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickUp__Don_ID__778AC167");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__206D91901B6F0327");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserEmail, "UQ__User__4C70A05CB2515E6B").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_At");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("User_Email");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .HasColumnName("User_Password");
            entity.Property(e => e.UserPhone)
                .HasMaxLength(20)
                .HasColumnName("User_Phone");
            entity.Property(e => e.UserStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Active")
                .HasColumnName("User_Status");
            entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .HasColumnName("User_Type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
