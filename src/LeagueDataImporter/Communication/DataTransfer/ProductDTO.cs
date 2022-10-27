using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer
{
    public class ProductDTO : BaseDTO
    {
        public string Name => "TestProduct";
        public double Price => 3.99;
        public int Quantity => 5;
    }
}
