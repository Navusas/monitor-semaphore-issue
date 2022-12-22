using System;
using System.Threading;
using Renci.SshNet;

namespace MonitorSemaphoreIssue
{
    public interface ISshConnection : IDisposable
    {
        string RunCommand(string commandText);

        bool IsConnected { get; }

        void Connect();
    }
public sealed class SshConnection : ISshConnection
{
    private readonly SshClient m_SshClient;
    private readonly string m_Hostname;
    private readonly string _username;
    private readonly string _password;
    private readonly TimeSpan m_CommandTimeOut;
    private readonly Guid m_Id;

    public SshConnection(string hostname, string username, string password, TimeSpan commandTimeOut)
    {
        m_Hostname = hostname;
        _username = username;
        _password = password;
        m_CommandTimeOut = commandTimeOut;
        m_SshClient = new SshClient(hostname, _username, _password);
        m_Id = Guid.NewGuid();

        // INFO: these are the relevant defaults
        // m_SshClient.KeepAliveInterval = Renci.SshNet.Session.InfiniteTimeSpan;
        // this.Timeout = ConnectionInfo.DefaultTimeout; // TimeSpan.FromSeconds(30.0)
        // this.ChannelCloseTimeout = ConnectionInfo.DefaultChannelCloseTimeout; // TimeSpan.FromSeconds(1.0);
        // this.RetryAttempts = 10;
        // this.MaxSessions = 10;
    }

    public string RunCommand(string commandText)
    {
        using var sshCommand = m_SshClient.CreateCommand(commandText);
        sshCommand.CommandTimeout = m_CommandTimeOut;
        sshCommand.Execute();

        if (string.IsNullOrWhiteSpace(sshCommand.Error))
        {
            if (sshCommand.Error.Contains("No such file or directory"))
            {
                throw new ArgumentException(
                    $"No such file or directory was found when executing command: {commandText}");
            }

            return sshCommand.Result ?? string.Empty;
        }

        if (sshCommand.ExitStatus == 127)
        {
            throw new ArgumentException(
                $"Command {commandText} is not available on {m_Hostname}, {sshCommand.Error}");
        }

        throw new ArgumentException(
            $"Error executing command {commandText} on {m_Hostname}, {sshCommand.Error}");
    }

    public bool IsConnected => m_SshClient.IsConnected;

    public void Connect()
    {
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Opening connection {m_Id} to {m_Hostname}");
        m_SshClient.Connect();
    }

    public void Dispose()
    {
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Disposing connection {m_Id} to {m_Hostname}");
        m_SshClient.Disconnect();
        m_SshClient.Dispose();
    }

    internal void Disconnect()
    {
        m_SshClient.Disconnect();
    }
}
}