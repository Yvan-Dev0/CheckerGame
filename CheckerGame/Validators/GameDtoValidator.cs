using CheckerGame.DTOs;
using FluentValidation;

namespace CheckerGame.Validators
{
    public class GameDtoValidator : AbstractValidator<GameDto>
    {
        public GameDtoValidator()
        {
            // Board must not be empty or null
            RuleFor(x => x.Board)
                .NotEmpty()
                .WithMessage("Board State cannot be empty")
                .Must(BeValidBoardJson)
                .WithMessage("Board must be a valid JSON representation");

            // CurrentTurnPlayerId must be 1 or 2
            RuleFor(x => x.CurrentTurnPlayerId)
                .InclusiveBetween(1, 2)
                .WithMessage("Current player must be either 1 or 2");

            // IsFinished should be consistent with board state
            RuleFor(x => x)
                .Must(game => !(game.IsFinished && string.IsNullOrWhiteSpace(game.Board)))
                .WithMessage("Finished games must have a valid board state");
        }

        private bool BeValidBoardJson(string boardJson)
        {
            try
            {
                var board = System.Text.Json.JsonSerializer.Deserialize<int[,]>(boardJson);
                return board != null && board.GetLength(0) == 8 && board.GetLength(1) == 8;
            }
            catch 
            { 
                return false; 
            }
        }
    }
}
