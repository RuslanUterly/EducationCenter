namespace EducationCenter.Tests;

/// <summary>
/// Тестовый дубль системных часов: «замораживает» время, чтобы тесты скидок не зависели от текущей даты
/// </summary>
internal sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
