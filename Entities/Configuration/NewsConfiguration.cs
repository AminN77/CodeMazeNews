using System;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Configuration
{
    public class NewsConfiguration :IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.HasData
            (
                new News
                {
                    Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                    Title = "Breaking news",
                    Description = "blah blah blah ...",
                    CategoryId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
                },
                new News
                {
                    Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                    Title = "Breaking news2",
                    Description = "blah blah blah ...",
                    CategoryId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
                },
                new News
                {
                    Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                    Title = "Breaking news3",
                    Description = "blah blah blah ...",
                    CategoryId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3")
                }
            );
        }
    }
}