using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Console_InvManagement.Models;

namespace Console_InvManagement.Services
{
    public class InventoryServiceProxy
    {
        private List<Product> inventory;
        private InventoryServiceProxy()
        {
            inventory = new List<Product>
           {
               //new Product{Name = "Apple", Description = "Red fruit.", Price = (double)0.99, Amount = 20}
           };
        }

        //Don't think that I need these yet, but may as well add them now
        private static InventoryServiceProxy? instance;
        private static object? instanceLock = new object();
        public static InventoryServiceProxy Current
        {

            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InventoryServiceProxy();
                    }
                }
                return instance;
            }
        }

        public ReadOnlyCollection<Product>? Items
        {
            get
            {
                return inventory?.AsReadOnly();
            }
        }
        //=================== Inventory Management =====================

        public void AddItem(Product newItem)
        {
            //check if item is already in system
            if (inventory.Any(item => item.Id.Equals(newItem.Id)))
            {
                //if already in system, add its stock value to existing stock
                inventory.First(item => item.Id.Equals(newItem.Id)).Amount += newItem.Amount;
            }
            else
            {
                inventory.Add(newItem);
            }
        }
        
        public void UpdateItem(Product item1, string newName, string newDescription, double newPrice, int newStock)
        {
            item1.Name = newName;
            item1.Description = newDescription;
            item1.Price = newPrice;
            item1.Amount = newStock;
        }

        public void RemoveItem(Product trash)
        {
            var removalItem = inventory.FirstOrDefault(item => item.Id.Equals(trash.Id));
            if (removalItem != null)
            {
                inventory.Remove(removalItem);
            }
        }
        private Product GetProductById(Guid id)
        {
            return inventory.FirstOrDefault(item => item.Id.Equals(id));
        }

        public bool ContainsItem(Guid id)
        {
            return GetProductById(id) != null;
        }
    }
}
    