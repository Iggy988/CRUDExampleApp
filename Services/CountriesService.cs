using Enttities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field
    //private readonly List<Country> _countries;
    private readonly PersonsDbContext _db;
    //constructor
    public CountriesService(PersonsDbContext personsDbContext /*, bool initialize = true*/)
    {
        //_countries = new List<Country>();
        _db = personsDbContext;

        /*if (initialize)
        {
            _countries.AddRange(new List<Country>()
            {
                new Country() { CountryID = Guid.Parse("99028B14-DD57-48E8-BDD7-9DA61D667A38"), CountryName = "Serbia" },
                new Country() { CountryID = Guid.Parse("CB2115A6-DCCC-4ABF-80E9-BEF4028FC405"), CountryName = "Russia" },
                new Country() { CountryID = Guid.Parse("7589930A-907D-4155-8990-4FEE785D17B0"), CountryName = "USA" },
                new Country() { CountryID = Guid.Parse("56A24F2F-558A-4DCC-BD06-F4D8CA6FE184"), CountryName = "Japan" },
                new Country() { CountryID = Guid.Parse("C03F748F-ABBA-4DB1-81D0-6A09CD89B2AE"), CountryName = "India" }
            });
        }*/
    }
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
        if (await _db.Countries
            .CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
        {
            throw new ArgumentException("Given country name already exist");
        }

        //Convert object from CountryAddRequest to Country type
        Country country = countryAddRequest.ToCountry();
    
        //generate CountryID
        country.CountryID = Guid.NewGuid();

        //Add country object into _countries
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        //converting each element from country to ToCountryResponse -> returns ToCountryResponse
        return await _db.Countries
            .Select(country => country.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryID)
    {
        if(countryID == null)
        {
            return null;
        }
        // ako ni jedan element ne odgovara vracamo default(null)
        Country? country_response_from_list = await _db.Countries
            .FirstOrDefaultAsync(temp => temp.CountryID == countryID);
        if(country_response_from_list == null) 
        { 
            return null; 
        }
        //converting country obj to countryResponse type
        return country_response_from_list.ToCountryResponse();
    }
}