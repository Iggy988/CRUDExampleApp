using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enttities.Migrations
{
    public partial class GetPersonsStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryID", "CountryName" },
                values: new object[] { new Guid("h3681621-35c0-4078-ba94-afa7e46af34a"), "Mexico" });

            string sp_GetAllPersons = @"
                CREATE PROCEDURE [dbo].[GetAllPersons]
                AS BEGIN
                    SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters
                    FROM [dbo].Persons
                END
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        //to undo migrations
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("h3681621-35c0-4078-ba94-afa7e46af34a"));

            string sp_GetAllPersons = @"
                DROP PROCEDURE [dbo].[GetAllPersons]
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
