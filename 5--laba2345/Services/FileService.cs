using System.IO;
using System.Text.Json;
using DndAssistant.Interfaces;
using DndAssistant.Model;

namespace DndAssistant.Services
{
    /// <summary>
    /// Паттерн Facade (Фасад): скрывает детали JSON-сериализации и работы с файловой системой.
    ///
    /// Вызывающий код (GMViewModel) не знает ни о JsonSerializer,
    /// ни о File.ReadAllText — он просто вызывает Save() и Load().
    /// </summary>
    public class FileService : IFileService
    {
        public void Save(GameSession session, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(session, options);
            File.WriteAllText(filePath, json);
        }

        public GameSession Load(string filePath)
        {
            if (!File.Exists(filePath)) return new GameSession();
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<GameSession>(json) ?? new GameSession();
        }
    }
}
