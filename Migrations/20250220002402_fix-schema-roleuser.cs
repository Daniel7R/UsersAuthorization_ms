using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersAuthorization.Migrations
{
    /// <inheritdoc />
    public partial class fixschemaroleuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_roles",
                newName: "id_role");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_roles",
                newName: "id_user");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_id_role");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_id_role",
                table: "user_roles",
                column: "id_role",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_id_user",
                table: "user_roles",
                column: "id_user",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_id_role",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_id_user",
                table: "user_roles");

            migrationBuilder.RenameColumn(
                name: "id_role",
                table: "user_roles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "id_user",
                table: "user_roles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_id_role",
                table: "user_roles",
                newName: "IX_user_roles_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
