using AppController.Commands;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class ListCategoriesCommand(ICategoryService categoryService) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        var categories = await categoryService.GetAllAsync();
        return CommandResult.Ok("Categories:", categories);
    }
}
