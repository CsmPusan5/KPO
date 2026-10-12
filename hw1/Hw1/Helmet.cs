public class Helmet : Thing
{
    public Helmet(string inventoryNumber, string name, int easyScore)
        : base(inventoryNumber, name, easyScore) { }

    public override void Use()
        => Console.WriteLine($"Шлем {GetInventoryNumber()} надет");
}