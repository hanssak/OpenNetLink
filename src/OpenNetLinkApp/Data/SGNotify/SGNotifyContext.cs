using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGNotify;
using OpenNetLinkApp.Models.SGSideBar;
using Serilog;
using Serilog.Events;
using AgLogManager;

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
                        .Property(c => c.Id).IsRequired()
                        .HasAnnotation("Sqlite:Autoincrement", true)
                        .ValueGeneratedOnAdd();
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.GroupId).IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.CategoryId).IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .HasKey(k => k.Id).HasName("PK_T_SG_ALARM_ID");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Path).HasColumnType("varchar(64)");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.IconImage).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Head).HasColumnType("varchar(64)").IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Body).HasColumnType("varchar(255)").IsRequired();
            modelBuilder.Entity<SGAlarmData>()
                        .Property(c => c.Time).HasColumnType("TEXT").HasDefaultValueSql("datetime('now','localtime')").IsRequired();
            /* TABLE: SGNotiData */
            modelBuilder.Entity<SGNotiData>()
                        .ToTable("T_SG_NOTI");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Id).IsRequired()
                        .HasAnnotation("Sqlite:Autoincrement", true)
                        .ValueGeneratedOnAdd();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.GroupId).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.CategoryId).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .HasKey(k => k.Id).HasName("PK_T_SG_NOTI_ID");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Path).HasColumnType("varchar(64)");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.IconImage).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Head).HasColumnType("varchar(64)").IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Body).HasColumnType("varchar(255)").IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Time).HasColumnType("TEXT").HasDefaultValueSql("datetime('now','localtime')").IsRequired();
        }
    }
    public class SGDBProc
    {
        /* Insert to SGNotiInfo */
        public static bool InsertNotiInfo(int groupId, LSIDEBAR categoryId, string path, string iconImage, string head, string body)
        {
            using (var db = new SGNotifyContext())
            {
                // Create
                Log.Information("Inserting a new NotiInfo, {NotiHead}, {NotiBody}", head, body);
                db.Add(new SGNotiData 
                           { 
                               Id = 0, 
                               GroupId = groupId, 
                               CategoryId = categoryId,
                               Path = path, 
                               IconImage = iconImage, 
                               Head = head, 
                               Body = body,
                               Time = DateTime.Now
                           }
                       );
                db.SaveChanges();
            }
            return true;
        }
        /* Select * from SGNotiInfo */
        public static List<SGNotiData> SelectNotiInfoLimit(int groupId, int nLimit)
        {
            List<SGNotiData> NotiList;
            using (var db = new SGNotifyContext())
            {
                // Read
                NotiList = db.Notis
                    .Where(x => x.GroupId == groupId)
                    .OrderByDescending(x => x.Time).Take(nLimit)
                    .ToList();
                Log.Information("Querying for a NotiInfo Limit {nLimit}", nLimit);
            }
            return NotiList;
        }
        /* Select count(*) from SGNotiInfo */
        public static int SelectNotiInfoCount(int groupId)
        {
            int nCount;
            using (var db = new SGNotifyContext())
            {
                // Read
                nCount = db.Notis
                    .Where(x => x.GroupId == groupId)
                    .Count();
                Log.Information("Querying for a NotiInfo Count {nCount}", nCount);

            }
            return nCount;
        }
        /* Delete from SGNotiInfo */
        public static bool DeleteNotiInfo(SGNotiData notiData)
        {
            using (var db = new SGNotifyContext())
            {
                // Delete
                db.Remove(notiData);
                db.SaveChanges();
                Log.Information("Delete the SGNotiData, {NotiData}", notiData);

            }
            return true;
        }
        /* Insert to SGAlarmInfo */
        /* Select from SGAlarmInfo */
    }
}