using AppController.Commands;
using AppController.Commands.Admin;
using AppController.Commands.Query;
using AppController.Commands.System;

namespace AppController.Providers;

public class AdminCommandProvider(
    FindCommand findCommand,
    ShowCommand showCommand,
    CostCommand costCommand,
    AdminHelpCommand adminHelpCommand,
    AddApplianceCommand addCommand,
    UpdateApplianceCommand updateCommand,
    DeleteApplianceCommand deleteCommand,
    SwitchToUserCommand switchToUserCommand,
    ExitCommand exitCommand,
    WrongCommand wrongCommand)
{
    public ICommand Resolve(ParsedRequest request) => request.CommandName switch
    {
        "find"   => findCommand,
        "show"   => showCommand,
        "cost"   => costCommand,
        "help"   => adminHelpCommand,
        "add"    => addCommand,
        "update" => updateCommand,
        "delete" => deleteCommand,
        "switch" => switchToUserCommand,
        "exit"   => exitCommand,
        _        => wrongCommand
    };
}
