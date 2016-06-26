using System;
using System.Collections.Generic;
using System.Linq;
using CCT_App.Controllers.Api;
using CCT_App.Models;
using Xunit;
using Moq;
using System.Data.Entity;
using System.Web.Http.Results;

namespace CCT_App.Tests.UnitTests
{
    public class ActivitiesApiControllerTest
    {

        /* TESTS FOR THE GET METHODS */
         
        [Fact]
        public void Get_Returns_Everything()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var mockSet = new Mock<DbSet<ACT_CLUB_DEF>>();
            var controller = new ActivitiesController(mockRepository.Object);
            var data = new List<ACT_CLUB_DEF>
            {
                new ACT_CLUB_DEF { ACT_CDE = "TEST1" },
                new ACT_CLUB_DEF { ACT_CDE = "TEST2" }
            }.AsQueryable();

           
            mockSet.As<IQueryable<ACT_CLUB_DEF>>().Setup(x => x.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<ACT_CLUB_DEF>>().Setup(x => x.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<ACT_CLUB_DEF>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<ACT_CLUB_DEF>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

            mockRepository
                .Setup(repo => repo.ACT_CLUB_DEF)
                .Returns(mockSet.Object);
                          
            //Act
            var result = controller.Get();

            //Assert
            Assert.Equal(2, result.Count());
            Assert.IsType(typeof(List<ACT_CLUB_DEF>), result);
        }
        [Fact]
        public void Get_By_ID_Returns_Correctly_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new ActivitiesController(mockRepository.Object);
            string id = "valid_id";
            var activity = new ACT_CLUB_DEF { ACT_CDE="valid_id" };
            mockRepository
                .Setup(mockRepo => mockRepo.ACT_CLUB_DEF.Find(id))
                .Returns(activity);
             
            //Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<ACT_CLUB_DEF>;

            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Equal("valid_id", contentresult.Content.ACT_CDE);

        }
        [Fact]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new ActivitiesController(mockRepository.Object);
            string id = "id-that-doesn't-exist";
            mockRepository
                .Setup(repo => repo.ACT_CLUB_DEF.Find(id))
                .Returns((ACT_CLUB_DEF)null);
            //Act
            var result = controller.Get(id);

            //Assert
            Assert.IsType(typeof(NotFoundResult),result);
        }
 
        [Fact]
        public void Get_By_ID_Returns_Bad_Request_Given_Empty_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new ActivitiesController(mockRepository.Object);
            string emptyID = String.Empty;

            //Act
            var result = controller.Get(emptyID);

            //Assert
            Assert.IsType(typeof(BadRequestResult),result);
        }
 
    }
}
