namespace Hw1.Domain;

public class Bike : Transport
{
    public Bike(string inventoryNumber, string name, int easyScore)
        : base(inventoryNumber, name, easyScore) { }

    public override void Move()
        => Console.WriteLine($"Велосипед {GetInventoryNumber()} едет");
}
