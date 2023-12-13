using (Client.Client client = new Client.Client())
{
    if (client.Connect())
    {
        client.StartSolving();
    }
    Console.WriteLine("Press any button to close the programm...");
    Console.ReadKey();
}
