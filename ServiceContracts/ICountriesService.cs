using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represent business logic for manipulating Country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Return a country object to the list of coutries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Returns all countries from the list
    /// </summary>
    /// <returns>All countries from the list as List of CountryResponse></returns>
    List<CountryResponse> GetAllCountries();
}