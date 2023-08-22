using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class teklif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrunId",
                table: "Teklif",
                newName: "TeklifSuresi");

            migrationBuilder.RenameColumn(
                name: "TeklifDegeri",
                table: "Teklif",
                newName: "ToplamFiyat");

            migrationBuilder.AddColumn<decimal>(
                name: "IskontoOrani",
                table: "Teklif",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TeklifNo",
                table: "Teklif",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TeklifTarihi",
                table: "Teklif",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TeklifItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeklifId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    Adet = table.Column<int>(type: "INTEGER", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeklifItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeklifItem_Teklif_TeklifId",
                        column: x => x.TeklifId,
                        principalTable: "Teklif",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeklifItem_TeklifId",
                table: "TeklifItem",
                column: "TeklifId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeklifItem");

            migrationBuilder.DropColumn(
                name: "IskontoOrani",
                table: "Teklif");

            migrationBuilder.DropColumn(
                name: "TeklifNo",
                table: "Teklif");

            migrationBuilder.DropColumn(
                name: "TeklifTarihi",
                table: "Teklif");

            migrationBuilder.RenameColumn(
                name: "ToplamFiyat",
                table: "Teklif",
                newName: "TeklifDegeri");

            migrationBuilder.RenameColumn(
                name: "TeklifSuresi",
                table: "Teklif",
                newName: "UrunId");
        }
    }
}
