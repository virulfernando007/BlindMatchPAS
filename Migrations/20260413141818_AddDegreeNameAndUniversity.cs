using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindMatchPAS.Migrations
{
    /// <inheritdoc />
    public partial class AddDegreeNameAndUniversity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Degree",
                table: "AspNetUsers",
                newName: "University");

            migrationBuilder.AddColumn<string>(
                name: "DegreeName",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegreeName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "University",
                table: "AspNetUsers",
                newName: "Degree");
        }
    }
}
