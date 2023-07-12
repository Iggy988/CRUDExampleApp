﻿using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    //constructor
    public CountriesServiceTest()
    {
        _countriesService = new CountriesService();
    }

    #region AddCountry
    // When CountryAddRequest is null it should throw ArgumentNullException
    [Fact]
    public void AddCountry_NullCountry()
    {
        //Arrange
        CountryAddRequest? request = null;

        //Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            //Act
            _countriesService.AddCountry(request);
        });
    }

    // When CountryName is null it should throw ArgumentException
    [Fact]
    public void AddCountry_CountryNameIsNull()
    {
        //Arrange
        CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _countriesService.AddCountry(request);
        });
    }

    // When CountryName is duplicaate it should throw ArgumentException
    [Fact]
    public void AddCountry_DuplicateContryName()
    {
        //Arrange
        CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Serbia" };
        CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Serbia" };

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper country name it should insert(add) the country to existing list of countries
    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        //Arrange
        CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

        //Act
        CountryResponse response = _countriesService.AddCountry(request);
        List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();

        //Assert
        Assert.True(response.CountryId != Guid.Empty);
        //takes equals method between 2 object
        Assert.Contains(response, countries_from_GetAllCountries);
        
        //objA.Equals(objB) -> dasn't compare values
    }
    
    #endregion


    #region GetAllCountries

    //The list of countries should be empty by default (before adding any countries)
    [Fact]
    public void GetAllCountries_EmptyList()
    {
        //Act
        List<CountryResponse> actual_country_response_list =
        _countriesService.GetAllCountries();

        //Assert
        Assert.Empty(actual_country_response_list); // ako je collection list pun test je prosao
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
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
            countries_list_from_add_country.Add (_countriesService.AddCountry(country_request));
        }
        
        List<CountryResponse> actualCountryResponseList =  _countriesService.GetAllCountries();

        //read each element from countries_list_from_add_country
        foreach(CountryResponse expected_country in countries_list_from_add_country)
        {
            //Assert
            Assert.Contains(expected_country, actualCountryResponseList);
        }
    }


    #endregion

}
