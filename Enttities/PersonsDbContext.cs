using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enttities;

public class PersonsDbContext : DbContext
{
    //sve sto je uneseno u options u Program.cs (DbContextOptionsBuilder) bice proslijedjeno ovdje u options preko base
    public PersonsDbContext(DbContextOptions options):base(options)
    {
        
    }

    //db set per model class
    public DbSet<Country> Countries { get; set; }
    public DbSet<Person> Persons { get; set; }

    //overridujemo metodu za spajanje dbSeta sa model class
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // za spajanje dbseta za tablu(dajemo ime table)
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        //Seed to Countries
        //modelBuilder.Entity<Country>().HasData(new Country { CountryID = Guid.NewGuid(), CountryName = "Sample" });

        string countriesJson = File.ReadAllText("countries.json");
        List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

        foreach(Country country in countries)
        {
            modelBuilder.Entity<Country>().HasData(country);
        }

        //Seed to Persons
        //modelBuilder.Entity<Country>().HasData(new Country { CountryID = Guid.NewGuid(), CountryName = "Sample" });

        string personsJson = File.ReadAllText("persons.json");
        List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

        foreach (Person person in persons)
        {
            modelBuilder.Entity<Person>().HasData(person);
        }

    }

    public List<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
    }

    public int sp_InsertPerson(Person person)
    {
        // to supply parasmeters to stored procedure
        SqlParameter[] parameters = new SqlParameter[] {
        new SqlParameter("@PersonID", person.PersonID),
        new SqlParameter("@PersonName", person.PersonName),
        new SqlParameter("@Email", person.Email),
        new SqlParameter("@DateOfBirth", person.DateOfBirth),
        new SqlParameter("@Gender", person.Gender),
        new SqlParameter("@CountryID", person.CountryID),
        new SqlParameter("@Address", person.Address),
        new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
      };
        return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
    }
}
