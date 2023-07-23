using System.ComponentModel.DataAnnotations;

namespace Enttities;

/// <summary>
/// Domain Model for storing Country details
/// </summary>
public class Country
{
    [Key]
    public Guid CountryID { get; set; }
    public string? CountryName { get; set; }
}