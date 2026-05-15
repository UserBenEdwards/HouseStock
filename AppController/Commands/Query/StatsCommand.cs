using AppController.Commands;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class StatsCommand(IApplianceService applianceService) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        var stats = await applianceService.GetStatsAsync();
        return CommandResult.Ok("Warehouse statistics:", stats);
    }
}
