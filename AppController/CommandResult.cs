namespace AppController;

public class CommandResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Data { get; init; }
    public bool ShouldExit { get; init; }

    public static CommandResult Ok(string message, object? data = null) =>
        new() { Success = true, Message = message, Data = data };

    public static CommandResult Fail(string message) =>
        new() { Success = false, Message = message };

    public static CommandResult Unauthorized() =>
        Fail("Access denied. Switch to admin mode first.");

    public static CommandResult Exit() =>
        new() { Success = true, Message = "Goodbye!", ShouldExit = true };
}
