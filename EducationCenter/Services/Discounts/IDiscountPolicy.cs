using EducationCenter.Api.Domain;

namespace EducationCenter.Api.Services.Discounts;

/// <summary>Результат применения политики скидки.</summary>
/// <param name="Percentage">Размер скидки в долях единицы (0.2 = 20%).</param>
/// <param name="PolicyName">Название политики, применившей скидку.</param>
public sealed record DiscountResult(decimal Percentage, string PolicyName);

/// <summary>
/// Стратегия расчёта скидки на курс (паттерн Strategy).
/// Каждое бизнес-правило скидки реализуется отдельной политикой, что позволяет
/// добавлять новые правила без изменения существующего кода:
/// новый класс + одна строка регистрации в DI.
/// </summary>
public interface IDiscountPolicy
{
    /// <summary>Название политики — попадает в ответ API.</summary>
    string Name { get; }

    /// <summary>
    /// Возвращает скидку, если политика применима к курсу для данного пользователя,
    /// иначе <c>null</c> (политика неприменима).
    /// </summary>
    DiscountResult? TryCalculate(Course course, User user);
}
