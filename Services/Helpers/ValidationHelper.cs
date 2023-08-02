﻿
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public class ValidationHelper
{
    internal static void ModelValidation(object obj)  
    {
        //Model Validations
        ValidationContext validationContext = new(obj);
        List<ValidationResult> validationResults = new List<ValidationResult>();
        // validiramo object instance, ValidationContext, IColections<ValidationResult> i stavljamo true, da bi validirali sve propertije
        // jer u suprotnom ce validirati samo required propertije
        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

        if (!isValid)
        {
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        }
    } 
}
