abstract class Transaction
{
    protected decimal _amount;
    protected bool _success;
    private bool _executed;
    private bool _reversed;
    private DateTime _dateStamp;

    protected decimal _balanceAtExecution;
    // this is to take a snapshot for the balance history
    private bool _printHistory;


    public Transaction(decimal _amount)
    {
        this._amount = _amount;
    }

    abstract public bool Success
    {
        get;
    }

    public abstract IEnumerable<Account> Accounts
    {
        get;
    } //this is only here to grab information from the child that is needed to streamline the parent


    // public Transaction ShallowClone()
    // {
    //     return (Transaction)this.MemberwiseClone();
    // }

    // not needed for this 



    public bool Executed
    {
        get { return _executed; }
    }

    public bool PrintHistory
    {
        get { return _printHistory; }
        set { _printHistory = value; }
    } //this flag will be used in all transactions for the print



    public bool Reversed
    {
        get { return _reversed; }
    }

    public DateTime DateStamp
    {
        get { return _dateStamp; }
    }

    public abstract void Print();



    public virtual void Execute()
    {
        foreach (var account in Accounts)
        {
            _balanceAtExecution = account.Balance;
        }
        // this is to take a snapshot for the balance history
        //this then uses the information from the child to do something


        _dateStamp = DateTime.Now;
        _executed = true;
    }

    public virtual void Rollback()
    {
        _dateStamp = DateTime.Now;

        if (!Success)
        {
            throw new InvalidOperationException("Cannot Rollback: Transaction was never completed");
        }

        if (!Executed)
        {
            throw new InvalidOperationException("Cannot Rollback: Transaction was never executed");
        }

        if (Reversed)
        {
            throw new InvalidOperationException("Cannot Rollback: Transaction was already reversed");
        }
        _reversed = true;

    }

    protected void MarkUnexecuted()
    {
        _executed = false;
    }





}