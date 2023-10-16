using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGLogData;
using Serilog;
using AgLogManager;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenNetLinkApp.Data.SGLogData
{
    /// <summary>
    /// Implement SQLite DB Context 
    /// </summary>
    public class SGLogDataContext : DbContext
    {
        public DbSet<SGTransferLogData> TransferLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=wwwroot/db/SGLogDB.db;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /* TABLE: T_SG_TRANSFER_LOG */
            modelBuilder.Entity<SGTransferLogData>()
                        .ToTable("T_SG_TRANSFER_LOG");

            modelBuilder.Entity<SGTransferLogData>()
                        .Property(c => c.Id).IsRequired()
                        .HasAnnotation("Sqlite:Autoincrement", true)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<SGTransferLogData>()
                        .Property(k => k.GroupId).IsRequired();
            modelBuilder.Entity<SGTransferLogData>()
                          .HasKey(k => k.Id).HasName("PK_T_SG_TRANSFER_LOG_ID");


            modelBuilder.Entity<SGTransferLogData>()
                        .Property(k => k.UserSeq).HasColumnType("varchar(512)").IsRequired();
            modelBuilder.Entity<SGTransferLogData>()
                        .Property(k => k.LogDate).HasColumnType("varchar(512)").IsRequired();
            modelBuilder.Entity<SGTransferLogData>()
                        .Property(k => k.LogDatetime).HasColumnType("varchar(512)").IsRequired();
            modelBuilder.Entity<SGTransferLogData>()
                        .Property(k => k.Time).HasColumnType("TEXT").HasDefaultValueSql("datetime('now','localtime')").IsRequired();
        }
    }

    /// <summary>
    /// Singleton SGNtfyDBProc
    /// Implement SQLite DB Operate Function Object
    /// </summary>
    public sealed class SGLogDBProc
    {
        // SQLite DB Context        
        private SGLogDataContext DBSDataCtx { get; set; }

        private static Mutex mut = new Mutex();
        //private 생성자 
        private SGLogDBProc()
        {
            DBSDataCtx = new SGLogDataContext();
            if (!DBSDataCtx.Database.GetPendingMigrations().Any())
                DBSDataCtx.Database.Migrate();
        }
        //private static 인스턴스 객체
        private static readonly Lazy<SGLogDBProc> _instance = new Lazy<SGLogDBProc>(() => new SGLogDBProc());
        //public static 의 객체반환 함수
        public static SGLogDBProc Instance { get { return _instance.Value; } }

        /* Insert to SGSettingData */
        public bool InsertTransferLogInfo(int groupId, string userSeq, DateTime LogTime, string LogContents)
        {
            try
            {
                // Create
                mut.WaitOne();
                Log.Logger.Here().Information($"Inserting a TransferLog, GROUPID:{groupId}, USERSEQ:{userSeq}, LOGTIME:{LogTime.ToString()}");

                DBSDataCtx.Add(new SGTransferLogData
                {
                    GroupId = groupId,
                    UserSeq = userSeq,
                    LogDate = LogTime.ToString("yyyy-MM-dd"),
                    LogDatetime = LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    Log = LogContents
                }
                    );
                DBSDataCtx.SaveChanges();
                mut.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"Inserting a TransferLog(ERROR:{ex.Message}, GROUPID:{groupId}, USERSEQ:{userSeq}, LOGTIME:{LogTime.ToString()}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 기준 날짜를 받아 그 이전의 로그 모두 삭제
        /// </summary>
        /// <param name="standardDate"></param>
        /// <param name="bIsAlarm"></param>
        /// <returns></returns>
        public bool DeleteAllInfo(DateTime standardDate, bool bIsTransfer)
        {
            try
            {
                mut.WaitOne();
                // Delete
                if (bIsTransfer)
                {
                    List<SGTransferLogData> TransferLogDic = null;
                    TransferLogDic = DBSDataCtx.TransferLog.
                        Where(x => x.Time != null && x.Time < standardDate.Date).
                        ToList<SGTransferLogData>();

                    if (TransferLogDic != null && TransferLogDic.Count > 0)
                    {
                        DBSDataCtx.TransferLog.RemoveRange(TransferLogDic);
                        DBSDataCtx.SaveChanges();
                    }
                }
               
                Log.Logger.Here().Information($"Delete the Data, StandardDate Before Data");
                mut.ReleaseMutex();
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Here().Error($"Delete the Data(ERROR:{e.Message}) StandardDate Before Data");
                return false;
            }
        }
        /// <summary>
        /// Select * from SGSettingDataUpdate
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="nLimit"></param>
        /// <returns></returns>
        public List<SGTransferLogData> SelectTransferLogDataList(int groupId, string userSeq)
        {
            mut.WaitOne();

            // Read
            List<SGTransferLogData> SDataList = DBSDataCtx.TransferLog
                                                .Where(x=>x.GroupId == groupId && x.UserSeq == userSeq;
            


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

            bool bAutoLogin = nAutoLogin == 1 ? true : false;
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
