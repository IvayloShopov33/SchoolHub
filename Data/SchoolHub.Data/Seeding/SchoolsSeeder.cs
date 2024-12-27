namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class SchoolsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Schools.Any())
            {
                return;
            }

            await dbContext.Schools.AddRangeAsync(new List<School>
            {
                new School { Name = "PMG Veliko Tarnovo", Address = "5000 гр. Велико Търново, ул. \"Вела Благоева\" №10", WebsiteUrl = "http://www.pmgvt.org/" },
                new School { Name = "Language School VT", Address = "5000 гр. Велико Търново, ул.\"Славянска\" №2", WebsiteUrl = "https://ezikovavt.com/" },
            });
        }
    }
}
