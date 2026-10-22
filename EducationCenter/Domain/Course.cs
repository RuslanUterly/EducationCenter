namespace EducationCenter.Api.Domain;

public sealed class Course
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
}
