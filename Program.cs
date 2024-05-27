using Console_InvManagement.Models;
using Console_InvManagement.Services;
using System.Diagnostics;
using System.Xml.Linq;

namespace Console_InvManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Don't need separate class for shoppers cart
            //Just need two lists of Product
            var inventorySvc = InventoryServiceProxy.Current;
            var isExit = false;

            //I hope this is a new instance of InventoryServiceProxy and two variables pointing to the same place
            var shoppingCart = CartServiceProxy.Current;

            void addProduct_inv()
            {
                var isValid = true;
                double price = 0;
                int Amount = 0;
                string name = string.Empty;
                Console.WriteLine("Enter the name of the item: ");
                name = Console.ReadLine();
                Console.WriteLine("Enter the item description: ");
                var description = Console.ReadLine();
                do
                {
                    Console.WriteLine("Enter the price of the item: ");
                    var str_price = Console.ReadLine();
                    if(double.TryParse(str_price, out price))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        Console.WriteLine("Error: Please enter a valid price.\n");
                    }
                } while (!isValid);
                do
                {
                    Console.WriteLine("Enter the number of items in Amount: ");
                    var str_Amount = Console.ReadLine();
                    if (int.TryParse(str_Amount, out Amount))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        Console.WriteLine("Error: Please enter a valid number.\n");
                    }
                } while (!isValid);
                inventorySvc.AddItem(new Product
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    Amount = Amount
                });
            }
            void listInventory()
            {
                if (inventorySvc.Items.Any())
                {
                    foreach (Product product in inventorySvc.Items)
                    {
                        Console.WriteLine("\n");
                        Console.WriteLine(product.ToString(true));
                    }
                }
                    
            }
            void updateProduct()
            {
                Console.WriteLine("Enter the name of the product to be updated: ");
                string itemName = String.Empty;
                itemName = Console.ReadLine();

                foreach (Product product in inventorySvc.Items)
                {
                    if(product.Name == itemName)
                    {
                        string newName = String.Empty;
                        string newDescription = String.Empty;
                        double newPrice = 0;
                        int newAmount = 0;
                        Console.WriteLine("Product found! Please enter the new product information: \n");
                        Console.WriteLine("Name: ");
                        newName = Console.ReadLine();
                        Console.WriteLine("Description: ");
                        newDescription = Console.ReadLine();
                        Console.WriteLine("Price: ");
                        
                        if (double.TryParse(Console.ReadLine(), out newPrice))
                        {
                            Console.WriteLine("Amount: ");
                            if (int.TryParse(Console.ReadLine(), out newAmount))
                            {
                                inventorySvc.UpdateItem(product, newName, newDescription, newPrice, newAmount);
                            }
                            else
                            {
                                Console.WriteLine("Error: You did not enter a valid Amount.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: You did not enter a valid price.");
                        }
                    }
                }
            }
            void deleteProduct()
            {
                Console.WriteLine("Enter the name of the item you would like to delete: ");
                string name = String.Empty;
                name = Console.ReadLine();
                foreach (Product x in inventorySvc.Items.ToList()) {
                    if (x.Name == name) {
                        inventorySvc.RemoveItem(x);
                    }
                    else
                    {
                        Console.WriteLine("Error: Item does not exist.\n");
                    }
                }
            }

            //Shopping cart functions
            void addProduct()
            {
                Console.WriteLine("What is the name of the product you would like to add?\n");
                string productName = Console.ReadLine();
                int amount = 0;
                var productExists = false;
                foreach (Product x in inventorySvc.Items)
                {
                    if (x.Name == productName)
                    {
                        productExists = true;
                        Console.WriteLine($"How many would you like to add? (Current stock: {x.Amount})\n");
                        if (int.TryParse(Console.ReadLine(), out amount))
                        {
                            if (amount <= x.Amount && amount > 0)
                            {
                                shoppingCart.AddProduct(x, amount);
                                //no idea why the amount is messing up so much, but I am going to use a separate value from product to store it
                                //shoppingCart.UpdateAmount(x.Id, amount);
                                Console.WriteLine($"The current amount is {amount}\n");
                                Console.WriteLine($"ShowAmount has the value {shoppingCart.ShowAmount(x.Id)}\n");

                            }
                            else
                            {
                                Console.WriteLine("Error: Enter a valid amount.\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Enter a valid amount.\n");
                        }
                    }
                }
                if (!productExists)
                {
                    Console.WriteLine("Error: This product does not exist.\n");
                }
            }

            void listCart()
            {
                Console.WriteLine(shoppingCart.ToString());

            }
            void removeItemFromCart()
            {
                Console.WriteLine("Enter the name of the item you would like to remove: ");
                string name = String.Empty;
                int amount = 0;
                var itemExists = false;
                name = Console.ReadLine();
                foreach (Product x in shoppingCart.cartItems)
                {
                    if (x.Name == name)
                    {
                        itemExists = true;
                        Console.WriteLine("How many would you like to remove?\n");
                        if(int.TryParse(Console.ReadLine(), out amount))
                        {
                            if(amount <= x.Amount)
                            {
                                for(int i = 0; i < amount; i++)
                                {
                                    shoppingCart.RemoveProduct(x);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Cannot remove more items than in cart, removing all instances of {name} instead.\n");
                                for(int i = 0;i < x.Amount; i++)
                                {
                                    shoppingCart.RemoveProduct(x);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Please enter a valid number.\n");
                        }
                        
                    }
                }
                if (!itemExists)
                {
                    Console.WriteLine("Error: Could not find item to remove.\n");
                }
            }



            while (!isExit)
            {
                //Menu
                Console.WriteLine("Welcome to the inventory management system!\n");
                Console.WriteLine("What would you like to do?\n");
                Console.WriteLine("[1] Manage Inventory\n");
                Console.WriteLine("[2] Shop \n");
                Console.WriteLine("[3] Exit System\n");

                var menuChoice = Console.ReadLine();
                int menuNum = 0;
                try
                {
                    menuNum = int.Parse(menuChoice);
                }
                catch { Console.WriteLine("Error: Please enter a valid input (1, 2, 3).\n"); }
                switch (menuNum)
                {
                    case 1:
                        int invChoice;
                        do
                        {
                            Console.WriteLine("Welcome to the Inventory! What would you like to do?\n");
                            Console.WriteLine("[1] Add Product\n");
                            Console.WriteLine("[2] List all items\n");
                            Console.WriteLine("[3] Update an item\n");
                            Console.WriteLine("[4] Delete an item\n");
                            Console.WriteLine("[5] Exit Inventory");

                            if (int.TryParse(Console.ReadLine(), out invChoice))
                            {
                                switch (invChoice)
                                {
                                    case 1:
                                        addProduct_inv();
                                        break;
                                    case 2:
                                        listInventory();
                                        break;
                                    case 3:
                                        updateProduct();
                                        break;
                                    case 4:
                                        deleteProduct();
                                        break;
                                    case 5:
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: Please enter a valid input.\n");
                            }
                        } while (invChoice != 5);
                        break;
                    case 2:
                        int shopChoice;
                        var hasCheckedout = false;
                        do
                        {
                            Console.WriteLine("Welcome to the shop! What would you like to do?\n");
                            Console.WriteLine("[1] Add an item to the cart\n");
                            Console.WriteLine("[2] List all available items\n");
                            Console.WriteLine("[3] Show current cart\n");
                            Console.WriteLine("[4] Remove an item from the cart\n");
                            Console.WriteLine("[5] Checkout\n");
                            Console.WriteLine("[6] Exit (cart may not be saved)\n");
                            if (int.TryParse(Console.ReadLine(), out shopChoice))
                            {
                                switch (shopChoice)
                                {
                                    case 1:
                                        addProduct();
                                        break;
                                    case 2:
                                        listInventory();
                                        break;
                                    case 3:
                                        listCart();
                                        break;
                                    case 4:
                                        removeItemFromCart();
                                        break;
                                    case 5:
                                        Console.WriteLine(shoppingCart.Checkout());
                                        hasCheckedout = true;
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: Please enter a valid number.\n");
                            }
                        } while (!hasCheckedout && shopChoice != 6);
                        break;
                    case 3:
                        isExit = true;
                        break;
                }
            }
        }
    }
}