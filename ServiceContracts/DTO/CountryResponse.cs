using Enttities;
namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that is used as return type for most of CountriesService methods
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }
}

//pravimo extension class za injecting to Country class
// converting Country object to CountryResponse
public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
    }
}
