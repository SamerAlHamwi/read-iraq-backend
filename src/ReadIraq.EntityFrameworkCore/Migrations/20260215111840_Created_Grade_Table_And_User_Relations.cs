using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadIraq.Migrations
{
    /// <inheritdoc />
    public partial class Created_Grade_Table_And_User_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_GovernorateId",
                table: "AbpUsers",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_GradeId",
                table: "AbpUsers",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Cities_GovernorateId",
                table: "AbpUsers",
                column: "GovernorateId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Grades_GradeId",
                table: "AbpUsers",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Cities_GovernorateId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Grades_GradeId",
                table: "AbpUsers");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_GovernorateId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_GradeId",
                table: "AbpUsers");
        }
    }
}
