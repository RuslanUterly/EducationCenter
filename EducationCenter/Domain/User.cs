namespace EducationCenter.Api.Domain;

public sealed class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsGraduate { get; set; }
}
