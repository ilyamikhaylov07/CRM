using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldUserKeycloakId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "keycloak_user_id",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "keycloak_user_id",
                table: "users");
        }
    }
}
