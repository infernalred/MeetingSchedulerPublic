using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetingScheduler.Migrations
{
    public partial class fixUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Rooms_ResourceRoomId",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Meeting_MeetingId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_MeetingId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ResourceRoomId",
                table: "Meeting",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_ResourceRoomId",
                table: "Meeting",
                newName: "IX_Meeting_RoomId");

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
                name: "IX_EventsUsers_UserId",
                table: "EventsUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Rooms_RoomId",
                table: "Meeting",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Rooms_RoomId",
                table: "Meeting");

            migrationBuilder.DropTable(
                name: "EventsUsers");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Meeting",
                newName: "ResourceRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_RoomId",
                table: "Meeting",
                newName: "IX_Meeting_ResourceRoomId");

            migrationBuilder.AddColumn<int>(
                name: "MeetingId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MeetingId",
                table: "Users",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Rooms_ResourceRoomId",
                table: "Meeting",
                column: "ResourceRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Meeting_MeetingId",
                table: "Users",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
