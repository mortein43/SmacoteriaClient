namespace Smacoteria.Model;

public class DishInOrder
{
    public int Id { get; set; }

    public int Count { get; set; } = 1;

    public List<Addition>? Additions { get; set; } = new ();

    public Dish? Dish { get; set; }

    public int? DishId { get; set; }

    public Order? Order { get; set; }

    public int? OrderId { get; set; }

    public float TotalCoast { get; set; }
}
