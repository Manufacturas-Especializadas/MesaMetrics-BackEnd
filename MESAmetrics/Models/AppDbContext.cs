using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MESAmetrics.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lines> Lines { get; set; }

    public virtual DbSet<MachineStops> MachineStops { get; set; }

    public virtual DbSet<MachinesIds> MachinesIds { get; set; }

    public virtual DbSet<RealTime> RealTime { get; set; }

    public virtual DbSet<RealTimeTags> RealTimeTags { get; set; }

    public virtual DbSet<Shifts> Shifts { get; set; }

    public virtual DbSet<Tags> Tags { get; set; }

    public virtual DbSet<Telemetry> Telemetry { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lines>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lines__3214EC073B91A259");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.LinesName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("linesName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<MachineStops>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MachineS__3214EC07C872EC9A");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DurationMinutes).HasColumnName("durationMinutes");
            entity.Property(e => e.RealTimeId).HasColumnName("realTimeId");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.StopTime).HasColumnName("stopTime");

            entity.HasOne(d => d.RealTime).WithMany(p => p.MachineStops)
                .HasForeignKey(d => d.RealTimeId)
                .HasConstraintName("FK__MachineSt__realT__1CBC4616");
        });

        modelBuilder.Entity<MachinesIds>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Machines__3214EC071362FC7A");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Machine)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("machine");
        });

        modelBuilder.Entity<RealTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RealTime__3214EC0790619D4E");

            entity.Property(e => e.Availability).HasColumnName("availability");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.EvenTime).HasColumnName("evenTime");
            entity.Property(e => e.LineId).HasColumnName("lineId");
            entity.Property(e => e.MachineId).HasColumnName("machineId");
            entity.Property(e => e.ProductionTime).HasColumnName("productionTime");
            entity.Property(e => e.ShiftId).HasColumnName("shiftId");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.Strikes).HasColumnName("strikes");
            entity.Property(e => e.Title)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Line).WithMany(p => p.RealTime)
                .HasForeignKey(d => d.LineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__RealTime__lineId__0E6E26BF");

            entity.HasOne(d => d.Machine).WithMany(p => p.RealTime)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__RealTime__machin__0F624AF8");

            entity.HasOne(d => d.Shift).WithMany(p => p.RealTime)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__RealTime__update__0D7A0286");
        });

        modelBuilder.Entity<RealTimeTags>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RealTime__3214EC07C3CC46D2");

            entity.HasIndex(e => new { e.RealTimeId, e.TagId }, "UQ__RealTime__2CD3530C62E57D62").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.RealTimeId).HasColumnName("realTimeId");
            entity.Property(e => e.TagId).HasColumnName("tagId");

            entity.HasOne(d => d.RealTime).WithMany(p => p.RealTimeTags)
                .HasForeignKey(d => d.RealTimeId)
                .HasConstraintName("FK__RealTimeT__realT__14270015");

            entity.HasOne(d => d.Tag).WithMany(p => p.RealTimeTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RealTimeT__tagId__151B244E");
        });

        modelBuilder.Entity<Shifts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shifts__3214EC07F7E42CEF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("shiftName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Tags>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07C1B8B9E0");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.TagsName)
                .IsUnicode(false)
                .HasColumnName("tagsName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Telemetr__3214EC07C49CAAA9");

            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CycleCount).HasColumnName("cycleCount");
            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.RealTimeId).HasColumnName("realTimeId");
            entity.Property(e => e.StopButton).HasColumnName("stopButton");

            entity.HasOne(d => d.RealTime).WithMany(p => p.Telemetry)
                .HasForeignKey(d => d.RealTimeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Telemetry__creat__18EBB532");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}