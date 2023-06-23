using Gordon360;
using Gordon360.Static.Names;
using Gordon360.Authorization;

// Tests function names are made of three parts:
//   1. The name of the method being tested
//   2. The scenario under which it's being tested
//   3. The expected behavior when the scenario is invoked
//
// Body of test should have three parts:
//   1. Arrange your objects, create and set them up
//   2. Act on an object
//   3. Assert that something is as expected

namespace UnitTestsGordon360
{
    public class AuthenticationTests
    { 
        [Theory]
        // [InlineData(Resource.NEWS)] // Resource.NEWS
        [InlineData(Resource.SLIDER)] // Resource.SLIDER
        public void CanReadPublic_X_Y(string resource)
        {
            // Arrange
            var result = true;

            // Act
            //var result = StateYourBusiness.CanReadPublic(resource);

            // Here is the body of CanReadPublic() but with some changes just to see if this
            // test would run at all.  (I was learning how namespaces are implemented in C#)
            switch (resource)
            {
                case Resource.SLIDER:
                    result = true;
                    break;
                case Resource.NEWS:
                    result = false;
                    break;
                default:
                    result = false;
                    break;
            }

            // Assert
            Assert.True(result); // Should be true for NEWS
        }
    }
}