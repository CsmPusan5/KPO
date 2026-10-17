using Hw1.Domain;

namespace Hw1.Application;

public class ReportData()
{
    private int totalItems;
    private double totalEnergy;
    private List<Transport> transport = [];
    private List<Thing> things = [];
    // Можно расширить
    
    public void SetTotalItems(int totalItems) { this.totalItems = totalItems; }
    public void SetTotalEnergy(double totalEnergy) { this.totalEnergy = totalEnergy; }
    public void SetTransport(List<Transport> transport) { this.transport = transport; }
    public void SetThings(List<Thing> things) { this.things = things; }

    public int GetTotalItems() => totalItems;
    public double GetTotalEnergy() => totalEnergy;
    public List<Transport> GetTransport() => transport;
    public List<Thing> GetThings() => things;  
}