using System.ComponentModel;
using System.Diagnostics;

namespace Lab4.Core.Processes;

public sealed record LaunchableApp(string DisplayName, string ExecutableOrCommand, string? Arguments = null);

public sealed class ProcessManagerService
{
    public IReadOnlyList<LaunchableApp> GetPresetApps() =>
    [
        new("Калькулятор", "calc.exe"),
        new("Microsoft Word", "winword.exe"),
        new("Блокнот", "notepad.exe"),
        new("Провідник", "explorer.exe"),
        new("Командний рядок", "cmd.exe")
    ];

    public Process StartApplication(string fileName, string? arguments = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Executable name is required.", nameof(fileName));

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments ?? string.Empty,
            UseShellExecute = true
        };

        return Process.Start(psi) ?? throw new InvalidOperationException($"Failed to start '{fileName}'.");
    }

    public List<ProcessInfoDto> GetRunningProcesses()
    {
        var result = new List<ProcessInfoDto>();

        foreach (var proc in Process.GetProcesses())
        {
            using (proc)
            {
                string startTimeText = "N/A";
                try
                {
                    startTimeText = proc.StartTime.ToString("dd.MM.yyyy HH:mm:ss");
                }
                catch (Win32Exception) { }
                catch (InvalidOperationException) { }

                string priorityText = "N/A";
                try
                {
                    priorityText = proc.PriorityClass.ToString();
                }
                catch (Win32Exception) { }
                catch (InvalidOperationException) { }

                int threadCount = 0;
                try
                {
                    threadCount = proc.Threads.Count;
                }
                catch (Win32Exception) { }
                catch (InvalidOperationException) { }

                double memoryMb = 0;
                try
                {
                    memoryMb = Math.Round(proc.WorkingSet64 / (1024.0 * 1024.0), 2);
                }
                catch (Win32Exception) { }
                catch (InvalidOperationException) { }

                result.Add(new ProcessInfoDto
                {
                    Id = proc.Id,
                    Name = proc.ProcessName,
                    MemoryMb = memoryMb,
                    StartTimeFormatted = startTimeText,
                    Priority = priorityText,
                    ThreadCount = threadCount
                });
            }
        }

        return result.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public void KillProcess(int processId)
    {
        using var proc = Process.GetProcessById(processId);
        proc.Kill(entireProcessTree: true);
        proc.WaitForExit(3000);
    }

    public void SetProcessPriority(int processId, ProcessPriorityClass priority)
    {
        using var proc = Process.GetProcessById(processId);
        proc.PriorityClass = priority;
    }
}