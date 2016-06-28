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
    /*  Test for Sessioncs Controller */
    public class Sessions
    {
        /* TEST FOR GET METHODS */
        [Fact]
        public void GetAll_With_Returns_Empty_List()
        {

            // Arrange
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
            theservice
                .Setup(x => x.GetAll())
                .Returns(new List<CM_SESSION_MSTR>());

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<CM_SESSION_MSTR>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<CM_SESSION_MSTR>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Empty(contentresult.Content);

        }

        [Fact]
        public void GetAll_Returns_With_Popluated_List()
        {
            // Arrange
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
            var data = new List<CM_SESSION_MSTR> { new CM_SESSION_MSTR { }, new CM_SESSION_MSTR { }, new CM_SESSION_MSTR { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<CM_SESSION_MSTR>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<CM_SESSION_MSTR>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.NotEmpty(contentresult.Content);
            Assert.Equal(3, contentresult.Content.Count());

        }

        [Fact]
        public void Get_Returns_Not_Found_Given_Non_Existent_Id()
        {
            // Arrange
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
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
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
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
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
            var data = new CM_SESSION_MSTR { };
            var id = "id";
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<CM_SESSION_MSTR>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<CM_SESSION_MSTR>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
       
        /* TEST FOR MISC METHODS */
    }
}

