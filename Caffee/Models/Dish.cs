using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffee.Models
{
    public class Dish
    {
        public int ID {  get; set; }
        public string? Name { get; set; }
        public Category? Category { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
    }
}