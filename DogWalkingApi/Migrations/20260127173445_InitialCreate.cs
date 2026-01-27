using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DogWalkingApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialNeeds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Walks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogId = table.Column<int>(type: "int", nullable: false),
                    WalkerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Walks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Dogs",
                columns: new[] { "Id", "Age", "Breed", "Name", "OwnerId", "Size", "SpecialNeeds" },
                values: new object[,]
                {
                    { 1, 3, "Labrador", "Buddy", 1, "Large", null },
                    { 2, 5, "Beagle", "Max", 2, "Medium", "Allergic to chicken" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash" },
                values: new object[,]
                {
                    { 1, "alice@example.com", "Alice", "hashedpassword1" },
                    { 2, "bob@example.com", "Bob", "hashedpassword2" }
                });

            migrationBuilder.InsertData(
                table: "Walks",
                columns: new[] { "Id", "DogId", "Duration", "Status", "WalkDate", "WalkerId" },
                values: new object[,]
                {
                    { 1, 1, new TimeSpan(0, 1, 0, 0, 0), "Scheduled", new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 2, 2, new TimeSpan(0, 0, 30, 0, 0), "Scheduled", new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dogs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Walks");
        }
    }
}
