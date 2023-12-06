using (NodeServer.NodeServer nodeServer = new NodeServer.NodeServer())
{
    try
    {
        bool isSlaeSolved = false;
        while (!isSlaeSolved)
        {
            var data = nodeServer.GetDataFromServer();
            if (data.IsSlaeSolved)
            {
                isSlaeSolved = data.IsSlaeSolved;
                continue;
            }

            var sum = nodeServer.JacobiHandle(data);
            nodeServer.SendMessage(sum.ToString());
            Console.WriteLine($"Sent data to server: {sum}");
        }

        Console.WriteLine("SLAE solved:)");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
}

Console.WriteLine("\nPress Enter to close the programm...");
Console.Read();