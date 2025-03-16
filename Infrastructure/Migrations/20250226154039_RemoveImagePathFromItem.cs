using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImagePathFromItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewievs_Users_RewieverId",
                table: "Rewievs");

            migrationBuilder.DropIndex(
                name: "IX_Rewievs_RewieverId",
                table: "Rewievs");

            migrationBuilder.DropColumn(
                name: "RewieverId",
                table: "Rewievs");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_Rewievs_ReceiverId",
                table: "Rewievs",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewievs_Users_ReceiverId",
                table: "Rewievs",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewievs_Users_ReceiverId",
                table: "Rewievs");

            migrationBuilder.DropIndex(
                name: "IX_Rewievs_ReceiverId",
                table: "Rewievs");

            migrationBuilder.AddColumn<Guid>(
                name: "RewieverId",
                table: "Rewievs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Rewievs_RewieverId",
                table: "Rewievs",
                column: "RewieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewievs_Users_RewieverId",
                table: "Rewievs",
                column: "RewieverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
