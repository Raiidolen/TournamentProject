namespace TournamentApp.API.DTOs;

public record CreateTournamentDto(string Name, string StrategyType, List<string> PlayerNames);

public record TournamentResponseDto(
    Guid Id,
    string Name,
    string JoinCode,
    string Status,
    string StrategyType,
    List<PlayerDto> Players,
    List<MatchDto> Matches
);

public record PlayerDto(Guid Id, string Name, int Seed);

public record MatchDto(
    Guid Id,
    int RoundNumber,
    int MatchOrder,
    Guid? Player1Id,
    string? Player1Name,
    int Score1,
    Guid? Player2Id,
    string? Player2Name,
    int Score2,
    Guid? WinnerId,
    string Status,
    Guid? NextMatchId
);

public record UpdateScoreDto(int Score1, int Score2);