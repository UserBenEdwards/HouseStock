using AppController.Commands;
using Domain.Exceptions;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class DeleteApplianceCommand(IApplianceService applianceService, AppSession session) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!session.IsAdmin)
            return CommandResult.Unauthorized();

        if (!int.TryParse(request.Get("id"), out var id))
            return CommandResult.Fail("Invalid ID. Usage: delete <id>");

        try
        {
            await applianceService.DeleteAsync(id);
            return CommandResult.Ok($"Appliance #{id} deleted successfully.");
        }
        catch (ApplianceNotFoundException ex)
        {
            return CommandResult.Fail(ex.Message);
        }
    }
}
