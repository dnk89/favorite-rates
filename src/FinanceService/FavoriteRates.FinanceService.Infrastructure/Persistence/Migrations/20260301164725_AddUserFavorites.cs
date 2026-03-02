using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FavoriteRates.FinanceService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_favorites",
                schema: "finance",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<string>(type: "character varying(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorites", x => new { x.UserId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_user_favorites_currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "finance",
                        principalTable: "currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_favorites_CurrencyId",
                schema: "finance",
                table: "user_favorites",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_favorites",
                schema: "finance");
        }
    }
}
