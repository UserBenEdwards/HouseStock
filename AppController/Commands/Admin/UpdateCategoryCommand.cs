using AppController.Commands;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class UpdateCategoryCommand(
    ICategoryService categoryService,
    AppSession session,
    ILogger<UpdateCategoryCommand> logger) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!session.IsAdmin)
            return CommandResult.Unauthorized();

        if (!int.TryParse(request.Get("id"), out var id))
            return CommandResult.Fail("Invalid ID. Usage: update category <id>");

        var name = request.Get("name");
        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Fail("Category name is required.");

        try
        {
            await categoryService.UpdateAsync(new UpdateCategoryRequest(id, name, request.Get("description")));
            logger.LogInformation("Category #{Id} updated by admin", id);
            return CommandResult.Ok($"Category #{id} updated successfully.");
        }
        catch (CategoryNotFoundException ex) { return CommandResult.Fail(ex.Message); }
        catch (DuplicateApplianceException ex) { return CommandResult.Fail(ex.Message); }
    }
}
