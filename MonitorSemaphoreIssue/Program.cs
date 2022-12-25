// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitorSemaphoreIssue;

Console.WriteLine("Go?");

var sshClient = new SshConnection("20.77.67.91", "clive", "skdsklskldk9292939032A", TimeSpan.FromSeconds(60));
sshClient.Connect();

for (var i = 0; i < 12; i++) 
{ 
    var threadName = $"Thread {i}";
    Task.Factory.StartNew(() => doStuff(threadName));
}

Console.ReadLine();

void doStuff(string strName)
{
    while (true)
    {
        try
        {
        var random = new Random();
        if (!sshClient.IsConnected)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: connecting");
            sshClient.Connect();
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} connected");
        }

        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} running command");
        
        sshClient.RunCommand($"ps aux;");
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: finishing");
        
        }
        catch (Exception e)
        {
            Console.WriteLine($"**** Exception: {e}");
        }

    }
}
