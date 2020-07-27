using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenNetLinkApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_SG_ALARM",
                columns: table => new
                {
                    AlarmId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(type: "varchar(64)", nullable: true),
                    IconImage = table.Column<string>(type: "varchar(128)", nullable: true),
                    Head = table.Column<string>(type: "varchar(64)", nullable: false),
                    Body = table.Column<string>(type: "varchar(255)", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SG_ALARM_ALARMID_GROUPID", x => new { x.AlarmId, x.GroupId });
                });

            migrationBuilder.CreateTable(
                name: "T_SG_NOTI",
                columns: table => new
                {
                    NotiId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(type: "varchar(64)", nullable: true),
                    IconImage = table.Column<string>(type: "varchar(128)", nullable: true),
                    Head = table.Column<string>(type: "varchar(64)", nullable: false),
                    Body = table.Column<string>(type: "varchar(255)", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SG_NOTI_NOTIID_GROUPID", x => new { x.NotiId, x.GroupId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_SG_ALARM");

            migrationBuilder.DropTable(
                name: "T_SG_NOTI");
        }
    }
}
