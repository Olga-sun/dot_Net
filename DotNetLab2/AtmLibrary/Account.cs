using System;

namespace AtmLibrary
{
    
    public class AtmEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }

        public AtmEventArgs(string message, bool isSuccess = true)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }

   
    public class Account
    {
        public string CardNumber { get; set; }
        public string PinCode { get; set; }
        public string OwnerFullName { get; set; }
        public decimal Balance { get; set; }

        public Account(string cardNumber, string pinCode, string ownerFullName, decimal balance)
        {
            CardNumber = cardNumber;
            PinCode = pinCode;
            OwnerFullName = ownerFullName;
            Balance = balance;
        }
    }
}