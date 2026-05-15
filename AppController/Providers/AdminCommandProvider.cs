using AppController.Commands;
using AppController.Commands.Admin;
using AppController.Commands.Query;
using AppController.Commands.System;

namespace AppController.Providers;

public class AdminCommandProvider(
    FindCommand findCommand,
    ShowCommand showCommand,
    CostCommand costCommand,
    StatsCommand statsCommand,
    AdminHelpCommand adminHelpCommand,
    ListCategoriesCommand listCategoriesCommand,
    AddApplianceCommand addCommand,
    UpdateApplianceCommand updateCommand,
    DeleteApplianceCommand deleteCommand,
    AddCategoryCommand addCategoryCommand,
    UpdateCategoryCommand updateCategoryCommand,
    DeleteCategoryCommand deleteCategoryCommand,
    SwitchToUserCommand switchToUserCommand,
    ExitCommand exitCommand,
    WrongCommand wrongCommand)
{
    public ICommand Resolve(ParsedRequest request) => request.CommandName switch
    {
        "find"            => findCommand,
        "show"            => showCommand,
        "cost"            => costCommand,
        "stats"           => statsCommand,
        "help"            => adminHelpCommand,
        "categories"      => listCategoriesCommand,
        "add"             => addCommand,
        "update"          => updateCommand,
        "delete"          => deleteCommand,
        "add-category"    => addCategoryCommand,
        "update-category" => updateCategoryCommand,
        "delete-category" => deleteCategoryCommand,
        "switch"          => switchToUserCommand,
        "exit"            => exitCommand,
        _                 => wrongCommand
    };
}
