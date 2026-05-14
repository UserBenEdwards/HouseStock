namespace AppController;

public record ParsedRequest(
    string CommandName,
    IReadOnlyDictionary<string, string> Args
)
{
    public string? Get(string key) =>
        Args.TryGetValue(key, out var value) ? value : null;
}
