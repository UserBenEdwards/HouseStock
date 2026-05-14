using AppController.Commands;
using Domain.Exceptions;
using Service.DTOs;
using Service.Interfaces;

namespace AppController.Commands.Admin;

public class AddCategoryCommand(ICategoryService categoryService, AppSession session) : ICommand
{
    public async Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        if (!session.IsAdmin)
            return CommandResult.Unauthorized();

        var name = request.Get("name");
        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Fail("Category name is required.");

        try
        {
            await categoryService.AddAsync(new AddCategoryRequest(name, request.Get("description")));
            return CommandResult.Ok($"Category '{name}' created successfully.");
        }
        catch (DuplicateApplianceException ex) { return CommandResult.Fail(ex.Message); }
    }
}
