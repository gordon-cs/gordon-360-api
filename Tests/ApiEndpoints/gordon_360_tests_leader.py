# Activity Leader Test Suite for Project Raymond
# Some of the tests are replicates from the Regular member suite.
# This repetitions is intended. We want to make sure some permission don't accross roles.


from test_components import requests
from test_components import TestCase

# Public configuration options
import test_config
# Private configuration options
import test_credentials
import test_components as api

# Constants
LEADERSHIP_POSITIONS = test_config.leadership_positions
REQUEST_STATUS_APPROVED = 'Approved'
REQUEST_STATUS_DENIED = 'Denied'
REQUEST_STATUS_PENDING = 'Pending'


# Configuration Details 
username = test_credentials.username_activity_leader
password = test_credentials.password_activity_leader
my_id_number = test_credentials.id_number_activity_leader
grant_payload = { 'username':username, 'password':password, 'grant_type':'password' }

random_id_number = test_config.random_id_number # When we need to impersonate someone
activity_code = test_config.activity_code # The activity we are a leader of
hostURL = test_config.hostURL


# Runner
def main():
    """ Test Runner
    
    Runs all subclasses of the TestCase class.
    """
    # Create an authorized session to test authorized calls.
    r = requests.post(hostURL+'token',grant_payload)
    access_token = r.json()["access_token"]
    authorization_header = "Bearer " + access_token
    authorized_session = requests.Session()
    authorized_session.verify = True
    authorized_session.headers.update({ "Authorization":authorization_header })

    print ('****************************************')
    print ('TESTING GORDON 360 AS ACTIVITY LEADER...')
    print ('****************************************')
    testCount = 0
    # Loop through all Test Cases and run tests.
    for cls in TestCase.__subclasses__():
        if '___activity_leader' in cls.__name__:
            if cls.__name__ == 'authenticate_with_valid_credentials':
                testclass = cls()
                testclass.runTest()
                testCount += 1
            else:
                testclass = cls(session=authorized_session)
                testclass.runTest()
                testCount += 1
    print ('Ran {0} tests.'.format(testCount))


# # # # # # # # # # # #
# AUTHENTICATION TESTS #
# # # # # # # # # # # #

class authenticate_with_valid_credentials___activity_leader(TestCase):
    """ Given valid credentials, verify that authentication is successful.

    Expectations:
    Endpoint --  token/
    Expected Status code -- 200 Ok
    Expected Content -- Json Object with access_token attribute.
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'token'
        self.token_payload = { 'username':username, 'password':password, 'grant_type':'password' }

    def test(self):
        response = api.post(self.session, self.url, self.token_payload)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json, got {0}.'.format(response.text))
        else:
            if not 'access_token' in response.json():
                self.log_error('Expected access token in response, got {0}.'.format(response.json()))


# # # # # # # # # # #
# MEMBERSHIP TESTS #
# # # # # # # # # # #
class get_all_memberships___activity_leader(TestCase):
    """ Test retrieving all membership resources.

    Pre-conditions:
    Valid Authentication Header.
    Expectations:
    Endpoint -- memberships/
    Expected Status code -- 200 Ok
    Expected Content -- List
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/'

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Response was not a list')


class get_one_membership___activity_leader(TestCase):
    """ Retrieve a specific membership resource.

    Pre-conditions:
    Valid Authentication header
    Expectations:
    Endpoint -- api/memberships/:id
    Expected Status Code -- 200 OK
    Expected Content -- A Json Object with a MembershipID attribute.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/'
        self.membershipID = -1

        
    def setup(self):
        # Find a valid membership id
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Error in setup for {0}. Expected 200 OK, got {1}.'.format(self.test_name, response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Error in setup for {0}. Expected json response body, got {1}.'.format(self.test_name, response.text))
        else:
            try:
                self.membershipID = response.json()[0]['MembershipID']
                self.url = self.url + str(self.membershipID)
            except KeyError:
                self.log_error('Error in setup for {0}. Expected MembershipID in response body, got {1}.'.format(self.test_name, self.response.json()))
            
    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not ('MembershipID' in response.json()):
                self.log_error('Expected MembershipID in jsob object, got {0}.'.format(response.json()))

class get_all_my_memberships___activity_leader(TestCase):
    """ Verify that a regular member can fetch memberships associated with them.
    
    Pre-Conditions:
    Valid Authentication Header.
    Expectations:
    Endpoints -- api/memberships/student/:id
    Expected Status Code -- 200 OK
    Expected Reponse Content -- A list of json objects
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/student/' + str(my_id_number)

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Response was not a list.')
        


