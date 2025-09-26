using System.ComponentModel.DataAnnotations;

namespace QuestionService.Validators;

public class TagsValidator(int min, int max):ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is List<string> tags)
        {
            if (tags.Count >= min && tags.Count <= max) return ValidationResult.Success;
            if (tags.Count < min) return new ValidationResult($"Must have at least {min} Tags selected");
            if (tags.Count > max) return new ValidationResult($"Must have less than {max} Tags selected");
        }

        return new ValidationResult("value was not list of strings");
    }
}