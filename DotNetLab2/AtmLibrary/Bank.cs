using System.Collections.Generic;
using System.Linq;

namespace AtmLibrary
{
    public class Bank
    {
        public string Name { get; set; }
        public List<AutomatedTellerMachine> Atms { get; set; }
        public List<Account> Accounts { get; set; }

        public Bank(string name)
        {
            Name = name;
            Atms = new List<AutomatedTellerMachine>();
            Accounts = new List<Account>();
        }

        public Account FindAccount(string cardNumber)
        {
            return Accounts.FirstOrDefault(a => a.CardNumber == cardNumber);
        }
    }
}