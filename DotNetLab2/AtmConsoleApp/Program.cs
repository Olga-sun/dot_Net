using System;
using System.Text;
using AtmLibrary; 

namespace AtmConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Bank bank = new Bank("Політех Банк");
            bank.Accounts.Add(new Account("1111222233334444", "1234", "Литвинчук Ольга", 5000m));
            bank.Accounts.Add(new Account("5555666677778888", "0000", "Петренко Петро", 2500m));

            AutomatedTellerMachine atm = new AutomatedTellerMachine("ATM-01", "м. Житомир, вул. Чуднівська, 103", 20000m, bank);
            bank.Atms.Add(atm);

            EventHandler<AtmEventArgs> consoleHandler = (sender, e) =>
            {
                Console.ForegroundColor = e.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"\n[ПОВІДОМЛЕННЯ]: {e.Message}");
                Console.ResetColor();
            };

            atm.Authenticated += consoleHandler;
            atm.BalanceViewed += consoleHandler;
            atm.CashWithdrawn += consoleHandler;
            atm.CashDeposited += consoleHandler;
            atm.FundsTransferred += consoleHandler;

            bool isRunning = true;

            while (isRunning)
            {
                if (atm.CurrentAccount == null)
                {
                    Console.WriteLine("\n==================================");
                    Console.WriteLine("    ВІТАЄМО У ПОЛІТЕХ БАНКУ");
                    Console.WriteLine("==================================");
                    Console.Write("Введіть номер картки (або 'exit' для виходу): ");
                    string cardInput = Console.ReadLine();

                    if (cardInput.ToLower() == "exit")
                    {
                        isRunning = false;
                        continue;
                    }

                    Console.Write("Введіть ПІН-код: ");
                    string pinInput = Console.ReadLine();

                    atm.Authenticate(cardInput, pinInput);
                }
                else
                {
                    
                    Console.WriteLine("\n==================================");
                    Console.WriteLine($" Клієнт: {atm.CurrentAccount.OwnerFullName}");
                    Console.WriteLine("==================================");
                    Console.WriteLine("1. Переглянути баланс");
                    Console.WriteLine("2. Зняти кошти");
                    Console.WriteLine("3. Поповнити рахунок");
                    Console.WriteLine("4. Переказати на іншу картку");
                    Console.WriteLine("5. Завершити сеанс (Повернути картку)");
                    Console.WriteLine("==================================");
                    Console.Write("Оберіть операцію (1-5): ");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            atm.ViewBalance();
                            break;

                        case "2":
                            Console.Write("Введіть суму для зняття (грн): ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                            {
                                atm.Withdraw(withdrawAmount);
                            }
                            else
                            {
                                Console.WriteLine("Помилка вводу. Введіть числове значення.");
                            }
                            break;

                        case "3":
                            Console.Write("Введіть суму для поповнення (грн): ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                            {
                                atm.Deposit(depositAmount);
                            }
                            else
                            {
                                Console.WriteLine("Помилка вводу. Введіть числове значення.");
                            }
                            break;

                        case "4":
                            Console.Write("Введіть номер картки отримувача: ");
                            string targetCard = Console.ReadLine();

                            Console.Write("Введіть суму переказу (грн): ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount))
                            {
                                atm.Transfer(targetCard, transferAmount);
                            }
                            else
                            {
                                Console.WriteLine("Помилка вводу. Введіть числове значення.");
                            }
                            break;

                        case "5":
                            atm.Logout();
                            Console.WriteLine("\nСеанс завершено. Заберіть вашу картку.");
                            break;

                        default:
                            Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                            break;
                    }
                }
            }
        }
    }
}