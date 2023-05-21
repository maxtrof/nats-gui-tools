using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace UI.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    public ObservableCollection<UserVariable> UserVariables { get; }
    public ReactiveCommand<Unit, Dictionary<string, string>> UpdateUserDictionaryCommand { get; }
    public ICommand CreateNewVariable { get; }

    public SettingsViewModel() : this(new Dictionary<string, string>())
    {
       
    }
   

    public SettingsViewModel(Dictionary<string, string> userDictionary)
    {
        // Removing first symbol from name ($ symbol)
        UserVariables = new ObservableCollection<UserVariable>
            (userDictionary.Select(x => new UserVariable(x.Key[1..], x.Value, RemoveEventHandler)).ToList());

        CreateNewVariable = ReactiveCommand.Create(() =>
        {
            UserVariables.Add(new UserVariable("","", RemoveEventHandler));
        });

        UpdateUserDictionaryCommand = ReactiveCommand.Create(() =>
            UserVariables.ToDictionary(keySelector: m => m.NameWithPrefix, elementSelector: m => m.Value));
    }

    private void RemoveEventHandler(object? o, string s) 
    {
        var item = UserVariables.First(x => x.Name == s);
        UserVariables.Remove(item);
    }
}

/// <summary>
/// Single element from User variables list
/// </summary>
public sealed class UserVariable : ViewModelBase
{
    private string _name = default!;
    private string _value = default!;

    public UserVariable(string name, string value, EventHandler<string> removeMeHandler)
    {
        Name = name;
        Value = value;
        RemoveMe = ReactiveCommand.Create<Action>(x => RemoveMeHandler?.Invoke(this, Name));
        RemoveMeHandler += removeMeHandler;
    }
    public ICommand RemoveMe { get; }

    [Required]
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    [Required]
    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public string NameWithPrefix => $"${Name}";

    public event EventHandler<string> RemoveMeHandler;
}