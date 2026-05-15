namespace DndAssistant.Model.Items
{
    public class Weapon : Item
    {
        public int Damage { get; set; }
        public string DamageType { get; set; } = "physical";
        public string WeaponType { get; set; } = "melee";
        public bool IsMagical { get; set; }
        public int MagicalBonus { get; set; }
    }
}
