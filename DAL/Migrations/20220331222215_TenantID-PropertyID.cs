using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class TenantIDPropertyID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_AspNetUsers_Id",
                table: "TenantRequests");

            migrationBuilder.AddColumn<string>(
                name: "PropertyID",
                table: "TenantRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantID",
                table: "TenantRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRequests_TenantID",
                table: "TenantRequests",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_AspNetUsers_TenantID",
                table: "TenantRequests",
                column: "TenantID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_AspNetUsers_TenantID",
                table: "TenantRequests");

            migrationBuilder.DropIndex(
                name: "IX_TenantRequests_TenantID",
                table: "TenantRequests");

            migrationBuilder.DropColumn(
                name: "PropertyID",
                table: "TenantRequests");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "TenantRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_AspNetUsers_Id",
                table: "TenantRequests",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
