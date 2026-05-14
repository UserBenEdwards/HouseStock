namespace AppController;

public class AppSession
{
    public bool IsAdmin { get; private set; }

    public void SwitchToAdmin() => IsAdmin = true;
    public void SwitchToUser() => IsAdmin = false;
}
