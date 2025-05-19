using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TenXCards.API.Migrations
{
    /// <inheritdoc />
    public partial class OrginalContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalContent",
                table: "Cards");

            migrationBuilder.AddColumn<int>(
                name: "OriginalContentId",
                table: "Cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OriginalContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginalContents", x => x.Id);
                    table.CheckConstraint("CK_OriginalContent_ContentLength", "char_length(\"Content\") BETWEEN 1000 AND 10000");
                    table.ForeignKey(
                        name: "FK_OriginalContents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_OriginalContentId",
                table: "Cards",
                column: "OriginalContentId");

            migrationBuilder.CreateIndex(
                name: "IX_OriginalContents_UserId",
                table: "OriginalContents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_OriginalContents_OriginalContentId",
                table: "Cards",
                column: "OriginalContentId",
                principalTable: "OriginalContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_OriginalContents_OriginalContentId",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "OriginalContents");

            migrationBuilder.DropIndex(
                name: "IX_Cards_OriginalContentId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "OriginalContentId",
                table: "Cards");

            migrationBuilder.AddColumn<string>(
                name: "OriginalContent",
                table: "Cards",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                defaultValue: "");
        }
    }
}
