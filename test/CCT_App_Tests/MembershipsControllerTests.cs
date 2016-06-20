using Xunit;
using cct_api.controllers;
using cct_api.models;
using Moq;

namespace UnitTests.MemershipsControllerTest
{
     /********************************************
     * Class for the Memberships Controller Unit Tests
     *********************************************
     */ 

     public class Test
     {
        /* TESTS FOR THE GET METHODS */

        [FactAttribute]
        public void Get_By_ID_Returns_Correctly_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            string id = "valid_id";
            mockRepository
                .Setup(repo => repo.Find("valid_id"))
                .Returns(new Membership());



            //Act
            var result = controller.GetById(id);

            //Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
        }
        [FactAttribute]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            string id = "id-that-doesn't-exist";
            Membership nullMembership = null;
            mockRepository
                .Setup(repo => repo.Find("id-that-doesn't-exist"))
                .Returns(nullMembership);


            //Act
            var result = controller.GetById(id);

            //Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
        }

        [FactAttribute]
        public void Get_By_ID_Returns_Bad_Request_Given_Invalid_Model_State()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            controller.ModelState.AddModelError("Errro","Some Errror occured");
            string id = "1";

            //Act
            var result = controller.GetById(id);

            //Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }
        [FactAttribute]
        public void Get_By_ID_Returns_Bad_Request_Given_Null_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            string nullId = null;


            //Act
            var result = controller.GetById(nullId);

            //Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }
        [FactAttribute]
        public void GetAll_Returns_Correctly()    
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>(); 
            var controller = new MembershipsController(mockRepository.Object);
            var array = new Membership[10];
            mockRepository            
                .Setup(repo => repo.GetAll())
                .Returns(array);

            // Act
            var result = controller.GetAll();


            // Assert
            Assert.IsType<Membership[]>(result);

        }   

        /* TESTS FOR THE CREATE METHOD */
        [FactAttribute]
        public void Create_Returns_BadRequest_Given_Null_Membership()
        {
            //Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership membership = null;

            //Act
            var result = controller.Create(membership);
            
            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result);

        }
        [FactAttribute]
        public void Create_Returns_Bad_Request_Given_InValid_Model_State()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            controller.ModelState.AddModelError("Error","An error occured");
            var testMembership = new Membership();

            // Act
            var result = controller.Create(testMembership);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

        }

        [FactAttribute]
        public void Create_Returns_Object_Given_Non_Null_Object_And_Valid_Model_State()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            var testMembership = new Membership();
            
            // Act
            var result = controller.Create(testMembership);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtRouteResult>(result);

        }

        /* TESTS FOR THE UPDATE METHOD */
        [FactAttribute]
        public void Update_Returns_Bad_Request_Given_Invalid_Model_State()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            controller.ModelState.AddModelError("Error","An error occured");
            var testMembership = new Membership{
                membership_id = "1"
            };
            // Act
            // It doesn't matter what's passed in here, model state is invalid.
            var result = controller.Update("1", testMembership);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

        }

        [FactAttribute]
        public void Update_Returns_Bad_Request_Given_No_Provided_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            var testMembership = new Membership();

            // Act
            var result = controller.Update(null, testMembership);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result);

        }

        [FactAttribute]
        public void Update_Returns_Bad_Request_Given_Model_With_ID_Mismatch()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            var testMembership = new Membership{
                membership_id = "2" // ID Mismatch!!
            };

            // Act
            // Try to update Object with ID of 1, with Object with ID of 2, should not work.
            var result = controller.Update("1", testMembership);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result);

        }

        [FactAttribute]
        public void Update_Returns_Bad_Request_Given_Null_object()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            Membership testMembership = null;
            
            // Act
            var result = controller.Update("1", testMembership);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result);

        }


        /* TESTS FOR THE DELETE METHOD */

        [FactAttribute]
        public void Delete_Returns_Bad_Request_Given_Invalid_Model()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            controller.ModelState.AddModelError("Error","An error occured");
    
            // Act
            var result = controller.Delete("1");

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

        }

         [FactAttribute]
        public void Delete_Returns_Bad_Request_Given_Null_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            string nullID = null;            
            
            // Act
            var result = controller.Delete(nullID);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

        }

         [FactAttribute]
        public void Delete_Returns_Bad_Request_Given_Wrong_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var controller = new MembershipsController(mockRepository.Object);
            string id = "id-that-doesn't-exist";  
            Membership nullMembership = null;          
            mockRepository
                .Setup(repo => repo.Remove(id))
                .Returns(nullMembership);

            // Act
            var result = controller.Delete(id);

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result);

        }

        [FactAttribute]
        public void Delete_Returns_No_Content_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<IMembershipRepository>();
            var membership = new Membership
            {
                membership_id = "valid_id"
            };
            mockRepository
                .Setup(x => x.Remove("valid_id"))
                .Returns(membership);

            var controller = new MembershipsController(mockRepository.Object);

            // Act
            var result = controller.Delete("valid_id");

            //Assert
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);

        }      

     }
}

