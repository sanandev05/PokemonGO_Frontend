using System.Collections.Generic;

public record PokemonDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int XP { get; set; }
    public int Level { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int AttackPower { get; set; }

    public int CategoryId { get; set; }
    public List<int>? TrainerIds { get; set; }

    public List<int> AbilityIds { get; set; }
}

