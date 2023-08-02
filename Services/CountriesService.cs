using Enttities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field
    // u repository pattern prebacujemo iz ApplicationDbContext u ICountriesRepository
    private readonly ICountriesRepository _countriesRepository;
    //constructor
    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;


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
        if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
        {
            throw new ArgumentException("Given country name already exist");
        }

        //Convert object from CountryAddRequest to Country type
        Country country = countryAddRequest.ToCountry();
    
        //generate CountryID
        country.CountryID = Guid.NewGuid();

        //Add country object into _countries
        await _countriesRepository.AddCountry(country);
        // ne treba ponovo pozivati SaveChangesAsync(); - pozvali smo u Repository class
        //await _countriesRepository.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        //returns list of country object
        List<Country> countries = await _countriesRepository.GetAllCountries();
        //converting all countryResponse object to plain list
        return countries.Select(country => country.ToCountryResponse()).ToList();
       
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryID)
    {
        if(countryID == null)
        {
            return null;
        }
        // ako ni jedan element ne odgovara vracamo default(null)
        Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);
        if(country_response_from_list == null) 
        { 
            return null; 
        }
        //converting country obj to countryResponse type
        return country_response_from_list.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        MemoryStream memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        int countriesInserted = 0;

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];

            int rowCount = worksheet.Dimension.Rows;
            

            // row 1 je header row (CountryName)
            for(int row= 2; row <= rowCount; row++)
            {
                //row 2 col 1
                string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

                if (!string.IsNullOrEmpty(cellValue))
                {
                    string? countryName = cellValue;

                    // ako ima jedan matching object onda ne unosimo(ako nema duplih imena)
                    if(_countriesRepository.GetCountryByCountryName(countryName) == null)
                    {
                        Country country = new Country() { CountryName = countryName};
                        await _countriesRepository.AddCountry(country);

                        countriesInserted++;
                    }
                }
            }
        }

        return countriesInserted;
    }
}