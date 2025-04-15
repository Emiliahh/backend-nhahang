using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class add_promo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Promos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPercentage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MaxUsage = table.Column<int>(type: "int", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "PromoUsages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PromoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoUsages", x => x.id);
                    table.ForeignKey(
                        name: "FK_PromoUsages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoUsages_Promos_PromoId",
                        column: x => x.PromoId,
                        principalTable: "Promos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Promos_Code",
                table: "Promos",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoUsages_PromoId",
                table: "PromoUsages",
                column: "PromoId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoUsages_UserId_PromoId",
                table: "PromoUsages",
                columns: new[] { "UserId", "PromoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoUsages");

            migrationBuilder.DropTable(
                name: "Promos");
        }
    }
}
