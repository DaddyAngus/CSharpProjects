class Account
{
    private readonly string _name;
    private decimal _balance;

    public Account(string name, decimal balance)
    {
        this._name = name;
        this._balance = balance;
    }


    public String Name
    {
        get { return _name; }
    }

    public decimal Balance
    {
        get { return _balance; }
        set { _balance = value; }
    }


    public bool Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            return true;
        }

        else
        {
            return false;
        }
    }


    public bool Withdraw(decimal amount)
    {
        if (amount > 0 && amount <= Balance)
        {
            Balance -= amount;//changed these to reflect the property
            return true;
        }

        else
        {
            return false;
        }
    }






    public void Print()
    {
        Console.WriteLine("\nThe account name is: " + Name + "\nThe balance is: " + Balance.ToString("C"));
    }






  // public void DepositTransaction(){

    // }
    // public void TransferTransaction(){
    // }

    //dunno why this is here
}


