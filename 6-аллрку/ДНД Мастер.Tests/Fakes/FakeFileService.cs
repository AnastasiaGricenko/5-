using System.Threading;
using System.Threading.Tasks;
using DndAssistant.Interfaces;
using DndAssistant.Model;

namespace DndAssistant.Tests.Fakes
{
    /// <summary>
    /// Фейковая реализация IFileService для тестов.
    /// Хранит данные в памяти, не трогает диск.
    /// </summary>
    public class FakeFileService : IFileService
    {
        public GameSession? SavedSession { get; private set; }
        public bool SaveCalled { get; private set; }

        public Task SaveAsync(GameSession session, string filePath, CancellationToken ct = default)
        {
            SaveCalled = true;
            SavedSession = session;
            return Task.CompletedTask;
        }

        public Task<GameSession> LoadAsync(string filePath, CancellationToken ct = default)
        {
            return Task.FromResult(SavedSession ?? new GameSession());
        }
    }
}
