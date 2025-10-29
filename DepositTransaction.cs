class DepositTransaction : Transaction
{
    private readonly Account _account;


    public DepositTransaction(Account _account, decimal _amount) : base(_amount)
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
                Console.WriteLine("\nThe deposit to " + Account.Name + " was: Reversed" + "\nThe amount that was reversed was: " + Amount.ToString("C") + "\nThe balance of the account: " + displayBalance.ToString("C"));
            }
            else
            {
                Console.WriteLine("\nThe deposit to " + Account.Name + " was: Successful" + "\nThe amount that was deposited was: " + Amount.ToString("C") + "\nThe balance of the account: " + displayBalance.ToString("C"));

            }
        }
        else if (!Success)
        {
            Console.WriteLine("\nThe deposit to " + Account.Name + " was: Unsuccessful" + "\nThe amount that you tried to deposit was: " + Amount.ToString("C") + "\nThe balance of the account: " + displayBalance.ToString("C"));
        }
    }


    public override void Execute()
    {
        if (Executed)
        {
            Console.WriteLine();
            throw new InvalidOperationException("\nThe transaction involving the user: " + Account.ToString() + " has already taken place");
        }
        try
        {
            base.Execute();

            if (Account.Deposit(Amount))
            {
                _success = true;
            }

            else
            {
                _success = false;
                Console.WriteLine();
                throw new InvalidOperationException("\n" + Account.Name + "'s account tried to deposit the invalid number: " + Amount.ToString());
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

        if (Account.Withdraw(Amount))
        {
            Console.WriteLine("\nThe deposit amount of: " + Amount.ToString("C") + " has been withdrawn");
            Console.WriteLine(Account.Name + "'s new balance is: " + Account.Balance.ToString("C"));


            MarkUnexecuted(); // this needs to be reworked somehow
            // _success = false; //because the proccess was never executed as it was reversed - changed due to being misleading
        }

        else
        {
            throw new InvalidOperationException("\n" + Account.Name + "'s lacked sufficient funds to rollback the deposit, current balance: " + Account.Balance.ToString("C"));
        }


    }


}