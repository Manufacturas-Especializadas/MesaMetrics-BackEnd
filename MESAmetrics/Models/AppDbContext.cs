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

    public virtual DbSet<Telemetry> Telemetry { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Telemetr__3214EC07CBE58B86");

            entity.Property(e => e.CycleCount).HasColumnName("cycleCount");
            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.StopButton).HasColumnName("stopButton");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}