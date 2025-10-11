namespace Pizzeria.Domain;
public enum PizzaSize { Small, Medium, Large }
public class Pizza
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public PizzaSize Size { get; private set; }
    public decimal Price { get; private set; }

    private Pizza() {} // EF
    public Pizza(int id, string name, PizzaSize size, decimal price)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));

        Id = id; Name = name.Trim(); Size = size; Price = price;
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0) throw new ArgumentOutOfRangeException(nameof(newPrice));
        Price = newPrice;
    }
}