using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHome.Migrations
{
    /// <inheritdoc />
    public partial class WdrozenieDoniczek : Migration
    {
        /// <inheritdoc />
        protected override void Up ( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.CreateTable(
          name: "DeviceTypes",
          columns: table => new
          {
              TypeId = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
              TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
          },
          constraints: table =>
          {
              table.PrimaryKey("PK_DeviceTypes", x => x.TypeId);
          });

            // [LOG] Tworzenie tabeli fizycznych doniczek
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MacAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceTypeId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);

                    // Relacja do typu (Doniczka)
                    table.ForeignKey(
                        name: "FK_Devices_DeviceTypes_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade);

                    // Relacja do Twojej tabeli Users
                    table.ForeignKey(
                        name: "FK_Devices_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "usr_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceTypeId",
                table: "Devices",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_OwnerId",
                table: "Devices",
                column: "OwnerId");
        }
    }
}