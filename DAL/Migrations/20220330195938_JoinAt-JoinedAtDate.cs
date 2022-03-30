using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomesForAll.DAL.Migrations
{
    public partial class JoinAtJoinedAtDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JoinedAt",
                table: "AspNetUsers",
                newName: "JoinedAtDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JoinedAtDate",
                table: "AspNetUsers",
                newName: "JoinedAt");
        }
    }
}
