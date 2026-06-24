namespace ADONET_Final_project.Models;

public class User
{
    public string Login { get; set; } = "";
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string? Patronomic { get; set; }
    public string Password { get; set; } = "";
    public DateTime? BirthDate { get; set; }
    public int? GroupId { get; set; }
    public int? AddressId { get; set; }
    
    // Для отображения
    public string? GroupName { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? House { get; set; }
    public string? Apartment { get; set; }
}