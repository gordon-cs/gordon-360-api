using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Gordon360.Controllers.Api;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using System.Web.Http.Results;

namespace Gordon360.Tests.ControllerUnitTests
{
    /*  Test for Participation Controller */
    public class Roles
    {
        /* TEST FOR GET METHODS */
        [Fact]
        public void GetAll_With_Returns_Empty_List()
        {

            // Arrange
            var theservice = new Mock<IParticipationService>();
            var controller = new ParticipationsController(theservice.Object);
            theservice
                .Setup(x => x.GetAll())
                .Returns(new List<ParticipationViewModel>());

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<ParticipationViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<ParticipationViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Empty(contentresult.Content);

        }

        [Fact]
        public void GetAll_Returns_With_Popluated_List()
        {
            // Arrange
            var theservice = new Mock<IParticipationService>();
            var controller = new ParticipationsController(theservice.Object);
            var data = new List<ParticipationViewModel> { new ParticipationViewModel { }, new ParticipationViewModel { }, new ParticipationViewModel { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<ParticipationViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<ParticipationViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.NotEmpty(contentresult.Content);
            Assert.Equal(3, contentresult.Content.Count());

        }

        [Fact]
        public void Get_Returns_Not_Found_Given_Non_Existent_Id()
        {
            // Arrange
            var theservice = new Mock<IParticipationService>();
            var controller = new ParticipationsController(theservice.Object);
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
            var theservice = new Mock<IParticipationService>();
            var controller = new ParticipationsController(theservice.Object);
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
            var theservice = new Mock<IParticipationService>();
            var controller = new ParticipationsController(theservice.Object);
            var data = new ParticipationViewModel { };
            var id = "id";
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<ParticipationViewModel>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<ParticipationViewModel>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
                /* TEST FOR MISC METHODS */
    }
}
