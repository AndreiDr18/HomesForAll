using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class CascadeIssuesSolved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_AspNetUsers_TenantID",
                table: "TenantRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_Properties_PropertyID",
                table: "TenantRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyID",
                table: "AspNetUsers",
                column: "AcceptedAtPropertyID",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_AspNetUsers_TenantID",
                table: "TenantRequests",
                column: "TenantID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_Properties_PropertyID",
                table: "TenantRequests",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_AspNetUsers_TenantID",
                table: "TenantRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_Properties_PropertyID",
                table: "TenantRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyID",
                table: "AspNetUsers",
                column: "AcceptedAtPropertyID",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_AspNetUsers_TenantID",
                table: "TenantRequests",
                column: "TenantID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_Properties_PropertyID",
                table: "TenantRequests",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id");
        }
    }
}
