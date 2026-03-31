using System;
using System.Text.Json;

namespace PortfolioApi.Domain.Entities;

public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DataType { get; set; } = "string"; // string/int/bool/json
    public string Category { get; set; } = "general"; // ui/security/notifications
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    
    // Helper methods for type conversion
    public T? GetValue<T>()
    {
        if (string.IsNullOrEmpty(Value)) return default;
        
        try
        {
            return DataType.ToLower() switch
            {
                "bool" => (T)(object)bool.Parse(Value),
                "int" => (T)(object)int.Parse(Value),
                "json" => JsonSerializer.Deserialize<T>(Value),
                _ => (T)(object)Value
            };
        }
        catch
        {
            return default;
        }
    }
    
    public void SetValue<T>(T value)
    {
        Value = value switch
        {
            bool b => b.ToString().ToLower(),
            int i => i.ToString(),
            _ when typeof(T) == typeof(object) || (typeof(T).IsClass && typeof(T) != typeof(string)) 
                => JsonSerializer.Serialize(value),
            _ => value?.ToString() ?? string.Empty
        };
        
        DataType = value switch
        {
            bool => "bool",
            int => "int",
            _ when typeof(T) == typeof(object) || (typeof(T).IsClass && typeof(T) != typeof(string)) 
                => "json",
            _ => "string"
        };
        
        UpdatedAt = DateTime.UtcNow;
    }
}
