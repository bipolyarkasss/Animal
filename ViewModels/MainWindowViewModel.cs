using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Laba._2_Animal.Models;

namespace Laba._2_Animal.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Panther _panther;
    private readonly Dog _dog;
    private readonly Turtle _turtle;

    [ObservableProperty]
    private string _statusMessage = "Приложение запущено";

    public ObservableCollection<string> LogMessages { get; } = new();

    public MainWindowViewModel()
    {
        _panther = new Panther(maxSpeed: 30.0, speedStep: 5.0);
        _dog = new Dog(maxSpeed: 15.0, speedStep: 2.5);
        _turtle = new Turtle(maxSpeed: 1.0, speedStep: 0.1);

        _panther.VoiceGiven += (s, e) => AddLogMessage("Пантера: *рычит*");
        _dog.VoiceGiven += (s, e) => AddLogMessage("Собака: *лает*");
    }

    private void AddLogMessage(string message)
    {
        LogMessages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
        StatusMessage = message;
    }

    [RelayCommand]
    private void PantherMove()
    {
        if (_panther.IsOnTree)
        {
            AddLogMessage("Пантера на дереве и не может двигаться");
            return;
        }
        
        _panther.Move();
        AddLogMessage(_panther.GetInfo());
    }

    [RelayCommand]
    private void PantherStand()
    {
        if (_panther.IsOnTree)
        {
            AddLogMessage("Пантера на дереве и не может стоять");
            return;
        }
        
        _panther.Stand();
        AddLogMessage(_panther.GetInfo());
    }

    [RelayCommand]
    private void PantherVoice()
    {
        _panther.GiveVoice();
    }

    [RelayCommand]
    private void PantherClimbTree()
    {
        _panther.ClimbTree();
        AddLogMessage("Пантера залезла на дерево");
    }

    [RelayCommand]
    private void PantherGetDown()
    {
        _panther.GetDownFromTree();
        AddLogMessage("Пантера спустилась с дерева");
    }

    [RelayCommand]
    private void DogMove()
    {
        _dog.Move();
        AddLogMessage(_dog.GetInfo());
    }

    [RelayCommand]
    private void DogStand()
    {
        _dog.Stand();
        AddLogMessage(_dog.GetInfo());
    }

    [RelayCommand]
    private void DogVoice()
    {
        _dog.GiveVoice();
    }

    [RelayCommand]
    private void TurtleMove()
    {
        _turtle.Move();
        AddLogMessage(_turtle.GetInfo());
    }

    [RelayCommand]
    private void TurtleStand()
    {
        _turtle.Stand();
        AddLogMessage(_turtle.GetInfo());
    }
}
