using System.Text.Json.Serialization;
using EducationCenter.Api.Contracts.Cart;
using EducationCenter.Api.Contracts.Leads;
using EducationCenter.Api.Domain.Context;
using EducationCenter.Api.Infrastructure.Email;
using EducationCenter.Api.Middlware;
using EducationCenter.Api.Services.Discounts;
using EducationCenter.Api.Services.Leads;
using EducationCenter.Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core InMemory 
builder.Services.AddDbContext<EducationDbContext>(options =>
    options.UseInMemoryDatabase("EducationCenter"));

// Валидация входных данных
builder.Services.AddScoped<IValidator<LeadRequest>, LeadRequestValidator>();
builder.Services.AddScoped<IValidator<CalculateCartRequest>, CalculateCartValidator>();

// Регистрация слоя сервисов и инфраструктуры
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IDiscountPolicy, HotGroupDiscountPolicy>();
builder.Services.AddScoped<IDiscountPolicy, GraduateDiscountPolicy>();
builder.Services.AddScoped<CartPriceCalculator>();

// В демо-режиме письма «уходят» в лог (TestEmailSender); для реальной SMTP-отправки
// заменить регистрацию на EmailSender.
builder.Services.AddScoped<IEmailSender, TestEmailSender>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<FallbackEmailOptions>(builder.Configuration.GetSection("FallbackEmail"));
builder.Services.AddScoped<LeadService>();

// Глобальная обработка ошибок
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
    var clock = scope.ServiceProvider.GetRequiredService<TimeProvider>();

    await DbSeeder.SeedAsync(db, clock);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
