namespace DndAssistant.Model
{
    public class PlayerGroup
    {
        public string Name { get; set; } = string.Empty;
        public List<Character> Members { get; set; } = new();
    }
}
