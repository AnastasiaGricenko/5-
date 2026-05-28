using System.Windows;
using DndAssistant.Model;
using DndAssistant.Model.Items;

namespace DndAssistant
{
    /// <summary>
    /// ViewModel для окна редактирования предмета.
    /// Адаптирует любой тип Item (Weapon, Armor, Artifact, Trinket)
    /// к единому набору DependencyProperty для отображения в UI.
    /// </summary>
    public class ItemEditorViewModel : DependencyObject
    {
        private Item _item;
        private readonly ItemMemento _memento;

        public string ItemName
        {
            get => (string)GetValue(ItemNameProperty);
            set => SetValue(ItemNameProperty, value);
        }
        public static readonly DependencyProperty ItemNameProperty =
            DependencyProperty.Register(nameof(ItemName), typeof(string), typeof(ItemEditorViewModel));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(ItemEditorViewModel));

        public double Weight
        {
            get => (double)GetValue(WeightProperty);
            set => SetValue(WeightProperty, value);
        }
        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register(nameof(Weight), typeof(double), typeof(ItemEditorViewModel));

        // Weapon properties
        public int Damage
        {
            get => (int)GetValue(DamageProperty);
            set => SetValue(DamageProperty, value);
        }
        public static readonly DependencyProperty DamageProperty =
            DependencyProperty.Register(nameof(Damage), typeof(int), typeof(ItemEditorViewModel));

        public string DamageType
        {
            get => (string)GetValue(DamageTypeProperty);
            set => SetValue(DamageTypeProperty, value);
        }
        public static readonly DependencyProperty DamageTypeProperty =
            DependencyProperty.Register(nameof(DamageType), typeof(string), typeof(ItemEditorViewModel));

        public string WeaponType
        {
            get => (string)GetValue(WeaponTypeProperty);
            set => SetValue(WeaponTypeProperty, value);
        }
        public static readonly DependencyProperty WeaponTypeProperty =
            DependencyProperty.Register(nameof(WeaponType), typeof(string), typeof(ItemEditorViewModel));

        public bool IsMagical
        {
            get => (bool)GetValue(IsMagicalProperty);
            set => SetValue(IsMagicalProperty, value);
        }
        public static readonly DependencyProperty IsMagicalProperty =
            DependencyProperty.Register(nameof(IsMagical), typeof(bool), typeof(ItemEditorViewModel));

        public int MagicalBonus
        {
            get => (int)GetValue(MagicalBonusProperty);
            set => SetValue(MagicalBonusProperty, value);
        }
        public static readonly DependencyProperty MagicalBonusProperty =
            DependencyProperty.Register(nameof(MagicalBonus), typeof(int), typeof(ItemEditorViewModel));

        // Armor properties
        public int ArmorClass
        {
            get => (int)GetValue(ArmorClassProperty);
            set => SetValue(ArmorClassProperty, value);
        }
        public static readonly DependencyProperty ArmorClassProperty =
            DependencyProperty.Register(nameof(ArmorClass), typeof(int), typeof(ItemEditorViewModel));

        public string ArmorType
        {
            get => (string)GetValue(ArmorTypeProperty);
            set => SetValue(ArmorTypeProperty, value);
        }
        public static readonly DependencyProperty ArmorTypeProperty =
            DependencyProperty.Register(nameof(ArmorType), typeof(string), typeof(ItemEditorViewModel));

        // Artifact properties
        public string Power
        {
            get => (string)GetValue(PowerProperty);
            set => SetValue(PowerProperty, value);
        }
        public static readonly DependencyProperty PowerProperty =
            DependencyProperty.Register(nameof(Power), typeof(string), typeof(ItemEditorViewModel));

        public int Rarity
        {
            get => (int)GetValue(RarityProperty);
            set => SetValue(RarityProperty, value);
        }
        public static readonly DependencyProperty RarityProperty =
            DependencyProperty.Register(nameof(Rarity), typeof(int), typeof(ItemEditorViewModel));

        // Trinket properties
        public string Effect
        {
            get => (string)GetValue(EffectProperty);
            set => SetValue(EffectProperty, value);
        }
        public static readonly DependencyProperty EffectProperty =
            DependencyProperty.Register(nameof(Effect), typeof(string), typeof(ItemEditorViewModel));

