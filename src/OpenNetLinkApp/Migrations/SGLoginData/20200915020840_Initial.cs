using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenNetLinkApp.Migrations.SGLoginData
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_SG_LOGIN_DATA",
                columns: table => new
                {
                    GROUPID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UID = table.Column<string>(type: "varchar(128)", nullable: true),
                    UPW = table.Column<string>(type: "varchar(128)", nullable: true),
                    APPRLINE = table.Column<string>(type: "varchar(2048)", nullable: true),
                    DELAYDISPLAYPW = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AUTOLOGINING = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SG_LOGIN_DATA_GROUPID", x => x.GROUPID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_SG_LOGIN_DATA");
        }
    }
}
