using System.ComponentModel.DataAnnotations;

namespace Sprint_1.DTOs;

public class EventRequestDto : IValidatableObject
{
    [Required(AllowEmptyStrings = false)]
    [MinLength(1)]
    public string? Title { get; init; }

    public string? Description { get; init; }

    [Required]
    public DateTime? StartAt { get; init; }

    [Required]
    public DateTime? EndAt { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartAt.HasValue && EndAt.HasValue && EndAt <= StartAt)
        {
            yield return new ValidationResult(
                "EndAt must be later than StartAt.",
                [nameof(StartAt), nameof(EndAt)]);
        }
    }
}