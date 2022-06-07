using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCompany.DataAccess.Migrations
{
    public partial class commentPropAddedToTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Tasks");
        }
    }
}
