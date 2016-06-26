using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using CCT_App.Controllers.Api;
using CCT_App.Models;
using System.Data.Entity;
using System.Web.Http.Results;
using System.Data.Entity.Core.Objects;

namespace CCT_App.Tests.UnitTests
{ 
    public class SessionsApiControllerTest
    {
        /* TESTS FOR THE GET METHODS */

        [Fact]
        public void Get_Returns_Everything()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var mockSet = new Mock<DbSet<CM_SESSION_MSTR>>();
            var controller = new SessionsController(mockRepository.Object);
            var data = new List<CM_SESSION_MSTR>()
            {
                new CM_SESSION_MSTR { SESS_CDE = "TEST" },
                new CM_SESSION_MSTR { SESS_CDE = "TEST1" }
            }.AsQueryable();

            var en = data.GetEnumerator();
            mockSet.As<IQueryable<CM_SESSION_MSTR>>().Setup(x => x.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<CM_SESSION_MSTR>>().Setup(x => x.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<CM_SESSION_MSTR>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<CM_SESSION_MSTR>>().Setup(x => x.GetEnumerator()).Returns(en);

            mockRepository
                .Setup(mockRepo => mockRepo.CM_SESSION_MSTR)
                .Returns(mockSet.Object);
            //Act
            var result = controller.Get();
            //Assert
            Assert.Equal(2, result.Count());
            Assert.IsType(typeof(List<CM_SESSION_MSTR>), result);
        }
     
        [Fact]
        public void Get_By_ID_Returns_Correctly_Given_Valid_ID()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SessionsController(mockRepository.Object);
            string id = "valid_id";
            var session = new CM_SESSION_MSTR { SESS_CDE = id };
            mockRepository
                .Setup(repo => repo.CM_SESSION_MSTR.Find(id))
                .Returns(session);

            //Act
            var result = controller.Get(id);
            var contentresult = result as OkNegotiatedContentResult<CM_SESSION_MSTR>;

            //Assert
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);
            Assert.Equal(id, contentresult.Content.SESS_CDE);

        }
        [Fact]
        public void Get_By_ID_Returns_Not_Found_Given_Non_Existent_ID()
        {
            // Arrange          
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SessionsController(mockRepository.Object);
            string id = "id-that-doesn't-exist";
            mockRepository
                .Setup(repo => repo.CM_SESSION_MSTR.Find(id))
                .Returns((CM_SESSION_MSTR)null);

            //Act
            var result = controller.Get(id);

            //Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }

        [Fact]
        public void Get_By_ID_Returns_Bad_Request_Given_Empty_ID_String()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var controller = new SessionsController(mockRepository.Object);
            string emptyID = String.Empty;

            //Act
            var result = controller.Get(emptyID);

            //Assert
            Assert.IsType(typeof(BadRequestResult), result);
        }

        [Fact]
        public void GetActivitiesForSession_Returns_OK_With_No_Activities()
        {
            //Arrange
            var mockRepository = new Mock<CCTEntities>();
            var mockResult = new Mock<ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result>>();
            var controller = new SessionsController(mockRepository.Object);
            string id = "session with no activities";
            mockRepository
                .Setup(repo => repo.ACTIVE_CLUBS_PER_SESS_ID(id))
                .Returns(mockResult.Object);
            //Act
            var result = controller.GetActivitiesForSession(id);
            var contentresult = result as OkNegotiatedContentResult<ACTIVE_CLUBS_PER_SESS_ID_Result>;

            //Assert
            Assert.IsType(typeof(OkNegotiatedContentResult<System.Data.Entity.Core.Objects.ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result>>), result);
            Assert.Null(contentresult);

        }

        public void GetActivitiesForSession_Returns_OK_With_List_Of_Valid_Activities()
        {
            // Arrange
            var mockRepository = new Mock<CCTEntities>();
            var mockResult = new Mock<ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result>>();
            var controller = new SessionsController(mockRepository.Object);
            var id = "session with 2 activities";
            var session = new CM_SESSION_MSTR { SESS_CDE = id };
            var data = new List<ACTIVE_CLUBS_PER_SESS_ID_Result>
            {
                new ACTIVE_CLUBS_PER_SESS_ID_Result { ACT_CDE="TEST1" },
                new ACTIVE_CLUBS_PER_SESS_ID_Result { ACT_CDE="TEST2" }
            }.AsQueryable();

            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.Provider).Returns(data.Provider);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.Expression).Returns(data.Expression);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockResult.As<IQueryable<ACTIVE_CLUBS_PER_SESS_ID_Result>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

            mockRepository
                .Setup(repo => repo.ACTIVE_CLUBS_PER_SESS_ID(id))
                .Returns(mockResult.Object);
            
            // Act
            var result = controller.GetActivitiesForSession(id);
            var contentresult = result as OkNegotiatedContentResult<ACTIVE_CLUBS_PER_SESS_ID_Result>;

            //Assert
            Assert.IsType(typeof(OkNegotiatedContentResult<ACTIVE_CLUBS_PER_SESS_ID_Result>), result);
            Assert.NotNull(contentresult);
            Assert.NotNull(contentresult.Content);

        }

    }
}
