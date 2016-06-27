using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using CCT_App.Models;
using CCT_App.Controllers.Api;
using System.Data.Entity;
using System.Web.Http.Results;

namespace CCT_App.Tests.UnitTests
{
    public class RolesApiControllerTest
    {
        /* TESTS FOR THE GET METHODS */

        [Fact]
        public void Get_Returns_Everything()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities> { DefaultValue = DefaultValue.Mock };
            var mockSet = mockRepository.Object.PART_DEF;
            var controller = new RolesController(mockRepository.Object);

            mockRepository
                .Setup(mockRepo => mockRepo.PART_DEF)
                .Returns(mockSet);

            //Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<List<PART_DEF>>;
            //Assert
            Assert.IsType(typeof(OkNegotiatedContentResult<List<PART_DEF>>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }

        [Fact]
        public void Get_By_ID_Returns_Correctly_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new RolesController(mockRepository.Object);
            string id = "valid_id";
            var role = new PART_DEF { PART_CDE = id };
            mockRepository
                .Setup(repo => repo.PART_DEF.Find(id))
                .Returns(role);

            //Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<PART_DEF>;

            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Equal(id, contentresult.Content.PART_CDE);

        }
        [Fact]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new RolesController(mockRepository.Object);
            string id = "id-that-doesn't-exist";
            mockRepository
                .Setup(repo => repo.PART_DEF.Find(id));

            //Act
            var result = controller.Get(id);

            //Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }

        [Fact]
        public void Get_By_ID_Returns_Bad_Request_Given_Empty_ID_String()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new RolesController(mockRepository.Object);
            string emptyId = String.Empty;

            //Act
            var result = controller.Get(emptyId);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);
        }

    }
}
