using System.Reflection.Emit;
using System;
using System.Collections.Generic;
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
            base.OnModelCreating(modelBuilder);

            /* TABLE: SGAlarmData */
            modelBuilder.Entity<SGAlarmData>()
                        .ToTable("T_SG_ALARM");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.AlarmId).IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .HasKey(k => new { k.AlarmId, k.GroupId }).HasName("PK_T_SG_ALARM_ALARMID_GROUPID");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Path).HasColumnType("varchar(64)");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.IconImage).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Head).HasColumnType("varchar(64)").IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Body).HasColumnType("varchar(255)").IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Time).HasColumnType("TEXT").HasDefaultValueSql("GETDATE()").IsRequired();
            /* TABLE: SGNotiData */
            modelBuilder.Entity<SGNotiData>()
                        .ToTable("T_SG_NOTI");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.NotiId).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .HasKey(k => new { k.NotiId, k.GroupId }).HasName("PK_T_SG_NOTI_NOTIID_GROUPID");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Path).HasColumnType("varchar(64)");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.IconImage).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Head).HasColumnType("varchar(64)").IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Body).HasColumnType("varchar(255)").IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Time).HasColumnType("TEXT").HasDefaultValueSql("GETDATE()").IsRequired();
        }
    }
}