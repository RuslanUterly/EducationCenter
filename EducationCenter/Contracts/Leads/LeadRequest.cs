using EducationCenter.Api.Domain;

namespace EducationCenter.Api.Contracts.Leads;

public sealed record LeadRequest(string Name, string Phone, string Email, ClientType ClientType);
