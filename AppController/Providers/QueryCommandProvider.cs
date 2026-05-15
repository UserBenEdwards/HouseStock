using AppController.Commands;
using AppController.Commands.Query;
using AppController.Commands.System;

namespace AppController.Providers;

public class QueryCommandProvider(
    FindCommand findCommand,
    ShowCommand showCommand,
    CostCommand costCommand,
    StatsCommand statsCommand,
    HelpCommand helpCommand,
    ListCategoriesCommand listCategoriesCommand,
    SwitchToAdminCommand switchToAdminCommand,
    ExitCommand exitCommand,
    WrongCommand wrongCommand)
{
    public ICommand Resolve(ParsedRequest request) => request.CommandName switch
    {
        "find"       => findCommand,
        "show"       => showCommand,
        "cost"       => costCommand,
        "stats"      => statsCommand,
        "help"       => helpCommand,
        "categories" => listCategoriesCommand,
        "switch"     => switchToAdminCommand,
        "exit"       => exitCommand,
        _            => wrongCommand
    };
}
