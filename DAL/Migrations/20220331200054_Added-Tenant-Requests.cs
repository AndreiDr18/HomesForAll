using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class AddedTenantRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyUser");

            migrationBuilder.CreateTable(
                name: "TenantRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRequests_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantRequests_Properties_Id",
                        column: x => x.Id,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantRequests");

            migrationBuilder.CreateTable(
                name: "PropertyUser",
                columns: table => new
                {
                    RequestedPropertiesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestingTenantsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyUser", x => new { x.RequestedPropertiesId, x.RequestingTenantsId });
                    table.ForeignKey(
                        name: "FK_PropertyUser_AspNetUsers_RequestingTenantsId",
                        column: x => x.RequestingTenantsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyUser_Properties_RequestedPropertiesId",
                        column: x => x.RequestedPropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyUser_RequestingTenantsId",
                table: "PropertyUser",
                column: "RequestingTenantsId");
        }
    }
}
