﻿using EntityFrameworkCoreMock;
using Enttities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace CRUDTests;

public class PersonsServiceTest
{

    //private field
    private readonly IPersonsService _personService;
    //za printanje u consoli
    private readonly ITestOutputHelper _testOutputHelper;
    //AutoFixture -> for filling dummy values into properties
    private readonly IFixture _fixture;

    private readonly IPersonsRepository _personsRepository;
    private readonly Mock<IPersonsRepository> _personRepositoryMock;

    //coonstructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();  
        _personRepositoryMock = new Mock<IPersonsRepository>();
        //koristimo dummy implementation of repo
        _personsRepository = _personRepositoryMock.Object;

        //_countriesService = new CountriesService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));
        _personService = new PersonsService(_personsRepository); //PersonsService has reference to personsRepository -> fake object(mocked obj) mock repository
        //_personsService = new PersonsService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options), _countriesService); 

        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        //Arrange
        PersonAddRequest? personAddRequest = null;

        //Act
        // fluent assertion package
        Func<Task> action = (async () => 
        {
            await _personService.AddPerson(personAddRequest);
        });
        
        await action.Should().ThrowAsync<ArgumentNullException>(); // fluent assertion package

      
    }

    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {

        //Arrange
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, null as string)
         .Create();

        Person person = personAddRequest.ToPerson();

        //When PersonsRepository.AddPerson is called, it has to return the same "person" object
        _personRepositoryMock
         .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
         .ReturnsAsync(person);

        //Act
        Func<Task> action = async () =>
        {
            await _personService.AddPerson(personAddRequest);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When we supply proper person details, it should insert the person into the persons list;
    //and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
   
        //Arrange
        //AutoFixture create dummy model
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com")
            // Create -> kreira model automatski, Build -> mozemo pristupiti pojedinacnim props sa With (za custom property)
            .Create();

        Person person = personAddRequest.ToPerson();
        PersonResponse person_response_expected = person.ToPersonResponse();

        //if we supply any argument value to the AddPerson method, it should return the same return value
        _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            //different data type (Person) from upload data type It.IsAny<Person>())
            .ReturnsAsync(person);

        //Act
        PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
        person_response_expected.PersonID = person_response_from_add.PersonID;

        //Assert
        //Assert.True(person_response_from_add.PersonID != Guid.Empty);
        person_response_from_add.PersonID.Should().NotBe(Guid.Empty); // fluent assertion package

        person_response_from_add.Should().Be(person_response_expected);
    }

    #endregion

    #region GetFilteredPersons

    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
    {
        //Arrange
        List<Person> persons = new List<Person>() {
    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_1@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_2@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_3@example.com")
    .With(temp => temp.Country, null as Country)
    .Create()
   };

        List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_expected)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        _personRepositoryMock.Setup(temp => temp
        .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
         .ReturnsAsync(persons);

        //Act
        List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
    }


    //Search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
        //Arrange
        List<Person> persons = new List<Person>() {
    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_1@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_2@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_3@example.com")
    .With(temp => temp.Country, null as Country)
    .Create()
   };

        List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_expected)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        _personRepositoryMock.Setup(temp => temp
        .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
         .ReturnsAsync(persons);

        //Act
        List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
    }

    #endregion

    #region GetSortedPersons

    //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
        //Arrange
        List<Person> persons = new List<Person>() {
    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_1@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_2@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_3@example.com")
    .With(temp => temp.Country, null as Country)
    .Create()
   };

        List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        _personRepositoryMock
         .Setup(temp => temp.GetAllPersons())
         .ReturnsAsync(persons);


        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_expected)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }
        List<PersonResponse> allPersons = await _personService.GetAllPersons();

        //Act
        List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_sort)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
    }
    #endregion

    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
    {
        //Arrange
        Guid? personID = null;

        //Act
        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

        //Assert
        person_response_from_get.Should().BeNull();
    }

    //If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSucessful()
    {
        //Arange
        Person person = _fixture.Build<Person>()
         .With(temp => temp.Email, "email@sample.com")
         .With(temp => temp.Country, null as Country)
         .Create();
        PersonResponse person_response_expected = person.ToPersonResponse();

        _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
         .ReturnsAsync(person);

        //Act
        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person.PersonID);

        //Assert
        person_response_from_get.Should().Be(person_response_expected);
    }


    #endregion

    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        //Arrange
        PersonUpdateRequest? person_update_request = null;

        //Assert
        /*await Assert.ThrowsAsync<ArgumentNullException>(async() => {
            //Act
            await _personService.UpdatePerson(person_update_request);
        });*/
        // fluent assertion package
        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        };
        //Assertion
        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
        //Arrange
        //PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };
        PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>().Create();

        //Act
        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        };
        //Assertion
        await action.Should().ThrowAsync<ArgumentException>();
        /*await Assert.ThrowsAsync<ArgumentException>(async() => {
            //Act
            await _personService.UpdatePerson(person_update_request);
        });*/
        

    }


    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
         .With(temp => temp.PersonName, null as string)
         .With(temp => temp.Email, "someone@example.com")
         .With(temp => temp.Country, null as Country)
         .With(temp => temp.Gender, "Male")
         .Create();

        PersonResponse person_response_from_add = person.ToPersonResponse();

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();


        //Act
        var action = async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }


    //First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
         .With(temp => temp.Email, "someone@example.com")
         .With(temp => temp.Country, null as Country)
         .With(temp => temp.Gender, "Male")
         .Create();

        PersonResponse person_response_expected = person.ToPersonResponse();

        PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

        _personRepositoryMock
         .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
         .ReturnsAsync(person);

        _personRepositoryMock
         .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
         .ReturnsAsync(person);

        //Act
        PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

        //Assert
        person_response_from_update.Should().Be(person_response_expected);
    }

    #endregion

    #region DeletePerson

    //If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
         .With(temp => temp.Email, "someone@example.com")
         .With(temp => temp.Country, null as Country)
         .With(temp => temp.Gender, "Female")
         .Create();


        _personRepositoryMock
         .Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
         .ReturnsAsync(true);

        _personRepositoryMock
         .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
         .ReturnsAsync(person);

        //Act
        bool isDeleted = await _personService.DeletePerson(person.PersonID);

        //Assert
        isDeleted.Should().BeTrue();
    }


    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        //Act
        bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

        //Assert
        isDeleted.Should().BeFalse();
    }

    #endregion

    #region GetAllPersons

    //The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_ToBeEmptyList()
    {
        //Arrange
        var persons = new List<Person>();
        _personRepositoryMock
         .Setup(temp => temp.GetAllPersons())
         .ReturnsAsync(persons);

        //Act
        List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

        //Assert
        persons_from_get.Should().BeEmpty();
    }

    //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
    {
        //Arrange
        List<Person> persons = new List<Person>() {
    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_1@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_2@example.com")
    .With(temp => temp.Country, null as Country)
    .Create(),

    _fixture.Build<Person>()
    .With(temp => temp.Email, "someone_3@example.com")
    .With(temp => temp.Country, null as Country)
    .Create()
   };

        List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_expected)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        //Act
        List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_get)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
    }
    #endregion
}

