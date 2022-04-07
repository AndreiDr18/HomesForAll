using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class tenantRequestStatusEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TenantRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TenantRequests");
        }
    }
}
