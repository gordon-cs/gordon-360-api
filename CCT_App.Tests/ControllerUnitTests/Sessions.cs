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
using CCT_App.Models.ViewModels;
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
                .Returns(new List<SessionViewModel>());

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<SessionViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<SessionViewModel>>>(result);
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
            var data = new List<SessionViewModel> { new SessionViewModel { }, new SessionViewModel { }, new SessionViewModel { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<SessionViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<SessionViewModel>>>(result);
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
            var data = new SessionViewModel { };
            var id = "id";
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<SessionViewModel>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<SessionViewModel>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
       
        [Fact]
        public void Get_Current_Session_Returns_Null_If_Current_Session_Was_Not_found()
        {
            // Arrange
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
            theservice
                .Setup(x => x.GetCurrentSession());

            // Act
            var result = controller.GetCurrentSession();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_Current_Session_Correctly_If_Session_Was_Found()
        {
            // Arrange
            var theservice = new Mock<ISessionService>();
            var controller = new SessionsController(theservice.Object);
            theservice
                .Setup(x => x.GetCurrentSession())
                .Returns(new CM_SESSION_MSTR { });

            // Act
            var result = controller.GetCurrentSession();
            var contentresult = result as OkNegotiatedContentResult<SessionViewModel>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<SessionViewModel>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
        /* TEST FOR MISC METHODS */
    }
}

