using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Lab4.Core.Encryption;

public sealed record EncryptionProgressReport(int Percentage, TimeSpan Elapsed);

public sealed record EncryptionResult(
    string OutputFilePath,
    string OutputFileName,
    long OutputFileSizeBytes,
    TimeSpan Elapsed);

public sealed class FileEncryptionService
{
    private const int BufferSize = 81920;

    public EncryptionResult TransformFile(
        string inputFilePath,
        string outputFilePath,
        string key,
        bool encrypt,
        Func<bool>? isCancellationRequested = null,
        Action<EncryptionProgressReport>? onProgressChanged = null)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
            throw new ArgumentException("Input file path is required.", nameof(inputFilePath));
        if (string.IsNullOrWhiteSpace(outputFilePath))
            throw new ArgumentException("Output file path is required.", nameof(outputFilePath));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Encryption key is required.", nameof(key));
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException("Input file was not found.", inputFilePath);

        byte[] keyBytes = DeriveKeyBytes(key);
        var stopwatch = Stopwatch.StartNew();
        long totalBytes = new FileInfo(inputFilePath).Length;
        long processedBytes = 0;
        int lastReportedPercent = -1;

        using var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize);
        using var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize);

        if (encrypt)
        {
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();
            outputStream.Write(aes.IV, 0, aes.IV.Length);

            using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            CopyWithProgress(inputStream, cryptoStream, totalBytes, stopwatch, ref processedBytes, ref lastReportedPercent, isCancellationRequested, onProgressChanged);
            cryptoStream.FlushFinalBlock();
        }
        else
        {
            byte[] iv = new byte[16];
            int ivRead = inputStream.Read(iv, 0, iv.Length);
            if (ivRead != iv.Length)
                throw new InvalidDataException("Invalid encrypted file header.");

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            using var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            CopyWithProgress(cryptoStream, outputStream, totalBytes, stopwatch, ref processedBytes, ref lastReportedPercent, isCancellationRequested, onProgressChanged);
        }

        stopwatch.Stop();
        var outputInfo = new FileInfo(outputStream.Name);

        return new EncryptionResult(
            outputInfo.FullName,
            outputInfo.Name,
            outputInfo.Length,
            stopwatch.Elapsed);
    }

    private static void CopyWithProgress(
        Stream source,
        Stream destination,
        long totalBytes,
        Stopwatch stopwatch,
        ref long processedBytes,
        ref int lastReportedPercent,
        Func<bool>? isCancellationRequested,
        Action<EncryptionProgressReport>? onProgressChanged)
    {
        byte[] buffer = new byte[BufferSize];
        int bytesRead;

        while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            if (isCancellationRequested?.Invoke() == true)
                throw new OperationCanceledException("Operation was cancelled by user.");

            destination.Write(buffer, 0, bytesRead);
            processedBytes += bytesRead;

            int percent = totalBytes > 0
                ? (int)Math.Min(100, (processedBytes * 100) / totalBytes)
                : 100;

            if (percent != lastReportedPercent)
            {
                lastReportedPercent = percent;
                onProgressChanged?.Invoke(new EncryptionProgressReport(percent, stopwatch.Elapsed));
            }
        }
    }

    private static byte[] DeriveKeyBytes(string key) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(key));
}