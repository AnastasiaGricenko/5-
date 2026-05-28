using DndAssistant;
using DndAssistant.Model;
using DndAssistant.Model.Items;
using DndAssistant.Services;
using Xunit;

namespace DndAssistant.Tests
{
    public class GameSessionTests
    {
        [Fact]
        public void NewGameSession_HasEmptyGroups()
        {
            var session = new GameSession();

            Assert.Empty(session.Groups);
        }

        [Fact]
        public void GameSession_TitleCanBeSet()
        {
            var session = new GameSession { Title = "Тест кампании" };

            Assert.Equal("Тест кампании", session.Title);
        }

        [Fact]
        public void GameSession_CanAddGroups()
        {
            var session = new GameSession();
            session.Groups.Add(new PlayerGroup { Name = "Группа А" });

            Assert.Single(session.Groups);
        }
    }

    public class CharacterTests
    {
        [Fact]
        public void Character_DefaultInventoryIsNotNull()
        {
            var ch = new Character();

            Assert.NotNull(ch.Inventory);
        }

        [Fact]
        public void Character_CanAddItemToInventory()
        {
            var ch = new Character();
            ch.Inventory.Items.Add(new Weapon { Name = "Меч" });

            Assert.Single(ch.Inventory.Items);
        }

        [Fact]
        public void Character_ToString_ContainsName()
        {
            var ch = new Character { ChName = "Гэндальф", ChRace = "Маг" };

            Assert.Contains("Гэндальф", ch.ToString());
        }

        [Fact]
        public void Character_Properties_CanBeSetAndRead()
        {
            var ch = new Character
            {
                ChName = "Фродо",
                ChRace = "Хоббит",
                ChDescription = "Хранитель Кольца"
            };

            Assert.Equal("Фродо", ch.ChName);
            Assert.Equal("Хоббит", ch.ChRace);
            Assert.Equal("Хранитель Кольца", ch.ChDescription);
        }
    }

    public class SessionManagerTests
    {
        [Fact]
        public void GetInstance_ReturnsSameInstance()
        {
            var first  = SessionManager.GetInstance();
            var second = SessionManager.GetInstance();

            Assert.Same(first, second);
        }

        [Fact]
        public void SessionManager_CurrentSession_CanBeReplaced()
        {
            var manager = SessionManager.GetInstance();
            var newSession = new GameSession { Title = "Новая кампания" };

            manager.CurrentSession = newSession;

            Assert.Equal("Новая кампания", manager.CurrentSession.Title);
        }

        [Fact]
        public void SessionManager_LastFilePath_CanBeSet()
        {
            var manager = SessionManager.GetInstance();

            manager.LastFilePath = "C:\\test\\session.json";

            Assert.Equal("C:\\test\\session.json", manager.LastFilePath);
        }
    }
}
