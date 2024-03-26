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
using OpenNetLinkApp.Services;
using System.Threading;
using System.Data;


namespace OpenNetLinkApp.Data.SGNotify
{
    /// <summary>
    /// Implement SQLite DB Context 
    /// </summary>
    public class SGNotifyContext : DbContext
    {
        public DbSet<SGAlarmData> Alarms { get; set; }
        public DbSet<SGNotiData> Notis { get; set; }

        public DbSet<SGReSendData> ReSend { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=wwwroot/db/SGNotifyDB.db;");
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
                        .Property(c => c.UserSeq).IsRequired();
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
                        .Property(c => c.Type).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.GroupId).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.UserSeq).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Seq).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.CategoryId).IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .HasKey(k => k.Id).HasName("PK_T_SG_NOTI_ID");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Path).HasColumnType("varchar(64)");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.IconImage).HasColumnType("varchar(128)");
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Head).HasColumnType("varchar(256)").IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Body).HasColumnType("varchar(4096)").IsRequired();
            modelBuilder.Entity<SGNotiData>()
                        .Property(c => c.Time).HasColumnType("TEXT").HasDefaultValueSql("datetime('now','localtime')").IsRequired();
            /* TABLE: SGReSendData */
            modelBuilder.Entity<SGReSendData>()
                        .ToTable("T_SG_RESEND");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.RESENDID).IsRequired()
                        .HasAnnotation("Sqlite:Autoincrement", true)
                        .ValueGeneratedOnAdd();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.GROUPID).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.USERSEQ).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.CLIENTID).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.MID).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.HSZNAME).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.ISEND).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.TRANSINFO).IsRequired();
            modelBuilder.Entity<SGReSendData>()
                        .HasKey(k => k.RESENDID).HasName("PK_T_SG_RESEND_ID");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.GROUPID).HasColumnType("INT");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.USERSEQ).HasColumnType("varchar(512)");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.CLIENTID).HasColumnType("varchar(512)");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.MID).HasColumnType("varchar(512)");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.HSZNAME).HasColumnType("varchar(512)");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.ISEND).HasColumnType("BOOLEAN");
            modelBuilder.Entity<SGReSendData>()
                        .Property(c => c.TRANSINFO).HasColumnType("BLOB");
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

        private static Mutex mut = new Mutex();
        //private 생성자 
        private SGNtfyDBProc() 
        { 
            DBCtx = new SGNotifyContext();
            if (DBCtx.Database.GetPendingMigrations().Any())
            {
                var migrationList = DBCtx.Database.GetPendingMigrations();
                foreach(var migration in migrationList)
                {
                    if(migration.Contains("20221026070638"))
                    {
                        if(TableExists("T_SG_RESEND"))
                        {
                            InsertMigration("20221026070638_InitialCreate1", "3.1.6");                        
                        }
                        else
                            DBCtx.Database.Migrate();
                    }
                }

                DBCtx.Database.Migrate();
                DBCtx.SaveChanges();
            }

        }
        //private static 인스턴스 객체
        private static readonly Lazy<SGNtfyDBProc> _instance = new Lazy<SGNtfyDBProc> (() => new SGNtfyDBProc());
        //public static 의 객체반환 함수
        public static SGNtfyDBProc Instance { get { return _instance.Value; } }

        public bool TableExists(string tableName)
        {
            var connection = DBCtx.Database.GetDbConnection();

            if (connection.State.Equals(ConnectionState.Closed))
                connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 1 FROM sqlite_master
            WHERE type = 'table'
            AND name = @TableName";

                var tableNameParam = command.CreateParameter();
                tableNameParam.ParameterName = "@TableName";
                tableNameParam.Value = tableName;
                command.Parameters.Add(tableNameParam);

                return command.ExecuteScalar() != null;
            }
        }

        public bool InsertMigration(string id, string version)
        {
            var connection = DBCtx.Database.GetDbConnection();

            if (connection.State.Equals(ConnectionState.Closed))
                connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            INSERT INTO __EFMigrationsHistory
            VALUES (@id, @version)";

                var idParam = command.CreateParameter();
                idParam.ParameterName = "@id";
                idParam.Value = id;
                command.Parameters.Add(idParam);

                var versionParam = command.CreateParameter();
                versionParam.ParameterName = "@version";
                versionParam.Value = version;
                command.Parameters.Add(versionParam);

                return command.ExecuteScalar() != null;
            }
        }
        /* Insert to SGReSendInfo */
        public bool InsertReSendInfo(int groupId, string userSeq, string clientId, string mid, string hszName, object transInfo)
        {
            // Create
            mut.WaitOne();
            Log.Logger.Here().Information("Inserting a ReSendInfo, {userSeq}, {clientId}, {mid}", userSeq
                                                                                                , HsNetWorkSG.HsNetWork.UseHiddenLog ? clientId : "xxx"
                                                                                                , HsNetWorkSG.HsNetWork.UseHiddenLog ? mid : "xxx");
            DBCtx.Add(new SGReSendData
            {
                RESENDID = 0,
                GROUPID = groupId,
                USERSEQ = userSeq,
                CLIENTID = clientId,
                MID = mid,
                HSZNAME = hszName,
                ISEND = false,
                TRANSINFO = transInfo
            });
            DBCtx.SaveChanges();
            mut.ReleaseMutex();
            return true;
        }
        public SGReSendData SelectReSendInfo(int groupId, string userSeq)
        {
            mut.WaitOne();
            SGReSendData reSendData;
            // Read

            reSendData = DBCtx.ReSend
                    .Where(x => x.GROUPID == groupId && x.USERSEQ == userSeq && x.ISEND == false)
                    .OrderByDescending(x => x.RESENDID).FirstOrDefault();
            
            Log.Logger.Here().Debug("Querying for a ReSendInfo");
            mut.ReleaseMutex();
            return reSendData;
        }
        public SGReSendData SelectReSendInfo(int reSendDataId)
        {
            mut.WaitOne();
            SGReSendData reSendData;
            // Read

            reSendData = DBCtx.ReSend
                    .Where(x => x.RESENDID == reSendDataId)
                    .FirstOrDefault();

            Log.Logger.Here().Debug("Querying for a ReSendInfo");
            mut.ReleaseMutex();
            return reSendData;
        }
        public bool UpdateReSendInfo(int groupId, string userSeq, bool isEnd)
        {
            mut.WaitOne();
            SGReSendData reSendData;
            // Read

            reSendData = DBCtx.ReSend
                    .Where(x => x.GROUPID== groupId && x.USERSEQ == userSeq && x.ISEND == false)
                    .OrderByDescending(x => x.RESENDID).FirstOrDefault();

            if (reSendData != null)
                reSendData.ISEND = isEnd;

            DBCtx.SaveChanges();

            Log.Logger.Here().Information("Update ReSendInfo");
            mut.ReleaseMutex();
            return true;
        }
        public bool UpdateReSendInfo(int groupId, string userSeq, object transInfo)
        {
            mut.WaitOne();
            SGReSendData reSendData;
            // Read

            reSendData = DBCtx.ReSend
                    .Where(x => x.GROUPID == groupId && x.USERSEQ == userSeq && x.ISEND == false)
                    .OrderByDescending(x => x.RESENDID).FirstOrDefault();

            if (reSendData != null)
                reSendData.TRANSINFO = transInfo;

            DBCtx.SaveChanges();

            Log.Logger.Here().Information("Update ReSendInfo");
            mut.ReleaseMutex();
            return true;
        }

        public bool DeleteReSendInfo(int groupId, string userSeq)
        {
            mut.WaitOne();
            SGReSendData reSendData;
            // Read

            reSendData = DBCtx.ReSend
                    .Where(x => x.GROUPID == groupId && x.USERSEQ == userSeq)
                    .OrderByDescending(x => x.RESENDID).FirstOrDefault();

            if (reSendData != null)
            {
                DBCtx.ReSend.Remove(reSendData);
            }

            DBCtx.SaveChanges();

            Log.Logger.Here().Information("Update ReSendInfo");
            mut.ReleaseMutex();
            return true;
        }

        public bool DeleteReSendInfo(int reSendDataId)
        {
            mut.WaitOne();
            SGReSendData reSendData;
            // Read

            reSendData = DBCtx.ReSend
                    .Where(x => x.RESENDID == reSendDataId)
                    .FirstOrDefault();

            if (reSendData != null)
            {
                DBCtx.ReSend.Remove(reSendData);
            }

            DBCtx.SaveChanges();

            Log.Logger.Here().Information("Update ReSendInfo");
            mut.ReleaseMutex();
            return true;
        }

        /* Insert to SGNotiInfo */
        public bool InsertNotiInfo(NOTI_TYPE type, int groupId, string userSeq, string seq, LSIDEBAR categoryId, string path, string iconImage, string head, string body)
        {
            // Create
            mut.WaitOne();
            Log.Logger.Here().Information("Inserting a NotiInfo, {NotiHead}, {NotiBody}", head, body);
            DBCtx.Add(new SGNotiData 
                        { 
                            Id = 0, 
                            Type = type,
                            GroupId = groupId, 
                            UserSeq = userSeq,
                            Seq = seq,
                            CategoryId = categoryId,
                            Path = path, 
                            IconImage = iconImage, 
                            Head = head, 
                            Body = body,
                            Time = DateTime.Now
                        }
                    );
            DBCtx.SaveChanges();
            mut.ReleaseMutex();
            return true;
        }
        /* Select * from SGNotiInfo */
        public List<SGNotiData> SelectNotiInfoLimit(NOTI_TYPE type, int groupId, string userSeq, int nLimit)
        {
            mut.WaitOne();
            List<SGNotiData> NotiList;
            // Read
            if(type == NOTI_TYPE.ALL) {
                NotiList = DBCtx.Notis
                    .Where(x => x.GroupId == groupId && x.UserSeq == userSeq)
                    .OrderByDescending(x => x.Time).Take(nLimit)
                    .ToList();
            } else {
                NotiList = DBCtx.Notis
                    .Where(x => x.Type == type && x.GroupId == groupId && x.UserSeq == userSeq)
                    .OrderByDescending(x => x.Time).Take(nLimit)
                    .ToList();
            }
            Log.Logger.Here().Debug($"Querying for a NotiInfo Limit {nLimit}");
            mut.ReleaseMutex();
            return NotiList;
        }

        /* Select count(*) from SGNotiInfo */
        public int SelectNotiInfoCount(NOTI_TYPE type, int groupId, string userSeq)
        {
            mut.WaitOne();
            int nCount;
            // Read
            if(type == NOTI_TYPE.ALL) {
                nCount = DBCtx.Notis
                    .Where(x => x.GroupId == groupId && x.UserSeq == userSeq)
                    .Count();
            } else {
                nCount = DBCtx.Notis
                    .Where(x => x.Type == type && x.GroupId == groupId && x.UserSeq == userSeq)
                    .Count();
            }
            Log.Logger.Here().Debug($"Querying for a NotiInfo Count {nCount}");
            mut.ReleaseMutex();
            return nCount;
        }

        /* Select group by count(*) from SGNotiInfo of CategoryId */
        public Dictionary<LSIDEBAR, int> SelectNotiInfoCategoryCount(NOTI_TYPE type, int groupId, string userSeq)
        {
            mut.WaitOne();
            Dictionary<LSIDEBAR, int> NotiDic;
            if(type == NOTI_TYPE.ALL) {
                NotiDic = DBCtx.Notis
                            .Where(x => x.GroupId == groupId && x.UserSeq == userSeq)
                            .GroupBy(x => x.CategoryId)
                            .Select(x => new
                                        {
                                            CategoryId = x.Key,
                                            CategoryCount = x.Count()
                                        }
                            )
                            .OrderBy(x => x.CategoryId)
                            .ToDictionary(x => x.CategoryId, x => x.CategoryCount);
            } else {
                NotiDic = DBCtx.Notis
                            .Where(x => x.Type == type && x.GroupId == groupId && x.UserSeq == userSeq)
                            .GroupBy(x => x.CategoryId)
                            .Select(x => new
                                        {
                                            CategoryId = x.Key,
                                            CategoryCount = x.Count()
                                        }
                            )
                            .OrderBy(x => x.CategoryId)
                            .ToDictionary(x => x.CategoryId, x => x.CategoryCount);
            }
            mut.ReleaseMutex();
            return NotiDic;
        }

        /* Delete from SGNotiInfo */
        public bool DeleteNotiInfo(SGNotiData notiData)
        {
            mut.WaitOne();

            int nCount = SelectNotiInfoCount(notiData.Type, notiData.GroupId, notiData.UserSeq);
            if (nCount < 1)
            {
                mut.ReleaseMutex();
                return true;
            }

            // Delete
            DBCtx.Remove(notiData);
            DBCtx.SaveChanges();
            Log.Logger.Here().Information($"Delete the SGNotiData, {notiData.GroupId}");
            mut.ReleaseMutex();

            return true;
        }
        /* Insert to SGAlarmInfo */
        public bool InsertAlarmInfo(int groupId, string userSeq, LSIDEBAR categoryId, string path, string iconImage, string head, string body)
        {
            mut.WaitOne();
            // Create
            Log.Logger.Here().Information($"Inserting a AlarmInfo, {head}, {body}");
            DBCtx.Add(new SGAlarmData 
                        { 
                            Id = 0, 
                            GroupId = groupId, 
                            UserSeq = userSeq,
                            CategoryId = categoryId,
                            Path = path, 
                            IconImage = iconImage, 
                            Head = head, 
                            Body = body,
                            Time = DateTime.Now
                        }
                    );
            DBCtx.SaveChanges();
            mut.ReleaseMutex();
            return true;
        }
        /* Select * from SGAlarmInfo */
        public List<SGAlarmData> SelectAlarmInfoLimit(int groupId, string userSeq, int nLimit)
        {
            mut.WaitOne();
            List<SGAlarmData> AlarmList;
            // Read
            AlarmList = DBCtx.Alarms
                .Where(x => x.GroupId == groupId && x.UserSeq == userSeq)
                .OrderByDescending(x => x.Time).Take(nLimit)
                .ToList();
            Log.Logger.Here().Debug($"Querying for a AlarmInfo Limit {nLimit}");
            mut.ReleaseMutex();
            return AlarmList;
        }
        /* Select count(*) from SGAlarmInfo */
        public int SelectAlarmInfoCount(int groupId, string userSeq)
        {
            mut.WaitOne();
            int nCount;
            // Read
            nCount = DBCtx.Alarms
                .Where(x => x.GroupId == groupId && x.UserSeq == userSeq)
                .Count();
            Log.Logger.Here().Debug($"Querying for a AlarmInfo Count {nCount}");
            mut.ReleaseMutex();
            return nCount;
        }

        /* Select group by count(*) from SGAlarmInfo of CategoryId */
        public Dictionary<LSIDEBAR, int> SelectAlarmInfoCategoryCount(int groupId, string userSeq)
        {
            mut.WaitOne();
            Dictionary<LSIDEBAR, int> AlarmDic;
            AlarmDic = DBCtx.Alarms
                        .Where(x => x.GroupId == groupId && x.UserSeq == userSeq)
                        .GroupBy(x => x.CategoryId)
                        .Select(x => new
                                    {
                                        CategoryId = x.Key,
                                        CategoryCount = x.Count()
                                    }
                        )
                        .OrderBy(x => x.CategoryId)
                        .ToDictionary(x => x.CategoryId, x => x.CategoryCount);
            mut.ReleaseMutex();
            return AlarmDic;
        }

        /// <summary>
        /// Delete from SGAlarmInfo
        /// </summary>
        /// <param name="alarmData"></param>
        /// <returns></returns>
        public bool DeleteAlarmInfo(SGAlarmData alarmData)
        {
            mut.WaitOne();
            // Delete
            DBCtx.Remove(alarmData);
            DBCtx.SaveChanges();
            Log.Logger.Here().Information("Delete the SGAlarmData, {alarmData.GroupId}");
            mut.ReleaseMutex();
            return true;
        }
        /// <summary>
        /// 그룹, 유저 아이디를 받아 Alram, Message 삭제
        /// </summary>
        /// <param name="nGroupID"></param>
        /// <param name="strUserSeq"></param>
        /// <param name="bIsAlarm"></param>
        /// <returns></returns>
        public bool DeleteAllInfo(int nGroupID, string strUserSeq, bool bIsAlarm)
        {

            try
            {
                mut.WaitOne();
                // Delete

                if (bIsAlarm)
                {
                    List<SGAlarmData> AlarmDic = null;
                    AlarmDic = DBCtx.Alarms
                                .Where(x => x.GroupId == nGroupID && x.UserSeq == strUserSeq).ToList<SGAlarmData>();
                    if (AlarmDic != null && AlarmDic.Count > 0)
                    {
                        DBCtx.Alarms.RemoveRange(AlarmDic);
                        //DBCtx.Alarms.ExecuteStoreCommand($"delete from T_SG_ALARM where UserSeq = {strUserSeq} AND GroupId = {nGroupID};");
                        //DBCtx.Alarms.FromSql($"delete from T_SG_ALARM where UserSeq = {strUserSeq} AND GroupId = {nGroupID};");
                        DBCtx.SaveChanges();
                    }
                }
                else
                {
                    List<SGNotiData> NotiDic = null;
                    NotiDic = DBCtx.Notis
                                .Where(x => x.GroupId == nGroupID && x.UserSeq == strUserSeq).ToList<SGNotiData>();
                    if (NotiDic != null && NotiDic.Count > 0)
                    {
                        DBCtx.Notis.RemoveRange(NotiDic);
                        DBCtx.SaveChanges();
                    }

                }

                Log.Logger.Here().Information($"Delete the SGAlarmData, UserSeq : {strUserSeq}, nGroupID : {nGroupID}");
                mut.ReleaseMutex();
            }
            catch(Exception e)
            {
                Log.Logger.Here().Error($"Delete the SGAlarmData(ERROR:{e.Message}), UserSeq : {strUserSeq}, nGroupID : {nGroupID}");
                return false;
            }

            return true;
        }
        /// <summary>
        /// Alram Or Message 모든 정보 삭제
        /// </summary>
        /// <param name="bIsAlarm"></param>
        /// <returns></returns>
        public bool DeleteAllInfo(bool bIsAlarm)
        {

            try
            {
                mut.WaitOne();
                // Delete

                if (bIsAlarm)
                {
                    List<SGAlarmData> AlarmDic = null;
                    AlarmDic = DBCtx.Alarms.ToList<SGAlarmData>();
                    if (AlarmDic != null && AlarmDic.Count > 0)
                    {
                        DBCtx.Alarms.RemoveRange(AlarmDic);
                        DBCtx.SaveChanges();
                    }
                }
                else
                {
                    List<SGNotiData> NotiDic = null;
                    NotiDic = DBCtx.Notis.ToList<SGNotiData>();
                    if (NotiDic != null && NotiDic.Count > 0)
                    {
                        DBCtx.Notis.RemoveRange(NotiDic);
                        DBCtx.SaveChanges();
                    }

                }

                Log.Logger.Here().Information($"Delete the SGAlarmData ALL");
                mut.ReleaseMutex();
            }
            catch (Exception e)
            {
                Log.Logger.Here().Error($"Delete the SGAlarmData(ERROR:{e.Message}) ALL");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 기준 날짜를 받아 그 이전의 Alram Or Message 모두 삭제
        /// </summary>
        /// <param name="standardDate"></param>
        /// <param name="bIsAlarm"></param>
        /// <returns></returns>
        public bool DeleteAllInfo(DateTime standardDate, bool bIsAlarm)
        {

            try
            {
                mut.WaitOne();
                // Delete

                if (bIsAlarm)
                {
                    List<SGAlarmData> AlarmDic = null;
                    AlarmDic = DBCtx.Alarms.
                        Where(x => x.Time != null && x.Time < standardDate.Date).
                        ToList<SGAlarmData>();
                    if (AlarmDic != null && AlarmDic.Count > 0)
                    {
                        DBCtx.Alarms.RemoveRange(AlarmDic);
                        DBCtx.SaveChanges();
                    }
                }
                else
                {
                    List<SGNotiData> NotiDic = null;
                    NotiDic = DBCtx.Notis.
                        Where(x => x.Time != null && x.Time < standardDate.Date).
                        ToList<SGNotiData>();
                    if (NotiDic != null && NotiDic.Count > 0)
                    {
                        DBCtx.Notis.RemoveRange(NotiDic);
                        DBCtx.SaveChanges();
                    }

                }

                Log.Logger.Here().Information($"Delete the Data, StandardDate Before Data");
                mut.ReleaseMutex();
            }
            catch (Exception e)
            {
                Log.Logger.Here().Error($"Delete the Data(ERROR:{e.Message}) StandardDate Before Data");
                return false;
            }

            return true;
        }
    }
}