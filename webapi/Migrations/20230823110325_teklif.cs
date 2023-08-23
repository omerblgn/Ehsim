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
                name: "ToplamFiyat",
                table: "Teklif",
                newName: "ToplamFiyatUSD");

            migrationBuilder.AddColumn<decimal>(
                name: "ToplamFiyatEUR",
                table: "Teklif",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ToplamFiyatTRY",
                table: "Teklif",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToplamFiyatEUR",
                table: "Teklif");

            migrationBuilder.DropColumn(
                name: "ToplamFiyatTRY",
                table: "Teklif");

            migrationBuilder.RenameColumn(
                name: "ToplamFiyatUSD",
                table: "Teklif",
                newName: "ToplamFiyat");
        }
    }
}
