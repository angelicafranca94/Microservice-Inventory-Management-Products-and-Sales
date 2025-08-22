using System.Net.Http.Json;

namespace Gateway.IntegrationTests;

public static class AuthHelper
{
    public static async Task<string> GetJwtToken(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return content["token"];
    }
}