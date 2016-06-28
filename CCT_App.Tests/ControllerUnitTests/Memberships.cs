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
    /*  Test for WXYZ Controller */
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
                .Returns(new List<Membership>());

            // Act 
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<Membership>>;

            // Assert
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<Membership>>>(result);
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
            var data = new List<Membership> { new Membership { }, new Membership { }, new Membership { } };
            theservice
                .Setup(x => x.GetAll())
                .Returns(data);

            // Act 
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<IEnumerable<Membership>>;

            // Assert 
            Assert.IsType<OkNegotiatedContentResult<IEnumerable<Membership>>>(result);
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
            var contentresult = result as OkNegotiatedContentResult<Membership>;

            // Assert 
            Assert.IsType<OkNegotiatedContentResult<Membership>>(result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }
        /* TEST FOR POST METHODS */

        [Fact]
        public void Post_Returns_Bad_Request_Given_Null_Object()
        {

        }

        [Fact]
        public void Post_Returns_Not_Found_Given_Invalid_Object()
        {

        }

        [Fact]
        public void Post_Returns_Created_Given_Valid_Object()
        {

        }

        /* TEST FOR PUT METHODS */

        [Fact]
        public void Put_Returns_Bad_Request_Given_Null_Object()
        {

        }

        [Fact]
        public void Put_Returns_Bad_Request_Given_Empty_ID()
        {

        }
        
        [Fact]
        public void Put_Returns_Bad_Request_Given_Id_Mismatch()
        {

        }
        
        [Fact]
        public void Put_Returns_Not_Found_Given_Non_Existent_id()
        {

        }

        [Fact]
        public void Put_Returns_Ok_Given_Valid_Model()
        {

        }
        /* TEST FOR DELETE METHODS */
        
        [Fact]
        public void Delete_Returns_Bad_Request_Given_Empty_Id()
        {

        }

        [Fact]
        public void Delete_Returns_Not_Found_Given_Non_Existent_Id()
        {

        }

        [Fact]
        public void Delete_Returns_Ok_Given_Valid_Id()
        {

        }
        /* TEST FOR MISC METHODS */
    }
}
