namespace Smacoteria.Model;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<Dish> Dishes { get; set; } = new();

    public List<Addition>? Additions { get; set; } = new();
}
