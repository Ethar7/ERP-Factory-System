using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErpFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE duplicateUsers
                FROM [Users] AS duplicateUsers
                INNER JOIN (
                    SELECT [Username], MIN([UserId]) AS [KeepUserId]
                    FROM [Users]
                    GROUP BY [Username]
                    HAVING COUNT(*) > 1
                ) AS duplicates
                    ON duplicateUsers.[Username] = duplicates.[Username]
                   AND duplicateUsers.[UserId] <> duplicates.[KeepUserId];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");
        }
    }
}
