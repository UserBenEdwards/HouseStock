using AppController.Commands;
using Domain.Exceptions;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class DeleteCategoryCommand(ICategoryService categoryService, AppSession session) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!session.IsAdmin)
            return CommandResult.Unauthorized();

        if (!int.TryParse(request.Get("id"), out var id))
            return CommandResult.Fail("Invalid ID. Usage: delete category <id>");

        try
        {
            await categoryService.DeleteAsync(id);
            return CommandResult.Ok($"Category #{id} deleted successfully.");
        }
        catch (CategoryNotFoundException ex) { return CommandResult.Fail(ex.Message); }
        catch (CategoryHasAppliancesException ex) { return CommandResult.Fail(ex.Message); }
    }
}
