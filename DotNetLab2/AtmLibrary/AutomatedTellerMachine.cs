using System;

namespace AtmLibrary
{
    public class AutomatedTellerMachine
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public decimal CashAvailable { get; private set; }
        public Account CurrentAccount { get; private set; }

        private Bank _bank;

        
        public event EventHandler<AtmEventArgs> Authenticated;
        public event EventHandler<AtmEventArgs> BalanceViewed;
        public event EventHandler<AtmEventArgs> CashWithdrawn;
        public event EventHandler<AtmEventArgs> CashDeposited;
        public event EventHandler<AtmEventArgs> FundsTransferred;

        public AutomatedTellerMachine(string id, string address, decimal initialCash, Bank bank)
        {
            Id = id;
            Address = address;
            CashAvailable = initialCash;
            _bank = bank;
        }

        public bool Authenticate(string cardNumber, string pinCode)
        {
            var account = _bank.FindAccount(cardNumber);
            if (account == null)
            {
                if (Authenticated != null)
                    Authenticated(this, new AtmEventArgs("Картку не знайдено.", false));
                return false;
            }

            if (account.PinCode != pinCode)
            {
                if (Authenticated != null)
                    Authenticated(this, new AtmEventArgs("Невірний ПІН-код.", false));
                return false;
            }

            CurrentAccount = account;
            if (Authenticated != null)
                Authenticated(this, new AtmEventArgs("Вітаємо, " + account.OwnerFullName + "! Аутентифікація успішна.", true));
            return true;
        }

        public void Logout()
        {
            CurrentAccount = null;
        }

        public void ViewBalance()
        {
            if (!EnsureAuthenticated(BalanceViewed)) return;
            if (BalanceViewed != null)
                BalanceViewed(this, new AtmEventArgs("Ваш баланс на картці: " + CurrentAccount.Balance + " грн.", true));
        }

        public bool Withdraw(decimal amount)
        {
            if (!EnsureAuthenticated(CashWithdrawn)) return false;

            if (amount <= 0)
            {
                if (CashWithdrawn != null)
                    CashWithdrawn(this, new AtmEventArgs("Сума повинна бути більшою за 0.", false));
                return false;
            }

            if (CurrentAccount.Balance < amount)
            {
                if (CashWithdrawn != null)
                    CashWithdrawn(this, new AtmEventArgs("Недостатньо коштів на картці.", false));
                return false;
            }

            if (CashAvailable < amount)
            {
                if (CashWithdrawn != null)
                    CashWithdrawn(this, new AtmEventArgs("У банкоматі недостатньо готівки.", false));
                return false;
            }

            CurrentAccount.Balance -= amount;
            CashAvailable -= amount;
            if (CashWithdrawn != null)
                CashWithdrawn(this, new AtmEventArgs("Знято " + amount + " грн. Залишок: " + CurrentAccount.Balance + " грн.", true));
            return true;
        }

        public bool Deposit(decimal amount)
        {
            if (!EnsureAuthenticated(CashDeposited)) return false;

            if (amount <= 0)
            {
                if (CashDeposited != null)
                    CashDeposited(this, new AtmEventArgs("Сума поповнення має бути більшою за 0.", false));
                return false;
            }

            CurrentAccount.Balance += amount;
            CashAvailable += amount;
            if (CashDeposited != null)
                CashDeposited(this, new AtmEventArgs("Зараховано " + amount + " грн. Новий баланс: " + CurrentAccount.Balance + " грн.", true));
            return true;
        }

        public bool Transfer(string targetCardNumber, decimal amount)
        {
            if (!EnsureAuthenticated(FundsTransferred)) return false;

            if (amount <= 0)
            {
                if (FundsTransferred != null)
                    FundsTransferred(this, new AtmEventArgs("Сума переказу має бути більшою за 0.", false));
                return false;
            }

            if (CurrentAccount.CardNumber == targetCardNumber)
            {
                if (FundsTransferred != null)
                    FundsTransferred(this, new AtmEventArgs("Не можна переказати кошти на власну картку.", false));
                return false;
            }

            var targetAccount = _bank.FindAccount(targetCardNumber);
            if (targetAccount == null)
            {
                if (FundsTransferred != null)
                    FundsTransferred(this, new AtmEventArgs("Картку отримувача не знайдено.", false));
                return false;
            }

            if (CurrentAccount.Balance < amount)
            {
                if (FundsTransferred != null)
                    FundsTransferred(this, new AtmEventArgs("Недостатньо коштів для переказу.", false));
                return false;
            }

            CurrentAccount.Balance -= amount;
            targetAccount.Balance += amount;
            if (FundsTransferred != null)
                FundsTransferred(this, new AtmEventArgs("Переказано " + amount + " грн отримувачу " + targetAccount.OwnerFullName + ".", true));
            return true;
        }

        private bool EnsureAuthenticated(EventHandler<AtmEventArgs> handler)
        {
            if (CurrentAccount == null)
            {
                if (handler != null)
                    handler(this, new AtmEventArgs("Спочатку пройдіть аутентифікацію!", false));
                return false;
            }
            return true;
        }
    }
}