﻿using Enttities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;
using EntityFrameworkCoreMock;
using Moq;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    //constructor
    public CountriesServiceTest()
    {
        var countriesInitialData = new List<Country>() { };
        // to create default dbContext options -moq dbcontext options
        DbContextMock<ApplicationDbContext> dbContextMock = 
            new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options); 

        //var dbContext= new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        ApplicationDbContext dbContext = dbContextMock.Object;

        //mock dbSets
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        _countriesService = new CountriesService(null);
    }

    #region AddCountry
    // When CountryAddRequest is null it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        //Arrange
        CountryAddRequest? request = null;

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async() =>
        {
            //Act
            await _countriesService.AddCountry(request);
        });
    }

    // When CountryName is null it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        //Arrange
        CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            //Act
            await _countriesService.AddCountry(request);
        });
    }

    // When CountryName is duplicaate it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicateContryName()
    {
        //Arrange
        CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Serbia" };
        CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Serbia" };

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async() =>
        {
            //Act
           await  _countriesService.AddCountry(request1);
           await _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper country name it should insert(add) the country to existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        //Arrange
        CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

        //Act
        CountryResponse response = await _countriesService.AddCountry(request);
        List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

        //Assert
        Assert.True(response.CountryID != Guid.Empty);
        //takes equals method between 2 object
        Assert.Contains(response, countries_from_GetAllCountries);
        
        //objA.Equals(objB) -> dasn't compare values
    }
    
    #endregion


    #region GetAllCountries

    //The list of countries should be empty by default (before adding any countries)
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        //Act
        List<CountryResponse> actual_country_response_list =
        await _countriesService.GetAllCountries();

        //Assert
        Assert.Empty(actual_country_response_list); // ako je collection list pun test je prosao
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        //Arrange
        List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
       {
            new CountryAddRequest() {CountryName = "USA"},
            new CountryAddRequest() {CountryName = "Russia"},
            new CountryAddRequest() {CountryName = "Japan"},
       };
        //Act
        List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

        foreach (CountryAddRequest country_request in country_request_list)
        {
            countries_list_from_add_country.Add (await _countriesService.AddCountry(country_request));
        }
        
        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        //read each element from countries_list_from_add_country
        foreach(CountryResponse expected_country in countries_list_from_add_country)
        {
            //Assert
            Assert.Contains(expected_country, actualCountryResponseList);
        }
    }


    #endregion

    #region GetCountryByCountryID

    [Fact]
    //If we supply null as CountryID, it should return null as CountryResponse
    public async Task GetCountryByCountryID_NullCountryID()
    {
        //Arrange
        Guid? countryID = null;

        //Act
        CountryResponse? country_respomse_from_get_method = await _countriesService.GetCountryByCountryId(countryID);

        //Assert
        Assert.Null(country_respomse_from_get_method);
    }

    [Fact]
    //If we supply a valid country id, it should return the matching country details as CountryResponse object
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        //Arrange
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "China" };
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        //Act
        CountryResponse? country_response_from_get =  await _countriesService.GetCountryByCountryId(country_response_from_add.CountryID);

        //Assert
        Assert.Equal(country_response_from_add, country_response_from_get);
    }

    #endregion

}
