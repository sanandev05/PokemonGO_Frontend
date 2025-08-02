using System.Collections.Generic;

public class BadgeDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Level { get; set; }

    public List<int> TrainerIds { get; set; }
}