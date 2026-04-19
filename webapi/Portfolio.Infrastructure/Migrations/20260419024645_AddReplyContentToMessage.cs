using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReplyContentToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyContent",
                table: "Messages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyContent",
                table: "Messages");
        }
    }
}
