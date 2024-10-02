using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddBookedDatesToRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<List<DateTime>>(
                name: "BookedDates",
                table: "Rooms",
                type: "timestamp with time zone[]",
                nullable: false,
                defaultValue: new List<DateTime>()
               );

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "BookedDates",
                table: "Rooms");

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
        }
    }
}
