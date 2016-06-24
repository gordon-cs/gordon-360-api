using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCT_App.Controllers.Api;
using CCT_App.Models;
using Xunit;
using Moq;
using System.Collections;
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
            var allActivities = new Mock<DbSet<ACT_CLUB_DEF>>();
            allActivities.Object.Add(new ACT_CLUB_DEF { ACT_CDE = "TEST1" });
            allActivities.Object.Add(new ACT_CLUB_DEF { ACT_CDE = "TEST2" });

            var controller = new ActivitiesController(mockRepository.Object);

            
            mockRepository
                .Setup(mockRepo => mockRepo.ACT_CLUB_DEF)
                .Returns(allActivities.Object);
            //Act
            var result = controller.Get();
            //Assert
            Assert.Equal("2", result.Count().ToString());
            Assert.IsType(typeof(DbSet<ACT_CLUB_DEF>), result);
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
                .SetReturnsDefault(activity);
             
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

            //Act
            var result = controller.Get(id);

            //Assert
            Assert.IsType(typeof(NotFoundResult),result);
        }
 
        [Fact]
        public void Get_By_ID_Returns_Bad_Request_Given_Null_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new ActivitiesController(mockRepository.Object);
            string nullId = null;

            //Act
            var result = controller.Get(nullId);

            //Assert
            Assert.IsType(typeof(BadRequestResult),result);
        }
 
    }
}
