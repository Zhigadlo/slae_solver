Server.Server? server = new Server.Server();

try
{
    server.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    server?.Dispose();
}

Console.WriteLine("\nPress Enter to continue...");
Console.Read();