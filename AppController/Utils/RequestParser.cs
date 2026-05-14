namespace AppController.Utils;

public class RequestParser
{
    // Парсит строки вида:
    //   "find all"               → ("find", {})
    //   "find laptops"           → ("find", { type: "laptops" })
    //   "find all price=10;500"  → ("find", { filter: "price", min: "10", max: "500" })
    //   "cost 10 500"            → ("cost", { min: "10", max: "500" })
    //   "show 3"                 → ("show", { id: "3" })
    //   "switch admin"           → ("switch", { mode: "admin" })
    //   "delete 3"               → ("delete", { id: "3" })
    //   "add ..."                → ("add", { name: ..., price: ..., ... })
    public ParsedRequest Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new ParsedRequest("unknown", new Dictionary<string, string>());

        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLower();
        var args = new Dictionary<string, string>();

        switch (command)
        {
            case "find" when parts.Length >= 2:
                var sub = parts[1].ToLower();

                // find all price=min;max
                if (sub == "all" && parts.Length == 3 && parts[2].StartsWith("price="))
                {
                    var range = parts[2]["price=".Length..].Split(';');
                    if (range.Length == 2)
                    {
                        args["filter"] = "price";
                        args["min"] = range[0];
                        args["max"] = range[1];
                    }
                }
                // find all
                else if (sub == "all")
                {
                    // нет доп. аргументов
                }
                // find <categoryName>
                else
                {
                    args["type"] = parts[1];
                }
                break;

            case "cost" when parts.Length == 3:
                args["min"] = parts[1];
                args["max"] = parts[2];
                break;

            case "show" when parts.Length == 2:
                args["id"] = parts[1];
                break;

            case "switch" when parts.Length == 2:
                args["mode"] = parts[1].ToLower();
                break;

            case "update" when parts.Length == 2:
                args["id"] = parts[1];
                break;

            case "delete" when parts.Length == 2:
                args["id"] = parts[1];
                break;

            // add, help, exit — без аргументов из командной строки
            // аргументы для add/update передаются Presentation слоем через args напрямую
        }

        return new ParsedRequest(command, args);
    }

    // Перегрузка для случаев когда Presentation уже собрал все поля (add, update)
    public ParsedRequest ParseWithArgs(string command, Dictionary<string, string> args) =>
        new ParsedRequest(command.ToLower(), args);
}
