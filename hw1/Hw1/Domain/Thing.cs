namespace Hw1.Domain;

public abstract class Thing(string inventoryNumber, string name, int easyScore) : InventoryItem(inventoryNumber, name, easyScore)
{
    public abstract void Use();
}