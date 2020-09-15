using System.ComponentModel;
using System.Collections.Specialized;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGSettings;
using Serilog;
using Serilog.Events;
using AgLogManager;

namespace OpenNetLinkApp.Data.SGSettings
{
    /// <summary>
    /// Implement SQLite DB Context 
    /// </summary>
    public class SGLoginDataContext : DbContext
    {
        public DbSet<SGLoginData> Login { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=SGSettingsDB.db;");
            base
                .OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*
            <SGLoginData>
                [Key]
                public int    GROUPID { get; set; }
                public string UID { get; set; } = String.Empty;
                public string UPW { get; set; } = String.Empty;
                public string APPRLINE { get; set; } = String.Empty;
                public DateTime? DELAYDISPLAYPW { get; set; } = null;
                public int    AUTOLOGINING { get; set; }
            */

            /* TABLE: SGLoginData */
            modelBuilder.Entity<SGLoginData>()
                        .ToTable("T_SG_LOGIN_DATA");
            modelBuilder.Entity<SGLoginData>()
                        .Property(c => c.GROUPID).IsRequired()
                        .HasColumnType("INTEGER");
            modelBuilder.Entity<SGLoginData>()
                        .HasKey(k => k.GROUPID).HasName("PK_T_SG_LOGIN_DATA_GROUPID");
            modelBuilder.Entity<SGLoginData>()
                        .Property(c => c.UID).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGLoginData>()
                        .Property(c => c.UPW).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGLoginData>()
                        .Property(c => c.APPRLINE).HasColumnType("varchar(2048)");
            modelBuilder.Entity<SGLoginData>()
                        .Property(c => c.DELAYDISPLAYPW).HasColumnType("TEXT");
            modelBuilder.Entity<SGLoginData>()
                        .Property(c => c.AUTOLOGINING).HasColumnType("INTEGER");
        }
    }
    /// <summary>
    /// Singleton SGNtfyDBProc
    /// Implement SQLite DB Operate Function Object
    /// </summary>
    public sealed class SGSettingsDBProc
    {
        // SQLite DB Context
        private SGLoginDataContext DBLDataCtx { get; set; }
        //private 생성자 
        private SGSettingsDBProc() 
        { 
            DBLDataCtx = new SGLoginDataContext();
        }
        //private static 인스턴스 객체
        private static readonly Lazy<SGSettingsDBProc> _instance = new Lazy<SGSettingsDBProc> (() => new SGSettingsDBProc());
        //public static 의 객체반환 함수
        public static SGSettingsDBProc Instance { get { return _instance.Value; } }

        /* Insert to SGLoginData */
        public bool InsertLoginData(int groupId, string uid, string upw, string apprline, DateTime delayDspPw, int autoLoginUse)
        {
            // Create
            Log.Information($"Inserting a LoginData, GROUPID:{groupId}, UID:{uid}, APPRLINE:{apprline}, DELAYDSPPW:{delayDspPw}, AUTOLOGIN:{autoLoginUse}");
            DBLDataCtx.Add(new SGLoginData 
                        { 
                            GROUPID = groupId, 
                            UID = uid,
                            UPW = upw, 
                            APPRLINE = apprline, 
                            DELAYDISPLAYPW = delayDspPw, 
                            AUTOLOGINING = autoLoginUse
                        }
                    );
            DBLDataCtx.SaveChanges();
            return true;
        }
        /* Insert to SGLoginData */
        public bool UpdateLoginData(int groupId, string uid, string upw, string apprline, DateTime delayDspPw, int autoLoginUse)
        {
            // Create
            Log.Information($"Updating a LoginData, GROUPID:{groupId}, UID:{uid}, APPRLINE:{apprline}, DELAYDSPPW:{delayDspPw}, AUTOLOGIN:{autoLoginUse}");
            DBLDataCtx.Update(new SGLoginData 
                        { 
                            GROUPID = groupId, 
                            UID = uid,
                            UPW = upw, 
                            APPRLINE = apprline, 
                            DELAYDISPLAYPW = delayDspPw, 
                            AUTOLOGINING = autoLoginUse
                        }
                    );
            DBLDataCtx.SaveChanges();
            return true;
        }
        /* Select * from SGLoginDatUpdate */
        public List<SGLoginData> SelectLoginData(int groupId, int nLimit)
        {
            List<SGLoginData> LDataList;
            // Read
            LDataList = DBLDataCtx.Login
                .Where(x => x.GROUPID == groupId).Take(nLimit)
                .ToList();
            Log.Information($"Querying for a SGLoginData Limit {nLimit}");
            return LDataList;
        }

        /* Delete from SGLoginData */
        public bool DeleteLoginData(SGLoginData loginData)
        {
            // Delete
            DBLDataCtx.Remove(loginData);
            DBLDataCtx.SaveChanges();
            Log.Information($"Delete the SGLoginData, {loginData}");

            return true;
        }
    }
}