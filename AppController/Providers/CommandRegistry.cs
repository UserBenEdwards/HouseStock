using AppController.Commands;

namespace AppController.Providers;

public class CommandRegistry(
    AppSession session,
    QueryCommandProvider queryProvider,
    AdminCommandProvider adminProvider)
{
    public ICommand Resolve(ParsedRequest request) =>
        session.IsAdmin
            ? adminProvider.Resolve(request)
            : queryProvider.Resolve(request);
}
