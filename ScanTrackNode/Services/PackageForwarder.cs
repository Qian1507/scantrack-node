using System.Text;
using System.Text.Json;
using ScanTrackNode.Models;

namespace ScanTrackNode.Services;

public class PackageForwarder
{
    private readonly NodeRegistry _registry;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<PackageForwarder> _logger;

    public PackageForwarder(NodeRegistry registry, IHttpClientFactory factory, ILogger<PackageForwarder> logger)
    {
        _registry = registry;
        _factory = factory;
        _logger = logger;
    }

    // DIN UPPGIFT: Vidarebefordra paketet till nästa nod i nätverket.
    //
    // Steg för steg:
    //   1. Hämta nodlistan: await _registry.GetNodesAsync()
    //      → returnerar Dictionary<string, string>  (stad → url)
    //   2. Slå upp URL:en för 'nextCity'
    //      → om staden inte finns: logga fel och returnera false
    //   3. Serialisera 'package' till JSON: JsonSerializer.Serialize(package)
    //   4. Skapa HTTP-body: new StringContent(json, Encoding.UTF8, "application/json")
    //   5. Skapa en HttpClient: _factory.CreateClient()
    //   6. Skicka: await http.PostAsync($"{url}/paket", content)
    //   7. Logga att du skickade (stad + packageId): _logger.LogInformation(...)
    //   8. Returnera response.IsSuccessStatusCode
    public async Task<bool> ForwardAsync(Package package, string nextCity)
    {
        // Get the list of currently registered nodes.
        // The dictionary maps city name -> node URL.
         var nodes = await _registry.GetNodesAsync();

        // Try to find the URL for the next city. 
        // If the city is not registered, log an error and stop forwarding.
        if (!nodes.TryGetValue(nextCity, out var url))
        {
            _logger.LogError("Noden {City} hittades inte i registret.", nextCity);
            return false;
        }

        // Convert the Package object into JSON.
        var json = JsonSerializer.Serialize(package);

        // Create the HTTP request body using the JSON data. 
        // The content type is application/json and UTF-8 encoding is used.
        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        // Create an HttpClient using IHttpClientFactory.
        var http = _factory.CreateClient();

        // Send the package to the /paket endpoint of the next node.
        var response = await http.PostAsync($"{url}/paket", content);

        // Log which package was forwarded and to which city.
        _logger.LogInformation(
            "Paket {PackageId} skickades vidare till {City}.",
            package.PackageId,
            nextCity);

        // Return true if the HTTP request was successful. 
        // For example, status codes such as 200-299 return true.
        return response.IsSuccessStatusCode;
    }
}
