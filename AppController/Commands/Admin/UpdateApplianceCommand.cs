using AppController.Commands;
using Domain.Exceptions;
using Service.DTOs;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class UpdateApplianceCommand(IApplianceService applianceService, AppSession session) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!session.IsAdmin)
            return CommandResult.Unauthorized();

        if (!int.TryParse(request.Get("id"), out var id))
            return CommandResult.Fail("Invalid ID. Usage: update <id>");

        var name = request.Get("name");
        var priceStr = request.Get("price");

        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Fail("Name is required.");

        if (!decimal.TryParse(priceStr, out var price) || price < 0)
            return CommandResult.Fail("Invalid price.");

        try
        {
            await applianceService.UpdateAsync(new UpdateApplianceRequest(
                Id: id,
                Name: name,
                Description: request.Get("description"),
                Price: price,
                CategoryName: request.Get("category")
            ));

            return CommandResult.Ok($"Appliance #{id} updated successfully.");
        }
        catch (ApplianceNotFoundException ex) { return CommandResult.Fail(ex.Message); }
        catch (DuplicateApplianceException ex) { return CommandResult.Fail(ex.Message); }
        catch (CategoryNotFoundException ex) { return CommandResult.Fail(ex.Message); }
    }
}
