using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetingScheduler.Migrations
{
    public partial class UsersId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Users_UserId",
                table: "Meeting");

            migrationBuilder.DropTable(
                name: "EventsUsers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Meeting_UserId",
                table: "Meeting");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Meeting",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserIds",
                table: "Meeting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Meeting");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Meeting",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CN = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    UserLastName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventsUsers",
                columns: table => new
                {
                    MeetingId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsUsers", x => new { x.MeetingId, x.UserId });
                    table.ForeignKey(
                        name: "FK_EventsUsers_Meeting_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventsUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meeting_UserId",
                table: "Meeting",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsUsers_UserId",
                table: "EventsUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Users_UserId",
                table: "Meeting",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
