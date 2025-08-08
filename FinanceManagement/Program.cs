using System;
using System.Collections.Generic;

#region Core Models

// Immutable record for transactions
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

#endregion

#region Transaction Processing

// Interface for processing transactions
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Different transaction processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}.");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Sent {transaction.Amount:C} for {transaction.Category}.");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Transferred {transaction.Amount:C} for {transaction.Category}.");
    }
}

#endregion

#region Accounts

// Base account
public class Account
{
    public string AccountNumber { get; private set; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"[Account] Deducted {transaction.Amount:C}. New Balance: {Balance:C}");
    }
}

// Specialized, sealed account
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Deducted {transaction.Amount:C}. Updated Balance: {Balance:C}");
        }
    }
}

#endregion

#region Finance Application

public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        Console.Write("Enter account number: ");
        string accNumber = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter initial balance: ");
        decimal initialBalance;
        while (!decimal.TryParse(Console.ReadLine(), out initialBalance))
        {
            Console.Write("Invalid amount. Please enter a valid initial balance: ");
        }

        var account = new SavingsAccount(accNumber, initialBalance);

        // Get three transactions from user
        for (int i = 1; i <= 3; i++)
        {
            Console.WriteLine($"\n--- Enter details for Transaction {i} ---");
            Console.Write("Category: ");
            string category = Console.ReadLine() ?? string.Empty;

            Console.Write("Amount: ");
            decimal amount;
            while (!decimal.TryParse(Console.ReadLine(), out amount))
            {
                Console.Write("Invalid amount. Please enter a valid decimal value: ");
            }

            var transaction = new Transaction(i, DateTime.Now, amount, category);

            // Assign processor based on transaction number
            ITransactionProcessor processor = i switch
            {
                1 => new MobileMoneyProcessor(),
                2 => new BankTransferProcessor(),
                _ => new CryptoWalletProcessor()
            };

            processor.Process(transaction);
            account.ApplyTransaction(transaction);

            _transactions.Add(transaction);
        }

        // Print summary
        Console.WriteLine("\n--- Transaction Summary ---");
        foreach (var t in _transactions)
        {
            Console.WriteLine($"{t.Id}: {t.Date} | {t.Category} - {t.Amount:C}");
        }

        Console.WriteLine($"\nFinal Account Balance: {account.Balance:C}");
    }
}

#endregion

#region Entry Point

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}

#endregion
