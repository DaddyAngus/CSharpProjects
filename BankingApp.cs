using System.IO.Compression;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Transactions;

enum MenuOption
{
    Withdraw = 0,
    Deposit = 1,

    Transfer = 2,
    Print = 3,

    Print_Transaction_History = 4,
    Add_New_Account = 5,
    Quit = 6,
}



class BankSystem
{
    static public string ConvertString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Im sorry the, argument was not accepted");
        }

        return input.Trim().ToUpper();
    }

    static public Account CheckNull(Account? account)
    {
        if (account == null)
        {
            throw new ArgumentException("\nCould not find the account specified");//dont like this but it does the job as i HAVE to return null in the findaccount
        }
        else
        {
            return account;
        }
    }


    static public Account? FindAccount(Bank bank)
    {
        Console.WriteLine("What is the name of the account you are trying to find: ");
        string AccName = ConvertString(Console.ReadLine()!);

        Account? foundAcc = bank.GetAccount(AccName);

        if (foundAcc == null)
        {
            Console.WriteLine("\nThe account you entered could not be found");
        }

        return foundAcc;
    }


    static void DoTransfer(Bank bank)
    {
        try
        {
            Console.WriteLine("Account to withdraw from: ");
            Account fromAccount = CheckNull(FindAccount(bank));
            Console.WriteLine("Account to deposit into: ");
            Account toAccount = CheckNull(FindAccount(bank));

            Console.WriteLine("How much would you like to transfer: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal User_Input))
            {
                TransferTransaction transfer = new TransferTransaction(fromAccount, toAccount, User_Input);
                bank.ExecuteTransaction(transfer); //fixed

                if (transfer.Executed && transfer.Success)
                {
                    Console.WriteLine("\nTransfer is now being attempted, you have entered the value: " + transfer.Amount.ToString("C") + " to be withdrawn from: " + fromAccount.Name + " and deposited to the account: " + toAccount.Name);
                    Console.WriteLine("Press any button to continue");
                    Console.ReadKey();
                }

                if (transfer.Success)
                {
                    Console.WriteLine("\nTransaction of: " + transfer.Amount.ToString("C") + " was successful, press enter to continue or backspace to roll-back the transaction");
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        transfer.Print();
                    }

                    else if (key == ConsoleKey.Backspace)
                    {
                        transfer.Rollback();
                    }
                }

                else if (!transfer.Success)
                {
                    transfer.Print();
                }
            }

            else
            {
                Console.WriteLine("\nYou did not enter a number...you are now being sent to the shadow realm");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.GetType().Name + ex.Message);
        }

    }

    static void DoDeposit(Bank bank)
    {
        try
        {
            Account account = CheckNull(FindAccount(bank));

            Console.WriteLine("How much would you like to deposit: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal User_Input))
            {
                DepositTransaction deposit = new DepositTransaction(account, User_Input);

                bank.ExecuteTransaction(deposit);

                if (deposit.Executed && deposit.Success)
                {
                    Console.WriteLine("\nDeposit is now being attempted, you have entered the value: " + deposit.Amount.ToString("C") + " to be deposited");
                    Console.WriteLine("Press any button to continue");
                    Console.ReadKey();
                }

                if (deposit.Success)
                {
                    Console.WriteLine("\nTransaction of: " + deposit.Amount.ToString("C") + " was successful, press enter to continue or backspace to roll-back the transaction");
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        deposit.Print();
                    }

                    else if (key == ConsoleKey.Backspace)
                    {
                        bank.RollbackTransaction(deposit); // hate this but cool
                    }
                }

                else if (!deposit.Success)
                {
                    deposit.Print();
                }

            }
            else
            {
                Console.WriteLine("\nYou did not enter a number...you are now being sent to the shadow realm");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.GetType().Name + ex.Message);
        }
    }


    static void DoWithdraw(Bank bank)
    {
        try
        {
            Account account = CheckNull(FindAccount(bank));
            Console.WriteLine("How much would you like to withdraw: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal User_Input))
            {
                WithdrawTransaction withdraw = new WithdrawTransaction(account, User_Input);
                bank.ExecuteTransaction(withdraw);
                if (withdraw.Executed && withdraw.Success)
                {
                    Console.WriteLine("\nWithdrawal is now being attempted, you have entered the value: " + withdraw.Amount.ToString("C") + " to be withdrawn");
                    Console.WriteLine("Press any button to continue");
                    Console.ReadKey();
                }

                if (withdraw.Success)
                {
                    Console.WriteLine("\nTransaction of: " + withdraw.Amount.ToString("C") + " was successful, press enter to continue or backspace to roll-back the transaction");
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        withdraw.Print();
                    }

                    else if (key == ConsoleKey.Backspace)
                    {
                        bank.RollbackTransaction(withdraw);
                    }
                }

                else if (!withdraw.Success)
                {
                    withdraw.Print();
                }
            }

            else
            {
                Console.WriteLine("\nYou did not enter a number...you are now being sent to the shadow realm");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.GetType().Name + ex.Message);
        }
    }

    static void DoPrint(Bank bank)
    {
        Account account = CheckNull(FindAccount(bank));
        account.Print();
    }


    static void doRollBack(Bank bank, int userInput)
    {
        int index = 0;
        bool found = false;
        foreach (Transaction transaction in bank.Transactions)
        {
            if (index == userInput - 1)
            {
                bank.RollbackTransaction(transaction);

                Console.WriteLine("\nYou succesfully reversed transaction " + userInput + " here is a summary below");
                transaction.Print();

                Console.WriteLine("\nPress Any Key To Continue...");
                Console.ReadKey();
                found = true;
                break;
            }
            index++;
        }
        if (!found)
        {
            throw new Exception("Couldnt find ");
        }
    }



    static void DoPrintTransactionHistory(Bank bank)
    {
        int userInput;
        do
        {
            try
            {
                bank.PrintTransactionHistory(); // to print values

                Console.WriteLine();
                Console.WriteLine("Would you like to rollback any transactions y/Y || n/N");
                string? choice = Console.ReadLine();


                if (choice == "y" || choice == "Y")
                {
                    Console.WriteLine("What entry would you like to select");
                    if (int.TryParse(Console.ReadLine(), out userInput))
                    {
                        doRollBack(bank, userInput);
                    }
                }
                else if (choice == "n" || choice == "N")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\nYou did not enter either y/Y or n/N....Automatically cancelling");
                    break;
                }
            }

            catch (Exception ex)//could print a message telling what the error was but considering this was a UI that the user is using there isnt much point
            {
                Console.WriteLine("\n" + ex.Message);
            }
        } while (true);
    }



    static MenuOption ReadUserOption()
    {
        int userInput;
        do
        {
            try
            {
                Console.WriteLine("\nWhat Option Would You Like");
                Console.WriteLine("1. Withdraw");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Transfer");
                Console.WriteLine("4. Print");
                Console.WriteLine("5. Print Transaction History");
                Console.WriteLine("6. Add New Account");
                Console.WriteLine("7. Quit");

                Console.WriteLine("What option would you like to choose....(Please enter a number between 1 and 7(Inclusive))");
                userInput = Convert.ToInt32(Console.ReadLine());//this isnt great as it doesnt deal with errors but its what you have said to add so it is here

                if (userInput >= 1 && userInput <= 7)
                {

                    Console.WriteLine("Are you sure.... " + ((MenuOption)userInput - 1).ToString() + " ....is what you want: y/n");
                    string? secondChoice = Console.ReadLine();

                    if (secondChoice == "y" || secondChoice == "Y")
                    {
                        return (MenuOption)userInput - 1;
                    }

                    else if (secondChoice == "n" || secondChoice == "N")
                    {
                        continue;
                    }

                    else
                    {
                        Console.WriteLine("\nYou did not enter either y/Y or n/N....Automatically cancelling");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("\nYou did not enter a number between 1 and 6 (inclusive)");
                    continue;
                }
            }

            catch (Exception)//could print a message telling what the error was but considering this was a UI that the user is using there isnt much point
            {
                Console.WriteLine("\nYou did not enter a valid number");
                continue;
            }
        } while (true);
    }


    static void Main(string[] args)
    {
        Bank bank = new Bank();
        bool switchBool = true;
        while (switchBool == true)
        {

            try
            {
                switch (ReadUserOption())
                {
                    case MenuOption.Withdraw:
                        DoWithdraw(bank);
                        continue;

                    case MenuOption.Deposit:
                        DoDeposit(bank);
                        continue;

                    case MenuOption.Transfer:
                        DoTransfer(bank);
                        continue;

                    case MenuOption.Print:
                        DoPrint(bank);
                        continue;

                    case MenuOption.Print_Transaction_History:
                        DoPrintTransactionHistory(bank);
                        continue;

                    case MenuOption.Add_New_Account:
                        try
                        {
                            Console.WriteLine("What is the name of the account:");
                            string AccName = ConvertString(Console.ReadLine()!);

                            Console.WriteLine("What is the balance you want to set for the account");
                            if (decimal.TryParse(Console.ReadLine(), out decimal AccDec))
                            {
                                Account account = new Account(AccName, AccDec);
                                bank.AddAccount(account);
                            }
                            else
                            {
                                Console.WriteLine("\nYou did not enter a number...you are now being sent to the shadow realm");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine();
                            Console.WriteLine(ex.GetType().Name + ex.Message);
                        }
                        continue;


                    case MenuOption.Quit:
                        switchBool = false;
                        break;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("\n"+ ex.Message + " error");
            }
        }
    }
}

