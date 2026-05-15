using System.Globalization;
using AppController.Commands;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class UpdateApplianceCommand(
    IApplianceService applianceService,
    AppSession session,
    ILogger<UpdateApplianceCommand> logger) : ICommand
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

        if (!decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price < 0)
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

            logger.LogInformation("Appliance #{Id} updated by admin", id);
            return CommandResult.Ok($"Appliance #{id} updated successfully.");
        }
        catch (ValidationException ex)         { return CommandResult.Fail(ex.Message); }
        catch (ApplianceNotFoundException ex)  { return CommandResult.Fail(ex.Message); }
        catch (DuplicateApplianceException ex) { return CommandResult.Fail(ex.Message); }
        catch (CategoryNotFoundException ex)   { return CommandResult.Fail(ex.Message); }
        catch (PersistenceException ex)        { return CommandResult.Fail(ex.Message); }
    }
}
