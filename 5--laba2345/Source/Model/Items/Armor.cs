namespace DndAssistant.Model.Items
{
    public class Armor : Item
    {
        public int ArmorClass { get; set; }
        public string ArmorType { get; set; } = "light";
        public bool IsMagical { get; set; }
        public int MagicalBonus { get; set; }
    }
}
