using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiSSH.Auth.ECC.Migrations
{
    /// <inheritdoc />
    public partial class AddedEncryptedKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EncryptedKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<byte[]>(type: "bytea", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    Iv = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedKeys", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncryptedKeys");
        }
    }
}
