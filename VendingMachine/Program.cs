using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace VendingMachine
{
    internal class Program
    {

        static int totalcoins = 0;
        static string[] prices = { "2.00", "1.50", "1.50", "1.75", "1.75", "1.75", "2.00", "2.00", "2.25", "2.25" };
        static string[] items = { "Crisps", "Mars bar", "Snickers", "M&Ms", "Skittles", "Haribos", "Cookie", "Water", "Sprite", "CocaCola" }; //Listing item details in arrays
        static int[] stock = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
        static int number;
        static int attempt;
        static int addstock;
        static int removestock;
        static string addstockstring;
        static string removestockstring;
        static string itemnumberstring;
        static string ans;                          //Making all variables global
        static string moneystring;
        static string coin;
        static string password;
        static string passattempt;
        static double totalmoney;
        static double moneyentered;
        static bool quit;

        static void Main()
        {
            //Vending machine
            Console.Clear();
            Machine(); //Printing vending machine
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nA: Purchase item\nB: Admin Panel\nC: Exit");  //Options

            while (true)
            {
                ans = Console.ReadLine();     //Reading users answer
                switch (ans.ToUpper())
                {
                    case "A": ChoosingItem(); break;                    //
                    case "B": Password(); break;                        //Cases for user choice
                    case "C": Environment.Exit(0); break;               //
                    default: Console.WriteLine("Invalid entry, try again: "); break; //If the user chooses something not in the list
                }
            }
        }

        static void ChoosingItem() //Choosing item page
        {
            Console.Clear();
            Machine();  //Printing vending machine
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nEnter item number OR enter X to go back: ");
            itemnumberstring = Console.ReadLine();
            itemnumbercheck(itemnumberstring, number);     //Checking users answer
            Console.WriteLine($"\nYou chose {items[number - 1]}, is this correct?");     //Validating users answer
            ans = Console.ReadLine();
            answercheck(ans);      //Checking users response
        }

        static int itemnumbercheck(string temp, int num) //Checking if the users answer is okay
        {
            num = 0;
            while (true)
                try //attempting to parse the answer first, to find out if it is an integer
                {
                    int.Parse(temp);
                    if (int.Parse(temp) > 0 && int.Parse(temp) < 11 && stock[int.Parse(temp) - 1] > 0) //if it is an integer, and within the right boundaries (between 1 and 10) and is in stock
                    {
                        number = int.Parse(temp); //setting number as the users answer
                        return (number); //returning number
                    }
                    if (int.Parse(temp) > 0 && int.Parse(temp) < 11 && stock[int.Parse(temp) - 1] < 1) //if it is an integer, and within the right boundaries (between 1 and 10) but NOT in stock
                    {
                        Console.WriteLine("Sorry! Item out of stock. Please choose a different item: "); //Asking user to pick again
                        temp = Console.ReadLine(); //Reading user response
                    }
                    else
                    {
                        Console.Write("Invalid entry. Try again: "); //Otherwise printing invalid and asking for a new response
                        temp = Console.ReadLine(); 
                    }
                }
                catch //if the try does not work (if the answer was not an integer)
                {
                    if(temp.ToUpper() == "X") //if the answer was 'X'
                    {
                        Main(); //Moving back to main screen
                        return (number); //This does nothing because number has not been defined in this case, only here to make the code path return a value and have no error
                    }
                    else //If the answer was not an integer, nor 'X', print invalid and ask for a new response
                    {
                        Console.Write("Invalid entry. Try again: "); 
                        temp = Console.ReadLine();
                    }
                }
        }

        static void answercheck(string temp) //Checking the users answer to 'You chose (item), is this correct?
        {
            while (true)
            {               
                if (temp.ToUpper() == "YES") //if the answer is yes
                {
                    stock[number - 1] = stock[number - 1] - 1; //remove 1 stock from the item
                    entermoney(); //move to the entering money screen
                    break;
                }
                else if (temp.ToUpper() == "NO") //if the answer is no
                {
                    Console.Clear(); //clear console
                    Machine(); //reprint the vending machine
                    ChoosingItem(); //reprint choosing screen
                    break;
                }
                else //otherwise print invalid and ask for new response
                {
                    Console.Write("Invalid entry. Try again: ");
                    temp = Console.ReadLine();
                }
            }
        }

        static void entermoney() //entering money screen
        {
            totalmoney = 0;
            moneyentered = 0;
            quit = false;

            while (quit == false && moneyentered < double.Parse(prices[number - 1])) //while the user does not want to quit, and money entered is less than the price
            {
                Console.Clear();
                Machine();
                Console.BackgroundColor = ConsoleColor.Black; //clearing and reprinting stuff
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine($"\nPurchasing {items[number - 1]}\nPlease enter £{prices[number-1]}."); //Tell the user what they're buying and how much it costs
                Console.WriteLine($"\nYou have entered: £{moneyentered}"); //Tell the user how much money they have entered
                Console.WriteLine("\nEnter Coin: \n x = Quit \n 1 = 5p \n 2 = 10p \n 3 = 20p \n 4 = 50p \n 5 = £1 \n 6 = £2"); //Give users options for entering money
                coin = Console.ReadLine();
                switch (coin)
                {
                    case "1": moneyentered = moneyentered + 0.05; break;
                    case "2": moneyentered = moneyentered + 0.10; break;
                    case "3": moneyentered = moneyentered + 0.20; break; //Cases for how much money they chose to add
                    case "4": moneyentered = moneyentered + 0.50; break;
                    case "5": moneyentered = moneyentered + 1.00; break;
                    case "6": moneyentered = moneyentered + 2.00; break;
                    case "x": quit = true; ChoosingItem(); break;     //User wants to quit so take them back to choosing page
                    default: Console.WriteLine("No such coin"); break; //If answer does not match any case, print no such coin and try entire loop again
                }
            }
            dispensing(); //Money entered is equal to or greater than the price so move onto dispensing item
        }
        static void dispensing() //Dispensing item
        {
            Console.WriteLine($"Dispensing {items[number - 1]}..."); //Telling the user what is being dispensed
            System.Threading.Thread.Sleep(1000);
            if (moneyentered > double.Parse(prices[number-1])) //If the user has change
            {
                Console.WriteLine($"Your change is: £{moneyentered - double.Parse(prices[number-1])}"); //Tell user amount of change
                Console.WriteLine("Would you like to eject it?"); //Ask if they want it back
                while(true)
                {
                    ans = Console.ReadLine();
                    if (ans.ToUpper() == "YES") //If yes
                    {
                        Console.Write("Ejecting change... ");
                        System.Threading.Thread.Sleep(1000);
                        Console.Write("Ejected!");
                        Console.WriteLine("\nPlease take your item from the black flap below."); //Eject change and then return to main page
                        Console.WriteLine("\nEnjoy! Returning...");
                        System.Threading.Thread.Sleep(4000);
                        Main();
                        break;
                    }
                    if (ans.ToUpper() == "NO") //If no
                    {
                        Console.WriteLine("\nPlease take your item from the black flap below.");
                        Console.WriteLine("\nEnjoy! Returning...");     //Return to main page
                        System.Threading.Thread.Sleep(4000);
                        Main();
                        break;
                    }
                    else //If answer is neither yes or no
                    {
                        Console.WriteLine("Invalid entry. Try again: "); //Ask for a new answer
                    }
                }
            }
            else //If user has no change
            {
                Console.WriteLine("Please take your item from the black flap below."); 
                Console.Write("\nEnjoy! Returning...");         //Return to main page
                System.Threading.Thread.Sleep(4000);
                Main();
            }
        }

        static void Password() //Password for admin
        {
            Console.Clear();
            Console.WriteLine("---------------\n[ ADMIN PANEL ]\n---------------");
            password = "Staff123"; //Setting password
            attempt = 1; //Setting the attempt to first attempt
            Console.Write($"-Case sensitive-\n\nAttempt {attempt}/3. Enter password: "); //Ask for password
            passattempt = Console.ReadLine();
            while (attempt < 3) //While password attempts are less than 3
            {
                if (passattempt == "Staff123") //If password is correct
                {
                    Admin(); //Move onto admin page
                    break;
                }
                else //If password is incorrect
                {
                    attempt = attempt + 1; //Add 1 to attempts
                    Console.Write($"Attempt {attempt}/3. Try again: "); //Tell user the amount of attempts and ask them to try again
                    passattempt = Console.ReadLine();
                }
            }
            Console.WriteLine("Maximum amount of attempts reached!"); //If the user get 3 incorrect guesses, redirect to main page
            Console.WriteLine("Redirecting to home page...");
            System.Threading.Thread.Sleep(2000);
            Main();
        }

        static void Admin() //Admin page
        {
            Console.Clear();
            Console.WriteLine("---------------\n[ ADMIN PANEL ]\n---------------");
            Console.WriteLine("\nA: Add Stock\nB: Remove Stock\nC: Exit"); //Giving user options

            while (true)
            {
                ans = Console.ReadLine();
                switch (ans.ToUpper()) //Cases for user answers
                {
                    case "A": //If user wants to add stock
                        {
                            Console.WriteLine($"\nWhich item would you like to add stock to?\n1: Crisps\n2: Mars bar\n3: Snickers\n4: M&Ms\n5: Skittles\n6: Haribos\n7: Cookie\n8: Water\n9: Sprite\n10: CocaCola\n"); //Item options
                            while(true)
                            {
                                ans = Console.ReadLine();
                                if (CheckInt(ans) && int.Parse(ans) > 0 && int.Parse(ans) < 11) //If number choice is valid (an integer and between 1-10)
                                {
                                    if(stock[int.Parse(ans) - 1] == 10) //If stock is at max capacity
                                    {
                                        Console.WriteLine("You cannot add more stock to this item!");
                                        Console.WriteLine("Returning to admin page...");
                                        System.Threading.Thread.Sleep(2000);
                                        Admin();
                                        break;
                                    }
                                    
                                    Console.WriteLine($"How much stock would you like to add? (Max: {10 - stock[int.Parse(ans)-1]})"); //Asking how much stock they want to add, and telling user the amount of stock they can add
                                    addstockstring = Console.ReadLine();                                                                                                                             //^^if stock is at 7 they can only add 3 (max is 10)
                                    if (CheckInt(addstockstring) && int.Parse(addstockstring) <= 10 - stock[int.Parse(ans) - 1]) //If the user entered and integer, and it is less than or equal to the max amount of stock addable
                                    {
                                        addstock = int.Parse(addstockstring); //Setting addstock to the amount of stock the user wants to add
                                        stock[int.Parse(ans) - 1] = stock[int.Parse(ans) - 1] + addstock; //Adding the stock
                                        break;
                                    }
                                    else //If input is invalid or the choice is over the amount of stock addable, print invalid and ask for new response (repeat loop)
                                    {
                                        Console.WriteLine("Invalid input or amount. Try again: ");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input or amount. Try again: ");
                                }
                            }
                            Console.WriteLine("Stock added! Returning to admin panel...");
                            System.Threading.Thread.Sleep(1000);
                            Admin();
                        } break;
                    case "B": //If user wants to remove stock
                        {
                            Console.WriteLine($"\nWhich item would you like to remove stock from?\n1: Crisps\n2: Mars bar\n3: Snickers\n4: M&Ms\n5: Skittles\n6: Haribos\n7: Cookie\n8: Water\n9: Sprite\n10: CocaCola\n"); //Item options
                            while (true)
                            {
                                ans = Console.ReadLine();
                                if (CheckInt(ans) && int.Parse(ans) > 0 && int.Parse(ans) < 11) //If number choice is valid (an integer and between 1-10)
                                {
                                    if (stock[int.Parse(ans) - 1]<1) //If stock is empty
                                    {
                                        Console.WriteLine("You cannot remove more stock from this item!");
                                        Console.WriteLine("Returning to admin page...");
                                        System.Threading.Thread.Sleep(2000);
                                        Admin();
                                        break;
                                    }
                                    Console.WriteLine($"How much stock would you like to remove? (Max: {stock[int.Parse(ans) - 1]})"); //Asking how much stock they want to remove, and telling user the amount of stock they can remove
                                    removestockstring = Console.ReadLine();                                                                                                             //^^if stock is at 7 they can only remove 7
                                    if (CheckInt(removestockstring) && int.Parse(removestockstring) <= stock[int.Parse(ans) - 1]) //If the user entered and integer, and it is less than or equal to the max amount of stock removeable
                                    {
                                        removestock = int.Parse(removestockstring); //Setting removestock to the amount of stock the user wants to remove
                                        stock[int.Parse(ans) - 1] = stock[int.Parse(ans) - 1] - removestock; //Removing stock
                                        break;
                                    }
                                    else //If input is invalid or the choice is over the amount of stock removeable, print invalid and ask for new response (repeat loop)
                                    {
                                        Console.WriteLine("Invalid input or amount. Try again: ");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input or amount. Try again: ");
                                }
                            }
                            Console.WriteLine("Stock removed! Returning to admin panel...");
                            System.Threading.Thread.Sleep(1000);
                            Admin();
                            
                        } break;
                    case "C": Console.WriteLine("Exiting..."); System.Threading.Thread.Sleep(1000); Main(); ; break; //If user wants to exit, move back to main admin page
                    default: Console.WriteLine("Invalid entry, try again: "); break; //If user inputs something other than the cases, print invalid and ask again
                }
            }
        }

        static void Machine() //Vending machine stuff
        {

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n |==========================| ");
            Console.WriteLine(" |     VENDING  MACHINE     | ");
            Console.WriteLine(" |==========================| ");
            Console.WriteLine(" |   5p 10p 20p 50p £1 £2   | ");
            Console.WriteLine(" |--------------------------| ");
            Console.WriteLine($" |-1: Crisps   - £2.00 ({stock[0]})-| "); //item 1
            Console.WriteLine($" |-2: Mars bar - £1.50 ({stock[1]})-| "); //item 2
            Console.WriteLine($" |-3: Snickers - £1.50 ({stock[2]})-| "); //item 3
            Console.WriteLine($" |-4: M&Ms     - £1.75 ({stock[3]})-| "); //item 4
            Console.WriteLine($" |-5: Skittles - £1.75 ({stock[4]})-| "); //item 5  //Vending machine design and details
            Console.WriteLine($" |-6: Haribos  - £1.75 ({stock[5]})-| "); //item 6
            Console.WriteLine($" |-7: Cookie   - £2.00 ({stock[6]})-| "); //item 7
            Console.WriteLine($" |-8: Water    - £2.00 ({stock[7]})-| "); //item 8
            Console.WriteLine($" |-9: Sprite   - £2.25 ({stock[8]})-| "); //item 9
            Console.WriteLine($" |-10:CocaCola - £2.25 ({stock[9]})-| "); //item 10
            Console.WriteLine(" |--------------------------| ");
            Console.WriteLine(" |                          | ");
            Console.WriteLine(" |==========================| ");
            Console.WriteLine(" | # |[                ]| # | ");
            Console.WriteLine(" |==========================| ");
            Console.BackgroundColor = ConsoleColor.Black;
            int Length = 1;
            int Width = 21;
            int Positionx = 0;
            int Positiony = 0;
            DrawRectangle(ConsoleColor.DarkGray, Positionx, Positiony, Length, Width);
            Length = 1;
            Width = 21;
            Positionx = 29;
            Positiony = 0;
            DrawRectangle(ConsoleColor.DarkGray, Positionx, Positiony, Length, Width);
            Length = 29;
            Width = 1;
            Positionx = 0;
            Positiony = 0;
            DrawRectangle(ConsoleColor.DarkGray, Positionx, Positiony, Length, Width);      //Vending machine colouring
            Length = 26;
            Width = 1;
            Positionx = 2;
            Positiony = 17;
            DrawRectangle(ConsoleColor.DarkGray, Positionx, Positiony, Length, Width);
            Length = 16;
            Width = 1;
            Positionx = 7;
            Positiony = 19;
            DrawRectangle(ConsoleColor.Black, Positionx, Positiony, Length, Width);
            Length = 30;
            Width = 1;
            Positionx = 0;
            Positiony = 21;
            DrawRectangle(ConsoleColor.DarkGray, Positionx, Positiony, Length, Width);
        }
        static void DrawRectangle(ConsoleColor SquareColor, int xpos, int ypos, int L, int W) //Method to draw rectangle
        {
            Console.BackgroundColor = SquareColor;
            for (int i = 0; i < L; i++) // Outer loop
            {
                for (int j = 0; j < W; j++) // Inner loop
                {
                    Console.SetCursorPosition(xpos + i, ypos + j);
                    Console.WriteLine(" ");
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }
        static bool CheckInt(string temp)  //method to check if a value is an integer
        {
            int number = 0;
            return int.TryParse(temp, out number);
        }
        static bool CheckDouble(string temp)  //method to check if a value is a double
        {
            double number = 0;
            return double.TryParse(temp, out number);
        }
    }
}
