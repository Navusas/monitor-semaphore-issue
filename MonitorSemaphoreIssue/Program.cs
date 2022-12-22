// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitorSemaphoreIssue;
using Renci.SshNet.Abstractions;

var sshClient = new SshConnection("", "", "", TimeSpan.FromSeconds(60));
sshClient.Connect();

while (true)
{
    List<Task> toWait = new();
    for (var i = 0; i < 50; i++)
    {
        var threadName = $"Thread {i}";
        toWait.Add(Task.Factory.StartNew(() => doStuff(threadName)));
    }

    Task.WaitAll(toWait.ToArray());
}
void doStuff(string strName)
{
    if (!sshClient.IsConnected)
    {
        DiagnosticAbstraction.Log("Connecting");
        sshClient.Connect();
        DiagnosticAbstraction.Log("Connected");
    }
    DiagnosticAbstraction.Log("Running command 'sleep 30;'");
    sshClient.RunCommand("sleep 30;");
    DiagnosticAbstraction.Log("Finished");
}