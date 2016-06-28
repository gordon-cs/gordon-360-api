using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CCT_App.Controllers.Api;
using CCT_App.Services;
using CCT_App.Models;
using System.Web.Http.Results;

namespace CCT_App.Tests.ControllerUnitTests
{
    /*  Test for Roles Controller */
    public class Roles
    {
        /* TEST FOR GET METHODS */
        [Fact]
        public void GetAll_With_Returns_Empty_List()
        {

            // Arrange
            var theservice = new Mock<IRoleService>();
            var controller = new RolesController(theservice.Object);
            theservice
                .Setup(x => x.GetAll())
                .Returns(new List<PART_DEF>());

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<PART_DEF>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<PART_DEF>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Empty(contentresult.Content);

        }

        [Fact]
        public void GetAll_Returns_With_Popluated_List()
        {
            // Arrange
            var theservice = new Mock<IRoleService>();
            var controller = new RolesController(theservice.Object);
            var data = new List<PART_DEF> { new PART_DEF { }, new PART_DEF { }, new PART_DEF { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<PART_DEF>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<PART_DEF>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.NotEmpty(contentresult.Content);
            Assert.Equal(3, contentresult.Content.Count());

        }

        [Fact]
        public void Get_Returns_Not_Found_Given_Non_Existent_Id()
        {
            // Arrange
            var theservice = new Mock<IRoleService>();
            var controller = new RolesController(theservice.Object);
            var id = "id";
            theservice
                .Setup(x => x.Get(id));

            // Act
            var result = controller.Get(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public void Get_Returns_Bad_Request_With_Empty_id()
        {
            // Arrange
            var theservice = new Mock<IRoleService>();
            var controller = new RolesController(theservice.Object);
            var id = string.Empty;

            // Act
            var result = controller.Get(id);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void Get_Returns_Ok_With_Object_Model_Given_Valid_id()
        {
            // Arrange
            var theservice = new Mock<IRoleService>();
            var controller = new RolesController(theservice.Object);
            var data = new PART_DEF { };
            var id = "id";
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<PART_DEF>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<PART_DEF>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
                /* TEST FOR MISC METHODS */
    }
}
