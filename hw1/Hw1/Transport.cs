public abstract class Transport(string inventoryNumber, string name, int easyScore) : InventoryItem(inventoryNumber, name, easyScore)
{
    public abstract void Move();
}