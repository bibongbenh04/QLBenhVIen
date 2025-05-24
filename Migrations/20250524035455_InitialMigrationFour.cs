using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationFour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPaid",
                table: "Appointments",
                newName: "IsPaidToDoctor");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidByPatient",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaidByPatient",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "IsPaidToDoctor",
                table: "Appointments",
                newName: "IsPaid");
        }
    }
}
