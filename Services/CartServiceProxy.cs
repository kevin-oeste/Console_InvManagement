using Console_InvManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Console_InvManagement.Services
{
    
    public class CartServiceProxy
        {
        private static CartServiceProxy? instance;
        private static object? instanceLock = new object();
        public static CartServiceProxy Current
        {

            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CartServiceProxy();
                    }
                }
                return instance;
            }
        }
        public ReadOnlyCollection<Product>? cartItems
        {
            get
            {
                return shoppingCart?.AsReadOnly();
            }
        }
        private List<Product> shoppingCart;
            private double taxRate = 0.07;
        public double Subtotal
        {
            get
            {
                double subtotal = 0;
                foreach(Product x in shoppingCart)
                {
                    subtotal += (ShowAmount(x.Id) * x.Price);
                }
                return subtotal;
            }
        }
        public double Taxes
        {
            get
            {
                return taxRate * Subtotal;
            }
        }
        public double Total
        {
            get
            {
                return Subtotal + Taxes;
            }
        }

        public string Receipt
        {
            get
            {
                var receipt = "Thank you for shopping with us!\n";
                foreach (var item in shoppingCart) {
                    receipt += ($"{ShowAmount(item.Id)} {item.Name}\t{item.Price}\n");
                }
                receipt += $"\n\nSubtotal: {Subtotal:C}\nTaxes: {Total:C}";
                return receipt;
            }
        }
        public string Checkout() {
            return Receipt;
        }
        //This is a workaround for my Amount value in product messing up in the shoppingCart list (it is the same amount as total stock)
        private List<Tuple<Guid, int>> cartAmounts = new List<Tuple<Guid, int>> { };

        public void addTuple(Guid id, int amount)
        {
            //this function shouldn't be needed in theory, but UpdateAmount isn't working properly
            var newEntry = Tuple.Create(id, amount);
            cartAmounts.Add(newEntry);
        }

        public int ShowAmount(Guid id1)
        {
            foreach (Tuple<Guid, int> item in cartAmounts)
            {
                if (item.Item1 == id1)
                {
                    return item.Item2;
                }
            }
            return 0;
        }
        private CartServiceProxy()
        {
            shoppingCart = new List<Product>{
               //new Product{Name = "Apple", Description = "Red fruit.", Price = (float)0.99, Amount = 1}
            };
        }
        public void AddProduct(Product newProduct, int addAmount)
        {
            //check if item is already in system
            if (shoppingCart.Any(item => item.Id.Equals(newProduct.Id)))
            {
                //if already in system, add its stock value to existing stock
                shoppingCart.First(item => item.Id.Equals(newProduct.Id)).Amount += newProduct.Amount;
                foreach (Tuple<Guid, int> item in cartAmounts)
                {
                    if (item.Item1 == newProduct.Id)
                    {
                        UpdateAmount(newProduct.Id, (item.Item2 + newProduct.Amount));
                        Console.WriteLine($"The amount added to the tuple is {(item.Item2 + newProduct.Amount)}\n");
                    }
                }
            }
            else
            {
                //var newerProduct = Clone(newProduct);
                shoppingCart.Add(newProduct);
                UpdateAmount(newProduct.Id, addAmount);
            }
        }
        public Product Clone(Product p)
        {
            var newProduct = new Product();
            newProduct.Name = p.Name;
            newProduct.Description = p.Description;
            newProduct.Price = p.Price;
            newProduct.Amount = p.Amount;
            return newProduct;
        }
        public void UpdateAmount(Guid Id1, int amount)
        {
            foreach(Product x in shoppingCart)
            {
                if(x.Id == Id1)
                {
                    var newEntry = Tuple.Create(x.Id, amount);
                    cartAmounts.Add(newEntry);
                }
            }
        }
        public void RemoveProduct(Product trash)
        {
            var removalItem = shoppingCart.FirstOrDefault(item => item.Id.Equals(trash.Id));
            if (removalItem != null)
            {
                foreach (Tuple<Guid, int> x in cartAmounts)
                {
                    if(x.Item1 == trash.Id)
                    {
                        UpdateAmount(x.Item1, (x.Item2 - trash.Amount));
                        if ( x.Item2 <= 0)
                        {
                            shoppingCart.Remove(trash);
                        }
                    }
                }
                
            }
        }
        private Product GetProductById(Guid id)
        {
            return shoppingCart.FirstOrDefault(product => product.Id.Equals(id));
        }

        public bool ContainsItem(Guid id)
        {
            return GetProductById(id) != null;
        }

        public override string ToString()
        {
            string output = String.Empty;
            if (shoppingCart.Any())
            {
                foreach (Product product in shoppingCart)
                {
                    //more workarounds -_-
                    output += ($"{ShowAmount(product.Id)} {product.Name}\t{product.Price}\n");
                }
            }
            return output;
        }
    }
}
