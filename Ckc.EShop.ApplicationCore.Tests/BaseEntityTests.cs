using Ckc.EShop.ApplicationCore.Entities;
using Xunit;

namespace Ckc.EShop.ApplicationCore.Tests
{
    public class BaseEntityTests
    {
        [Fact]
        public void BaseEntity_IdCanBeSetAndRetrieved()
        {
            // Arrange
            var entity = new BaseEntity();
            var testId = 42;

            // Act
            entity.Id = testId;

            // Assert
            Assert.Equal(testId, entity.Id);
        }

        [Fact]
        public void BaseEntity_IdDefaultsToZero()
        {
            // Arrange & Act
            var entity = new BaseEntity();

            // Assert
            Assert.Equal(0, entity.Id);
        }

        [Fact]
        public void BaseEntity_IdCanBeSetToNegative()
        {
            // Arrange
            var entity = new BaseEntity();
            var testId = -5;

            // Act
            entity.Id = testId;

            // Assert
            Assert.Equal(testId, entity.Id);
        }
    }
}