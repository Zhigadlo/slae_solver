Server.Server? server = null;

try
{
    server = new Server.Server();
    server.Start();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
}
finally
{
    server?.Dispose();
}

Console.WriteLine("\nPress Enter to continue...");
Console.Read();