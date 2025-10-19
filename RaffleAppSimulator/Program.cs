using System.Diagnostics;

Console.WriteLine("=== Raffle App Simulator ===\n");

// Configuration
Console.Write("Enter the Raffle App URL: ");
string? baseUrl = Console.ReadLine();
if (string.IsNullOrWhiteSpace(baseUrl))
{
    Console.WriteLine("Invalid URL. Exiting...");
    return;
}

Console.Write("Enter the number of requests to send: ");
if (!int.TryParse(Console.ReadLine(), out int numberOfRequests) || numberOfRequests <= 0)
{
    Console.WriteLine("Invalid number. Exiting...");
    return;
}

Console.WriteLine($"\nStarting simulation: {numberOfRequests} requests to {baseUrl}");
Console.WriteLine(new string('-', 60));

// Initialize counters
int winnerCount = 0;
int nonWinnerCount = 0;
var stopwatch = Stopwatch.StartNew();

// Create HttpClient with cookie handling disabled
var handler = new HttpClientHandler
{
    UseCookies = false // Disable cookie container to ensure each request is independent
};

using var httpClient = new HttpClient(handler)
{
    Timeout = TimeSpan.FromSeconds(30)
};

// Send requests
for (int i = 1; i <= numberOfRequests; i++)
{
    try
    {
        // Create a new request without cookies
        var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
        
        // Send the request
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        // Read the response content
        string content = await response.Content.ReadAsStringAsync();
        
        // Check if response contains winner-card
        if (content.Contains("winner-card", StringComparison.OrdinalIgnoreCase))
        {
            winnerCount++;
        }
        else
        {
            nonWinnerCount++;
        }
        
        // Calculate current stats
        int totalProcessed = winnerCount + nonWinnerCount;
        double winnerRate = (double)winnerCount / totalProcessed * 100;
        
        // Print progress every request
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write($"Progress: {totalProcessed}/{numberOfRequests} | " +
                     $"Winners: {winnerCount} | " +
                     $"Non-Winners: {nonWinnerCount} | " +
                     $"Win Rate: {winnerRate:F2}%");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"\nError on request {i}: {ex.Message}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine($"\nRequest {i} timed out.");
    }
}

stopwatch.Stop();

// Final Results
Console.WriteLine("\n" + new string('-', 60));
Console.WriteLine("\n=== Final Results ===");
Console.WriteLine($"Total Requests:    {numberOfRequests}");
Console.WriteLine($"Winners:           {winnerCount}");
Console.WriteLine($"Non-Winners:       {nonWinnerCount}");
Console.WriteLine($"Winner Rate:       {(double)winnerCount / numberOfRequests * 100:F2}%");
Console.WriteLine($"Elapsed Time:      {stopwatch.Elapsed.TotalSeconds:F2} seconds");
Console.WriteLine($"Requests/Second:   {numberOfRequests / stopwatch.Elapsed.TotalSeconds:F2}");
Console.WriteLine("\n" + new string('=', 60));
