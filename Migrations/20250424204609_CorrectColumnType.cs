using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShelterHelper.Migrations
{
    /// <inheritdoc />
    public partial class CorrectColumnType : Migration
    {
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
