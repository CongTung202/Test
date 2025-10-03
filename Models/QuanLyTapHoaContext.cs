using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NemeShop.Models;

public partial class QuanLyTapHoaContext : DbContext
{
    public QuanLyTapHoaContext()
    {
    }

    public QuanLyTapHoaContext(DbContextOptions<QuanLyTapHoaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DanhGiaSanPham> DanhGiaSanPhams { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }

    public virtual DbSet<MaGiamGium> MaGiamGia { get; set; }

    public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=BOKACHAN\\MSSQLSERVER01;Database=QuanLyTapHoa;uid=sa;pwd=12345678; MultipleActiveResultSets=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__ChiTietH__1E4FA77180508239");

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaCthd).HasColumnName("MaCTHD");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHoa__MaHD__5CD6CB2B");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHoa__MaSP__5DCAEF64");
        });

        modelBuilder.Entity<DanhGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGiaS__AA9515BFB8B42E8A");

            entity.ToTable("DanhGiaSanPham");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaSan__MaKH__6477ECF3");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaSan__MaSP__656C112C");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E0EF203880");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(200);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaVoucher)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SdtgiaoHang)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDTGiaoHang");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienGiamGia)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaKH__5629CD9C");

            entity.HasOne(d => d.MaVoucherNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaVoucher)
                .HasConstraintName("FK__HoaDon__MaVouche__571DF1D5");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1EAB303733");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Email, "UQ__KhachHan__A9D105347604BC23").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .HasDefaultValue("CoBaChan123");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiSanP__730A57599EE7833E");

            entity.ToTable("LoaiSanPham");

            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenLoai).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaGiamGium>(entity =>
        {
            entity.HasKey(e => e.MaVoucher).HasName("PK__MaGiamGi__0AAC5B11B9B8F52A");

            entity.Property(e => e.MaVoucher)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GiaTriDonToiThieu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiamTienTrucTiep)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.PhanTramGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TenVoucher).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<QuanTriVien>(entity =>
        {
            entity.HasKey(e => e.MaQtv).HasName("PK__QuanTriV__396E99963D226F76");

            entity.ToTable("QuanTriVien");

            entity.HasIndex(e => e.Email, "UQ__QuanTriV__A9D1053403B295ED").IsUnique();

            entity.Property(e => e.MaQtv).HasColumnName("MaQTV");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081CA42EBA7E");

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.PhanTramGiam).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenSp)
                .HasMaxLength(100)
                .HasColumnName("TenSP");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK__SanPham__MaLoai__46E78A0C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
