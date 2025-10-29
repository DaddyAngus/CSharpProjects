using System.Runtime.CompilerServices;

class Bank
{
    private List<Account> _accounts;
    private List<Transaction> _transactions;

    public Bank()
    {
        this._accounts = new List<Account>();
        this._transactions = new List<Transaction>();
    }

    public void AddAccount(Account account)
    {
        this._accounts.Add(account);
    }

    public Account? GetAccount(string name)
    {
        foreach (Account account in this._accounts)
        {
            if (account.Name == name)
            {
                return account;
            }
        }
        return null;
    }
    
    public List<Transaction> Transactions
    {
        get {return this._transactions;}
    }

    public void ExecuteTransaction(Transaction transaction)
    {
        transaction.Execute();
        this._transactions.Add(transaction);
        
        // this._transactions.Add(transaction.ShallowClone()); //could use this if you wanted an actual log of all things that happened but simplified as wasnt expressely forbidden

    }

    public void RollbackTransaction(Transaction transaction)
    {
        transaction.Rollback();

        // this._transactions.Add(transaction.ShallowClone()); // again same here

    }



    public void PrintTransactionHistory()
    {
        foreach (Transaction transaction in this._transactions) //maybe turn to for loop if it suitsno b
        {
            transaction.PrintHistory = true;
            Console.WriteLine();
            Console.WriteLine(this._transactions.IndexOf(transaction) + 1 + ". This transaction took place " + transaction.DateStamp.ToString() + ", the account balance is before transaction, below is relevant information");
            transaction.Print();
            transaction.PrintHistory = false;

        }
      
    }

}

