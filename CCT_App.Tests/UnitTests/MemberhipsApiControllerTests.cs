using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using CCT_App.Models;
using CCT_App.Controllers.Api;
using System.Data.Entity;
using System.Web.Http.Results;
using System.Data.Entity.Core.Objects;

namespace CCT_App.Tests.UnitTests
{
    public class MemberhipsApiControllerTests
    {
        /* TESTS FOR THE GET METHODS */

        [Fact]
        public void Get_Returns_Everything()
        {
            // Arrange
            // Fake database
            var mockRepository = new Mock<CCTEntities>();
            // Fake dbset that will return all our data
            var mockSet = new Mock<DbSet<Membership>>();
            var controller = new MembershipsController(mockRepository.Object);
            var data = new List<Membership>()
            {
                new Membership { MEMBERSHIP_ID = 123 },
                new Membership { MEMBERSHIP_ID = 321 }
            }.AsQueryable();

            // Some IQueryable Kung-Fu that I 60% understand. Seems like the only way to load the data into the fake dbset.
            mockSet.As<IQueryable<Membership>>().Setup(x => x.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Membership>>().Setup(x => x.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Membership>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Membership>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            
           // Tell fake database how to respond to the call that will be made.
            mockRepository
                .Setup(mockRepo => mockRepo.Memberships)
                .Returns(mockSet.Object);

            //Act
            var result = controller.Get();
            //Assert
            Assert.Equal(2, result.Count());
            Assert.IsType(typeof(List<Membership>), result);
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
                .Setup(repo => repo.Memberships.Find(id))
                .Returns(membership);

            //Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<Membership>;

            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Equal(id, contentresult.Content.MEMBERSHIP_ID);
            Assert.IsType(typeof(OkNegotiatedContentResult<Membership>), result);

        }
        [Fact]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            // Fake database
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            // Id that doesn't exist
            int id = 321;
            // Tell database how to respond to the call that will be made
            mockRepository
                .Setup(repo => repo.Memberships.Find(id))
                .Returns((Membership)null);

            //Act
            var result = controller.Get(id);

            //Assert
            //Asser that a not Found result was returned
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /* TESTS FOR THE CREATE METHOD */
        [Fact]
        public void Create_Returns_BadRequest_Given_Null_Membership()
        {
            // The method should not attempt to insert a null object
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership membership = null;

            //Act
            var result = controller.Post(membership);

            //Assert
             Assert.IsType(typeof(BadRequestResult), result);

        }

        [Fact]
        public void Create_Returns_Object_Given_Valid_Model()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var mockResult = new Mock<ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result>>();
            var controller = new MembershipsController(mockRepository.Object);
            var data = new List<ACTIVE_CLUBS_PER_SESS_ID_Result>
            {
                new ACTIVE_CLUBS_PER_SESS_ID_Result { ACT_CDE="TEST1" },
                new ACTIVE_CLUBS_PER_SESS_ID_Result { ACT_CDE="TEST2" }
            }.AsQueryable();

            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.Provider).Returns(data.Provider);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.Expression).Returns(data.Expression);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

            // The membership to be added has an activity code that matches one of the activities being offered.    
            var membership = new Membership { ACT_CDE = "TEST1", SESSION_CDE = "TEST_SESS" };
            mockRepository
                .Setup(repo => repo.ACTIVE_CLUBS_PER_SESS_ID("TEST_SESS"))
                .Returns(mockResult.Object);
            mockRepository
                .Setup(repo => repo.Memberships.Add(membership))
                .Returns(membership);
            mockRepository
                .Setup(repo => repo.SaveChanges());
            // Act
            var result = controller.Post(membership);
            var contentresult = result as CreatedNegotiatedContentResult<Membership>;

            //Assert
            Assert.IsType(typeof(CreatedNegotiatedContentResult<Membership>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }

        /* TESTS FOR THE UPDATE METHOD */

        [Fact]
        public void Update_Returns_Bad_Request_Given_Model_With_ID_Mismatch()
        {
            // Method should not attempt to update if there is an ID mismatch
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
            // Method should not attempt to update object is null
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership membership = null;

            // Act
            var result = controller.Put(1, membership);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);

        }

        [Fact]
        public void Update_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership membership = new Membership { MEMBERSHIP_ID = 123 };
            int id = 123;
            mockRepository
                .Setup(repo => repo.Memberships.Find(id))
                .Returns((Membership)null);

            // Act
            var result = controller.Put(id, membership);

            // Assert
            Assert.IsType(typeof(NotFoundResult), result);

        }

        // TEST IS FAILING. Can't properly mock the database.Entry method.
        [Fact]
        public void Update_Returns_Created_Given_Valid_Parameters()
        {
            //Arrange
            var mockRepo = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepo.Object);
            int id = 123;
            var newmembership = new Membership { MEMBERSHIP_ID = 123 , DESCRIPTION = "NEW" };
            var oldmebership = new Membership { MEMBERSHIP_ID = 123, DESCRIPTION = "OLD" };
            mockRepo.Setup(repo => repo.Memberships.Find(id)).Returns(oldmebership);
            
            //Act
            var result = controller.Put(id, newmembership);
            var contentresult = result as NegotiatedContentResult<Membership>;

            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Equal(System.Net.HttpStatusCode.Created, contentresult.StatusCode);

        }


        /* TESTS FOR THE DELETE METHOD */
       
        [Fact]
        public void Delete_Returns_Bad_Request_Given_NonExistent_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new MembershipsController(mockRepository.Object);
            int id = 132;
            mockRepository
                .Setup(x => x.Memberships.Find(id))
                .Returns((Membership)null);

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
            var controller = new MembershipsController(mockRepository.Object);
            int id = 123;
            var membership = new Membership
            {
                MEMBERSHIP_ID = id
            };
            mockRepository
                .Setup(x => x.Memberships.Find(id))
                .Returns(membership);
            mockRepository
                .Setup(x => x.Memberships.Remove(membership))
                .Returns(membership);

            // Act
            var result = controller.Delete(id);


            //Assert
            Assert.IsType(typeof(OkResult), result);

        }
    }
}
