using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTypeInReservationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationRoom");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "11caf93d-6733-4313-be08-4410f9638a7c");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "b8c62543-e362-4293-9af6-2efe5912a31f");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "d08a9816-dc54-484e-bc79-229de814ed58");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1aee2e89-d070-4558-a3f6-a59f0ed2ee93", null, "User", "USER" },
                    { "c6a2b360-0e94-4294-b0c3-c6127a9b4dec", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "9db5fb3a-1c2a-436e-8c6a-81cf21c0dc0c", 0, "393f9c6a-573a-4090-b3f2-8b51fed3bd5e", "User", "admin@admin.com", false, "Admin", "Admin", false, null, null, null, "AQAAAAIAAYagAAAAEFDemzLvE8PNTd4sStZVNu8HMj/L/zy5bedFDwTA1SzCFTwpHX8ofkFZyyNQ9Yd50w==", "123456789", false, "ab446df1-0e24-4b0c-b811-df3ef8178ef2", false, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "1aee2e89-d070-4558-a3f6-a59f0ed2ee93");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "c6a2b360-0e94-4294-b0c3-c6127a9b4dec");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "9db5fb3a-1c2a-436e-8c6a-81cf21c0dc0c");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Reservations");

            migrationBuilder.CreateTable(
                name: "ReservationRoom",
                columns: table => new
                {
                    ReservationsId = table.Column<int>(type: "integer", nullable: false),
                    RoomsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRoom", x => new { x.ReservationsId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_ReservationRoom_Reservations_ReservationsId",
                        column: x => x.ReservationsId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationRoom_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11caf93d-6733-4313-be08-4410f9638a7c", null, "Admin", "ADMIN" },
                    { "b8c62543-e362-4293-9af6-2efe5912a31f", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d08a9816-dc54-484e-bc79-229de814ed58", 0, "fff42f31-c91d-4685-bb8b-e6c86654daa5", "User", "admin@admin.com", false, "Admin", "Admin", false, null, null, null, "AQAAAAIAAYagAAAAEFccl9YQqTVCIBW5gE5GUNomJeFCIPCkKD24D/fMm6+H843Alh2lofZqdC6bwtiLBQ==", "123456789", false, "fabd9953-f8b7-4510-aaa3-0a4f6a942e42", false, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRoom_RoomsId",
                table: "ReservationRoom",
                column: "RoomsId");
        }
    }
}
