using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenNetLinkApp.Migrations.SGNotify
{
    public partial class InitialCreate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_SG_RESEND",
                columns: table => new
                {
                    RESENDID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GROUPID = table.Column<int>(type: "INT", nullable: false),
                    USERSEQ = table.Column<string>(type: "varchar(512)", nullable: false),
                    CLIENTID = table.Column<string>(type: "varchar(512)", nullable: false),
                    MID = table.Column<string>(type: "varchar(512)", nullable: false),
                    HSZNAME = table.Column<string>(type: "varchar(512)", nullable: false),
                    ISEND = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    TRANSINFO = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SG_RESEND_ID", x => x.RESENDID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_SG_RESEND");
        }
    }
}