        public string TrinketRarity
        {
            get => (string)GetValue(TrinketRarityProperty);
            set => SetValue(TrinketRarityProperty, value);
        }
        public static readonly DependencyProperty TrinketRarityProperty =
            DependencyProperty.Register(nameof(TrinketRarity), typeof(string), typeof(ItemEditorViewModel));

        public bool IsWeapon => _item is Weapon;
        public bool IsArmor => _item is Armor;
        public bool IsArtifact => _item is Artifact;
        public bool IsTrinket => _item is Trinket;

        public ItemEditorViewModel(Item item)
        {
            _item = item;
            ItemName = item.Name;
            Description = item.Description;
            Weight = item.Weight;

            if (item is Weapon weapon)
            {
                Damage = weapon.Damage;
                DamageType = weapon.DamageType;
                WeaponType = weapon.WeaponType;
                IsMagical = weapon.IsMagical;
                MagicalBonus = weapon.MagicalBonus;
            }
            else if (item is Armor armor)
            {
                ArmorClass = armor.ArmorClass;
                ArmorType = armor.ArmorType;
                IsMagical = armor.IsMagical;
                MagicalBonus = armor.MagicalBonus;
            }
            else if (item is Artifact artifact)
            {
                Power = artifact.Power;
                Rarity = artifact.Rarity;
            }
            else if (item is Trinket trinket)
            {
                Effect = trinket.Effect;
                TrinketRarity = trinket.Rarity;
            }

            // Паттерн Memento: сохранить состояние предмета до редактирования
            _memento = CreateMemento();
        }

        public void SaveChanges()
        {
            _item.Name = ItemName;
            _item.Description = Description;
            _item.Weight = Weight;

            if (_item is Weapon weapon)
            {
                weapon.Damage = Damage;
                weapon.DamageType = DamageType;
                weapon.WeaponType = WeaponType;
                weapon.IsMagical = IsMagical;
                weapon.MagicalBonus = MagicalBonus;
            }
            else if (_item is Armor armor)
            {
                armor.ArmorClass = ArmorClass;
                armor.ArmorType = ArmorType;
                armor.IsMagical = IsMagical;
                armor.MagicalBonus = MagicalBonus;
            }
            else if (_item is Artifact artifact)
            {
                artifact.Power = Power;
                artifact.Rarity = Rarity;
            }
            else if (_item is Trinket trinket)
            {
                trinket.Effect = Effect;
                trinket.Rarity = TrinketRarity;
            }
        }

        /// <summary>
        /// Паттерн Memento (Originator): создать снимок текущего состояния.
        /// </summary>
        private ItemMemento CreateMemento() => new(
            ItemName ?? string.Empty,
            Description ?? string.Empty,
            Weight,
            Damage,
            DamageType ?? string.Empty,
            WeaponType ?? string.Empty,
            IsMagical,
            MagicalBonus,
            ArmorClass,
            ArmorType ?? string.Empty,
            Power ?? string.Empty,
            Rarity,
            Effect ?? string.Empty,
            TrinketRarity ?? string.Empty
        );

        /// <summary>
        /// Паттерн Memento (Originator): восстановить состояние предмета из снимка.
        /// Вызывается при нажатии «Отмена» — откатывает все изменения.
        /// </summary>
        public void RestoreFromMemento()
        {
            _item.Name        = _memento.Name;
            _item.Description = _memento.Description;
            _item.Weight      = _memento.Weight;

            if (_item is Weapon weapon)
            {
                weapon.Damage       = _memento.Damage;
                weapon.DamageType   = _memento.DamageType;
                weapon.WeaponType   = _memento.WeaponType;
                weapon.IsMagical    = _memento.IsMagical;
                weapon.MagicalBonus = _memento.MagicalBonus;
            }
            else if (_item is Armor armor)
            {
                armor.ArmorClass   = _memento.ArmorClass;
                armor.ArmorType    = _memento.ArmorType;
                armor.IsMagical    = _memento.IsMagical;
                armor.MagicalBonus = _memento.MagicalBonus;
            }
            else if (_item is Artifact artifact)
            {
                artifact.Power  = _memento.Power;
                artifact.Rarity = _memento.Rarity;
            }
            else if (_item is Trinket trinket)
            {
                trinket.Effect  = _memento.Effect;
                trinket.Rarity  = _memento.TrinketRarity;
            }
        }
    }
}
