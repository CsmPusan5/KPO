public class Helmet : IThing
{
    private readonly string _inventoryNumber;
    private readonly string _name;
    private readonly int _easyScore;
    public Helmet(string inventoryNumber, string name, int easyScore)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Инвентарный номер обязателен", nameof(inventoryNumber));

        if (easyScore < 1 || easyScore > 10)
            throw new ArgumentOutOfRangeException(nameof(easyScore), "Оценка простоты должна быть от 1 до 10");
        
        _inventoryNumber = inventoryNumber;
        _name = name;
        _easyScore = easyScore;
    }

    public string GetInventoryNumber() => _inventoryNumber;
    public string GetName() => _name;

    public int GetEasyScore() => _easyScore;
    public bool IsEasyForBeginner() => _easyScore >= 6;

    public void Use()
    {
        Console.WriteLine($"Шлем {_inventoryNumber} надет");
    }
}