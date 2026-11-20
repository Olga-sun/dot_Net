using System.ComponentModel;
using System.Diagnostics;
using Lab4.Core.Processes;

namespace ProcessManagerApp;

public partial class MainForm : Form
{
    private readonly ProcessManagerService _processService = new();
    private readonly BindingSource _bindingSource = new();

    private readonly ComboBox _cmbApps = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
    private readonly Button _btnStartApp = new() { Text = "Запустити", Width = 110 };
    private readonly Button _btnRefresh = new() { Text = "Оновити список", Width = 130 };
    private readonly ComboBox _cmbPriority = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
    private readonly Button _btnSetPriority = new() { Text = "Змінити пріоритет", Width = 140 };
    private readonly Button _btnKill = new() { Text = "Зупинити процес", Width = 135 };
    private readonly DataGridView _grid = new()
    {
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = false
    };

    public MainForm()
    {
        Text = "Менеджер процесів";
        Size = new Size(920, 560);
        StartPosition = FormStartPosition.CenterScreen;

        InitializeControls();
        RefreshProcesses();
    }

    private void InitializeControls()
    {
        _cmbApps.DataSource = _processService.GetPresetApps();
        _cmbApps.DisplayMember = nameof(LaunchableApp.DisplayName);
        _cmbApps.Location = new Point(16, 16);

        _btnStartApp.Location = new Point(246, 15);
        _btnStartApp.Click += OnStartAppClick;

        _btnRefresh.Location = new Point(370, 15);
        _btnRefresh.Click += (_, _) => RefreshProcesses();

        _cmbPriority.DataSource = Enum.GetValues<ProcessPriorityClass>();
        _cmbPriority.SelectedItem = ProcessPriorityClass.Normal;
        _cmbPriority.Location = new Point(16, 52);

        _btnSetPriority.Location = new Point(186, 51);
        _btnSetPriority.Click += OnSetPriorityClick;

        _btnKill.Location = new Point(336, 51);
        _btnKill.Click += OnKillProcessClick;

        _grid.Location = new Point(16, 90);
        _grid.Size = new Size(872, 415);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _grid.AutoGenerateColumns = false;
        _grid.DataSource = _bindingSource;

        _grid.Columns.AddRange(
        [
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProcessInfoDto.Id), HeaderText = "PID", FillWeight = 65 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProcessInfoDto.Name), HeaderText = "Назва процесу", FillWeight = 220 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProcessInfoDto.MemoryMb), HeaderText = "Пам'ять (МБ)", FillWeight = 110 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProcessInfoDto.StartTimeFormatted), HeaderText = "Час запуску", FillWeight = 150 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProcessInfoDto.Priority), HeaderText = "Пріоритет", FillWeight = 110 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProcessInfoDto.ThreadCount), HeaderText = "Потоків", FillWeight = 85 }
        ]);

        Controls.AddRange([_cmbApps, _btnStartApp, _btnRefresh, _cmbPriority, _btnSetPriority, _btnKill, _grid]);
    }

    private void RefreshProcesses()
    {
        try
        {
            _bindingSource.DataSource = _processService.GetRunningProcesses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не вдалося отримати список процесів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnStartAppClick(object? sender, EventArgs e)
    {
        if (_cmbApps.SelectedItem is not LaunchableApp app)
            return;

        try
        {
            _processService.StartApplication(app.ExecutableOrCommand, app.Arguments);
            RefreshProcesses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Помилка запуску '{app.DisplayName}': {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private ProcessInfoDto? GetSelectedProcess() =>
        _grid.CurrentRow?.DataBoundItem as ProcessInfoDto;

    private void OnSetPriorityClick(object? sender, EventArgs e)
    {
        var selected = GetSelectedProcess();
        if (selected is null || _cmbPriority.SelectedItem is not ProcessPriorityClass priority)
        {
            MessageBox.Show(this, "Оберіть процес у таблиці.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _processService.SetProcessPriority(selected.Id, priority);
            RefreshProcesses();
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show(this, $"Відмовлено у доступі або операція недоступна: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Помилка зміни пріоритету: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnKillProcessClick(object? sender, EventArgs e)
    {
        var selected = GetSelectedProcess();
        if (selected is null)
        {
            MessageBox.Show(this, "Оберіть процес у таблиці.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Завершити процес '{selected.Name}' (PID: {selected.Id})?",
            "Підтвердження",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            _processService.KillProcess(selected.Id);
            RefreshProcesses();
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show(this, $"Не вдалося зупинити процес: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Помилка зупинки процесу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}