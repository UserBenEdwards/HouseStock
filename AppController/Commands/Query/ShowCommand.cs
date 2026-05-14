using AppController.Commands;
using Domain.Exceptions;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class ShowCommand(IApplianceService applianceService) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!int.TryParse(request.Get("id"), out var id))
            return CommandResult.Fail("Invalid ID. Usage: show <id>");

        try
        {
            var appliance = await applianceService.GetByIdAsync(id);
            return CommandResult.Ok($"Appliance #{id}:", appliance);
        }
        catch (ApplianceNotFoundException ex)
        {
            return CommandResult.Fail(ex.Message);
        }
    }
}
