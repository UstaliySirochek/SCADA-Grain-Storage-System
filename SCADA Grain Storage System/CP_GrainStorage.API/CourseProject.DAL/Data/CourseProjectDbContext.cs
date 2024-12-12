using CourseProject.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.DAL.Data
{
    public class CourseProjectDbContext : DbContext
    {
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<IndicatorValue> IndicatorValues { get; set; }
        public DbSet<BackgroundImage> BackgroundImages { get; set; }

        public CourseProjectDbContext(DbContextOptions<CourseProjectDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Indicator>()
                .HasMany(e => e.IndicatorValues)
                .WithOne(v => v.Indicator)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}