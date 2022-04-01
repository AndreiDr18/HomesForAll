using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class AcceptedAtPropertyID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AcceptedAtPropertyId",
                table: "AspNetUsers",
                newName: "AcceptedAtPropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_AcceptedAtPropertyId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_AcceptedAtPropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyID",
                table: "AspNetUsers",
                column: "AcceptedAtPropertyID",
                principalTable: "Properties",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyID",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AcceptedAtPropertyID",
                table: "AspNetUsers",
                newName: "AcceptedAtPropertyId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_AcceptedAtPropertyID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_AcceptedAtPropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Properties_AcceptedAtPropertyId",
                table: "AspNetUsers",
                column: "AcceptedAtPropertyId",
                principalTable: "Properties",
                principalColumn: "Id");
        }
    }
}
