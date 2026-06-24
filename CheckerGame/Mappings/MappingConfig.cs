using CheckerGame.DTOs;
using CheckerGame.Models;
using Mapster;

namespace CheckerGame.Mappings
{
    public class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<GameState, GameDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Board, src => src.Board)
                .Map(dest => dest.CurrentTurnPlayerId, src => src.CurrentTurnPlayerId)
                .Map(dest => dest.IsFinished, src => src.IsFinished);

            TypeAdapterConfig<GameDto, GameState>.NewConfig()
                .Map(dest => dest.Board, src => src.Id)
                .Map(dest => dest.CurrentTurnPlayerId, src => src.CurrentTurnPlayerId)
                .Map(dest => dest.IsFinished, src => src.IsFinished);
        }
    }
}
