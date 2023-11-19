using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace acomba.zuper_api.Models;

public partial class DbService : DbContext
{
    public DbService()
    {
    }

    public DbService(DbContextOptions<DbService> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=acombametrospec.database.windows.net; Database=metrospecdb; User Id=metrodb; Password=pA425tg8gZMN; Integrated Security=False; MultipleActiveResultSets=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.CompanyUid).HasMaxLength(250);
            entity.Property(e => e.CustomerEmail).HasMaxLength(250);
            entity.Property(e => e.CustomerFirstName).HasMaxLength(250);
            entity.Property(e => e.CustomerLastName).HasMaxLength(250);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
