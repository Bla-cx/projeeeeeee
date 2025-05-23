using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _221103018_OmerFarukBayraktutar.Migrations
{
    /// <inheritdoc />
    public partial class AddIptalAndKullanildiColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Iptal",
                table: "Rezervasyonlar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Kullanildi",
                table: "Rezervasyonlar",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Iptal",
                table: "Rezervasyonlar");

            migrationBuilder.DropColumn(
                name: "Kullanildi",
                table: "Rezervasyonlar");
        }
    }
}
