using DndAssistant.Model;

namespace DndAssistant.Interfaces
{
    /// <summary>
    /// Паттерн Facade (Фасад): интерфейс фасада над файловым вводом-выводом.
    ///
    /// GMViewModel не знает о JSON, путях к файлам и деталях сериализации.
    /// Он работает только с этим контрактом.
    /// </summary>
    public interface IFileService
    {
        void Save(GameSession session, string filePath);
        GameSession Load(string filePath);
    }
}
