using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public partial class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TaiwanCity> TaiwanCities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaiwanCity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__taiwan_c__3213E83F54B83AA9");

            entity.ToTable("taiwan_cities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityName)
                .HasMaxLength(5)
                .HasColumnName("city_name");
            entity.Property(e => e.Region)
                .HasMaxLength(2)
                .HasColumnName("region");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
