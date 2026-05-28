using System.Collections.Generic;
using System.Threading.Tasks;
using DndAssistant.Model;
using DndAssistant.Tests.Fakes;
using DndAssistant.Services;
using Xunit;

namespace DndAssistant.Tests
{
    public class GMViewModelTests
    {
        [Fact]
        public void NameGenerator_Generate_ReturnsNonEmptyString()
        {
            var gen = new NameGenerator();

            var name = gen.Generate();

            Assert.False(string.IsNullOrWhiteSpace(name));
        }

        [Fact]
        public void NameGenerator_Generate_ReturnsDifferentNamesOverTime()
        {
            var gen = new NameGenerator();
            var names = new HashSet<string>();

            for (int i = 0; i < 10; i++)
                names.Add(gen.Generate());

            Assert.True(names.Count > 1);
        }

        [Fact]
        public async Task FakeFileService_SaveAsync_IsCalled()
        {
            var fake = new FakeFileService();
            var session = new GameSession { Title = "Тест" };

            await fake.SaveAsync(session, "test.json");

            Assert.True(fake.SaveCalled);
            Assert.Equal("Тест", fake.SavedSession!.Title);
        }
    }
}
