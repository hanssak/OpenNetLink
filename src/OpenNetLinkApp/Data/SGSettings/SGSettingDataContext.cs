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
    public class SGSettingDataContext : DbContext
    {
        public DbSet<SGSettingData> setting { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=wwwroot/db/SGSettingsDB.db;");
            base
                .OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*
            <SGSettingData>
                [Key]
                public int    GROUPID { get; set; }
                public string UID { get; set; } = String.Empty;
                public string UPW { get; set; } = String.Empty;
                public string APPRLINE { get; set; } = String.Empty;
                public string DELAYDISPLAYPW { get; set; } = String.Empty;
                public int    AUTOLOGINING { get; set; } = 0;
            */

            /* TABLE: SGSettingData */
            modelBuilder.Entity<SGSettingData>()
                        .ToTable("T_SG_SETTING_DATA");
            modelBuilder.Entity<SGSettingData>()
                        .Property(c => c.GROUPID).IsRequired()
                        .HasAnnotation("Sqlite:Autoincrement", false)
                        .ValueGeneratedNever()
                        .HasColumnType("INTEGER");
            modelBuilder.Entity<SGSettingData>()
                        .HasKey(k => k.GROUPID).HasName("PK_T_SG_SETTING_DATA_GROUPID");
            modelBuilder.Entity<SGSettingData>()
                        .Property(c => c.UID).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGSettingData>()
                        .Property(c => c.UPW).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGSettingData>()
                        .Property(c => c.APPRLINE).HasColumnType("varchar(2048)");
            modelBuilder.Entity<SGSettingData>()
                        .Property(c => c.DELAYDISPLAYPW).HasColumnType("TEXT");
            modelBuilder.Entity<SGSettingData>()
                        .Property(c => c.AUTOLOGINING).HasColumnType("INTEGER");
        }
    }
    /// <summary>
    /// Singleton SGNtfyDBProc
    /// Implement SQLite DB Operate Function Object
    /// </summary>
    public sealed class SGSettingsDBProc
    {

        // thread safe
        static readonly object _locker = new object();

        // SQLite DB Context        
        private SGSettingDataContext DBSDataCtx { get; set; }
        //private 생성자 
        private SGSettingsDBProc() 
        { 
            DBSDataCtx = new SGSettingDataContext();
            if (!DBSDataCtx.Database.GetPendingMigrations().Any())
                DBSDataCtx.Database.Migrate();
        }
        //private static 인스턴스 객체
        private static readonly Lazy<SGSettingsDBProc> _instance = new Lazy<SGSettingsDBProc> (() => new SGSettingsDBProc());
        //public static 의 객체반환 함수
        public static SGSettingsDBProc Instance { get { return _instance.Value; } }

        /* Insert to SGSettingData */
        public bool InsertSettingData(int groupId, string uid, string upw, string apprline, string delayDspPw, int autoLoginUse)
        {
            // Create
            Log.Logger.Here().Information($"Inserting a SettingData, GROUPID:{groupId}, UID:{uid}, APPRLINE:{apprline}, DELAYDSPPW:{delayDspPw}, AUTOLOGIN:{autoLoginUse}");

            lock (_locker)
            {
                DBSDataCtx.Add(new SGSettingData
                {
                    GROUPID = groupId,
                    UID = uid,
                    UPW = upw,
                    APPRLINE = apprline,
                    DELAYDISPLAYPW = delayDspPw,
                    AUTOLOGINING = autoLoginUse
                }
                    );
                DBSDataCtx.SaveChanges();
            }

            return true;
        }
        /* Update to SGSettingData */
        public bool UpdateSettingData(int groupId, string uid, string upw, string apprline, string delayDspPw, int autoLoginUse)
        {
            // Create
            Log.Logger.Here().Information($"Updating a SettingData, GROUPID:{groupId}, UID:{uid}, APPRLINE:{apprline}, DELAYDSPPW:{delayDspPw}, AUTOLOGIN:{autoLoginUse}");

            lock (_locker)
            {
                DBSDataCtx.Update(new SGSettingData
                {
                    GROUPID = groupId,
                    UID = uid,
                    UPW = upw,
                    APPRLINE = apprline,
                    DELAYDISPLAYPW = delayDspPw,
                    AUTOLOGINING = autoLoginUse
                }
                    );
                DBSDataCtx.SaveChanges();
            }

            return true;
        }
        public bool UpdateSettingDataObj(SGSettingData obj)
        {
            // Create
            Log.Logger.Here().Information($"Updating a SettingData, GROUPID:{obj.GROUPID}, UID:{obj.UID}, APPRLINE:{obj.APPRLINE}, DELAYDSPPW:{obj.DELAYDISPLAYPW}, AUTOLOGIN:{obj.AUTOLOGINING}");
            lock (_locker)
            {
                DBSDataCtx.Update(obj);
                DBSDataCtx.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool SetSettingUID(int groupId, string uid)
        {
            // Create
            Log.Logger.Here().Information($"Set a SettingData, GROUPID({groupId})=>UID({uid})");

            SGSettingData SData;
            lock (_locker)
            {
                SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
                SData.UID = uid;
                DBSDataCtx.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="upw"></param>
        /// <returns></returns>
        public bool SetSettingUPW(int groupId, string upw)
        {
            // Create
            Log.Logger.Here().Information($"Set a SettingData, GROUPID({groupId})=>UPW({upw})");
            SGSettingData SData;

            lock (_locker)
            {
                SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
                SData.UPW = upw;
                DBSDataCtx.SaveChanges();
            }

            return true;
        }
        public bool SetSettingApprLine(int groupId, string apprline)
        {
            // Create
            Log.Logger.Here().Information($"Set a SettingData, GROUPID({groupId})=>APPRLINE({apprline})");
            SGSettingData SData;

            lock (_locker)
            {
                SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
                SData.APPRLINE = apprline;
                DBSDataCtx.SaveChanges();
            }

            return true;
        }
        public bool SetSettingDelayDspPw(int groupId, string delayDspPw)
        {
            // Create
            Log.Logger.Here().Information($"Set a SettingData, GROUPID({groupId})=>DELAYDISPLAYPW({delayDspPw})");
            SGSettingData SData;

            lock (_locker)
            {
                SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
                SData.DELAYDISPLAYPW = delayDspPw;
                DBSDataCtx.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="autoLogin"></param>
        /// <returns></returns>
        public bool SetSettingAutoLogin(int groupId, bool autoLogin)
        {
            // Create
            Log.Logger.Here().Information($"Set a SettingData, GROUPID({groupId})=>AUTOLOGINING({autoLogin})");
            SGSettingData SData;

            lock (_locker)
            {
                SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
                SData.AUTOLOGINING = autoLogin ? 1 : 0;
                DBSDataCtx.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// Select * from SGSettingDataUpdate
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="nLimit"></param>
        /// <returns></returns>
        public List<SGSettingData> SelectSettingDataList(int groupId, int nLimit)
        {
            List<SGSettingData> SDataList;
            // Read

            lock (_locker)
            {
                SDataList = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId).Take(nLimit)
                .ToList();
            }

            Log.Logger.Here().Information($"Querying for a SGSettingData Limit {nLimit}");
            return SDataList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public SGSettingData SelectSettingData(int groupId)
        {
            SGSettingData SData;
            // Read

            lock (_locker)
            {
                SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            }
            Log.Logger.Here().Information($"Querying for a SGSettingData {SData}");
            return SData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public string GetSettingUID(int groupId)
        {
            string strUID;

            lock (_locker)
            {
                // Read
                strUID = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.UID)
                .FirstOrDefault();
            }

            Log.Logger.Here().Information($"Get for a SGSettingData, GroupId({groupId})=>UID({strUID})");
            return strUID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public string GetSettingUPW(int groupId)
        {
            string strUPW;

            lock (_locker)
            {
                // Read
                strUPW = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.UPW)
                .FirstOrDefault();
            }

            Log.Logger.Here().Information($"Get for a SGSettingData, GroupId({groupId})=>UPW({strUPW})");
            return strUPW;
        }

        public string GetSettingApprLine(int groupId)
        {
            string strApprLine;
            // Read

            lock (_locker)
            {
                strApprLine = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.APPRLINE)
                .FirstOrDefault();
            }

            Log.Logger.Here().Information($"Get for a SGSettingData, GroupId({groupId})=>APPRLINE({strApprLine})");
            return strApprLine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public string GetSettingDelayDspPw(int groupId)
        {
            string strDelayDspPw;

            lock (_locker)
            {
                // Read
                strDelayDspPw = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.DELAYDISPLAYPW)
                .FirstOrDefault();
            }

            Log.Logger.Here().Information($"Get for a SGSettingData, GroupId({groupId})=>DELAYDISPLAYPW({strDelayDspPw})");
            return strDelayDspPw;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool GetSettingAutoLogin(int groupId)
        {
            int nAutoLogin;

            lock (_locker)
            {
                // Read
                nAutoLogin = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.AUTOLOGINING)
                .FirstOrDefault();
            }

            bool bAutoLogin = nAutoLogin==1?true:false;
            Log.Logger.Here().Information($"Get for a SGSettingData, GroupId({groupId})=>DELAYDISPLAYPW({bAutoLogin})");
            return bAutoLogin;
        }

        /// <summary>
        /// Delete from SGSettingData
        /// </summary>
        /// <param name="settingData"></param>
        /// <returns></returns>
        public bool DeleteSettingData(SGSettingData settingData)
        {
            // Delete
            lock (_locker)
            {
                DBSDataCtx.Remove(settingData);
                DBSDataCtx.SaveChanges();
            }
            Log.Logger.Here().Information($"Delete the SGSettingData, {settingData}");

            return true;
        }
    }
}