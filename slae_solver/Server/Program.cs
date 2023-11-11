using Server;

TcpServer? server = null;
try
{
    server = new TcpServer();
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

Console.WriteLine("\nHit enter to continue...");
Console.Read();