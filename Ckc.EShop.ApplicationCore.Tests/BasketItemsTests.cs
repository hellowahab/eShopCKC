using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ckc.EShop.ApplicationCore.Entities;
using Xunit;

namespace Ckc.EShop.ApplicationCore.Tests
{
    public class BasketItemsTests
    {
        [Fact]
        public void BasketItems_ShouldHaveDefaultValues()
        {
            var basketItem = new BasketItems();

            Assert.Equal(0, basketItem.Id);
            Assert.Equal(0, basketItem.UnitPrice);
            Assert.Equal(0, basketItem.Quantity);
            Assert.Equal(0, basketItem.CatalogItemId);
        }

        [Fact]
        public void BasketItems_ShouldSetPropertiesCorrectly()
        {
            var basketItem = new BasketItems
            {
                Id = 1,
                UnitPrice = 10.5m,
                Quantity = 2,
                CatalogItemId = 5
            };

            Assert.Equal(1, basketItem.Id);
            Assert.Equal(10.5m, basketItem.UnitPrice);
            Assert.Equal(2, basketItem.Quantity);
            Assert.Equal(5, basketItem.CatalogItemId);
        }
    }
}