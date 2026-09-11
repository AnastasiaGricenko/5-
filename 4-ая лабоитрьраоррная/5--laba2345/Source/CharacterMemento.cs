namespace DndAssistant.Model
{
    /// <summary>
    /// Паттерн Memento (Хранитель) — неизменяемый снимок состояния персонажа
    /// в момент открытия редактора предмета.
    ///
    /// Зачем это нужно:
    /// Когда пользователь нажимает «Отмена» в ItemEditorWindow, изменения должны
    /// быть откачены. CharacterMemento сохраняет исходные значения предмета,
    /// чтобы их можно было восстановить без сохранения на диск.
    ///
    /// Как это работает:
    /// 1) ItemEditorViewModel (Originator) создаёт снимок через CreateMemento()
    ///    в момент открытия окна редактора.
    /// 2) Снимок хранится внутри ItemEditorViewModel.
    /// 3) При нажатии «Отмена» вызывается RestoreFromMemento() —
    ///    исходные значения возвращаются в объект Item.
    /// </summary>
    public record ItemMemento(
        string Name,
        string Description,
        double Weight,
        // Weapon
        int Damage,
        string DamageType,
        string WeaponType,
        bool IsMagical,
        int MagicalBonus,
        // Armor
        int ArmorClass,
        string ArmorType,
        // Artifact
        string Power,
        int Rarity,
        // Trinket
        string Effect,
        string TrinketRarity
    );
}
