using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelWriteApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ExcelWriteApi.Data
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {   // Here we use FluentApi instead of DataAnnotations in our entities
        public DataContext(DbContextOptions options) : base(options) 
        {
           
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Transaction>()
                .Property(i => i.Amount).HasColumnType("money");

            builder.Entity<Transaction>()
                .HasKey(i => i.TransactionId);

            builder.Entity<Transaction>()
                .Property(i => i.TransactionId).ValueGeneratedNever();            
        }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
