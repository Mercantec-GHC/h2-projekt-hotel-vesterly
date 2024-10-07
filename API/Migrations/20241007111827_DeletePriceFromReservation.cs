using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class DeletePriceFromReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "17749369-c81a-4fd2-86ee-5ca4dfba1d73");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "53383087-4004-46c1-8934-6ede674c1ff6");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c1c76fff-713e-4c61-aad5-8a8c0d379612");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Reservations");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2afc818e-3a1e-466e-bdf0-e1f7e215c294", null, "Admin", "ADMIN" },
                    { "cdc6513c-ec61-4782-ac89-538e8d4e75a0", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "479982ae-5781-43a5-af38-34dd24c30640", 0, "8a176724-1a88-4b4a-b41b-06761ef8dae8", "User", "admin@admin.com", false, "Admin", "Admin", false, null, null, null, "AQAAAAIAAYagAAAAEP3hmWMngUrQRDTzMKIW5bnOWb+RPTyXZJ08Z/BKR6vCSUGYBIF4T1W+cPgJJXm6VQ==", "123456789", false, "bb6041e4-0155-46b6-a66f-7c66e4ab21f0", false, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "2afc818e-3a1e-466e-bdf0-e1f7e215c294");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "cdc6513c-ec61-4782-ac89-538e8d4e75a0");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "479982ae-5781-43a5-af38-34dd24c30640");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "17749369-c81a-4fd2-86ee-5ca4dfba1d73", null, "Admin", "ADMIN" },
                    { "53383087-4004-46c1-8934-6ede674c1ff6", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c1c76fff-713e-4c61-aad5-8a8c0d379612", 0, "686ac623-6a2e-4088-b48d-97b693498669", "User", "admin@admin.com", false, "Admin", "Admin", false, null, null, null, "AQAAAAIAAYagAAAAELlMFTzf4ct2ikmxxZi+jmIpkP10PcbxDMkFbw21IVPMjWsuRh+udW221tYfWZm1gQ==", "123456789", false, "8f6f016c-ab00-4346-a47e-0f394c0b52a7", false, "admin" });
        }
    }
}
