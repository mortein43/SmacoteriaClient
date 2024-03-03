namespace Smacoteria.Model;

public class Dish
{
    public enum CountTypes
    {
        г,
        мл,
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public float Price { get; set; }

    public int Count { get; set; }

    public CountTypes CountType { get; set; }

    public Category? Category { get; set; }

    public int? CategoryId { get; set; }

    public string? Photo { get; set; }

    public List<DishInOrder>? Orders { get; set; } = new ();
}
