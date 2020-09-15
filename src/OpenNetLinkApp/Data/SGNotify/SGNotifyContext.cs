using System.ComponentModel;
using System.Collections.Specialized;
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
    /// <summary>
    /// Implement SQLite DB Context 
    /// </summary>
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
    /// <summary>
    /// Singleton SGNtfyDBProc
    /// Implement SQLite DB Operate Function Object
    /// </summary>
    public sealed class SGNtfyDBProc
    {
        // SQLite DB Context
        private SGNotifyContext DBCtx { get; set; }
        //private 생성자 
        private SGNtfyDBProc() 
        { 
            DBCtx = new SGNotifyContext();
        }
        //private static 인스턴스 객체
        private static readonly Lazy<SGNtfyDBProc> _instance = new Lazy<SGNtfyDBProc> (() => new SGNtfyDBProc());
        //public static 의 객체반환 함수
        public static SGNtfyDBProc Instance { get { return _instance.Value; } }

        /* Insert to SGNotiInfo */
        public bool InsertNotiInfo(int groupId, LSIDEBAR categoryId, string path, string iconImage, string head, string body)
        {
            // Create
            Log.Information("Inserting a new NotiInfo, {NotiHead}, {NotiBody}", head, body);
            DBCtx.Add(new SGNotiData 
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
            DBCtx.SaveChanges();
            return true;
        }
        /* Select * from SGNotiInfo */
        public List<SGNotiData> SelectNotiInfoLimit(int groupId, int nLimit)
        {
            List<SGNotiData> NotiList;
            // Read
            NotiList = DBCtx.Notis
                .Where(x => x.GroupId == groupId)
                .OrderByDescending(x => x.Time).Take(nLimit)
                .ToList();
            Log.Information("Querying for a NotiInfo Limit {nLimit}", nLimit);
            return NotiList;
        }
        /* Select count(*) from SGNotiInfo */
        public int SelectNotiInfoCount(int groupId)
        {
            int nCount;
            // Read
            nCount = DBCtx.Notis
                .Where(x => x.GroupId == groupId)
                .Count();
            Log.Information("Querying for a NotiInfo Count {nCount}", nCount);
            return nCount;
        }

        /* Select group by count(*) from SGNotiInfo of CategoryId */
        public Dictionary<LSIDEBAR, int> SelectNotiInfoCategoryCount(int groupId)
        {
            Dictionary<LSIDEBAR, int> NotiDic;
            NotiDic = DBCtx.Notis
                        .Where(Noti => Noti.GroupId == groupId)
                        .GroupBy(Noti => Noti.CategoryId)
                        .Select(Noti => new
                                    {
                                        CategoryId = Noti.Key,
                                        CategoryCount = Noti.Count()
                                    }
                        )
                        .OrderBy(Noti => Noti.CategoryId)
                        .ToDictionary(Noti => Noti.CategoryId, Noti => Noti.CategoryCount);
            return NotiDic;
        }

        /* Delete from SGNotiInfo */
        public bool DeleteNotiInfo(SGNotiData notiData)
        {
            // Delete
            DBCtx.Remove(notiData);
            DBCtx.SaveChanges();
            Log.Information("Delete the SGNotiData, {NotiData}", notiData);

            return true;
        }
        /* Insert to SGAlarmInfo */
        public bool InsertAlarmInfo(int groupId, LSIDEBAR categoryId, string path, string iconImage, string head, string body)
        {
            // Create
            Log.Information("Inserting a new AlarmInfo, {AlarmHead}, {AlarmBody}", head, body);
            DBCtx.Add(new SGAlarmData 
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
            DBCtx.SaveChanges();
            return true;
        }
        /* Select * from SGAlarmInfo */
        public List<SGAlarmData> SelectAlarmInfoLimit(int groupId, int nLimit)
        {
            List<SGAlarmData> AlarmList;
            // Read
            AlarmList = DBCtx.Alarms
                .Where(x => x.GroupId == groupId)
                .OrderByDescending(x => x.Time).Take(nLimit)
                .ToList();
            Log.Information("Querying for a AlarmInfo Limit {nLimit}", nLimit);
            return AlarmList;
        }
        /* Select count(*) from SGAlarmInfo */
        public int SelectAlarmInfoCount(int groupId)
        {
            int nCount;
            // Read
            nCount = DBCtx.Alarms
                .Where(x => x.GroupId == groupId)
                .Count();
            Log.Information("Querying for a AlarmInfo Count {nCount}", nCount);
            return nCount;
        }

        /* Select group by count(*) from SGAlarmInfo of CategoryId */
        public Dictionary<LSIDEBAR, int> SelectAlarmInfoCategoryCount(int groupId)
        {
            Dictionary<LSIDEBAR, int> AlarmDic;
            AlarmDic = DBCtx.Alarms
                        .Where(Alarm => Alarm.GroupId == groupId)
                        .GroupBy(Alarm => Alarm.CategoryId)
                        .Select(Alarm => new
                                    {
                                        CategoryId = Alarm.Key,
                                        CategoryCount = Alarm.Count()
                                    }
                        )
                        .OrderBy(Alarm => Alarm.CategoryId)
                        .ToDictionary(Alarm => Alarm.CategoryId, Alarm => Alarm.CategoryCount);
            return AlarmDic;
        }

        /* Delete from SGAlarmInfo */
        public bool DeleteAlarmInfo(SGAlarmData alarmData)
        {
            // Delete
            DBCtx.Remove(alarmData);
            DBCtx.SaveChanges();
            Log.Information("Delete the SGAlarmData, {AlarmData}", alarmData);

            return true;
        }
    }
}