using System;
using System.IO;
using System.Threading.Tasks;
using DndAssistant.Model;
using DndAssistant.Model.Items;
using DndAssistant.Services;
using Xunit;

namespace DndAssistant.Tests
{
    public class FileServiceTests : IDisposable
    {
        private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"dnd_test_{Guid.NewGuid()}.json");
        private readonly FileService _sut = new();

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);

            string backupFile = _tempFile + ".bak";
            if (File.Exists(backupFile))
                File.Delete(backupFile);
        }

        [Fact]
        public async Task SaveAsync_CreatesFile()
        {
            var session = new GameSession { Title = "Async тест" };

            await _sut.SaveAsync(session, _tempFile);

            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public async Task SaveAsync_WritesTitleToFile()
        {
            var session = new GameSession { Title = "Async кампания" };

            await _sut.SaveAsync(session, _tempFile);

            var json = await File.ReadAllTextAsync(_tempFile);
            Assert.Contains("Async кампания", json);
        }

        [Fact]
        public async Task SaveAsync_WritesGroupsToFile()
        {
            var session = new GameSession();
            session.Groups.Add(new PlayerGroup
            {
                Name = "Main Party",
                Members = { new Character { ChName = "Арагорн", ChRace = "Человек" } }
            });

            await _sut.SaveAsync(session, _tempFile);

            var json = await File.ReadAllTextAsync(_tempFile);
            Assert.Contains("Main Party", json);
            Assert.Contains("Арагорн", json);
        }

        [Fact]
        public async Task SaveAsync_WritesPolymorphicItemsToFile()
        {
            var session = new GameSession();
            session.Groups.Add(new PlayerGroup
            {
                Name = "Main Party",
                Members =
                {
                    new Character
                    {
                        ChName = "Леголас",
                        ChRace = "Эльф",
                        Inventory =
                        {
                            Items =
                            {
                                new Weapon
                                {
                                    Name = "Лук",
                                    Damage = 8,
                                    DamageType = "Piercing",
                                    WeaponType = "Bow"
                                }
                            }
                        }
                    }
                }
            });

            await _sut.SaveAsync(session, _tempFile);

            var json = await File.ReadAllTextAsync(_tempFile);
            Assert.Contains("weapon", json);
            Assert.Contains("Лук", json);
        }

        [Fact]
        public async Task SaveAndLoad_RestoresSession()
        {
            var session = new GameSession
            {
                Title = "Full session",
                Notes = "Test save and load"
            };
            session.Groups.Add(new PlayerGroup
            {
                Name = "Main Party",
                Members =
                {
                    new Character
                    {
                        ChName = "Арья",
                        ChRace = "Человек",
                        Inventory =
                        {
                            Items =
                            {
                                new Weapon { Name = "Меч", Damage = 7, DamageType = "Slashing", WeaponType = "Sword" }
                            }
                        }
                    }
                }
            });

            await _sut.SaveAsync(session, _tempFile);
            var loaded = await _sut.LoadAsync(_tempFile);

            Assert.Equal("Full session", loaded.Title);
            Assert.Single(loaded.Groups);
            Assert.Single(loaded.Groups[0].Members);
            Assert.Equal("Меч", loaded.Groups[0].Members[0].Inventory.Items[0].Name);
        }

        [Fact]
        public async Task LoadAsync_WhenFileMissing_ReturnsEmptySession()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);

            var loaded = await _sut.LoadAsync(_tempFile);

            Assert.Equal(string.Empty, loaded.Title);
            Assert.Empty(loaded.Groups);
        }

        [Fact]
        public async Task LoadAsync_WhenJsonIsBroken_ReturnsEmptySession()
        {
            await File.WriteAllTextAsync(_tempFile, "{ broken json");

            var loaded = await _sut.LoadAsync(_tempFile);

            Assert.Equal(string.Empty, loaded.Title);
            Assert.Empty(loaded.Groups);
        }

        [Fact]
        public async Task SaveAsync_WhenCancelled_ThrowsTaskCanceledException()
        {
            using var cts = new System.Threading.CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await _sut.SaveAsync(new GameSession(), _tempFile, cts.Token));
        }

        [Fact]
        public async Task LoadAsync_WhenCancelled_ThrowsTaskCanceledException()
        {
            await File.WriteAllTextAsync(_tempFile, "{ \"Title\": \"test\" }");

            using var cts = new System.Threading.CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await _sut.LoadAsync(_tempFile, cts.Token));
        }

        [Fact]
        public async Task SaveAsync_CreatesBackupFile_WhenTargetExists()
        {
            await File.WriteAllTextAsync(_tempFile, "original content");

            await _sut.SaveAsync(new GameSession { Title = "Backup test" }, _tempFile);

            Assert.True(File.Exists(_tempFile + ".bak"));
            var backup = await File.ReadAllTextAsync(_tempFile + ".bak");
            Assert.Contains("original content", backup);
        }
    }
}