class get_all_memberships_for_someone_else___activity_leader(TestCase):
    """ Verify that regular member can fetch someone else's memberships.
    
    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as regular member.
    Expectations
    Endpoint -- api/memberships/student/:id
    Expected Status Code -- 200 OK.
    Expected Response Content --  A list of json objects.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/student/' + str(random_id_number)

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Response was not a list')



    
class get_memberships_for_an_activity___activity_leader(TestCase):
    """ Verify that a regular member can fetch memberships for an activity.

    Pre-Conditions:
    Valid Authentication Header.
    Expectations:
    Endpoint -- api/memberships/activity/:id
    Expected Status Code -- 200 OK
    Expected Response Content -- A list of json Objects.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/activity/' + activity_code

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Response was not a list.')

        
    
class get_leader_memberships_for_an_activity___activity_leader(TestCase):
    """ Verify that a regular member can fetch all leaders for a specific activity.

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as regular member.
    Expectations:
    Endpoint -- api/memberships/activity/:id/leaders
    Expected Status Code -- 200 OK
    Expected Response Content -- A list of json objects.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/activity/' + activity_code + '/leaders'

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Response was not a list.')

                
class post_new_guest_membership_for_someone_else__activity_leader(TestCase):
    """ Verify that an activity leader can create a Guest membership for someone.

    Pre-conditions:
    Valid Authentication Header.
    Expectations:
    Endpoints -- api/memberships/
    Expected Statis Cpde -- 201 Created.
    Expected Content -- A Json object with a MEMBERSHIP_ID attribute.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': random_id_number,
            'PART_CDE':'GUEST',
            'BEGIN_DTE':'06/10/2016',
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        # We will get the actual id when we post.
        # Setting it -1 to check later that we got an id from the post.
        self.createdMembershipID = -1

    def test(self):
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            self.log_error('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not ('MEMBERSHIP_ID' in response.json()):
                self.log_error('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
            else:
                self.createdMembershipID = response.json()['MEMBERSHIP_ID']

    def cleanup(self):
        # We try to delete the membership we created
        if self.createdMembershipID < 0: # The creation wasn't successfull
            self.log_error('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
        else:
            d = api.delete(self.session, self.url + str(self.createdMembershipID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}'.format(self.test_name))

        return

class post_new_membership_for_someone___activity_leader(TestCase):
    """ Verify that an activity leader can create a membership for someone.
    
    Pre-Conditions:
    Valid Authentication Header
    Authenticated as Activity Leader
    Expectations:
    Endpoint -- api/memberships/
    Expected Status Code -- 200 OK
    Expected Content -- A json response with the created membership
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/'
        self.data = {}
        self.createdMembershipID = -1

    def setup(self):
        # Add a new participant
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': random_id_number, 
            'PART_CDE':'PART',
            'BEGIN_DTE':'06/10/2016',
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            
            }

    def test(self):
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            self.log_error('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                self.createdMembershipID = response.json()['MEMBERSHIP_ID']
            except KeyError:
                self.log_error('Expected MEMBERSHIP ID in response, got {0}.'.format(response.json()))
            
    def cleanup(self):
        # We try to delete the membership we created
        if self.createdMembershipID < 0: # The creation was not successfull
            self.log_error('Expected valid memberhsip ID, got {0}.'.format(self.createdMembershipID))
        else:
            d = api.delete(self.session, self.url + str(self.createdMembershipID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}. Expected , got {1}.'.format(self.test_name, d.status_code))


class post_new_leader_membership_for_someone___activity_leader(TestCase):
    """ Verify that an activity leader can assign a new leader
    
    Pre-Conditions:
    Valid Authentication Header
    Authenticated as Activity Leader
    Expectations:
    Endpoint -- api/memberships/
    Expected Status Code -- 200 OK
    Expected Content -- A json response with the created membership
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/'
        self.data = {}
        self.createdMembershipID = -1

    def setup(self):
        # Add a new leader
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': random_id_number, 
            'PART_CDE':'DIREC',
            'BEGIN_DTE':'06/10/2016',
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }

    def test(self):
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            self.log_error('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                self.createdMembershipID = response.json()['MEMBERSHIP_ID']
            except KeyError:
                self.log_error('Expected MEMBERSHIP_ID in response, got {0}.'.format(response.json()))
            
    def cleanup(self):        
        # We try to delete the membership we created
        if self.createdMembershipID < 0: # The creation was not successful
            self.log_error('Expected valid memberhsip ID, got {0}.'.format(self.createdMembershipID))
        else:
            d = api.delete(self.session, self.url + str(self.createdMembershipID))
            if not d.status_code == 200:                
                self.log_error('Error in cleanup for {0}. Expected , got {1}.'.format(self.test_name, d.status_code))
        

class put_edited_membership_member_to_leader___activity_leader(TestCase):
    """ Verify that an activity leader can upgrade a normal membership to leader status.

    Pre-Conditions:
    Valid Authentication Header
    Authenticated as Activity Leader
    Expectations:
    Endpoint -- api/memberships/:id
    Expected Status Code -- 200 OK
    Expected Content -- A json object with a MEMBERSHIP_ID attribute.
    """    

    def __init__(self , session=None):
        super().__init__(session)                               
        self.url = hostURL + 'api/memberships/'
        self.data = {}
        self.createdMembershipID = -1

    def setup(self):                               
        # The membership to modify
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': my_id_number,
            'PART_CDE':'PART', # Is a participant at first.
            'BEGIN_DTE':'06/10/2016', # Old start date
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:                
            self.createdMembershipID = r.json()["MEMBERSHIP_ID"]
            # Updated Data
            self.data = {
                'MEMBERSHIP_ID' : self.createdMembershipID,
                'ACT_CDE': activity_code,
                'SESS_CDE' : '201501',
                'ID_NUM': my_id_number,
                'PART_CDE':'DIREC', # Upgrade him to director.
                'BEGIN_DTE':'02/10/2016', # New start date
                'END_DTE':'07/16/2016',
                'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
                }
        except (KeyError, ValueError):
            self.log_error('Error in setup for {0}.'.format(self.test_name))

    def test(self):
        response = api.putAsJson(self.session, self.url + str(self.createdMembershipID), self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                response.json()['MEMBERSHIP_ID']
            except KeyError:
                self.log_error('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
                
                    
    def cleanup(self):
        # We try to delete the membership we created
        if self.createdMembershipID < 0: # The Creation wasn't successfull. ID is still -1.
            self.log_error('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
        else:
            d = api.delete(self.session, self.url + str(self.createdMembershipID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}'.format(self.test_name))

class put_edited_membership_leader_to_member___activity_leader(TestCase):
    """ Verify that an activity leader can demote a leader membership.

    Pre-Conditions:
    Valid Authentication Header
    Authenticated as Activity Leader
    Expectations:
    Endpoint -- api/memberships/:id
    Expected Status Code -- 200 OK
    Expected Content -- A json object with a MEMBERSHIP_ID attribute.
    """    

    def __init__(self , session=None):
        super().__init__(session)                               
        self.url = hostURL + 'api/memberships/'
        self.data = {}
        self.createdMembershipID = -1

    def setup(self):                               
        # The membership to modify
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': my_id_number,
            'PART_CDE':'DIREC', # Is a leader at first
            'BEGIN_DTE':'06/10/2016', # Old start date
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:                
            self.createdMembershipID = r.json()["MEMBERSHIP_ID"]
            # Updated Data
            self.data = {
                'MEMBERSHIP_ID' : self.createdMembershipID,
                'ACT_CDE': activity_code,
                'SESS_CDE' : '201501',
                'ID_NUM': my_id_number,
                'PART_CDE':'PART', # Demote him to participant
                'BEGIN_DTE':'02/10/2016', # New start date
                'END_DTE':'07/16/2016',
                'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
                }
        except (KeyError, ValueError):
            self.log_error('Error in setup for {0}.'.format(self.test_name))

    def test(self):
        response = api.putAsJson(self.session, self.url + str(self.createdMembershipID), self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:                
                response.json()['MEMBERSHIP_ID']
            except KeyError:
                self.log_error('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
                
                    
    def cleanup(self):
        # We try to delete the membership we created
        if self.createdMembershipID < 0: # The Creation wasn't successfull. ID is still -1.
            self.log_error('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
        else:
            d = api.delete(self.session, self.url + str(self.createdMembershipID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}'.format(self.test_name))



class delete_valid_membership___activity_leader(TestCase):
    """ Verify that a an activity leader can delete someone else's membership.

    Pre-Conditions:
    Valid Authentication header.
    Authenticated as regular member for the activity.
    Expectations:
    Endpoint -- api/memberships/
    Expected Status Code -- 200 OK
    Expected Response Content -- The membership resource that wad delteed.
    """

    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/memberships/'
        self.createdMembershipID = -1

    def setup(self):
        # Create a Memerships that we'll eventually delete
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': random_id_number,
            'PART_CDE':'PART',
            'BEGIN_DTE':'06/10/2016',
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
        }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:
            self.createdMembershipID = r.json()['MEMBERSHIP_ID']
        except (ValueError, KeyError):
            self.log_error('Error doing setup for {0}'.format(self.test_name))

    def test(self):
        response = api.delete(self.session, self.url + str(self.createdMembershipID))
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not ('MEMBERSHIP_ID' in response.json()):
                self.log_error(self.log_error('Expected MEMBERSHIP_ID in response, got {0}.'.format(response.json())))



# # # # # # # # # # # # # #
# MEMBERSHIP REQUEST TESTS #
# # # # # # # # # # # # # #

class get_all_membership_requests___activity_leader(TestCase):
    """ Verify that an activity leader  cannot access all membership requests.

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as activity leader.
    Expectations:
    Endpoint -- api/requests/
    Expected Status Code -- 401 Unauthorized
    Expected Response Content -- Empty response content.
    """

    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/'

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty response body, got {0}.'.format(response.text))

class get_membership_requests_for_activity___activity_leader(TestCase):
    """ Verify that the activity leader can get requests to join the activity he/she is leading

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as activity leader.
    Expectations:
    Endpoint -- api/requests/activity/:id
    Expected Status Code -- 200 OK
    Expected Response Body -- List of json objects representing the membership requests.
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/activity/' + activity_code

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Expected list in response body, got {0}.'.format(response.json()))

                
class get_membership_requests_for_someone_else___activity_leader(TestCase):
    """ Verify that an activity leader cannot get the membership requests of someone else.
    
    Pre-Conditions:
    Valid Authentication Header
    Authenticated as activity leader
    Expectations:
    Endpoint -- api/requests/student/:id
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """

    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/student/' + str(random_id_number)

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty response bodty, got {0}.'.format(response.text))

class get_all_my_membership_requests___activity_leader(TestCase):
    """ Verify that an activity leader can retrieve all requests belonging to them.
    
    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as regular member.
    Expectations:
    Endpoint -- api/requests/student/:id
    Expected Status Code -- 200 OK
    Expected Response Body -- A list of membership requests 
    """

    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/student/' + str(my_id_number)

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Expected list, got {0}.'.format(response.json()))

                
class post_valid_membership_request___activity_leader(TestCase):
    """ Verify that we can create a membership request.

    Pre-conditions:
    Valid Authentication Header.
    Expectations:
    Endpoints -- api/requests/
    Expected Status Cpde -- 201 Created.
    Expected Content -- A Json object with a REQUEST_ID attribute.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
                    }
        # We will get the actual id when we post.
        # Setting it -1 to check later that we got an id from the post.
        self.requestID = -1

    def test(self):
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            self.log_error('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected json response, got {0}.'.format(response.text))
        else:
            try:
                self.requestID = response.json()['REQUEST_ID']
                if not response.json()['STATUS'] == REQUEST_STATUS_PENDING:
                    self.log_error('Expected Pending status , got {0}.'.format(resposne.json()))
            except KeyError:
                self.log_error('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))

    def cleanup(self):
        # We try to delete the request we created
        if self.requestID < 0: # The creation wasn't successfull
            self.log_error('Could not delete request with id of {0}'.format(self.requestID))
        else:
            d = api.delete(self.session, self.url + str(self.requestID))
            if not d.status_code == 200:
                self.log_error('There was a problem performing cleanup for {0}'.format(self.test_name))

class post_membership_request_for_someone_else___activity_leader(TestCase):
    """ Verify that we can't create a membership request for someone else.

    Pre-conditions:
    Valid Authentication Header.
    Authenticated as Activity Leader member.
    Expectations:
    Endpoints -- api/requests/
    Expected Status Code -- 401 Unauthorized.
    Expected Response Content -- Empty Response.
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': random_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        # We will get the actual id when we post.
        self.requestID = -1

    def test(self):
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty response, got {0}.'.format(response.text))
        if response.status_code == 201:
            self.log_error('Will try to delete resource...')
            try:
                self.requestID = response.json()['REQUEST_ID']
            except (ValueError, KeyError):
                self.log_error('Error in test for {0}.'.format(self.test_name))
                
    def cleanup(self):
        # If the creation went through, let's delete the resource
        if self.requestID >=  0:
            self.log_error('Request {0} was created even though it was supposed to be unauthorized'.format(self.requestID))
            d = api.delete(self.session, self.url + str(self.requestID))
            if  d.status_code == 200:            
                self.log_error('Unauthorized resource deleted.')
            else:
                self.log_error('Error deleting unauthorized resource.')


class put_membership_request___activity_leader(TestCase):
    """ Verify that an activity leader can't edit a membership request through a put request.

    Pre-Conditions:
    Valid Authorization Header.
    Authenticated as activity leader.
    Expectations:
    Endpoint -- api/requests/:id
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + '/api/requests/'
        self.predata = {}
        self.data = {}
        self.requestID = -1

    def setup(self):
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.predata)
        try:
            self.requestID = response.json()['REQUEST_ID']
            self.data = {
                'REQUEST_ID': self.requestID,
                'ACT_CDE': activity_code,
                'SESS_CDE' : '201501',
                'ID_NUM': random_id_number, #Changing values to emulate attacker muhahah
                'PART_CDE':'PART',
                'DATE_SENT' : '07/06/2016',
                'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'            
                }            
        except ValueError:
            self.log_error('Error performing setup for {0}.'.format(self.test_name))


    def test(self):
        response = api.putAsJson(self.session, self.url + str(self.requestID), self.data)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty response body, got {0}.'.format(response.text))

    def cleanup(self):
        d = api.delete(self.session, self.url + str(self.requestID))
        if not d.status_code == 200:
            self.log_error('There was error performing cleanup for {0}.'.format(self.test_name))


class delete_memberhsip_request___activity_leader(TestCase):
    """ Verify that an activity leader can delete a membership request for his activity

    Pre-Conditions:

    Expectations:
    Endpoints -- api/requests/:id
    Expected Status Code -- 200 OK
    Expected Response Body -- The request that was deleted
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/'
        self.predata = {}
        self.requestID = -1

    def setup(self):
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE': '201501',
            'PART_CDE': 'PART',
            'ID_NUM': my_id_number,
        
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.predata)
        print (response.text)
        if not response.status_code == 201:
            self.log_error('Error in setup for {0}. Expected 201 Created, got {1}.'.format(self.test_name, response.status_code))
        else:
            self.requestID = response.json()['REQUEST_ID']

    def test(self):
        response = api.delete(self.session, self.url + '/' + str(self.requestID))
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                response.json()['REQUEST_ID']
            except KeyError:
                self.log_error('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))
            
        



class allow_someone_to_join_my_activity___activity_leader(TestCase):
    """ Verify that the activity leader can accept a request directed at their activity.

    Pre-Conditions:
    Valid Authentication Header
    Authenticated as Activity Leader
    Expectations:
    Endpoints -- api/requests/:id/approve
    Expected Status Code -- 200 OK
    Expected Response Body -- Json response with the request that was accepted.
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/'
        self.data = {}
        self.requestID = -1
        self.membershipID = -1

    def setup(self):
        #Create a memberships request for the trash club.
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            self.log_error('Error in setup for {0}. Expected 201 Created, got {1}.'.format(self.test_name, response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Error in setup for {0}. Expected json response, got {1}.'.format(self.test_name, response.text))
        else:
            try:
                self.requestID = response.json()['REQUEST_ID']
            except KeyError:
                self.log_error('Error in setup for {0}. Expected REQUEST_ID in response, got {1}.'.format(self.test_name, response.json()))
        
    def test(self):
        response = api.postAsJson(self.session, self.url + str(self.requestID) + '/approve', None)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                self.membershipID = response.json()['MEMBERSHIP_ID']
            except KeyError:
                self.log_error('Expected MEMBERSHIP_ID in response bady, got {0}.'.format(response.json()))
                    

    def cleanup(self):
        # We try to delete the request we made
        if self.requestID < 0: # The request was not successful
            self.log_error('Error in cleanup for {0}. Expected valid request ID, got {1}.'.format(self.test_name, self.requestID))
        else:
            d = api.delete(self.session, self.url + str(self.requestID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}. Expected 200 OK when deleting request, got {1}.'.format(self.test_name, d.status_code))
        # We try to delete the membership we created
        if self.membershipID < 0: # membership creatino was not successful
            self.log_error('Error in cleanup for {0}. Expected valid membership ID, got {1}.'.format(self.test_name, self.membershipID))
        else:
            d = api.delete(self.session, hostURL + 'api/memberships/' + str(self.membershipID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}. Expected 200 OK when deleting membership, got {1}.'.format(self.test_name, d.status_code))
        


class deny_someone_joining_my_activity___activity_leader(TestCase):
    """ Verify that the activity leader can deny a request directed at their activity.

    Pre-Conditions:
    Valid Authentication Header
    Authenticated as Activity Leader
    Expectations:
    Endpoints -- api/requests/:id/deny
    Expected Status Code -- 200 OK
    Expected Response Body -- Json response with the request that was denied
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/'
        self.data = {}
        self.requestID = -1

    def setup(self):
        #Create a memberships request for the trash club.
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            self.log_error('Error in setup for {0}. Expected 201 Created, got {1}.'.format(self.test_name, response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Error in setup for {0}. Expected json response, got {1}.'.format(self.test_name, response.text))
        else:
            try:
                self.requestID = response.json()['REQUEST_ID']
            except KeyError:
                self.log_error('Error in setup for {0}. Expected REQUEST_ID in response, got {1}.'.format(self.test_name, response.json()))
        
    def test(self):
        response = api.postAsJson(self.session, self.url + str(self.requestID) + '/deny', None)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                if not response.json()['STATUS'] == REQUEST_STATUS_DENIED:
                    self.log_error('Expected approved request, got {0}.'.format(response.json()))
            except KeyError:
                self.log_error('Expected STATUS in response bady, got {0}.'.format(response.json()))

    def cleanup(self):
        # We try to delete the request we made
        if self.requestID < 0: # The request was not successful
            self.log_error('Error in cleanup for {0}. Expected valid request ID, got {1}.'.format(self.test_name, self.requestID))
        else:
            d = api.delete(self.session, self.url + str(self.requestID))
            if not d.status_code == 200:
                self.log_error('Error in cleanup for {0}. Expected 200 OK, got {1}.'.format(self.test_name, d.status_code))


# # # # # # # # # # #
# SUPERVISOR TESTS #
# # # # # # # # # #

class get_all_supervisors___activity_leader(TestCase):
    """ Verify that an activity leader cannot view all supervisors.

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as Activity leader
    Expectations:
    Endpoint -- api/supervisors/
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/supervisors/'

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            self.log_error('Expected Status Code 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Got non-empty response body.')


class get_supervisors_for_activity___activity_leader(TestCase):
    """ Verify that an activity leader can get supervisors for activity

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as activity leader.
    Expectations:
    Endpoint -- api/supervisors/activity/:id
    Expected Status Code -- 200 OK
    Expected Response Body -- list of json objects
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/supervisors/activity/' + activity_code

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Expected list, got {0}.'.format(response.json()))

class post_supervisor___activity_leader(TestCase):
    """ verify that an activity leader can't add a supervisor

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as activity leader.
    Expectations:
    Endpoint -- api/supervisors/
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/supervisors/'
        self.data = {}
        self.supervisorID = -1

    def setup(self):
        self.data = {
            'ID_NUM' : my_id_number,
            'ACT_CDE' : activity_code,
            'SESS_CDE' : '201501'
            }

    def test(self):
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty response body, got {0}.'.format(response.text))
        if response.status_code == 201:
            self.log_error('Delete the unauthorized resource...')
            try:
                self.supervisorID = response.json()['SUP_ID']
            except ValueError:
                self.log_error('Problem accessing compromised resource.')
        

    def cleanup(self):
        if self.supervisorID > 0: # Unauthorized creation took place
            d = api.delete(self.session, self.url + str(self.supervisorID))
            if response.status_code == 200: # Deletion was not successful
                self.log_error('Unauthorized resource deleted.')
            else:
                self.log_error('Error deleting Unauthorized resource.')

class put_supervisor___activity_leader(TestCase):
    """ Verify that an activity leader can't edit an existing supervisor

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as regular member.
    Expectations:
    Endpoint -- api/supervisors/:id
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """
    def __init__(self , session=None):
        super().__init__(session)
        # Trying to update a random supervisor
        # The authorization step comes before finding the supervisor.
        # Even if the supervisor doesn't exist, we should be unauthorized.
        self.url = hostURL + 'api/supervisors/2' 
        self.data = {
            'SUP_ID' : 2,
            'ACT_CDE' : activity_code,
            'SESS_CDE' : '201501',
            'ID_NUM' : my_id_number
        }
        self.supervisorID = -1
        
    def test(self):
        response = api.putAsJson(self.session, self.url, self.data)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty respone body, got {0}.'.format(response.text))
        if response.status_code == 200: # If supervisor was updated
            self.log_error('Unauthorized update. Attempting to delete...')
            try:
                self.supervisorID = response.json()['SUP_ID']
            except (ValueError, KeyError):
                self.log_error('Error accessing compromised supervisor.')
                        
    def cleanup(self):
        if self.supervisorID > 0: # The supervisor was updated.
            d = api.delete(self.session, self.url)
            if d.status_code == 200:
                self.log_error('Compromised resource deleted.')
            else:
                self.log_error('Unable to delete compromised resource.')

# This test might need to be removed if the authorization process for supervisors is altered.
class delete_supervisor___activity_leader(TestCase):
    """ Verify that an activity leader can't delete a supervisor 

    Pre-Conditions:
    Valid Authentication Header
    Authenticated as activity leader
    Expectations:
    Endpoint -- api/supervisors/:id
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/supervisors/2'

    def test(self):
        response = api.delete(self.session, self.url)
        if not response.status_code == 401:
            self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            self.log_error('Expected empty body, got {0}.'.format(response.text))


# # # # # # # #
# EMAIL  TEST #
# # # # # # # #


class get_emails_for_activity___activity_leader(TestCase):
    """ Verify that a regular member cannot get the emails for the members of an activity
    
    Pre-conditions:
    Valid Authentication Header
    Authenticated as Activity leader
    Expectations:
    Endpoint -- api/emails/activity/:id
    Expected Status Code -- 200 OK
    Expected Response Body -- A list of json objects 
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/emails/activity/' + activity_code

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json in response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                self.log_error('Expected list, got {0}.'.format(response.json))

    
class get_emails_for_leaders___activity_leader(TestCase):
    """ Verify that a supervisor can get the emails for any activity leader
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/emails/activity/:id/leaders
    Expected Status Code -- 200 OK
    Expected Respones Body -- Json response with a list of emails
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/emails/activity/' + activity_code + '/leaders'

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                response.json()[0]['Email']
            except KeyError:
                self.log_error('Expected Email in response, got{0}.'.format(response.json()))

                



if __name__ == '__main__':
    main()
