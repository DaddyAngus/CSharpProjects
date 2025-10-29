using System.Data.SqlTypes;

class WithdrawTransaction : Transaction
{
    private readonly Account _account;

    public WithdrawTransaction(Account _account, decimal _amount) : base(_amount)
    {
        this._account = _account;
    }

    public override bool Success
    {
        get { return _success; }
    }

    public Account Account
    {
        get { return _account; }
    }


    public override IEnumerable<Account> Accounts
    {
        get { yield return _account; }
    }


    public decimal Amount
    {
        get { return _amount; }
    }




    public override void Print()
    {
        decimal displayBalance = Account.Balance;
        Console.WriteLine("The transaction occured at: " + DateStamp.ToString());
        
        if (PrintHistory)
        {
            displayBalance = _balanceAtExecution;
        }


        if (Success)
        {
             if (Reversed)
            {
                Console.WriteLine("\nThe withdrawal from " + Account.Name + " was: Reversed" + "\nThe amount that was reversed was: " + Amount.ToString("C") + "\nThe balance of the account: " + displayBalance.ToString("C"));
            }
            else
            {
               Console.WriteLine("\nThe withdrawal from " + Account.Name + " was: Successful" + "\nThe amount that was withdrawn was: " + Amount.ToString("C") + "\nThe current balance of the account: " + displayBalance.ToString("C"));

            }
            
        }

        else if (!Success)
        {
            Console.WriteLine("\nThe withdrawal from " + Account.Name + " was: Unsuccessful" + "\nThe amount that you tried to withdrawn was: " + Amount.ToString("C") + "\nThe balance of the account: " + displayBalance.ToString("C"));
        }


    }

    public override void Execute()
    {
        
        if (Executed)
        {
            throw new InvalidOperationException("\nThe transaction involving the user: " + Account.Name.ToString() + " has already taken place");
        }

        try
        {
            base.Execute();

            if (Account.Withdraw(Amount))
            {
                _success = true;
            }

            else
            {
                _success = false;
                Console.WriteLine();
                throw new InvalidOperationException("\n" + Account.Name + "'s balance: " + Account.Balance.ToString("C") + " is too low to withdraw the amount: " + Amount.ToString("C"));
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.GetType().Name + ex.Message);
        }

    }

    public override void Rollback()
    {
        base.Rollback();

        if (Account.Deposit(Amount))//final failsafe
        {
            Console.WriteLine("\nThe withdrawl amount of: " + Amount.ToString("C") + " has been deposited");
            Console.WriteLine(Account.Name + "'s new balance is: " + Account.Balance.ToString("C"));
            // _executed = false;
            MarkUnexecuted();
        }

        else
        {
            throw new InvalidOperationException("\n" + Account.Name + "'s account could not be withdrawn from due to an error");
        }


    }
}



