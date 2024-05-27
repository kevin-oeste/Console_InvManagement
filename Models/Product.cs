using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_InvManagement.Models
{
    public class Product
    {
        //mostly the same as in inventory, with a few differences
        public Guid Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price {  get; set; }
        public int Amount { get; set; }

        //based on shopping cart example 3
        public Product()
        {
            Id = Guid.NewGuid();
        }
        public Product(Guid id)
        {
            this.Id = id;
        }
        public override string ToString()
        {
            return $"{Amount} {Name}\t{Price}";
        }
        public virtual string ToString(bool forInventory)
        {
            if (forInventory)
            {

                return $"{Name}: {Description} - ${Price} ({Amount} in stock)";

            }
            return ToString();
        }
    }
}
