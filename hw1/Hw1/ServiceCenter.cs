
public class ServiceCenter : IServiceCenter
{
    public bool Check(IInventoryItem thing)
    {
        // Тестовая проверка
        if (3 <= thing.GetName().Length && thing.GetName().Length <= 20)
            return true;
        return false;
    }
}
