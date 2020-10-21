using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenNetLinkApp.Migrations.SGNotify
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_SG_ALARM",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(nullable: false),
                    UserSeq = table.Column<string>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(type: "varchar(64)", nullable: true),
                    IconImage = table.Column<string>(type: "varchar(128)", nullable: true),
                    Head = table.Column<string>(type: "varchar(64)", nullable: false),
                    Body = table.Column<string>(type: "varchar(255)", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now','localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SG_ALARM_ID", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_SG_NOTI",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    UserSeq = table.Column<string>(nullable: false),
                    Seq = table.Column<string>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(type: "varchar(64)", nullable: true),
                    IconImage = table.Column<string>(type: "varchar(128)", nullable: true),
                    Head = table.Column<string>(type: "varchar(256)", nullable: false),
                    Body = table.Column<string>(type: "varchar(4096)", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now','localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SG_NOTI_ID", x => x.Id);
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
