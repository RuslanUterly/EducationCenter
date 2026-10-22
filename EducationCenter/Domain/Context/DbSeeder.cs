using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Api.Domain.Context;

public static class DbSeeder
{
    public static async Task SeedAsync(
        EducationDbContext db,
        TimeProvider clock,
        CancellationToken ct = default)
    {
        if (await db.Courses.AnyAsync(ct))
            return;

        var today = clock.GetUtcNow().UtcDateTime.Date;

        db.Courses.AddRange(
            new Course
            {
                Id = Guid.NewGuid(),
                Name = "C#: основы программирования",
                Price = 24_900m,
                StartDate = today.AddDays(2)     // hot (< 5 дней)
            },
            new Course
            {
                Id = Guid.NewGuid(),
                Name = "ASP.NET Core: продвинутый уровень",
                Price = 39_900m,
                StartDate = today.AddDays(12)    // не hot
            },
            new Course
            {
                Id = Guid.NewGuid(),
                Name = "SQL и базы данных",
                Price = 19_900m,
                StartDate = today.AddDays(-1)    // уже начался — hot
            },
            new Course
            {
                Id = Guid.NewGuid(),
                Name = "Архитектура ПО на .NET",
                Price = 49_900m,
                StartDate = today.AddDays(30)    // не hot
            });

        db.Users.AddRange(
            new User
            {
                Id = Guid.NewGuid(),
                Name = "Роман",
                IsGraduate = true
            },
            new User
            {
                Id = Guid.NewGuid(),
                Name = "Игорь",
                IsGraduate = false
            });

        await db.SaveChangesAsync(ct);
    }
}

