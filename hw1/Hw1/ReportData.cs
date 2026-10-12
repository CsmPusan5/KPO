public class ReportData()
{
    private int totalItems;
    private double totalEnergy;
    private List<ITransport> transport = [];
    private List<IThing> things = [];
    // Можно расширить
    
    public void SetTotalItems(int totalItems) { this.totalItems = totalItems; }
    public void SetTotalEnergy(double totalEnergy) { this.totalEnergy = totalEnergy; }
    public void SetTransport(List<ITransport> transport) { this.transport = transport; }
    public void SetThings(List<IThing> things) { this.things = things; }

    public int GetTotalItems() => totalItems;
    public double GetTotalEnergy() => totalEnergy;
    public List<ITransport> GetTransport() => transport;
    public List<IThing> GetThings() => things;  
}