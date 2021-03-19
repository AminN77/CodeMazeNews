using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class configNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "NewsId", "CategoryId", "Description", "ImageUrl", "Title" },
                values: new object[] { new Guid("80abbca8-664d-4b20-b5de-024705497d4a"), new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), "blah blah blah ...", null, "Breaking news" });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "NewsId", "CategoryId", "Description", "ImageUrl", "Title" },
                values: new object[] { new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"), new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), "blah blah blah ...", null, "Breaking news2" });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "NewsId", "CategoryId", "Description", "ImageUrl", "Title" },
                values: new object[] { new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"), new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"), "blah blah blah ...", null, "Breaking news3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "NewsId",
                keyValue: new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"));

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "NewsId",
                keyValue: new Guid("80abbca8-664d-4b20-b5de-024705497d4a"));

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "NewsId",
                keyValue: new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"));
        }
    }
}
