using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KiemTra.Models;

public partial class KiemTraContext : DbContext
{
    public KiemTraContext()
    {
    }

    public KiemTraContext(DbContextOptions<KiemTraContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DanhMucCapBac> DanhMucCapBacs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<QuaTrinhCongTac> QuaTrinhCongTacs { get; set; }

    public virtual DbSet<ThaoTacSua> ThaoTacSuas { get; set; }

    public virtual DbSet<VanBang> VanBangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=CONGKC\\SQLEXPRESS;Initial Catalog=KiemTra;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DanhMucCapBac>(entity =>
        {
            entity.HasKey(e => e.MaCapBac).HasName("PK__DanhMucC__219088257535B1C3");

            entity.ToTable("DanhMucCapBac");

            entity.Property(e => e.MoTaCapBac).HasMaxLength(500);
            entity.Property(e => e.TenCapBac).HasMaxLength(100);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA476E893766");

            entity.ToTable("NhanVien");

            entity.Property(e => e.ChucVu).HasMaxLength(100);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.QueQuan).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<QuaTrinhCongTac>(entity =>
        {
            entity.HasKey(e => e.MaQuaTrinh).HasName("PK__QuaTrinh__217D4F65CBCE93EB");

            entity.ToTable("QuaTrinhCongTac");

            entity.Property(e => e.ChucVu).HasMaxLength(100);
            entity.Property(e => e.PhongBan).HasMaxLength(100);

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.QuaTrinhCongTacs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuaTrinhNhanVien");
        });

        modelBuilder.Entity<ThaoTacSua>(entity =>
        {
            entity.HasKey(e => e.MaThaoTac).HasName("PK__ThaoTacS__32ACE293256F7197");

            entity.ToTable("ThaoTacSua");
            entity.Property(e => e.ChiTietThaoTac).HasMaxLength(500);
            entity.Property(e => e.LoaiThaoTac).HasMaxLength(50);
            entity.Property(e => e.NgayThaoTac).HasColumnType("datetime");
            entity.Property(e => e.username).HasMaxLength(50);
            //entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.ThaoTacSuas)
            //    .HasForeignKey(d => d.MaNhanVien)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_ThaoTacNhanVien");
        });

        modelBuilder.Entity<VanBang>(entity =>
        {
            entity.HasKey(e => e.MaVanBang).HasName("PK__VanBang__8EAF235C8FCC380C");

            entity.ToTable("VanBang");

            entity.Property(e => e.NoiCap).HasMaxLength(255);
            entity.Property(e => e.TenVanBang).HasMaxLength(255);

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.VanBangs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VanBangNhanVien");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
