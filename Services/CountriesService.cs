using Enttities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field
    private readonly List<Country> _countries;
    //constructor
    public CountriesService()
    {
        _countries = new List<Country>();
    }
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        //Validation: countryAddRequest parameter cant be null
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }
        
      
        //Validation: countryName cant be null
        if (countryAddRequest.CountryName == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        }

        //Validation: CountryName cant be duplicate
        if (_countries.Where(temp => temp.CountryName ==
            countryAddRequest.CountryName).Count() > 0)
        {
            throw new ArgumentException("Given country name already exist");
        }

        //Convert object from CountryAddRequest to Country type
        Country country = countryAddRequest.ToCountry();
    
        //generate CountryID
        country.CountryId = Guid.NewGuid();

        //Add country object into _countries
        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        //converting each element from country to ToCountryResponse -> returns ToCountryResponse
        return _countries.Select(country => country.ToCountryResponse()).ToList();
    }
}