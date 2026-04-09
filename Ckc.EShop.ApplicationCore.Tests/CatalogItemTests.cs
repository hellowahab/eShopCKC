using Ckc.EShop.ApplicationCore.Entities;
using Xunit;

namespace Ckc.EShop.ApplicationCore.Tests
{
    public class CatalogItemTests
    {
        [Fact]
        public void CatalogItem_InheritsIdFromBaseEntity()
        {
            // Arrange
            var item = new CatalogItem();

            // Act
            item.Id = 10;

            // Assert
            Assert.Equal(10, item.Id);
        }

        [Fact]
        public void CatalogItem_CanSetAndGet_Name()
        {
            // Arrange
            var item = new CatalogItem();
            var testName = "Test Product Name";

            // Act
            item.Name = testName;

            // Assert
            Assert.Equal(testName, item.Name);
        }

        [Fact]
        public void CatalogItem_CanSetAndGet_Description()
        {
            // Arrange
            var item = new CatalogItem();
            var testDescription = "This is a test product description";

            // Act
            item.Description = testDescription;

            // Assert
            Assert.Equal(testDescription, item.Description);
        }

        [Fact]
        public void CatalogItem_CanSetAndGet_Price()
        {
            // Arrange
            var item = new CatalogItem();
            var testPrice = 29.99m;

            // Act
            item.Price = testPrice;

            // Assert
            Assert.Equal(testPrice, item.Price);
        }

        [Fact]
        public void CatalogItem_CanSetAndGet_PictureUri()
        {
            // Arrange
            var item = new CatalogItem();
            var testUri = "http://example.com/image.jpg";

            // Act
            item.PictureUri = testUri;

            // Assert
            Assert.Equal(testUri, item.PictureUri);
        }

        [Fact]
        public void CatalogItem_CanSetAndGet_CatalogTypeId()
        {
            // Arrange
            var item = new CatalogItem();
            var testTypeId = 7;

            // Act
            item.CatalogTypeId = testTypeId;

            // Assert
            Assert.Equal(testTypeId, item.CatalogTypeId);
        }

        [Fact]
        public void CatalogItem_CanSetAndGet_CatalogBrandId()
        {
            // Arrange
            var item = new CatalogItem();
            var testBrandId = 3;

            // Act
            item.CatalogBrandId = testBrandId;

            // Assert
            Assert.Equal(testBrandId, item.CatalogBrandId);
        }

        [Fact]
        public void CatalogItem_CanSetNavigationProperties()
        {
            // Arrange
            var item = new CatalogItem();
            var catalogType = new CatalogType { Id = 1, Type = "Electronics" };
            var catalogBrand = new CatalogBrand { Id = 2, Brand = "TechCorp" };

            // Act
            item.CatalogType = catalogType;
            item.CatalogBrand = catalogBrand;

            // Assert
            Assert.Same(catalogType, item.CatalogType);
            Assert.Same(catalogBrand, item.CatalogBrand);
            // Note: Foreign keys are not automatically set when navigation properties are set
            // This is expected behavior for this entity without change tracking
        }

        [Fact]
        public void CatalogItem_NavigationPropertiesCanBeNull()
        {
            // Arrange
            var item = new CatalogItem();

            // Act & Assert
            Assert.Null(item.CatalogType);
            Assert.Null(item.CatalogBrand);
        }

        [Fact]
        public void CatalogItem_CanSetAllPropertiesAtOnce()
        {
            // Arrange
            var item = new CatalogItem();
            var testId = 100;
            var testName = "Premium Product";
            var testDescription = "High-quality product";
            var testPrice = 99.99m;
            var testPictureUri = "http://example.com/premium.jpg";
            var testTypeId = 5;
            var testBrandId = 8;

            // Act
            item.Id = testId;
            item.Name = testName;
            item.Description = testDescription;
            item.Price = testPrice;
            item.PictureUri = testPictureUri;
            item.CatalogTypeId = testTypeId;
            item.CatalogBrandId = testBrandId;

            // Assert
            Assert.Equal(testId, item.Id);
            Assert.Equal(testName, item.Name);
            Assert.Equal(testDescription, item.Description);
            Assert.Equal(testPrice, item.Price);
            Assert.Equal(testPictureUri, item.PictureUri);
            Assert.Equal(testTypeId, item.CatalogTypeId);
            Assert.Equal(testBrandId, item.CatalogBrandId);
        }
    }
}