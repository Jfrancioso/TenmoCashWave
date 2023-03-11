using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;
using TenmoServer.Models;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                decimal Balance = tenmoApiService.GetBalance(tenmoApiService.UserId);
                Console.WriteLine($"Your current balance:{Balance:C2}");
                console.Pause();
            }

            if (menuSelection == 2)
            {
                Account loginAccount = tenmoApiService.GetAccount(tenmoApiService.UserId);
                string loginUsername = tenmoApiService.Username;
                // View your past transfers
                printMenu:
                IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                console.PrintTransfers(transfers, loginAccount, loginUsername);
                string otherUserUsername = "";
                Transfer transfer = new Transfer();
                while (otherUserUsername == null || otherUserUsername == "") 
                { 

                    Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");
                    string transferId = Console.ReadLine();
                    int parsedId;
                    if (!Int32.TryParse(transferId, out parsedId)) //try to parse it into an int, if it does not, print Please try again.
                    {
                        Console.WriteLine("Please Try again");//will goto next because parsedId == 0
                        console.Pause();
                        goto printMenu;
                    }
                    if (parsedId == 0)
                    {
                        goto next;
                    }
                    transfer = tenmoApiService.GetTransfer(parsedId);

                    otherUserUsername = tenmoApiService.GetUsernameByTransfer(parsedId, loginAccount.AccountId);
                    if (otherUserUsername != null && otherUserUsername != "")
                    {
                        break;
                    }

                    Console.WriteLine("Please try again.");
                }
                console.PrintTransferDetails(transfer, otherUserUsername, loginAccount, loginUsername);
                next:
                console.Pause();
            }

            if (menuSelection == 3)
            {
                //    // View your pending requests
                //    IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                //    console.PrintTransferDetails(transfers);
                //    console.Pause();
                Console.WriteLine("No.");
                console.Pause();
            }

            if (menuSelection == 4)
            {
                // Send TE bucks

                //Gets all the users on Tenmo and prints out a list with Ids and usernames
                IList<ApiUser> users = tenmoApiService.GetUsers();
                console.PrintSendingBucks(users);

                Console.Write("Id of the user you are sending to[0]: ");
                string toUserIdString = Console.ReadLine();
                int toUserId = Convert.ToInt32(toUserIdString);
                int toUserAccountId = tenmoApiService.GetAccount(toUserId).AccountId;
                int fromUserAccountId = tenmoApiService.GetAccount(tenmoApiService.UserId).AccountId;

                Console.Write("Enter amount to send: ");
                string transferAmountString = Console.ReadLine();
                decimal transferAmount = Convert.ToDecimal(transferAmountString);

                Transfer transfer = new Transfer(2, 2, fromUserAccountId, toUserAccountId, transferAmount);
                Transfer successfulTransfer = tenmoApiService.SendTransfer(transfer);
                if (successfulTransfer.TransferId != 0)
                {
                    Console.WriteLine("Transfer was successful!");
                }
                else
                {
                    Console.WriteLine("Transfer was unsuccessful.");
                }

                decimal balance = tenmoApiService.GetBalance(tenmoApiService.UserId);
                Console.WriteLine($"Your remaining balance is: {balance:C2}");

                console.Pause();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
                //IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                //console.PrintRequestingBucks(transfers);
                //decimal Balance = tenmoApiService.GetBalance(tenmoApiService.UserId);
                //Console.WriteLine($"Your current balance:{Balance:C2}");
               // Console.WriteLine("Id of the user you are requesting from[0]:");
                //decimal fromUserId = decimal.Parse(Console.ReadLine());
                // Console.WriteLine("Enter amount to request:");
                //decimal Amount = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Also no.");
                console.Pause();
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }


    }
}
