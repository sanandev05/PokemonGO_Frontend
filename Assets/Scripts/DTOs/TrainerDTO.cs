using System.Collections.Generic;

public class TrainerDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; } = "https://example.com/default-avatar.png";
    public int Level { get; set; }
    public int Experience { get; set; }
    public int MaxXP { get; set; }
    public int CurrentXP { get; set; }
    public decimal Gold { get; set; }


    public List<int> PokemonIds { get; set; } = new();
    public List<int>? BadgeIds { get; set; } = new();
    //public List<TournamentResult> TournamentResults { get; set; }
}

