using DndAssistant.Model;

namespace DndAssistant;

public class Character
{
    public string ChName { get; set; } = string.Empty;
    public string ChRace { get; set; } = string.Empty;
    public string ChDescription { get; set; } = string.Empty;
    public Inventory Inventory { get; set; } = new();
    public Location Location { get; set; } = new();

    public override string ToString()
    {
        return $"{ChName}|{ChRace}|{Location.Name}|{ChDescription}|{Inventory.Gold}";
    }
}




















//досвязи