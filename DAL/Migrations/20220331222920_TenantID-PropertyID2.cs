using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class TenantIDPropertyID2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRequests_Properties_Id",
                table: "TenantRequests");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyID",
                table: "TenantRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRequests_PropertyID",
                table: "TenantRequests",
                column: "PropertyID");

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
                name: "FK_TenantRequests_Properties_PropertyID",
                table: "TenantRequests");

            migrationBuilder.DropIndex(
                name: "IX_TenantRequests_PropertyID",
                table: "TenantRequests");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyID",
                table: "TenantRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRequests_Properties_Id",
                table: "TenantRequests",
                column: "Id",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
