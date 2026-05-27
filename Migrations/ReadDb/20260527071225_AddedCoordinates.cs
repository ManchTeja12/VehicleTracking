using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleMangement.Migrations.ReadDb
{
    /// <inheritdoc />
    public partial class AddedCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DestLat",
                table: "Vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DestLng",
                table: "Vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SourceLat",
                table: "Vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SourceLng",
                table: "Vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestLat",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DestLng",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SourceLat",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SourceLng",
                table: "Vehicles");
        }
    }
}
