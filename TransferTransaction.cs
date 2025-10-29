class TransferTransaction : Transaction
{
    private readonly Account _fromAccount;
    private readonly Account _toAccount;
    private DepositTransaction _deposit;
    private WithdrawTransaction _withdraw;

    public TransferTransaction(Account _fromAccount, Account _toAccount, decimal _amount) : base(_amount)
    {
        this._fromAccount = _fromAccount;
        this._toAccount = _toAccount;

        this._deposit = new DepositTransaction(ToAccount, Amount);
        this._withdraw = new WithdrawTransaction(FromAccount, Amount);
    }


    public override bool Success
    {
        get { return _withdraw.Success && _deposit.Success; }
    }

    public Account ToAccount
    {
        get { return _toAccount; }
    }

    public Account FromAccount
    {
        get { return _fromAccount; }
    }

    public override IEnumerable<Account> Accounts
    {
        get
        {
            yield return _toAccount;
            yield return _fromAccount;
        }
    }

    public decimal Amount
    {
        get { return _amount; }
    }

    public override void Print()
    {
        Console.WriteLine("The transaction occured at: " + DateStamp.ToString());
        if (Success)
        {
            Console.WriteLine("\nThe transfer was: Successful" + "\nTransferred: " + Amount.ToString("C") + " from " + FromAccount.Name + "'s account to " + ToAccount.Name + "'s account");
        }

        else if (Reversed)
        {
            Console.WriteLine("\nThe tranfer to " + ToAccount.Name + " from " + FromAccount.Name + " was: Reversed" + "\nThe amount that was reversed was: " + Amount.ToString("C"));
        }

        else if (!Success)
        {
            Console.WriteLine("\nThe Transfer was: Unsuccessful" + "\nThe amount that you tried to transfer was: " + Amount.ToString("C") + "\nThe current balance of " + FromAccount.Name + "'s account is: " + FromAccount.Balance.ToString("C"));

        }
        _withdraw.Print();
        _deposit.Print();
    }

    public override void Execute()
    {
        if (Executed)
        {
            throw new InvalidOperationException("\nThe transaction involving the users: " + ToAccount.Name + " and " + FromAccount.Name + " has already taken place");
        }

        try
        {
            base.Execute();

            _withdraw.Execute(); // dont need to throw as execute for the withdraw class already throws

            if (_withdraw.Success)
            {
                _deposit.Execute();
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

        _deposit.Rollback();
        _withdraw.Rollback();

        Console.WriteLine("\nThe transfer amount of: " + Amount.ToString("C") + " has been refunded" + "\n" + FromAccount.Name + "'s account balance has been returned to: " + FromAccount.Balance.ToString("C"));
        // _executed = false; //because the proccess was never executed as it was reversed

        MarkUnexecuted();
    }
}