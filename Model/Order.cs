namespace Smacoteria.Model;

public class Order
{
    public enum Statuses
    {
        Очікується,
        Прийняте,
        Готове,
        Видане,
    }

    public enum PaymentMethods
    {
        Картою,
        Готівкою,
    }

    public int Id { get; set; }

    public int TableNumber { get; set; }

    public Statuses Status { get; set; }

    public float TotalCost { get; set; } = 0;

    public string? Notice { get; set; }

    public PaymentMethods? PaymentMethod { get; set; } = null;

    public List<DishInOrder> Dishes { get; set; } = new();
}
