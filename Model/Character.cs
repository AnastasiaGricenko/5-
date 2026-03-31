namespace DndAssistant;

public class Character
{
    public string Name { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Inventory { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Name}|{Race}|{Location}|{Description}|{Inventory}";
    }
}