using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ckc.EShop.ApplicationCore.Entities.OrderAggregate
{
    public class CatalogItemOrdered  // Value object
    {
        public int CatalogItemId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUri { get; private set; }

        private CatalogItemOrdered() { }

        public CatalogItemOrdered(int catalogItemId, string productName,
            string pictureUri)
        {
            CatalogItemId = catalogItemId;
            ProductName = productName;
            PictureUri = pictureUri;
        
        }
    }
}
