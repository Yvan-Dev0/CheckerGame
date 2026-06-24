using CheckerGame.DTOs;
using FluentValidation;

namespace CheckerGame.Validators
{
    public class PlayerDtoValidator : AbstractValidator<PLayerDto>
    {
        public PlayerDtoValidator()
        {
            // Username validator
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be atleast 3 characters")
                .MaximumLength(15).WithMessage("Username must be atleast 15 characters")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

            // Wins must be positive
            RuleFor(x => x.Wins)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Wins must be positive");

            // Losses must be positive
            RuleFor(x => x.Losses)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Loses must be positive");

            // Sanity check for wins and losses counts
            RuleFor(x => x)
                .Must(p => p.Wins + p.Losses <= 100000)
                .WithMessage("Total games played exceeds allowed threshold");
        }
    }
}
