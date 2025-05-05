using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiSSH.Auth.ECC.Migrations
{
    /// <inheritdoc />
    public partial class AddedNamesToKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EncryptedKeys",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "EncryptedKeys");
        }
    }
}
