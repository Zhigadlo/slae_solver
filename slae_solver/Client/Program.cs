using Client;

Client.Client client = new Client.Client();
var reader = new SlaeDataReader();
try
{
    while (true)
    {
        
        var slaeData = reader.ReadSlaeData();
        Console.WriteLine();
        var x = client.SendRequestToSolve(slaeData);

        for (int i = 0; i < x.Length; i++)
        {
            Console.WriteLine(x[i]);
        }

        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
}

Console.WriteLine("\nPress Enter to continue...");
Console.Read();