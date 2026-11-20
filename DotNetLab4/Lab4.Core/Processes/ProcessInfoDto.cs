using System.Diagnostics;

namespace Lab4.Core.Processes;

public sealed class ProcessInfoDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public double MemoryMb { get; init; }
    public string StartTimeFormatted { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public int ThreadCount { get; init; }
}