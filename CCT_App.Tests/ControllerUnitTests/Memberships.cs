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
using CCT_App.Models.ViewModels;

namespace CCT_App.Tests.ControllerUnitTests
{
    /*  Test for Memberships Controller */
    public class Memberships
    {
        /* TEST FOR GET METHODS */
        [Fact]
        public void GetAll_With_Returns_Empty_List()
        {

            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            theservice
                .Setup(x => x.GetAll())
                .Returns(new List<MembershipViewModel>());

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Empty(contentresult.Content);

        }

        [Fact]
        public void GetAll_Returns_With_Popluated_List()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new List<MembershipViewModel> { new MembershipViewModel { }, new MembershipViewModel { }, new MembershipViewModel { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<MembershipViewModel>>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.NotEmpty(contentresult.Content);
            Assert.Equal(3, contentresult.Content.Count());

        }

        [Fact]
        public void Get_Returns_Not_Found_Given_Non_Existent_Id()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var id = 123;
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
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new Membership { };
            var id = 123;
            theservice
                .Setup(x => x.Get(id))
                .Returns(data);

            // Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<MembershipViewModel>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<MembershipViewModel>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
        /* TEST FOR POST METHODS */

        [Fact]
        public void Post_Returns_Bad_Request_Given_Null_Object()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = (Membership)null;

            // Act
            var result = controller.Post(data);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Post_Returns_Not_Found_Given_Invalid_Object()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new Membership { };

            // Act
            var result = controller.Post(data);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Post_Returns_Created_Given_Valid_Object()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new Membership { };
            theservice
              .Setup(x => x.Add(data))
              .Returns(data);

            // Act
            var result = controller.Post(data);
            var contentresult = result as CreatedNegotiatedContentResult<Membership>;

            //Assert
            Assert.IsType<CreatedNegotiatedContentResult<Membership>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }

        /* TEST FOR PUT METHODS */

        [Fact]
        public void Put_Returns_Bad_Request_Given_Null_Object()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = (Membership)null;
            var id = 123;

            // Act
            var result = controller.Put(id, data);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }


        [Fact]
        public void Put_Returns_Bad_Request_Given_Id_Mismatch()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new Membership { MEMBERSHIP_ID = 321 };
            var id = 123;

            // Act
            var result = controller.Put(id, data);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_Returns_Not_Found_Given_Non_Existent_id()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new Membership { MEMBERSHIP_ID = 123 };
            var id = 123;
            theservice
              .Setup(x => x.Update(id, data));

            // Act
            var result = controller.Put(id, data);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Put_Returns_Ok_Given_Valid_Model()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var data = new Membership { MEMBERSHIP_ID = 123 };
            var id = 123;
            theservice
              .Setup(x => x.Update(id, data))
              .Returns(data);

            // Act
            var result = controller.Put(id, data);
            var contentresult = result as OkNegotiatedContentResult<Membership>;

            //Assert
            Assert.IsType<OkNegotiatedContentResult<Membership>>(result);
        }
        /* TEST FOR DELETE METHODS */

        [Fact]
        public void Delete_Returns_Not_Found_Given_Non_Existent_Id()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var id = 123;

            // Act
            var result = controller.Delete(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Returns_Ok_Given_Valid_Id()
        {
            // Arrange
            var theservice = new Mock<IMembershipService>();
            var controller = new MembershipsController(theservice.Object);
            var id = 123;
            var data = new Membership { MEMBERSHIP_ID = id };
            theservice
              .Setup(x => x.Delete(id))
              .Returns(data);

            // Act
            var result = controller.Delete(id);
            var contentresult = result as OkNegotiatedContentResult<Membership>;
            //Assert
            Assert.IsType<OkNegotiatedContentResult<Membership>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
        /* TEST FOR MISC METHODS */
    }
}
