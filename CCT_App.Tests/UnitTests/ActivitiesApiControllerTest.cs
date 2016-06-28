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
            var mockRepository = new Mock<CCTEntities>() { DefaultValue = DefaultValue.Mock };
            var mockSet = mockRepository.Object.ACT_CLUB_DEF;
            var controller = new ActivitiesController(mockRepository.Object);

            mockRepository
                .Setup(repo => repo.ACT_CLUB_DEF)
                .Returns(mockSet);
                          
            //Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<List<ACT_CLUB_DEF>>;

            //Assert
            Assert.IsType(typeof(OkNegotiatedContentResult<List<ACT_CLUB_DEF>>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
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
                .Setup(repo => repo.ACT_CLUB_DEF.Find(id));
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
