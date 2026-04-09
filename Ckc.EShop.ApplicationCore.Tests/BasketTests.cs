using Ckc.EShop.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ckc.EShop.ApplicationCore.Tests
{
    public class BasketTests
    {
        [Fact]
        public void Basket_InheritsIdFromBaseEntity()
        {
            // Arrange
            var basket = new Basket();

            // Act
            basket.Id = 5;

            // Assert
            Assert.Equal(5, basket.Id);
        }

        [Fact]
        public void Basket_BuyerIdCanBeSetAndRetrieved()
        {
            // Arrange
            var basket = new Basket();
            var testBuyerId = "user123@example.com";

            // Act
            basket.BuyerID = testBuyerId;

            // Assert
            Assert.Equal(testBuyerId, basket.BuyerID);
        }

        [Fact]
        public void Basket_ItemsCollectionIsInitializedEmpty()
        {
            // Arrange & Act
            var basket = new Basket();

            // Assert
            Assert.NotNull(basket.Items);
            Assert.IsType<List<BasketItems>>(basket.Items);
            Assert.Empty(basket.Items);
        }

        [Fact]
        public void Basket_AddItem_WhenItemDoesNotExist_CreatesNewBasketItem()
        {
            // Arrange
            var basket = new Basket { BuyerID = "user1" };
            var catalogItemId = 10;
            var unitPrice = 9.99m;
            var quantity = 2;

            // Act
            basket.AddItem(catalogItemId, unitPrice, quantity);

            // Assert
            Assert.Single(basket.Items);
            var basketItem = basket.Items.First();
            Assert.Equal(catalogItemId, basketItem.CatalogItemId);
            Assert.Equal(unitPrice, basketItem.UnitPrice);
            Assert.Equal(quantity, basketItem.Quantity);
        }

        [Fact]
        public void Basket_AddItem_WhenItemDoesNotExist_WithDefaultQuantity_SetsQuantityToOne()
        {
            // Arrange
            var basket = new Basket { BuyerID = "user1" };
            var catalogItemId = 10;
            var unitPrice = 9.99m;

            // Act
            basket.AddItem(catalogItemId, unitPrice); // No quantity specified

            // Assert
            Assert.Single(basket.Items);
            var basketItem = basket.Items.First();
            Assert.Equal(catalogItemId, basketItem.CatalogItemId);
            Assert.Equal(unitPrice, basketItem.UnitPrice);
            Assert.Equal(1, basketItem.Quantity); // Default quantity
        }

        [Fact]
        public void Basket_AddItem_WhenItemAlreadyExists_IncreasesQuantity()
        {
            // Arrange
            var basket = new Basket { BuyerID = "user1" };
            var catalogItemId = 10;
            var unitPrice = 9.99m;
            var initialQuantity = 2;
            var additionalQuantity = 3;

            // Add item first time
            basket.AddItem(catalogItemId, unitPrice, initialQuantity);

            // Act
            basket.AddItem(catalogItemId, unitPrice, additionalQuantity);

            // Assert
            Assert.Single(basket.Items); // Still only one item
            var basketItem = basket.Items.First();
            Assert.Equal(catalogItemId, basketItem.CatalogItemId);
            Assert.Equal(unitPrice, basketItem.UnitPrice);
            Assert.Equal(initialQuantity + additionalQuantity, basketItem.Quantity); // 2 + 3 = 5
        }

        [Fact]
        public void Basket_AddItem_WithSameItemTwice_UsesSameBasketItemInstance()
        {
            // Arrange
            var basket = new Basket { BuyerID = "user1" };
            var catalogItemId = 10;
            var unitPrice = 9.99m;

            // Act
            basket.AddItem(catalogItemId, unitPrice, 1);
            var firstItem = basket.Items.First();
            basket.AddItem(catalogItemId, unitPrice, 1);
            var secondItem = basket.Items.First();

            // Assert
            Assert.Same(firstItem, secondItem); // Should be the same instance
            Assert.Equal(2, firstItem.Quantity); // Quantity should be 2
        }

        [Fact]
        public void Basket_AddItem_DifferentItems_CreatesSeparateBasketItems()
        {
            // Arrange
            var basket = new Basket { BuyerID = "user1" };
            var catalogItemId1 = 10;
            var catalogItemId2 = 20;
            var unitPrice = 9.99m;

            // Act
            basket.AddItem(catalogItemId1, unitPrice, 1);
            basket.AddItem(catalogItemId2, unitPrice, 1);

            // Assert
            Assert.Equal(2, basket.Items.Count);
            var item1 = basket.Items.First(i => i.CatalogItemId == catalogItemId1);
            var item2 = basket.Items.First(i => i.CatalogItemId == catalogItemId2);
            Assert.NotSame(item1, item2); // Different instances
            Assert.Equal(catalogItemId1, item1.CatalogItemId);
            Assert.Equal(catalogItemId2, item2.CatalogItemId);
        }

        [Fact]
        public void Basket_AddItem_WithZeroQuantity_StillAddsItem()
        {
            // Arrange
            var basket = new Basket { BuyerID = "user1" };
            var catalogItemId = 10;
            var unitPrice = 9.99m;

            // Act
            basket.AddItem(catalogItemId, unitPrice, 0);

            // Assert
            Assert.Single(basket.Items);
            var basketItem = basket.Items.First();
            Assert.Equal(catalogItemId, basketItem.CatalogItemId);
            Assert.Equal(unitPrice, basketItem.UnitPrice);
            Assert.Equal(0, basketItem.Quantity);
        }
    }
}