namespace BlazorApp.HttpServices;

internal static class ApiResponse
{
    public static async Task CheckAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message) ? response.ReasonPhrase : message,
                null, response.StatusCode);
        }
    }

    public static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        await CheckAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new HttpRequestException("The API returned an empty response.");
    }
}
