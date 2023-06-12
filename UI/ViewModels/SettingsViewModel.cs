using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Data;
using ReactiveUI;

namespace UI.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private bool _showAllVariablesShouldBeUniqueError;
    public ObservableCollection<UserVariable> UserVariables { get; }
    public ReactiveCommand<Unit, Dictionary<string, string>> UpdateUserDictionaryCommand { get; }
    public ICommand CreateNewVariable { get; }

    public SettingsViewModel() : this(new Dictionary<string, string>())
    {
       
    }

    public bool ShowAllVariablesShouldBeUniqueError
    {
        get => _showAllVariablesShouldBeUniqueError;
        set => this.RaiseAndSetIfChanged(ref _showAllVariablesShouldBeUniqueError, value);
    }

    public SettingsViewModel(Dictionary<string, string> userDictionary)
    {
        UserVariables = new ObservableCollection<UserVariable>
            (userDictionary.Select(x => new UserVariable(x.Key, x.Value, RemoveEventHandler)).ToList());

        CreateNewVariable = ReactiveCommand.Create(() =>
        {
            UserVariables.Add(new UserVariable("","", RemoveEventHandler));
        });
        
        UpdateUserDictionaryCommand = ReactiveCommand.Create(() =>
        {
            // Check that all values are unique before saving
            var variablesWithNames = UserVariables.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
            if (variablesWithNames.Select(x => x.Name).Distinct().Count() != variablesWithNames.Count)
            {
                ShowAllVariablesShouldBeUniqueError = true;
                throw new ArgumentException(nameof(UserVariables));
            }
            return variablesWithNames.ToDictionary(keySelector: m => m.Name, elementSelector: m => m.Value);
        });
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

    public event EventHandler<string> RemoveMeHandler;
}