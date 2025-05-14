using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Laba._2_Animal.Models;

namespace Laba4.Reflection.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _assemblyPath;
    
    private object? _currentInstance;
    private Type? _currentType;

    [ObservableProperty]
    private string _statusMessage = "Выберите сборку";

    public ObservableCollection<string> AvailableClasses { get; } = new();
    public ObservableCollection<MethodInfo> AvailableMethods { get; } = new();
    public ObservableCollection<MethodParameter> MethodParameters { get; } = new();
    public ObservableCollection<string> LogMessages { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableMethods))]
    private string? _selectedClass;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MethodParameters))]
    private MethodInfo? _selectedMethod;

    [RelayCommand]
    private void LoadAssembly()
    {
        if (string.IsNullOrWhiteSpace(AssemblyPath) || !File.Exists(AssemblyPath))
        {
            StatusMessage = "Неверный путь к сборке";
            return;
        }

        try
        {
            var assembly = Assembly.LoadFrom(AssemblyPath);
            var creatureTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Creature).IsAssignableFrom(t))
                .ToList();

            AvailableClasses.Clear();
            foreach (var type in creatureTypes)
            {
                AvailableClasses.Add(type.Name);
            }

            StatusMessage = $"Загружено классов: {creatureTypes.Count}";
            AddLogMessage($"Загружена сборка: {Path.GetFileName(AssemblyPath)}");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
            AddLogMessage($"Ошибка: {ex.Message}");
        }
    }

    partial void OnSelectedClassChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        try
        {
            // Сбрасываем текущий экземпляр при смене класса
            _currentInstance = null;
            _currentType = null;
            MethodParameters.Clear();

            var assembly = Assembly.LoadFrom(AssemblyPath!);
            _currentType = assembly.GetTypes().FirstOrDefault(t => t.Name == value);
            
            if (_currentType == null) return;

            AvailableMethods.Clear();
            foreach (var method in _currentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (method.DeclaringType != typeof(object) && !method.IsSpecialName)
                {
                    AvailableMethods.Add(method);
                }
            }

            StatusMessage = $"Выбран класс: {value}";
            AddLogMessage($"Выбран класс: {value}");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            AddLogMessage($"Ошибка: {ex.Message}");
        }
    }

    partial void OnSelectedMethodChanged(MethodInfo? value)
    {
        MethodParameters.Clear();
        
        if (value != null)
        {
            foreach (var param in value.GetParameters())
            {
                MethodParameters.Add(new MethodParameter
                {
                    Name = param.Name!,
                    Type = param.ParameterType,
                    Value = param.HasDefaultValue ? param.DefaultValue : null
                });
            }
        }
    }

    [RelayCommand]
    private void ExecuteMethod()
    {
        if (SelectedMethod == null || _currentType == null) return;

        try
        {
            // Проверяем, соответствует ли текущий экземпляр выбранному классу
            if (_currentInstance == null || _currentInstance.GetType() != _currentType)
            {
                _currentInstance = Activator.CreateInstance(_currentType, 10.0, 1.0);
                AddLogMessage($"Создан новый экземпляр класса: {_currentType.Name}");
            }

            var paramValues = MethodParameters.Select(p => p.Value).ToArray();
            var result = SelectedMethod.Invoke(_currentInstance, paramValues);
            
            StatusMessage = $"Метод выполнен: {SelectedMethod.Name}";
            AddLogMessage($"Выполнен метод: {SelectedMethod.Name}");
            
            // После выполнения метода обновляем информацию
            if (_currentInstance is Creature creature)
            {
                AddLogMessage(creature.GetInfo());
            }
            
            if (result != null)
            {
                AddLogMessage($"Результат: {result}");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка выполнения: {ex.Message}";
            AddLogMessage($"Ошибка: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ResetInstance()
    {
        _currentInstance = null;
        StatusMessage = "Экземпляр сброшен, будет создан новый при следующем вызове";
        AddLogMessage("Экземпляр сброшен");
        
        if (_currentType != null && SelectedMethod != null)
        {
            // Обновляем параметры метода после сброса
            OnSelectedMethodChanged(SelectedMethod);
        }
    }

    private void AddLogMessage(string message)
    {
        LogMessages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}

public class MethodParameter
{
    public string Name { get; set; } = string.Empty;
    public Type Type { get; set; } = typeof(object);
    public object? Value { get; set; }
}