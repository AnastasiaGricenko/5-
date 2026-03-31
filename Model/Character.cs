namespace DndAssistant;

public class Character
{
    public string ChName { get; set; } = string.Empty;
    public string ChRace { get; set; } = string.Empty;
    public string ChDescription { get; set; } = string.Empty;
    public string ChLocation { get; set; } = string.Empty;
    public string ChInventory { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{ChName}|{ChRace}|{ChLocation}|{ChDescription}|{ChInventory}";
    }
}




















//досвязи