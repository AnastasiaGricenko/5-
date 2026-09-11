using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using DndAssistant.Interfaces;
using System.Diagnostics;
using DndAssistant.Model;
using DndAssistant.Model.Items;

namespace DndAssistant.Services
{
    /// <summary>
    /// Паттерн Facade (Фасад): скрывает детали JSON-сериализации и работы с файловой системой.
    ///
    /// Вызывающий код (GMViewModel) работает только через SaveAsync().
    /// </summary>
    public class FileService : IFileService
    {

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ConfigureItemPolymorphism }
            }
        };

        public FileService() { }

        public async Task SaveAsync(GameSession session, string filePath, CancellationToken ct = default)
        {
            try
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Debug.WriteLine($"Created directory: {directory}");
                }

                if (File.Exists(filePath))
                {
                    File.Copy(filePath, filePath + ".bak", overwrite: true);
                    Debug.WriteLine($"Created backup: {filePath}.bak");
                }

                string json = JsonSerializer.Serialize(session, JsonOptions);
                await File.WriteAllTextAsync(filePath, json, ct);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public async Task<GameSession> LoadAsync(string filePath, CancellationToken ct = default)
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"File not found: {filePath}. Returning empty session.");
                return new GameSession();
            }

            try
            {
                string json = await File.ReadAllTextAsync(filePath, ct);
                return JsonSerializer.Deserialize<GameSession>(json, JsonOptions) ?? new GameSession();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine(ex);
                return new GameSession();
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
                return new GameSession();
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
                return new GameSession();
            }
        }

        private static void ConfigureItemPolymorphism(JsonTypeInfo jsonTypeInfo)
        {
            if (jsonTypeInfo.Type != typeof(Item))
                return;

            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type"
            };
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Weapon), "weapon"));
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Armor), "armor"));
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Artifact), "artifact"));
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Trinket), "trinket"));
        }
    }
}
