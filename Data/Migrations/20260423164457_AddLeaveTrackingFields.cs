using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeavePro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActualDays",
                table: "LeaveRequests",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "HolidayCount",
                table: "LeaveRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDays",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "HolidayCount",
                table: "LeaveRequests");
        }
    }
}
