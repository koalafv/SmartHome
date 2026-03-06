using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHome.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "WeatherRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Pressure",
                table: "WeatherRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "WeatherRecords");

            migrationBuilder.DropColumn(
                name: "Pressure",
                table: "WeatherRecords");
        }
    }
}
