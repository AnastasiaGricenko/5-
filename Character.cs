using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DndAssistant;
public class Character : INotifyPropertyChanged
{
    private string _name, _race, _description, _location, _inventory;

    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
    public string Race { get => _race; set { _race = value; OnPropertyChanged(); } }
    public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
    public string Location { get => _location; set { _location = value; OnPropertyChanged(); } }
    public string Inventory { get => _inventory; set { _inventory = value; OnPropertyChanged(); } }

    public override string ToString() => $"{Name}|{Race}|{Location}|{Description}|{Inventory}";

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
