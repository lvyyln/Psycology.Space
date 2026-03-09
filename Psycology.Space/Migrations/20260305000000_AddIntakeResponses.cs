using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Psycology.Space.Migrations
{
    /// <inheritdoc />
    public partial class AddIntakeResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntakeResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MoodScore = table.Column<int>(type: "INTEGER", nullable: false),
                    SleepScore = table.Column<int>(type: "INTEGER", nullable: false),
                    AnxietyScore = table.Column<int>(type: "INTEGER", nullable: false),
                    MainConcern = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalNotes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntakeResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntakeResponses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntakeResponses_UserId",
                table: "IntakeResponses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntakeResponses");
        }
    }
}
