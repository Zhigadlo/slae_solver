using (NodeServer.NodeServer nodeServer = new NodeServer.NodeServer())
{
    if (nodeServer.Connect())
    {
        nodeServer.Start();
    }
}

Console.WriteLine("\nPress Enter to close the programm...");
Console.Read();