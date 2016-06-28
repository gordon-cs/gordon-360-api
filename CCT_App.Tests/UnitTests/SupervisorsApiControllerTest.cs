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
using System.Data.Entity.Core.Objects;

namespace CCT_App.Tests.UnitTests
{
    public class SupervisorsApiControllerTest
    {
        /* TESTS FOR THE GET METHODS */

        [Fact]
        public void Get_Returns_Everything()
        {
            // Arrange
            // Fake database
            var mockRepository = new Mock<CCTEntities> { DefaultValue = DefaultValue.Mock };
            // Fake dbset that will return all our data
            var mockSet = mockRepository.Object.SUPERVISORs;
            var controller = new SupervisorsController(mockRepository.Object);
           

            // Tell fake database how to respond to the call that will be made.
            mockRepository
                .Setup(mockRepo => mockRepo.SUPERVISORs)
                .Returns(mockSet);

            //Act
            var result = controller.Get();
            var contentresult = result as OkNegotiatedContentResult<List<SUPERVISOR>>;

            //Assert
            Assert.IsType(typeof(OkNegotiatedContentResult<List<SUPERVISOR>>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
        }

        [Fact]
        public void Get_By_ID_Returns_Correctly_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            int id = 123;
            var supervisor = new SUPERVISOR { SUP_ID = id };
            mockRepository
                .Setup(repo => repo.SUPERVISORs.Find(id))
                .Returns(supervisor);

            //Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<SUPERVISOR>;


            //Assert
            Assert.IsType(typeof(OkNegotiatedContentResult<SUPERVISOR>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);            

        }
        [Fact]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange
            // Fake database
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            // Id that doesn't exist
            int id = 321;
            // Tell database how to respond to the call that will be made
            mockRepository
                .Setup(repo => repo.SUPERVISORs.Find(id));

            //Act
            var result = controller.Get(id);

            //Assert
            //Assert that a not Found result was returned
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /* TESTS FOR THE CREATE METHOD */
        [Fact]
        public void Create_Returns_BadRequest_Given_Null_Supervisor()
        {
            // The method should not attempt to insert a null object
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            SUPERVISOR supervisor = null;

            //Act
            var result = controller.Post(supervisor);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);

        }

        [Fact]
        public void Create_Returns_Object_Given_Valid_Model()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities> { DefaultValue = DefaultValue.Mock };
            var mockResult = new Mock<ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result>>();
            var controller = new SupervisorsController(mockRepository.Object);
            var supervisor = new SUPERVISOR { SESSION_CDE = "TEST_SESS", ACT_CDE = "TEST_ACT", ID_NUM = "TEST_IDNUM" };
            var data = new List<ACTIVE_CLUBS_PER_SESS_ID_Result>
            {
                new ACTIVE_CLUBS_PER_SESS_ID_Result { ACT_CDE="TEST_ACT" },
                new ACTIVE_CLUBS_PER_SESS_ID_Result { ACT_CDE="TEST2" }
            }.AsQueryable();

            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.Provider).Returns(data.Provider);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.Expression).Returns(data.Expression);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

            
            mockRepository
                .Setup(repo => repo.ACCOUNTs.Find("TEST_IDNUM"))
                .Returns(new ACCOUNT());
            mockRepository
                .Setup(repo => repo.CM_SESSION_MSTR.Find("TEST_SESS"))
                .Returns(new CM_SESSION_MSTR());
            mockRepository
                .Setup(repo => repo.ACTIVE_CLUBS_PER_SESS_ID("TEST_SESS"))
                .Returns(mockResult.Object);
            mockRepository
                .Setup(repo => repo.SUPERVISORs.Add(supervisor))
                .Returns(supervisor);

            // Act
            var result = controller.Post(supervisor);
            var contentresult = result as CreatedNegotiatedContentResult<SUPERVISOR>;

            //Assert
            Assert.IsType(typeof(CreatedNegotiatedContentResult<SUPERVISOR>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);            
        }
        [Fact]
        public void Create_Returns_Not_Found_Given_Non_Existent_Session_Code()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            var sess_id = "SESS_THAT_DOESN'T_EXIST";
            var account_id = "Random ID";
            var supervisor = new SUPERVISOR { SESSION_CDE = sess_id, ID_NUM = account_id };

            mockRepository
                .Setup(repo => repo.ACCOUNTs.Find(account_id))
                .Returns(new ACCOUNT());
            mockRepository
                .Setup(repo => repo.CM_SESSION_MSTR.Find(sess_id))
                .Returns((CM_SESSION_MSTR) null);

            // Act
            var result = controller.Post(supervisor);

            // Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }
        [Fact]
        public void Create_Returns_Not_Found_Given_Non_Existent_ID_Number()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            var id_num = "Id number that doesn't exist";
            var supervisor = new SUPERVISOR { ID_NUM = id_num };

            mockRepository
                .Setup(repo => repo.ACCOUNTs.Find(id_num))
                .Returns((ACCOUNT)null);

            //Act
            var result = controller.Post(supervisor);

            //Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }

        [Fact]
        public void Delete_Returns_Not_Found_With_Non_Existent_Supervisor()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            int id = 134;
            mockRepository
                .Setup(repo => repo.SUPERVISORs.Find(id))
                .Returns((SUPERVISOR)null);

            //Act
            var result = controller.Delete(id);
            //Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }

        [Fact]
        public void Delete_Returns_Ok_With_Valid_ID()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SupervisorsController(mockRepository.Object);
            int id = 134;
            var supervisor = new SUPERVISOR { SUP_ID = id };
            mockRepository
                .Setup(repo => repo.SUPERVISORs.Find(id))
                .Returns(supervisor);
            mockRepository
                .Setup(repo => repo.SUPERVISORs.Remove(supervisor))
                .Returns(supervisor);

            //Act
            var result = controller.Delete(id);

            //Assert
            Assert.IsType(typeof(OkResult), result);
        }
        


    }
}
