using System.ComponentModel;
using Lab4.Core.Encryption;

namespace FileEncryptorApp;

public partial class MainForm : Form
{
    private readonly FileEncryptionService _encryptionService = new();
    private readonly BackgroundWorker _worker = new();

    private readonly TextBox _txtFilePath = new() { ReadOnly = true, Width = 380 };
    private readonly Button _btnBrowse = new() { Text = "Обрати файл..." };
    private readonly TextBox _txtKey = new() { UseSystemPasswordChar = true, Width = 240 };
    private readonly CheckBox _chkShowKey = new() { Text = "Показати ключ", AutoSize = true };
    private readonly Button _btnEncrypt = new() { Text = "Зашифрувати", Width = 130, Height = 34 };
    private readonly Button _btnDecrypt = new() { Text = "Розшифрувати", Width = 130, Height = 34 };
    private readonly Button _btnCancel = new() { Text = "Скасувати", Width = 110, Height = 34, Enabled = false };
    private readonly ProgressBar _progressBar = new() { Minimum = 0, Maximum = 100, Width = 470, Height = 24 };
    private readonly Label _lblStatus = new() { Text = "Прогрес: 0% | Час: 00:00.0", AutoSize = true };

    private sealed record WorkerRequest(
        string InputPath,
        string OutputPath,
        string Key,
        bool Encrypt);

    public MainForm()
    {
        Text = "Шифрування файлів (BackgroundWorker)";
        Size = new Size(520, 260);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        InitializeLayout();
        ConfigureBackgroundWorker();
    }

    private void InitializeLayout()
    {
        var lblFile = new Label { Text = "Файл:", Location = new Point(20, 22), AutoSize = true };
        _txtFilePath.Location = new Point(75, 18);
        _txtFilePath.Width = 305;

        _btnBrowse.Location = new Point(390, 17);
        _btnBrowse.Size = new Size(105, 25);
        _btnBrowse.Click += OnBrowseClick;

        var lblKey = new Label { Text = "Ключ:", Location = new Point(20, 58), AutoSize = true };
        _txtKey.Location = new Point(75, 54);
        _txtKey.Width = 235;

        _chkShowKey.Location = new Point(320, 57);
        _chkShowKey.CheckedChanged += (_, _) => _txtKey.UseSystemPasswordChar = !_chkShowKey.Checked;

        _btnEncrypt.Location = new Point(75, 96);
        _btnEncrypt.Click += (_, _) => StartOperation(true);

        _btnDecrypt.Location = new Point(215, 96);
        _btnDecrypt.Click += (_, _) => StartOperation(false);

        _btnCancel.Location = new Point(355, 96);
        _btnCancel.Click += (_, _) =>
        {
            if (_worker.IsBusy && !_worker.CancellationPending)
                _worker.CancelAsync();
        };

        _progressBar.Location = new Point(20, 145);
        _progressBar.Width = 475;

        _lblStatus.Location = new Point(20, 178);

        Controls.AddRange(
        [
            lblFile, _txtFilePath, _btnBrowse,
        lblKey, _txtKey, _chkShowKey,
        _btnEncrypt, _btnDecrypt, _btnCancel,
        _progressBar, _lblStatus
        ]);
    }

    private void ConfigureBackgroundWorker()
    {
        _worker.WorkerReportsProgress = true;
        _worker.WorkerSupportsCancellation = true;
        _worker.DoWork += OnDoWork;
        _worker.ProgressChanged += OnProgressChanged;
        _worker.RunWorkerCompleted += OnRunWorkerCompleted;
    }

    private void OnBrowseClick(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Оберіть файл для шифрування або розшифрування",
            Filter = "Усі файли (*.*)|*.*"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _txtFilePath.Text = dialog.FileName;
        }
    }

    private void StartOperation(bool encrypt)
    {
        if (_worker.IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(_txtFilePath.Text) || !File.Exists(_txtFilePath.Text))
        {
            MessageBox.Show(this, "Оберіть коректний файл.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrEmpty(_txtKey.Text))
        {
            MessageBox.Show(this, "Введіть ключ шифрування.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string defaultName = encrypt
            ? Path.GetFileName(_txtFilePath.Text) + ".enc"
            : Path.GetFileNameWithoutExtension(_txtFilePath.Text);

        using var saveDialog = new SaveFileDialog
        {
            Title = encrypt ? "Зберегти зашифрований файл" : "Зберегти розшифрований файл",
            FileName = defaultName,
            Filter = "Усі файли (*.*)|*.*"
        };

        if (saveDialog.ShowDialog(this) != DialogResult.OK)
            return;

        SetUiBusy(true);
        _progressBar.Value = 0;
        _lblStatus.Text = "Прогрес: 0% | Час: 00:00.0";

        _worker.RunWorkerAsync(new WorkerRequest(_txtFilePath.Text, saveDialog.FileName, _txtKey.Text, encrypt));
    }

    private void OnDoWork(object? sender, DoWorkEventArgs e)
    {
        if (e.Argument is not WorkerRequest request)
            return;

        e.Result = _encryptionService.TransformFile(
            request.InputPath,
            request.OutputPath,
            request.Key,
            request.Encrypt,
            isCancellationRequested: () => _worker.CancellationPending,
            onProgressChanged: report => _worker.ReportProgress(report.Percentage, report.Elapsed));
    }

    private void OnProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        int percent = Math.Clamp(e.ProgressPercentage, 0, 100);
        _progressBar.Value = percent;

        var elapsed = e.UserState is TimeSpan ts ? ts : TimeSpan.Zero;
        _lblStatus.Text = $"Прогрес: {percent}% | Час: {elapsed:mm\\:ss\\.f}";
    }

    private void OnRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        SetUiBusy(false);

        if (e.Cancelled || e.Error is OperationCanceledException)
        {
            _progressBar.Value = 0;
            _lblStatus.Text = "Операцію скасовано.";
            MessageBox.Show(this, "Операцію скасовано користувачем.", "Скасовано", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (e.Error is not null)
        {
            _lblStatus.Text = "Помилка виконання.";
            MessageBox.Show(this, $"Помилка: {e.Error.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (e.Result is EncryptionResult result)
        {
            _progressBar.Value = 100;
            _lblStatus.Text = $"Завершено: 100% | Час: {result.Elapsed:mm\\:ss\\.f}";

            string info =
                $"Назва файлу: {result.OutputFileName}\n" +
                $"Розмір файлу: {result.OutputFileSizeBytes:N0} байт ({result.OutputFileSizeBytes / (1024.0 * 1024.0):F2} МБ)\n" +
                $"Затрачений час: {result.Elapsed:mm\\:ss\\.fff}";

            MessageBox.Show(this, info, "Операцію завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void SetUiBusy(bool busy)
    {
        _btnBrowse.Enabled = !busy;
        _txtKey.Enabled = !busy;
        _btnEncrypt.Enabled = !busy;
        _btnDecrypt.Enabled = !busy;
        _btnCancel.Enabled = busy;
    }
}