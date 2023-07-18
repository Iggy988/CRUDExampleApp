using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers;

public class PersonsController : Controller
{
    //private fields
    private readonly IPersonsService _personsService;

    //constructor
    public PersonsController(IPersonsService personsService)
    {
        _personsService = personsService;
    }

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName),
        SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        // Search
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryID), "Country" },
            { nameof(PersonResponse.Address), "Address" }

        };

        List<PersonResponse> persons =_personsService.GetFilteredPersons(searchBy, searchString);
        
        // da mozemo prenjeti vrijednost u view
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        //Sort
        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();
        //http://localhost:5267/persons/index?searchBy=&searchString=&sortBy=PersonName&sortOrder=DESC


        return View(sortedPersons); //Views/Persons/Index.cshtml
    }
}
