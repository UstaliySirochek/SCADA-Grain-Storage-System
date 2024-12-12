using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Program.cs;

class IndicatorEmulator
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string BaseUrl = "http://localhost:5079/api/Indicator";
    private static readonly Dictionary<Guid, double> IndicatorValues = new Dictionary<Guid, double>();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Indicator emulation has started...");

        while (true)
        {
            var indicators = await GetIndicators();

            foreach (var indicator in indicators)
            {
                Console.WriteLine($"Indicator ID: {indicator.Id}, Name: {indicator.Name}, Value: {indicator.Value}");
                if (string.IsNullOrWhiteSpace(indicator.Value) || !double.TryParse(indicator.Value, out var parsedValue))
                {
                    parsedValue = 0.0;
                }

                var currentValue = GetCurrentValue(indicator.Id, parsedValue);
                await UpdateIndicatorValue(indicator.Id.ToString(), currentValue.ToString("F2"));
                Console.WriteLine($"Indicator {indicator.Name} updated. New Value: {currentValue}");
            }

            await Task.Delay(1000);
        }
    }

    private static async Task<IndicatorResponse[]> GetIndicators()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Raw JSON Response: {json}");
        return JsonSerializer.Deserialize<IndicatorResponse[]>(json);
    }

    private static async Task UpdateIndicatorValue(string indicatorId, string newValue)
    {
        var model = new
        {
            Id = indicatorId,
            Value = newValue
        };

        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BaseUrl}", content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Indicator {indicatorId} updated successfully with value {newValue}");
        }
        else
        {
            Console.WriteLine($"Failed to update indicator {indicatorId}. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }
    }

    private static double GetCurrentValue(Guid indicatorId, double currentValue)
    {
        if (!IndicatorValues.ContainsKey(indicatorId))
        {
            IndicatorValues[indicatorId] = currentValue;
        }

        var random = new Random();
        var change = random.NextDouble() * 6 - 1;
        var newValue = IndicatorValues[indicatorId] + change;

        newValue = Math.Max(-100, Math.Min(100, newValue));

        IndicatorValues[indicatorId] = newValue;

        return newValue;
    }
}