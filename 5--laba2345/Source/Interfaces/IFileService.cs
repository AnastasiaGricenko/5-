using System.Threading;
using System.Threading.Tasks;
using DndAssistant.Model;

namespace DndAssistant.Interfaces
{
    /// <summary>
    /// Паттерн Facade (Фасад): интерфейс фасада над файловым вводом-выводом.
    ///
    /// GMViewModel работает только с этим контрактом.
    /// </summary>
    public interface IFileService
    {
        Task SaveAsync(GameSession session, string filePath, CancellationToken ct = default);
        Task<GameSession> LoadAsync(string filePath, CancellationToken ct = default);
    }
}
