using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ConfigStore.Api.Migrations
{
    public partial class AddUniqueIndexForNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_ApplicationId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Environments_ServiceId",
                table: "Environments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Services",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Environments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ApplicationId_Name",
                table: "Services",
                columns: new[] { "ApplicationId", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Environments_ServiceId_Name",
                table: "Environments",
                columns: new[] { "ServiceId", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Name",
                table: "Applications",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_ApplicationId_Name",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Environments_ServiceId_Name",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Name",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Services",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Environments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ApplicationId",
                table: "Services",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Environments_ServiceId",
                table: "Environments",
                column: "ServiceId");
        }
    }
}
