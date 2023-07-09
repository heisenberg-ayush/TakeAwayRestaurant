using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSale.Migrations
{
    /// <inheritdoc />
    public partial class HelloJi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "registrations",
                newName: "OperatorName");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "registrations",
                newName: "OperatorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OperatorName",
                table: "registrations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OperatorID",
                table: "registrations",
                newName: "id");
        }
    }
}
