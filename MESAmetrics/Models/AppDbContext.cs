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

    public virtual DbSet<RealTime> RealTime { get; set; }

    public virtual DbSet<Shifts> Shifts { get; set; }

    public virtual DbSet<Tags> Tags { get; set; }

    public virtual DbSet<Telemetry> Telemetry { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RealTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RealTime__3214EC07C35B19E9");

            entity.Property(e => e.Availability).HasColumnName("availability");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.EvenTime).HasColumnName("evenTime");
            entity.Property(e => e.ProductionTime).HasColumnName("productionTime");
            entity.Property(e => e.ShiftId).HasColumnName("shiftId");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.Strikes).HasColumnName("strikes");
            entity.Property(e => e.TagsId).HasColumnName("tagsId");
            entity.Property(e => e.Title)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Shift).WithMany(p => p.RealTime)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK__RealTime__update__46E78A0C");

            entity.HasOne(d => d.Tags).WithMany(p => p.RealTime)
                .HasForeignKey(d => d.TagsId)
                .HasConstraintName("FK__RealTime__tagsId__47DBAE45");
        });

        modelBuilder.Entity<Shifts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shifts__3214EC07CE5CE2EF");

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
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC076A2D64F9");

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
            entity.HasKey(e => e.Id).HasName("PK__Telemetr__3214EC072FFEBC88");

            entity.Property(e => e.CycleCount).HasColumnName("cycleCount");
            entity.Property(e => e.MachineId).HasColumnName("machineId");
            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.StopButton).HasColumnName("stopButton");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}