using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CCT_App.Models;
using CCT_App.Controllers.Api;
using System.Data.Entity;
using System.Web.Http.Results;

namespace CCT_App.Tests.UnitTests
{
    public class MemberhipsApiControllerTests
    {
        /* TESTS FOR THE GET METHODS */

        [Fact]
        public void Get_Returns_Everything()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var allmemberships = new Mock<DbSet<Membership>>();
            allmemberships.Object.Add(new Membership { MEMBERSHIP_ID = 123 });
            allmemberships.Object.Add(new Membership { MEMBERSHIP_ID = 123 });

            var controller = new MembershipsController(mockRepository.Object);


            mockRepository
                .Setup(mockRepo => mockRepo.Memberships)
                .Returns(allmemberships.Object);
            //Act
            var result = controller.Get();
            //Assert
            Assert.Equal("2", result.Count().ToString());
            Assert.IsType(typeof(DbSet<Membership>), result);
        }
        [Fact]
        public void Get_By_ID_Returns_Correctly_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            int id = 123;
            var membership = new Membership { MEMBERSHIP_ID = id };
            mockRepository
                .SetReturnsDefault(membership);

            //Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<Membership>;

            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Equal(id, contentresult.Content.MEMBERSHIP_ID);

        }
        [Fact]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            int id = 321;

            //Act
            var result = controller.Get(id);

            //Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }
/* This method doesn't apply here because an int is not nullable
        [Fact]
        public void Get_By_ID_Returns_Bad_Request_Given_Null_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            string nullId = null;

            //Act
            var result = controller.Get(nullId);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);
        }
*/
        /* TESTS FOR THE CREATE METHOD */
        [Fact]
        public void Create_Returns_BadRequest_Given_Null_Membership()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership membership = null;

            //Act
            var result = controller.Post(membership);

            //Assert
             Assert.IsType(typeof(BadRequestResult), result);

        }

        [FactAttribute]
        public void Create_Returns_Object_Given_Valid_Model()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            var membership = new Membership { ACT_CDE = "123" };

            // Act
            var result = controller.Post(membership);
            var contentresult = result as CreatedAtRouteNegotiatedContentResult<Membership>;
            
            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.RouteName);
            Assert.Equal("Memberships", contentresult.RouteName);
        }

        /* TESTS FOR THE UPDATE METHOD */
/* Since id is an int, there is no way it can be null
        [Fact]
        public void Update_Returns_Bad_Request_Given_No_Provided_ID()
        {
            // Arrange
            var mockRepository = new Mock<IActivityRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            var testActivity = new Activity();

            // Act
            var result = controller.Put(null, testActivity);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

        }
*/
        [Fact]
        public void Update_Returns_Bad_Request_Given_Model_With_ID_Mismatch()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            var membership = new Membership
            {
                MEMBERSHIP_ID = 1 // ID Mismatch!!
            };

            // Act
            // Try to update Object with ID of 1, with Object with ID of 2, should not work.
            var result = controller.Put(2, membership);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);
        }

        [Fact]
        public void Update_Returns_Bad_Request_Given_Null_object()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership membership = null;

            // Act
            var result = controller.Put(1, membership);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);

        }


        /* TESTS FOR THE DELETE METHOD */
/* Membership ID can't be null
        [Fact]
        public void Delete_Returns_Bad_Request_Given_Null_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            string nullID = null;

            // Act
            var result = controller.Delete(nullID);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

        }
*/
        [Fact]
        public void Delete_Returns_Bad_Request_Given_NonExistent_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            int id = 132;

            // Act
            var result = controller.Delete(id);

            //Assert
            Assert.IsType(typeof(NotFoundResult), result);

        }

        [Fact]
        public void Delete_Returns_Ok_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var membership = new Membership
            {
                MEMBERSHIP_ID = 123
            };
            mockRepository
                .Setup(x => x.Memberships.Find(123))
                .Returns(membership);
            mockRepository
                .Setup(x => x.Memberships.Remove(membership))
                .Returns(membership);
            var controller = new MembershipsController(mockRepository.Object);

            // Act
            var result = controller.Delete(123);


            //Assert
            Assert.IsType(typeof(OkResult), result);

        }
    }
}
