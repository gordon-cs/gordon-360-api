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
    /* Test Class for Activities Controller */
    public class Activities 
    {
        /* TEST FOR GET METHODS */

        [Fact]
        public void GetAll_With_Returns_Empty_List()
        {

            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            theservice
                .Setup(x => x.GetAll())
                .Returns(new List<ACT_CLUB_DEF>());

            // Act 
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<ACT_CLUB_DEF>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<ACT_CLUB_DEF>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Empty(contentresult.Content);   

        }

        [Fact]
        public void GetAll_Returns_With_Popluated_List()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var data = new List<ACT_CLUB_DEF> { new ACT_CLUB_DEF { }, new ACT_CLUB_DEF { }, new ACT_CLUB_DEF { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act 
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<ACT_CLUB_DEF>>;

            // Assert 
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<ACT_CLUB_DEF>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.NotEmpty(contentresult.Content);
            Assert.Equal(3, contentresult.Content.Count());

        }

        [Fact]
        public void Get_Returns_Bad_Request_Given_Empty_String()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = string.Empty;

            // Act 
            var result = controller.Get(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Get_Returns_Not_Found_Given_Non_Existent_Id()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "non-existent-id";
            theservice
                .Setup(x => x.Get(id));

            // Act 
            var result = controller.Get(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public void Get_Returns_Ok_With_Object_Model_Given_Valid_id()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var data = new ACT_CLUB_DEF { };
            var id = "id";
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act 
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<ACT_CLUB_DEF>;

            // Assert 
            Assert.IsType<OkNegotiatedContentResult<ACT_CLUB_DEF>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }

        [Fact]
        public void GetSupervisorForActivity_Returns_Bad_Request_Given_Empty_String()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = string.Empty;

            // Act 
            var result = controller.GetSupervisorForActivity(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void GetSupervisorForActivity_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "non-existent-id";
            theservice
                .Setup(x => x.GetSupervisorForActivity(id));

            // Act 
            var result = controller.GetSupervisorForActivity(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetSupervisorForActivity_Returns_Ok_With_Object_Model_Given_Valid_id()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "non-existent-id";
            var data = new SUPERVISOR { };
            theservice
                .Setup(x => x.GetSupervisorForActivity(id))
                .Returns(data);

            // Act 
            var result = controller.GetSupervisorForActivity(id);
            var contentresult = result as OkNegotiatedContentResult<SUPERVISOR>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<SUPERVISOR>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
    }
}
