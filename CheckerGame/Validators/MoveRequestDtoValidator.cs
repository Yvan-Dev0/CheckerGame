using CheckerGame.DTOs;
using FluentValidation;

namespace CheckerGame.Validators
{
    public class MoveRequestDtoValidator : AbstractValidator<MoveRequestDto>
    {
        public MoveRequestDtoValidator()
        {
            // GameId must be valid
            RuleFor(x => x.GameId)
                .GreaterThan(0)
                .WithMessage("GameId must be a positive integer.");

            // Coordinates must be within board bounds
            RuleFor(x => x.FromX)
                .InclusiveBetween(0, 7)
                .WithMessage("FromX must be between 0 and 7");
            
            RuleFor(x => x.FromY)
                .InclusiveBetween(0, 7)
                .WithMessage("FromY must be between 0 and 7");

            RuleFor(x => x.ToX)
                .InclusiveBetween(0, 7)
                .WithMessage("ToX must be between 0 and 7");

            RuleFor(x => x.ToY)
                .InclusiveBetween(0, 7)
                .WithMessage("ToY must be between 0 and 7");

            // Move must be diagonal
            RuleFor(x => x)
                .Must(m =>
                {
                    var dx = Math.Abs(m.ToX - m.FromX);
                    var dy = Math.Abs(m.ToY - m.FromY);
                    return (dx == 1 && dy == 1) || (dx == 2 && dy == 2);
                })
                .WithMessage("Move must be a valid step");
        }
    }
}
