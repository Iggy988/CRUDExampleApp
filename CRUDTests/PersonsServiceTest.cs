using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
namespace CRUDTests;

public class PersonsServiceTest
{
    //private field
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countryService;

    //coonstructor
    public PersonsServiceTest()
    {
        _personService = new PersonsService();
        _countryService = new CountriesService();
    }

    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_NullPerson()
    {
        //Arrange
        PersonAddRequest? personAddRequest = null;

        //Act
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personService.AddPerson(personAddRequest);
        });
    }

    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        //Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

        //Act
        Assert.Throws<ArgumentException>(() =>
        {
            _personService.AddPerson(personAddRequest);
        });
    }

    //When we supply proper person details, it should insert the person into the persons list;
    //and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        //Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest() 
        { PersonName = "Person name...", Email = "person@example.com", Address = "sample address",
            CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true
        };

        //Act
        PersonResponse person_response_from_add = _personService.AddPerson(personAddRequest);

        List<PersonResponse> persons_list = _personService.GetAllPersons();

        //Assert
        Assert.True(person_response_from_add.PersonID != Guid.Empty);

        Assert.Contains(person_response_from_add, persons_list);
    }

    #endregion

    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public void GetPersonByPersonID_NullPersonID()
    {
        //Arrange
        Guid? personID = null;

        //Act
        PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(personID);

        //Assert
        Assert.Null(person_response_from_get);

    }

    //If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public void GetPersonByPersonID_WithPersonID()
    {
        //Arange
        CountryAddRequest country_request = new(){CountryName = "Canada"};
        CountryResponse country_response =_countryService.AddCountry(country_request);

       //Act
        PersonAddRequest person_request = new PersonAddRequest() 
        { PersonName = "person name...", Email = "email@sample.com", Address = "address",
            CountryID = country_response.CountryId, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male,
            ReceiveNewsLetters = false };

        PersonResponse person_response_from_add = _personService.AddPerson(person_request);

        PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(person_response_from_add.PersonID);

        //Assert
        Assert.Equal(person_response_from_add, person_response_from_get);
    }


    #endregion
}
