import requests
import colors

# Test Components

def get(session, url):
    response = session.get(url)
    return response

def post(session, url, resource):
    response = session.post(url, resource)
    return response

def postAsJson(session, url, resource):
    response = session.post(url, json=resource)
    return response

def put(session, url, resource):
    response = session.put(url, resource)
    return response

def putAsJson(session, url, resource):
    response = session.put(url, json=resource)
    return response

def delete(session, url):
    response = session.delete(url)
    return response


# Test Case Base Class

TEST_PASS = "PASS"
TEST_FAIL = "FAIL"


class TestCase:
    """ Parent Test Case class to be inherited

    Describes attribute references common to all child classes.
    Describtes Expectations for the Test Case.
    """

    def __init__(self, session=None):
        """ Constructor

        Arguments:
        SSLVerify -- A boolean indicating if the ssl connection shoudl be verified.
        session -- A requests session object that will be used to make http calls.
        """
        if session is None:
            self.session = requests.Session()
        else:
            self.session = session

        self.test_name = self.__class__. __name__
        self.test_result = TEST_FAIL
        self.error_count = 0
        self.errors = []

    def setup(self):
        """ Perform any setup operations"""
        return

    def test(self):
        """ Call the endpoint to be tested

        Calls the endpoint under Test.
        Verifies that the specified expectations were met.
        Logs if expectation was not met.
        """
        return

    def print_results(self):
        """ Prints test results

        Reads the error count and prints results based on that.
        """
        if self.error_count == 0:
            self.test_result = TEST_PASS
        result =  ''
        if self.test_result == TEST_PASS:
            result = "{0}:".format(self.test_name) + colors.OKGREEN + " {0}".format(self.test_result) + colors.ENDC
            print (result)
        elif self.test_result == TEST_FAIL:
            result = "{0}:".format(self.test_name) + colors.FAIL + " {0}".format(self.test_result) + colors.ENDC
            print (result)
        if self.test_result == TEST_FAIL:
            for error in self.errors:
                print (colors.FAIL + "\t {0}".format(error) + colors.ENDC)
        return

    def cleanup(self):
        """ Perform any cleanup operations"""
        return

    def runTest(self):
        """ Run the Test Case"""
        self.setup()
        self.test()
        self.cleanup()
        self.print_results()


    def log_error(self, error):
        """ Helper method for logging errors"""
        self.error_count += 1
        self.errors.append(error)

