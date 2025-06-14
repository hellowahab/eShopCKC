using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ckc.EShop.ApplicationCore.Entities
{
    public class BasketItems : BaseEntity
    {
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int CatalogItemId { get; set; }

    }
}
