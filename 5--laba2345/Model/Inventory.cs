using DndAssistant.Model.Items;

namespace DndAssistant.Model
{
    public class Inventory
    {
        public List<Item> Items { get; set; } = new();
        public int Copper { get; set; }
        public int Silver { get; set; }
        public int Gold { get; set; }
    }
}
