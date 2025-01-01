using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TraCuuThongTinMVC.Data;

public partial class SchoolContext : DbContext
{
    public SchoolContext()
    {
    }

    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InfSch> InfSches { get; set; }

    public virtual DbSet<ProfessionDb> ProfessionDbs { get; set; }

    public virtual DbSet<UserAdmin> UserAdmins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ADMIN-PC;Initial Catalog=SCHOOL;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InfSch>(entity =>
        {
            entity.HasKey(e => e.ScId).HasName("PK__Inf_Sch__C402E49E22321E4F");

            entity.ToTable("INF_SCH");

            entity.Property(e => e.ScId)
                .HasMaxLength(255)
                .HasColumnName("SC_ID");
            entity.Property(e => e.Count).HasColumnName("COUNT");
            entity.Property(e => e.Fax)
                .HasMaxLength(50)
                .HasColumnName("FAX");
            entity.Property(e => e.HiuPho)
                .HasMaxLength(255)
                .HasColumnName("HIU_PHO");
            entity.Property(e => e.HiuTruong)
                .HasMaxLength(255)
                .HasColumnName("HIU_TRUONG");
            entity.Property(e => e.Mail)
                .HasMaxLength(255)
                .HasColumnName("MAIL");
            entity.Property(e => e.NguoiChiuTrachNhiemPhapLuat)
                .HasMaxLength(255)
                .HasColumnName("NGUOI_CHIU_TRACH_NHIEM_PHAP_LUAT");
            entity.Property(e => e.ScAddress)
                .HasMaxLength(255)
                .HasColumnName("SC_ADDRESS");
            entity.Property(e => e.ScAddress1)
                .HasMaxLength(255)
                .HasColumnName("SC_ADDRESS1");
            entity.Property(e => e.ScArea)
                .HasMaxLength(255)
                .HasColumnName("SC_AREA");
            entity.Property(e => e.ScNm)
                .HasMaxLength(255)
                .HasColumnName("SC_NM");
            entity.Property(e => e.ScTcd)
                .HasMaxLength(50)
                .HasColumnName("SC_TCD");
            entity.Property(e => e.Tel)
                .HasMaxLength(50)
                .HasColumnName("TEL");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("WEBSITE");
        });

        modelBuilder.Entity<ProfessionDb>(entity =>
        {
            entity.HasKey(e => e.Cd);

            entity.ToTable("PROFESSION_DB");

            entity.Property(e => e.Cd)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CD");
            entity.Property(e => e.Count).HasColumnName("COUNT");
            entity.Property(e => e.GvCn)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("GV_CN");
            entity.Property(e => e.MoTa).HasColumnName("MO_TA");
            entity.Property(e => e.Nm).HasColumnName("NM");
            entity.Property(e => e.ScId)
                .HasMaxLength(50)
                .HasColumnName("SC_ID");
        });

        modelBuilder.Entity<UserAdmin>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("USER_ADMIN");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("USER_ID");
            entity.Property(e => e.Passwork)
                .HasMaxLength(50)
                .HasColumnName("PASSWORK");
            entity.Property(e => e.UserNm)
                .HasMaxLength(50)
                .HasColumnName("USER_NM");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
