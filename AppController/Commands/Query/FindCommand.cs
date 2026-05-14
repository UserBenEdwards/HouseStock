using AppController.Commands;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class FindCommand(IApplianceService applianceService) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        // find all price=min;max
        if (request.Get("filter") == "price")
        {
            if (!decimal.TryParse(request.Get("min"), out var min) ||
                !decimal.TryParse(request.Get("max"), out var max))
                return CommandResult.Fail("Invalid price range. Usage: find all price=10;500");

            var byPrice = await applianceService.GetByPriceRangeAsync(min, max);
            return CommandResult.Ok($"Appliances from {min} to {max}:", byPrice);
        }

        // find <categoryName>
        var type = request.Get("type");
        if (type is not null)
        {
            var byCategory = await applianceService.GetByCategoryNameAsync(type);
            return CommandResult.Ok($"Appliances in category '{type}':", byCategory);
        }

        // find all
        var all = await applianceService.GetAllAsync();
        return CommandResult.Ok("All appliances:", all);
    }
}
