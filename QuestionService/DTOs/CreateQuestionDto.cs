using System.ComponentModel.DataAnnotations;
using QuestionService.Validators;

namespace QuestionService.DTOs;

public record CreateQuestionDto(
    [Required] string Title, 
    [Required] string Content, 
    [Required, TagsValidator(1,5)] List<string> Tags
);