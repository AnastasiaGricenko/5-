using System.Text.Json.Serialization;

namespace DndAssistant.Model.Items
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(Weapon), typeDiscriminator: "weapon")]
    [JsonDerivedType(typeof(Armor), typeDiscriminator: "armor")]
    [JsonDerivedType(typeof(Artifact), typeDiscriminator: "artifact")]
    [JsonDerivedType(typeof(Trinket), typeDiscriminator: "trinket")]
    public class Item 
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Weight { get; set; }
    }
}
