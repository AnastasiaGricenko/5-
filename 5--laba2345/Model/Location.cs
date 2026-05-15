namespace DndAssistant.Model
{
    public class Location
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Events { get; set; } = new();
    }
}
