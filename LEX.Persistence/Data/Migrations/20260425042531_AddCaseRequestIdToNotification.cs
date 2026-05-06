using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEX.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseRequestIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseRequestId",
                table: "Notifications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseRequestId",
                table: "Notifications");
        }
    }
}
