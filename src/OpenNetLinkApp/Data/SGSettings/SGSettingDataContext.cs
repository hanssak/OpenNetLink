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
                .UseSqlite(@"Data Source=SGSettingsDB.db;");
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
        // SQLite DB Context
        private SGSettingDataContext DBSDataCtx { get; set; }
        //private 생성자 
        private SGSettingsDBProc() 
        { 
            DBSDataCtx = new SGSettingDataContext();
        }
        //private static 인스턴스 객체
        private static readonly Lazy<SGSettingsDBProc> _instance = new Lazy<SGSettingsDBProc> (() => new SGSettingsDBProc());
        //public static 의 객체반환 함수
        public static SGSettingsDBProc Instance { get { return _instance.Value; } }

        /* Insert to SGSettingData */
        public bool InsertSettingData(int groupId, string uid, string upw, string apprline, string delayDspPw, int autoLoginUse)
        {
            // Create
            Log.Information($"Inserting a SettingData, GROUPID:{groupId}, UID:{uid}, APPRLINE:{apprline}, DELAYDSPPW:{delayDspPw}, AUTOLOGIN:{autoLoginUse}");
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
            return true;
        }
        /* Update to SGSettingData */
        public bool UpdateSettingData(int groupId, string uid, string upw, string apprline, string delayDspPw, int autoLoginUse)
        {
            // Create
            Log.Information($"Updating a SettingData, GROUPID:{groupId}, UID:{uid}, APPRLINE:{apprline}, DELAYDSPPW:{delayDspPw}, AUTOLOGIN:{autoLoginUse}");
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
            return true;
        }
        public bool UpdateSettingDataObj(SGSettingData obj)
        {
            // Create
            Log.Information($"Updating a SettingData, GROUPID:{obj.GROUPID}, UID:{obj.UID}, APPRLINE:{obj.APPRLINE}, DELAYDSPPW:{obj.DELAYDISPLAYPW}, AUTOLOGIN:{obj.AUTOLOGINING}");
            DBSDataCtx.Update(obj);
            DBSDataCtx.SaveChanges();
            return true;
        }
        public bool SetSettingUID(int groupId, string uid)
        {
            // Create
            Log.Information($"Set a SettingData, GROUPID({groupId})=>UID({uid})");
            SGSettingData SData;
            SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            SData.UID = uid;
            DBSDataCtx.SaveChanges();
            return true;
        }
        public bool SetSettingUPW(int groupId, string upw)
        {
            // Create
            Log.Information($"Set a SettingData, GROUPID({groupId})=>UPW({upw})");
            SGSettingData SData;
            SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            SData.UPW = upw;
            DBSDataCtx.SaveChanges();
            return true;
        }
        public bool SetSettingApprLine(int groupId, string apprline)
        {
            // Create
            Log.Information($"Set a SettingData, GROUPID({groupId})=>APPRLINE({apprline})");
            SGSettingData SData;
            SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            SData.APPRLINE = apprline;
            DBSDataCtx.SaveChanges();
            return true;
        }
        public bool SetSettingDelayDspPw(int groupId, string delayDspPw)
        {
            // Create
            Log.Information($"Set a SettingData, GROUPID({groupId})=>DELAYDISPLAYPW({delayDspPw})");
            SGSettingData SData;
            SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            SData.DELAYDISPLAYPW = delayDspPw;
            DBSDataCtx.SaveChanges();
            return true;
        }
        public bool SetSettingAutoLogin(int groupId, bool autoLogin)
        {
            // Create
            Log.Information($"Set a SettingData, GROUPID({groupId})=>AUTOLOGINING({autoLogin})");
            SGSettingData SData;
            SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            SData.AUTOLOGINING = autoLogin?1:0;
            DBSDataCtx.SaveChanges();
            return true;
        }
        /* Select * from SGSettingDataUpdate */
        public List<SGSettingData> SelectSettingDataList(int groupId, int nLimit)
        {
            List<SGSettingData> SDataList;
            // Read
            SDataList = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId).Take(nLimit)
                .ToList();
            Log.Information($"Querying for a SGSettingData Limit {nLimit}");
            return SDataList;
        }
        public SGSettingData SelectSettingData(int groupId)
        {
            SGSettingData SData;
            // Read
            SData = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .FirstOrDefault();
            Log.Information($"Querying for a SGSettingData {SData}");
            return SData;
        }
        public string GetSettingUID(int groupId)
        {
            string strUID;
            // Read
            strUID = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.UID)
                .FirstOrDefault();
            Log.Information($"Get for a SGSettingData, GroupId({groupId})=>UID({strUID})");
            return strUID;
        }
        public string GetSettingUPW(int groupId)
        {
            string strUPW;
            // Read
            strUPW = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.UPW)
                .FirstOrDefault();
            Log.Information($"Get for a SGSettingData, GroupId({groupId})=>UPW({strUPW})");
            return strUPW;
        }
        public string GetSettingApprLine(int groupId)
        {
            string strApprLine;
            // Read
            strApprLine = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.APPRLINE)
                .FirstOrDefault();
            Log.Information($"Get for a SGSettingData, GroupId({groupId})=>APPRLINE({strApprLine})");
            return strApprLine;
        }
        public string GetSettingDelayDspPw(int groupId)
        {
            string strDelayDspPw;
            // Read
            strDelayDspPw = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.DELAYDISPLAYPW)
                .FirstOrDefault();
            Log.Information($"Get for a SGSettingData, GroupId({groupId})=>DELAYDISPLAYPW({strDelayDspPw})");
            return strDelayDspPw;
        }
        public bool GetSettingAutoLogin(int groupId)
        {
            int nAutoLogin;
            // Read
            nAutoLogin = DBSDataCtx.setting
                .Where(x => x.GROUPID == groupId)
                .Select(x => x.AUTOLOGINING)
                .FirstOrDefault();
            bool bAutoLogin = nAutoLogin==1?true:false;
            Log.Information($"Get for a SGSettingData, GroupId({groupId})=>DELAYDISPLAYPW({bAutoLogin})");
            return bAutoLogin;
        }

        /* Delete from SGSettingData */
        public bool DeleteSettingData(SGSettingData settingData)
        {
            // Delete
            DBSDataCtx.Remove(settingData);
            DBSDataCtx.SaveChanges();
            Log.Information($"Delete the SGSettingData, {settingData}");

            return true;
        }
    }
}