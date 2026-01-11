using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProdutorIdToPropriedade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProdutorId",
                table: "Propriedades",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProdutorId",
                table: "Propriedades");
        }
    }
}
