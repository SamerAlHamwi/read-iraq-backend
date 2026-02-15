using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadIraq.Migrations
{
    /// <inheritdoc />
    public partial class Added_Grade_And_Governorate_To_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "AbpUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "AbpUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "AbpUsers");
        }
    }
}
