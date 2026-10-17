namespace Hw1.Domain;

public abstract class InventoryItem : IInventoryItem, IEasyForBeginners
{
    private readonly string _inventoryNumber;
    private readonly string _name;
    private readonly int _easyScore;

    protected InventoryItem(string inventoryNumber, string name, int easyScore)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Инвентарный номер обязателен", nameof(inventoryNumber));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование обязательно", nameof(name));

        if (easyScore < 1 || easyScore > 10)
            throw new ArgumentOutOfRangeException(nameof(easyScore), "Оценка простоты должна быть от 1 до 10");

        _inventoryNumber = inventoryNumber;
        _name = name;
        _easyScore = easyScore;
    }

    public string GetInventoryNumber() => _inventoryNumber;
    public string GetName() => _name;
    public int GetEasyScore() => _easyScore;
    public bool IsEasyForBeginners() => GetEasyScore() >= 6;
}