Server.Server? server = new Server.Server();

try
{
    server.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.ResetColor();
}
finally
{
    server?.Dispose();
}

Console.WriteLine("\nPress Enter to continue...");
Console.Read();