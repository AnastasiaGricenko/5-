namespace DndAssistant.Model
{
    public class GameSession
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
        public List<PlayerGroup> Groups { get; set; } = new();
    }
}
