using System;
using System.Windows;
using AtmLibrary;

namespace AtmWinFormsApp
{
    public partial class MainWindow : Window
    {
        private Bank _bank;
        private AutomatedTellerMachine _atm;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAtm();
        }

        private void InitializeAtm()
        {
            _bank = new Bank("Політех Банк");
            _bank.Accounts.Add(new Account("1111222233334444", "1234", "Литвинчук Ольга", 5000m));
            _bank.Accounts.Add(new Account("5555666677778888", "0000", "Петренко Петро", 2500m));

            _atm = new AutomatedTellerMachine("ATM-01", "м. Житомир, вул. Чуднівська, 103", 20000m, _bank);
            _bank.Atms.Add(_atm);

           
            EventHandler<AtmEventArgs> windowHandler = (sender, e) => MessageBox.Show(e.Message);

            _atm.Authenticated += windowHandler;
            _atm.BalanceViewed += windowHandler;
            _atm.CashWithdrawn += windowHandler;
            _atm.CashDeposited += windowHandler;
            _atm.FundsTransferred += windowHandler;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_atm.Authenticate(TxtCardNumber.Text.Trim(), TxtPin.Password.Trim()))
            {
                TxtWelcome.Text = "Клієнт: " + _atm.CurrentAccount.OwnerFullName;
                PanelLogin.Visibility = Visibility.Collapsed;
                PanelMenu.Visibility = Visibility.Visible;
            }
        }

        private void BtnBalance_Click(object sender, RoutedEventArgs e)
        {
            _atm.ViewBalance();
        }

        private void BtnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            decimal amount;
            if (decimal.TryParse(TxtAmount.Text, out amount))
                _atm.Withdraw(amount);
            else
                MessageBox.Show("Введіть коректну суму!");
        }

        private void BtnDeposit_Click(object sender, RoutedEventArgs e)
        {
            decimal amount;
            if (decimal.TryParse(TxtAmount.Text, out amount))
                _atm.Deposit(amount);
            else
                MessageBox.Show("Введіть коректну суму!");
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            decimal amount;
            if (decimal.TryParse(TxtAmount.Text, out amount))
                _atm.Transfer(TxtTargetCard.Text.Trim(), amount);
            else
                MessageBox.Show("Введіть коректну суму!");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _atm.Logout();
            TxtPin.Password = "";
            TxtAmount.Text = "";
            PanelMenu.Visibility = Visibility.Collapsed;
            PanelLogin.Visibility = Visibility.Visible;
            MessageBox.Show("Сеанс завершено. Заберіть вашу картку.");
        }
    }
}