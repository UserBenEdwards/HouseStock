using System.Globalization;
using AppController.Commands;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class AddApplianceCommand(
    IApplianceService applianceService,
    AppSession session,
    ILogger<AddApplianceCommand> logger) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!session.IsAdmin)
            return CommandResult.Unauthorized();

        var name = request.Get("name");
        var priceStr = request.Get("price");

        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Fail("Name is required.");

        if (!decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price < 0)
            return CommandResult.Fail("Invalid price.");

        try
        {
            await applianceService.AddAsync(new AddApplianceRequest(
                Name: name,
                Description: request.Get("description"),
                Price: price,
                CategoryName: request.Get("category")
            ));

            logger.LogInformation("Appliance '{Name}' added by admin", name);
            return CommandResult.Ok($"Appliance '{name}' added successfully.");
        }
        catch (ValidationException ex)        { return CommandResult.Fail(ex.Message); }
        catch (DuplicateApplianceException ex) { return CommandResult.Fail(ex.Message); }
        catch (CategoryNotFoundException ex)   { return CommandResult.Fail(ex.Message); }
        catch (PersistenceException ex)        { return CommandResult.Fail(ex.Message); }
    }
}
