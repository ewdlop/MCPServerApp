using MCPServerApp.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MCPServerApp.Services;

public class MonkeyService
{
    private readonly HttpClient httpClient;
    public MonkeyService(IHttpClientFactory httpClientFactory)
    {
        httpClient = httpClientFactory.CreateClient();
    }

    List<Monkey> monkeyList = new();
    public async Task<List<Monkey>> GetMonkeys()
    {
        if (monkeyList?.Count > 0)
            return monkeyList;

        var response = await httpClient.GetAsync("https://www.montemagno.com/monkeys.json");
        if (response.IsSuccessStatusCode)
        {
            monkeyList = await response.Content.ReadFromJsonAsync(MonkeyContext.Default.ListMonkey) ?? [];
        }

        monkeyList ??= [];

        return monkeyList;
    }

    public async Task<Monkey?> GetMonkey(string name)
    {
        var monkeys = await GetMonkeys();
        return monkeys.FirstOrDefault(m => m.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
    }
}
