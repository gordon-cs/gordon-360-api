using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Gordon360.Controllers.Api;
using Gordon360.Services;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using System.Web.Http.Results;

namespace Gordon360.Tests.ControllerUnitTests
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
                .Returns(new List<ActivityViewModel>());

            // Act 
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<ActivityViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<ActivityViewModel>>>(result);
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
            var data = new List<ActivityViewModel> { new ActivityViewModel { }, new ActivityViewModel { }, new ActivityViewModel { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act 
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<ActivityViewModel>>;

            // Assert 
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<ActivityViewModel>>>(result);
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
            var data = new ActivityViewModel { };
            var id = "id";
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act 
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<ActivityViewModel>;

            // Assert 
            Assert.IsType<OkNegotiatedContentResult<ActivityViewModel>>(result);
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
            var result = controller.GetSupervisorsForActivity(id);

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
                .Setup(x => x.GetSupervisorsForActivity(id));

            // Act 
            var result = controller.GetSupervisorsForActivity(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetSupervisorForActivity_Returns_Ok_With_Object_Model_Given_Valid_id()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "id";
            var data = new List<SupervisorViewModel> {
                new SupervisorViewModel { },
                new SupervisorViewModel { }
            };
            theservice
                .Setup(x => x.GetSupervisorsForActivity(id))
                .Returns(data);

            // Act 
            var result = controller.GetSupervisorsForActivity(id);
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<SupervisorViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<SupervisorViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }

        [Fact]
        public void GetMembershipsForActivity_Returns_Bad_Request_Given_Empty_String()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = string.Empty;

            // Act 
            var result = controller.GetSupervisorsForActivity(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void GetMembershipsForActivity_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "non-existent-id";
            theservice
                .Setup(x => x.GetMembershipsForActivity(id));

            // Act 
            var result = controller.GetMembershipsForActivity(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetMembershipsForActivity_Returns_Ok_With_Object_Model_Given_Valid_id()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "id";
            var data = new List<MembershipViewModel>
            {
                new MembershipViewModel { },
                new MembershipViewModel { },
                new MembershipViewModel { }
            };
            theservice
                .Setup(x => x.GetMembershipsForActivity(id))
                .Returns(data);

            // Act 
            var result = controller.GetMembershipsForActivity(id);
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }


        [Fact]
        public void GetLeadersForActivity_Returns_Bad_Request_Given_Empty_String()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = string.Empty;

            // Act 
            var result = controller.GetLeadersForActivity(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void GetLeadersForActivity_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "non-existent-id";
            theservice
                .Setup(x => x.GetLeadersForActivity(id));

            // Act 
            var result = controller.GetLeadersForActivity(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetLeadersForActivity_Returns_Ok_With_Object_Model_Given_Valid_id()
        {
            // Arrange
            var theservice = new Mock<IActivityService>();
            var controller = new ActivitiesController(theservice.Object);
            var id = "id";
            var data = new List<MembershipViewModel>
            {
                new MembershipViewModel { },
                new MembershipViewModel { },
                new MembershipViewModel { }
            };
            theservice
                .Setup(x => x.GetLeadersForActivity(id))
                .Returns(data);

            // Act 
            var result = controller.GetMembershipsForActivity(id);
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
    }

}
