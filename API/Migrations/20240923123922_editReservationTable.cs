using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class editReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "07616f93-6738-4aaa-86ae-af87390fe298");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "aafc45d7-d1f8-4487-b390-fa76b26e2c7f");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "0cf29eff-ebb0-4e20-9d14-eff962545aed");

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestPhoneNr",
                table: "Reservations",
                type: "text",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "GuestPhoneNr",
                table: "Reservations");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07616f93-6738-4aaa-86ae-af87390fe298", null, "Admin", "ADMIN" },
                    { "aafc45d7-d1f8-4487-b390-fa76b26e2c7f", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0cf29eff-ebb0-4e20-9d14-eff962545aed", 0, "1ef8fa46-a2f5-48d9-964a-176f98c91d8a", "User", "admin@admin.com", false, "Admin", "Admin", false, null, null, null, "AQAAAAIAAYagAAAAEHlcdVOeRVrHWSJ8dOLTmB0fBhl3Uym/p43jyuZ0Afu5f9RWxuYAdCN1+q8w1E71Og==", "123456789", false, "d60e1562-022e-416d-96ab-c72119188d88", false, "admin" });
        }
    }
}
