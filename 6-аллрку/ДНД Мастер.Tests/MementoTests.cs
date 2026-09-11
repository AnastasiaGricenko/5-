using DndAssistant;
using DndAssistant.Model.Items;
using Xunit;

namespace DndAssistant.Tests
{
    public class ItemMementoTests
    {
        [StaFact]
        public void ItemEditorViewModel_RestoreFromMemento_RestoresOriginalName()
        {
            var weapon = new Weapon { Name = "Меч", Damage = 6, DamageType = "Slashing", WeaponType = "Sword" };
            var vm = new ItemEditorViewModel(weapon);

            // Меняем имя через ViewModel
            vm.ItemName = "Топор";
            vm.SaveChanges();
            Assert.Equal("Топор", weapon.Name);

            // Откатываем — имя должно вернуться
            vm.RestoreFromMemento();

            Assert.Equal("Меч", weapon.Name);
        }

        [StaFact]
        public void ItemEditorViewModel_RestoreFromMemento_RestoresDamage()
        {
            var weapon = new Weapon { Name = "Лук", Damage = 8, DamageType = "Piercing", WeaponType = "Bow" };
            var vm = new ItemEditorViewModel(weapon);

            vm.Damage = 99;
            vm.SaveChanges();

            vm.RestoreFromMemento();

            Assert.Equal(8, weapon.Damage);
        }

        [StaFact]
        public void ItemEditorViewModel_SaveChanges_AppliesNewValues()
        {
            var armor = new Armor { Name = "Кольчуга", ArmorClass = 13, ArmorType = "Medium" };
            var vm = new ItemEditorViewModel(armor);

            vm.ItemName   = "Латный доспех";
            vm.ArmorClass = 18;
            vm.SaveChanges();

            Assert.Equal("Латный доспех", armor.Name);
            Assert.Equal(18, armor.ArmorClass);
        }
    }
}
