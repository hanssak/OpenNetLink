using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGNotify;

namespace OpenNetLinkApp.Data.SGNotify
{
    public class SGNotifyContext : DbContext
    {
        public DbSet<SGAlarmData> Alarms { get; set; }
        public DbSet<SGNotiData> Notis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=SGNotifyDB.db;");
            base
                .OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SGAlarmData>().ToTable("T_SG_ALARM");
            modelBuilder.Entity<SGNotiData>().ToTable("T_SG_NOTI");
        }
    }
}