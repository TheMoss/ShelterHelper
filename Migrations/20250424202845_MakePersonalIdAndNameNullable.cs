using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShelterHelper.Models;

#nullable disable

namespace ShelterHelper.Migrations
{
    /// <inheritdoc />
    public partial class MakePersonalIdAndNameNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                table:"Employee",
                name: "EmployeePersonalId", 
                nullable:true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AlterColumn<int>(
                table:"Employee",
                name: "EmployeePersonalId", 
                nullable:false);
        }
    }
}
