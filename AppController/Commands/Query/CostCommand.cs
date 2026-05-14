using AppController.Commands;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class CostCommand(IApplianceService applianceService) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!decimal.TryParse(request.Get("min"), out var min) ||
            !decimal.TryParse(request.Get("max"), out var max))
            return CommandResult.Fail("Invalid price range. Usage: cost <min> <max>");

        var appliances = await applianceService.GetByPriceRangeAsync(min, max);
        return CommandResult.Ok($"Appliances from {min} to {max}:", appliances);
    }
}
