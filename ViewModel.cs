using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DndAssistant;
public class GMViewModel : INotifyPropertyChanged
{
    private string _filePath = "characters.txt";
    private string _searchText = "";
    private string _selectedLocation = "Все";

    public ObservableCollection<Character> AllCharacters { get; set; } = new ObservableCollection<Character>();
    
    public IEnumerable<Character> FilteredCharacters => AllCharacters
        .Where(c => (c.Name.ToLower().Contains(_searchText.ToLower())) && 
                    (_selectedLocation == "Все" || c.Location == _selectedLocation));

    public string SearchText 
    { 
        get => _searchText; 
        set { _searchText = value; OnPropertyChanged(nameof(FilteredCharacters)); } 
    }

    public void SaveToFile()
    {
        var lines = AllCharacters.Select(c => c.ToString());
        File.WriteAllLines(_filePath, lines);
    }

    public void LoadFromFile()
    {
        if (!File.Exists(_filePath)) return;
        var lines = File.ReadAllLines(_filePath);
        AllCharacters.Clear();
        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts.Length == 5)
            {
                AllCharacters.Add(new Character { 
                    Name = parts[0], Race = parts[1], Location = parts[2], 
                    Description = parts[3], Inventory = parts[4] 
                });
            }
        }
    }
    public void NotifyCharactersChanged()
    {
        OnPropertyChanged(nameof(FilteredCharacters));
    }


    public string GenerateRandomName()
    {
        string[] firstNames = { "Арагорн", "Гимли", "Леголас", "Фродо", "Гэндальф" };
        string[] lastNames = { "Смелый", "Мудрый", "Железная Стопа", "Странник" };
        Random rnd = new Random();
        return firstNames[rnd.Next(firstNames.Length)] + " " + lastNames[rnd.Next(lastNames.Length)];
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
