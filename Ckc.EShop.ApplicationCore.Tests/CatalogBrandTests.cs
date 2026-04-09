using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ckc.EShop.ApplicationCore.Entities;
using Xunit;

namespace Ckc.EShop.ApplicationCore.Tests
{
    public class CatalogBrandTests
    {
        [Fact]
        public void CatalogBrand_ShouldHaveDefaultValues()
        {
            var brand = new CatalogBrand();

            Assert.Equal(0, brand.Id);
            Assert.Null(brand.Brand);
        }

        [Fact]
        public void CatalogBrand_ShouldSetPropertiesCorrectly()
        {
            var brand = new CatalogBrand
            {
                Id = 1,
                Brand = "Nike"
            };

            Assert.Equal(1, brand.Id);
            Assert.Equal("Nike", brand.Brand);
        }

        [Fact]
        public void CatalogBrand_ShouldAllowEmptyStringBrand()
        {
            var brand = new CatalogBrand
            {
                Id = 1,
                Brand = ""
            };

            Assert.Equal(1, brand.Id);
            Assert.Equal("", brand.Brand);
        }

        [Fact]
        public void CatalogBrand_ShouldAllowNullBrand()
        {
            var brand = new CatalogBrand
            {
                Id = 1,
                Brand = null
            };

            Assert.Equal(1, brand.Id);
            Assert.Null(brand.Brand);
        }
    }
}