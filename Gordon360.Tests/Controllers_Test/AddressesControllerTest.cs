namespace Gordon360.Tests.Controllers_Test;

public class AddressesControllerTests
{
    [Fact]
    public void GetAllStates_ReturnsOkWithListOfStates()
    {
        // Arrange
        var mockService = new Mock<IAddressesService>();

        var fakeStates = new List<States>
        {
            new States { Abbreviation = "MA", Name = "Massachusetts" },
            new States { Abbreviation = "CA", Name = "California" }
        };

        // Set up the mocked service to return the fake states list
        mockService.Setup(s => s.GetAllStates()).Returns(fakeStates);

        // Create the controller with the mocked service injected
        var controller = new AddressesController(mockService.Object);

        // Act
        var result = controller.GetAllStates(); // returns ActionResult<IEnumerable<States>>

        // Assert

        // 1. Assert the result is 200 OK
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // 2. Assert the result's Value is a list of States
        var returnValue = Assert.IsAssignableFrom<IEnumerable<States>>(okResult.Value);

        // 3. Assert the content of the list is exactly what we expect
        Assert.Collection(returnValue,
            state => Assert.Equal("MA", state.Abbreviation),
            state => Assert.Equal("CA", state.Abbreviation));
    }
}