# Regular Member Test Suite for Project Raymond
from test_components import requests
from test_components import TestCase

# Public configuration options
import test_config
# Private configuration options
import test_credentials
import test_components as api

# Constants
LEADERSHIP_POSITIONS = test_config.leadership_positions


# Configuration Details 
username = test_credentials.username
password = test_credentials.password
my_id_number = test_credentials.id_number
grant_payload = { 'username':username, 'password':password, 'grant_type':'password' }

random_id_number = test_config.random_id_number
activity_code = test_config.activity_code
hostURL = test_config.hostURL



# Runner
def main():
    """ Test Runner
    
    Runs all subclasses of the TestCase class.
    """

    # Create an authorized session to test authorized calls.
    r = requests.post(hostURL+'token',grant_payload)
    print(hostURL+'token')
    print(grant_payload)
    print(r)
    access_token = r.json()["access_token"]
    authorization_header = "Bearer " + access_token
    authorized_session = requests.Session()
    authorized_session.verify = True
    authorized_session.headers.update({ "Authorization":authorization_header })

    # Loop through all Test Cases and run tests.
    print ('***************************************')
    print ('TESTING GORDON 360 AS REGULAR MEMBER...')
    print ('***************************************')
    testCounter = 0
    for cls in TestCase.__subclasses__():
        if '___regular_member' in cls.__name__:
            if cls.__name__ == 'authenticate_with_valid_credentials':
                testclass = cls()
                testclass.runTest()
                testCounter += 1  
            else:
                testclass = cls(session=authorized_session)
                testclass.runTest()
                testCounter += 1
    print ('Ran {0} tests.'.format(testCounter))

# Test Cases
# # # # # # # # # # # #
# AUTHENTICATION TESTS #
# # # # # # # # # # # #

class authenticate_with_valid_credentials___regular_member(TestCase):
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


# # # # # # # # # # # #
# EVENTS & CLAW TESTS #
# # # # # # # # # # # #

class get_all_events___regular_member(TestCase):
    """ Verify that a regular member can get all events by type_ID
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/25Live/type/:Event_OR_Type_ID
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/25Live/type/10'
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


class get_all_events_multiple___regular_member(TestCase):
    """ Verify that a regular member can get all events by multiple type_ID
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/25Live/type/:Event_OR_Type_ID
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/25Live/type/10$11$12$14'
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


class get_all_claw___regular_member(TestCase):
    """ Verify that a regular member can get all upcoming chapel events (category_ID = 85)
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/25Live/CLAW
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/25Live/CLAW'
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


class get_all_25Live___regular_member(TestCase):
    """ Verify that a regular member can get all events in 25Live under predefined categories
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/25Live/All
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/25Live/All'
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


class get_all_25Live_by_event_id___regular_member(TestCase):
    """ Verify that a regular member can get information on specific event on 25Live
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/25Live/All
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/25Live/2911'
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


class get_all_25Live_by_multiple_event_id___regular_member(TestCase):
    """ Verify that a regular member can get information on specific event on 25Live
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/25Live/All
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/25Live/2911$2964$3030'
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


class get_all_chapel___regular_member(TestCase):
    """ Verify that a regular member can get information on chapel events attended
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/chapel/:user_name
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/chapel/sam.nguyen/'
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


class get_all_chapel_by_term___regular_member(TestCase):
    """ Verify that a regular member can get information on chapel events attended by specific school term
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/events/chapel/:user_name
    Expected Status Code -- 200 OK
    Expected Respones Body -- list of all events resources
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/events/chapel/matthew.felgate/FA16'
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


# # # # # # # # # # #
# MEMBERSHIP TESTS #
# # # # # # # # # # #
# class get_all_memberships___regular_member(TestCase):
#     """ Verify that a regular member can retrieve all memberships

#     Pre-conditions:
#     Valid Authentication Header.
#     Expectations:
#     Endpoint -- memberships/
#     Expected Status code -- 200 Ok
#     Expected Content -- List
#     """
#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'

#     def test(self):
#         response = api.get(self.session, self.url)
#         print (response)
#         print (api.get(self.session, self.url))
#         if not response.status_code == 401:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
            
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response, got >{0}<.'.format(response.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Response was not a list')

        
# class get_one_membership___regular_member(TestCase):
#     """ Retrieve a specific membership resource.

#     Pre-conditions:
#     Valid Authentication header
#     Expectations:
#     Endpoint -- api/memberships/:id
#     Expected Status Code -- 200 OK
#     Expected Content -- A Json Object with a MembershipID attribute.
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.membershipID = -1

        
#     def setup(self):
#         # Find a valid membership id
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Error in setup for {0}. Expected 200 OK, got {1}.'.format(self.test_name, response.status_code))

#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Error in setup for {0}. Expected json response body, got {1}.'.format(self.test_name, response.text))
#         else:
#             try:
#                 self.membershipID = response.json()[0]['MembershipID']
#                 self.url = self.url + str(self.membershipID)
#             except KeyError:
#                 self.log_error('Error in setup for {0}. Expected MembershipID in response body, got {1}.'.format(self.test_name, self.response.json()))
#         #exit(1)    
#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not ('MembershipID' in response.json()):
#                 self.log_error('Expected MembershipID in jsob object, got {0}.'.format(response.json()))

# class get_all_my_memberships___regular_member(TestCase):
#     """ Verify that a regular member cannot fetch memberships associated with them. 
#         This is because they can only see the members if they are part of the group.
    
#     Pre-Conditions:
#     Valid Authentication Header.
#     Expectations:
#     Endpoints -- api/memberships/student/:id
#     Expected Status Code -- 401 Server Error
#     Expected Reponse Content -- A list of json objects
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/student/' + str(my_id_number)

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 401:
#             self.log_error('Expected 401, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Response was not a list.')
        


# class get_all_memberships_for_someone_else___regular_member(TestCase):
#     """ Verify that regular member can fetch someone else's memberships.
    
#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as regular member.
#     Expectations
#     Endpoint -- api/memberships/student/:id
#     Expected Status Code -- 200 OK.
#     Expected Response Content --  A list of json objects.
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/student/' + str(random_id_number)

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Response was not a list')



    
# class get_memberships_for_an_activity___regular_member(TestCase):
#     """ Verify that a regular member can fetch memberships for an activity.

#     Pre-Conditions:
#     Valid Authentication Header.
#     Expectations:
#     Endpoint -- api/memberships/activity/:id
#     Expected Status Code -- 200 OK
#     Expected Response Content -- A list of json Objects.
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/activity/' + activity_code

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Response was not a list.')

        
    
class get_leader_memberships_for_an_activity___regular_member(TestCase):
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

                

# class post_valid_membership___regular_member(TestCase):
#     """ Given valid membership, verify that post works.

#     Pre-conditions:
#     Valid Authentication Header.
#     Expectations:
#     Endpoints -- api/memberships/
#     Expected Statis Cpde -- 201 Created.
#     Expected Content -- A Json object with a MEMBERSHIP_ID attribute.
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.data = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'GUEST',
#             'BEGIN_DTE':'06/10/2016',
#             'END_DTE':'07/16/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         # We will get the actual id when we post.
#         # Setting it -1 to check later that we got an id from the post.
#         self.createdMembershipID = -1

#     def test(self):
#         response = api.postAsJson(self.session, self.url, self.data)
#         if not response.status_code == 201:
#             self.log_error('Expected 201 Created, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             try:
#                 self.createdMembershipID = response.json()['MEMBERSHIP_ID']
#             except KeyError:                
#                 self.log_error('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
                

#     def cleanup(self):
#         # We try to delete the membership we created
#         if self.createdMembershipID < 0: # The creation wasn't successful
#             self.log_error('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
#         else:
#             d = api.delete(self.session, self.url + str(self.createdMembershipID))
#             if not d.status_code == 200:
#                 self.log_error('Error in cleanup for {0}'.format(self.test_name))

#         return

# class post_membership_with_id_that_is_not_you___regular_member(TestCase):
#     """ Verify that regular member can't create a membership for someone else.

#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as a regular member for this Activity.
#     Expectations:
#     Endpoint -- api/memberships/
#     Expected Status Code -- 401 Unauthorized
#     Expected Content -- No Content
#     """

#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.data = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': random_id_number,
#             'PART_CDE':'GUEST',
#             'BEGIN_DTE':'06/10/2016',
#             'END_DTE':'07/16/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         self.membershipID = -1

#     def setup(self):
#         # Report if there any current memberships for the Club to avoid false negatives.
#         # If I am currently a director of the club, this test should fail.
#         response = api.get(self.session, self.url + 'student/' + str(my_id_number))
#         try:
#             for membership in response.json():
#                 if(membership['ActivityCode'] == activity_code and membership['Participation'] in LEADERSHIP_POSITIONS):
#                     self.log_error('False Negative: This user is a leader for the activity we are testing.')
#         except (ValueError, KeyError):
#             self.log_error('Error in setup for {0}'.format(self.test_name))    

#     def test(self):
#         response = api.post(self.session, self.url, self.data)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))
#         # If the unauthorized operation went through, try to recover.
#         if response.status_code == 201:
#             self.log_error('Expected 401 Unauthorized, got 201. Will try to delete the data ...')
#             try:
#                 self.membershipID = response.json()['MEMBERSHIP_ID']
#             except (KeyError, ValueError):
#                 self.log_error('Error in test for {0}'.format(self.test_name))

#     def cleanup(self):
#         if self.membershipID > 0: # The creation went through even though it wasn't supposed to.
#             d = api.delete(self.session, self.url + str(self.membershipID))
#             if d.status_code == 200:
#                 self.log_error('Unauthorized resource was deleted.')
#             else:
#                 self.log_error('Error deleting unauthorized resource.')

        

# class post_non_guest_membership___regular_member(TestCase):
#     """ Verify that regular member can not create a non-guest membership.

#     Pre-Conditions:
#     Authentication Header is Valid.
#     Authenticated as a regular member for this Activity.
#     Expectations:
#     Endpoint -- apo/memberships
#     Expected Status Code -- 401 Unauthorized
#     Expected Content -- No Content
#     """

#     def __init__(self,
#                  SSLVerify=False,
#                  session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.data = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'DIREC', #A regular user cannot do this directly.
#             'BEGIN_DTE':'06/10/2016',
#             'END_DTE':'07/16/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         self.membershipID = -1

#     def setup(self):
#         # Report if there any current memberships for the Club to avoid false negatives.
#         # If I am currently a director of the club, this test should fail.
#         response = api.get(self.session, self.url + 'student/' + str(my_id_number))
#         try:
#             for membership in response.json():
#                 if(membership['ActivityCode'] == activity_code and membership['Participation'] in LEADERSHIP_POSITIONS):
#                     self.log_error('False Negative: This user is a leader for the activity we are testing.')
#         except (KeyError, ValueError):
#             self.log_error('Error in setup for {0}.'.format(self.test_name))


#     def test(self):
#         response = api.post(self.session, self.url, self.data)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))
#         if response.status_code == 201:
#             self.log_error('Expected 401 Unauthorized, got 201 Created. Will try to delete data ...')
#             try:
#                 self.membershipID = response.json()['MEMBERSHIP_ID']
#             except (KeyError, ValueError):
#                 self.log_error('Error in test for {0}.'.format(self.test_name))

#     def cleanup(self):
#         if self.membershipID > 0: # The creation went through even though it wasn't supposed to.
#             d = api.delete(self.session, self.url + str(self.membershipID))
#             if d.status_code == 200:
#                 self.log_error('Unauthorized resource was deleted.')
#             else:
#                 self.log_error('Error deleting unauthorized resource.')
            
            
        

# class put_valid_membership___regular_member(TestCase):
#     """ Verify that a membership can be updated by the owner.

#     Pre-Conditions:
#     Authenticated as Regular member.
#     Expectations:
#     Endpoint -- api/memberships/
#     Expected Status Code -- 200 OK
#     Expected Content -- A json object with a MEMBERSHIP_ID attribute.
#     """

#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.data = {}
#         self.createdMembershipID = -1

#     def setup(self):
#         # The membership to modify
#         self.predata = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'GUEST',
#             'BEGIN_DTE':'06/10/2016',
#             'END_DTE':'07/16/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         r = api.post(self.session, self.url, self.predata)
#         try:                
#             self.createdMembershipID = r.json()["MEMBERSHIP_ID"]
#             # Updated Data
#             self.data = {
#                 'MEMBERSHIP_ID' : self.createdMembershipID,
#                 'ACT_CDE': activity_code,
#                 'SESS_CDE' : '201501',
#                 'ID_NUM': my_id_number,
#                 'PART_CDE':'GUEST',
#                 'BEGIN_DTE':'02/10/2016',
#                 'END_DTE':'07/16/2016',
#                 'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#                 }
#         except (KeyError, ValueError):
#             self.log_error('Error in setup for {0}.'.format(self.test_name))

#     def test(self):
#         response = api.putAsJson(self.session, self.url + str(self.createdMembershipID), self.data)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not ('MEMBERSHIP_ID' in response.json()):
#                 self.log_error('Expected MEMBERSHIP_ID in response body, got {0}.'.format(response.json()))

#     def cleanup(self):
#         # We try to delete the membership we created
#         if self.createdMembershipID < 0: # The Creation wasn't successfull. ID is still -1.
#             self.log_error('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
#         else:
#             d = api.delete(self.session, self.url + str(self.createdMembershipID))
#             if not d.status_code == 200:
#                 self.log_error('Error in cleanup for {0}'.format(self.test_name))

# class put_non_guest_membership___regular_member(TestCase):
#     """ Verify that regular member can't update their membership level.

#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as a regular member for the activity.
#     Expectations:
#     Endpoint -- api/memberships/
#     Expected Status Code -- 401 Unauthorized
#     Expected Content -- No Content
#     """

#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.data = {}
#         self.createdMembershipID = -1

#     def setup(self):
#         # Report if there any current leader memberships under my name to avoid false negatives.
#         # If I am currently a director of the club, this test should fail.
#         response = api.get(self.session, self.url + 'student/' + str(my_id_number))
#         try:
#             for membership in response.json():
#                 if(membership['ActivityCode'] == activity_code and membership['Participation'] in LEADERSHIP_POSITIONS):
#                     self.log_error('False Negative: This user is a leader for the activity we are testing.')
#         except (KeyError, ValueError):
#             self.log_error('Error in setup for {0}'.format(self.test_name))                


#         self.predata = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'GUEST',
#             'BEGIN_DTE':'06/10/2016',
#             'END_DTE':'07/16/2016',
#             'COMMENT_TXT':'Generated the Raymond Test Suite. IGNORE'
#         }
#         r = api.postAsJson(self.session, self.url, self.predata)
#         try:
#             self.createdMembershipID = r.json()['MEMBERSHIP_ID']
#             self.data = {
#                 'MEMBERSHIP_ID' : self.createdMembershipID,
#                 'ACT_CDE': activity_code,
#                 'SESS_CDE' : '201501',
#                 'ID_NUM': my_id_number,
#                 'PART_CDE':'DIREC', #This is not allowed
#                 'BEGIN_DTE':'02/10/2016',
#                 'END_DTE':'07/16/2016',
#                 'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#                 }

#         except (KeyError, ValueError):
#             self.log_error('Error in setup for {0}'.format(self.test_name))

#     def test(self):
#         response = api.putAsJson(self.session, self.url + str(self.createdMembershipID),self.data)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))

#     def cleanup(self):
#         if self.createdMembershipID < 0:
#             self.log_error('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
#         else:
#             d = api.delete(self.session, self.url + str(self.createdMembershipID))
#             if not d.status_code == 200:
#                 self.log_error('Error in cleanup for {0}'.format(self.test_name))


# class delete_membership_for_someone_else___regular_member(TestCase):
#     """ Verify that a regular member cannot delete someone else's membership.
    
#     Pre-Conditions:
#     Valid Authentication Header
#     Authenticated as Regular Member
#     Expectations:
#     Endpoint -- api/memberships/:id
#     Expected Status Code -- 401 Unauthorized.
#     Expected Response Body -- Empty.
#     """

#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.membershipID = -1

#     def setup(self):
#         # Get all memberships and pick first one that doesn't belong to me
#         response = api.get(self.session, self.url)
#         try:
#             memberships = response.json()
#         except ValueError:
#             self.log_error('Error in setup for {0}. Expected a json response, got {1}.'.format(self.test_name, response.text))
#         else:
#             try:
#                 for membership in memberships:
#                     if not membership['IDNumber'] == str(my_id_number):
#                         self.membershipID = membership['MembershipID']
#                         break
#             except KeyError:
#                 self.log_error('Error in setup for {0}. Expected MembershipID in json response, got {1}.'.format(self.test_name, response.json()))

#     def test(self):
#         response = api.delete(self.session, self.url + str(self.membershipID))
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))

    
# class delete_valid_membership___regular_member(TestCase):
#     """ Verify that a regular member can delete their membership.

#     Pre-Conditions:
#     Valid Authentication header.
#     Authenticated as regular member for the activity.
#     Expectations:
#     Endpoint -- api/memberships/
#     Expected Status Code -- 200 OK
#     Expected Response Content -- The membership resource that wad delteed.
#     """

#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/memberships/'
#         self.createdMembershipID = -1

#     def setup(self):
#         # Create a Memerships that we'll eventually delete
#         self.predata = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'GUEST',
#             'BEGIN_DTE':'06/10/2016',
#             'END_DTE':'07/16/2016',
#             'COMMENT_TXT':'Generated the Raymond Test Suite. IGNORE'
#         }
#         r = api.postAsJson(self.session, self.url, self.predata)
#         try:
#             self.createdMembershipID = r.json()['MEMBERSHIP_ID']
#         except ValueError:
#             self.log_error('Error doing setup for {0}'.format(self.test_name))

#     def test(self):
#         response = api.delete(self.session, self.url + str(self.createdMembershipID))
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not ('MEMBERSHIP_ID' in response.json()):
#                 self.log_error(self.log_error('Expected MEMBERSHIP_ID in response, got {0}.'.format(response.json())))



# # # # # # # # # # # # # # #
# # MEMBERSHIP REQUEST TESTS #
# # # # # # # # # # # # # # #

class get_all_membership_requests___regular_member(TestCase):
    """ Verify that a regular member cannot access all membership requests.

    Pre-Conditions:
    Valid Authentication Header.
    Authenticated as regular member.
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

# class get_my_membership_request___regular_member(TestCase):
#     """ Verify that a member can create a request and view it.

#     Pre-Conditions:
#     Valid Authentication Header
#     Authenticated as regular member
#     Expectations:
#     Endpoint -- api/requests/:id
#     Expected Status Code -- 200 OK
#     Expected Response Body -- Json with my membership Request.
#     """

#     def __init__(self,SSLVerify=False, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/requests/'
#         self.requestID = -1

#     def setup(self):
#         self.predata = {
#             'SESS_CDE' : '201501',
#             'ACT_CDE' : activity_code,
#             'ID_NUM' : random_id_number,
#             'DATE_SENT' : '07/06/2016',
#             'PART_CDE' : 'MEMBR',
#             'COMMENT_TXT' : 'Generated by the Raymond Test Suite. IGNORE'
#         }
#         r = api.postAsJson(self.session, self.url, self.predata)
#         try:
#             self.requestID = r.json()['REQUEST_ID']
#         except (KeyError, ValueError):
#             self.log_error('Error in setup for {0}.'.format(self.test_name))

#     def test(self):
#         response = api.get(self.session, self.url + str(self.requestID))
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not ('RequestID' in response.json()):
#                 self.log_error('Expected RequestID in response, got {0}.'.format(response.json()))

#     def cleanup(self):
#         if self.requestID < 0: # ID is still -1
#             self.log_error('Expected valid request id, got {0}.'.format(self.requestID))
#         else:
#             d = api.delete(self.session, self.url + str(self.requestID))
#             if not d.status_code == 200:
#                 self.log_error('Error in cleanup for {0}'.format(self.test_name))


# class get_all_my_membership_requests___regular_member(TestCase):
#     """ Verify that a regular member can retrieve all requests belonging to them.
    
#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as regular member.
#     Expectations:
#     Endpoint -- api/requests/student/:id
#     Expected Status Code -- 200 OK
#     Expected Response Body -- A list of membership requests 
#     """

#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/requests/student/' + str(my_id_number)

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Expected list, got {0}.'.format(response.json()))
    
class get_membership_requests_for_someone_else___regular_member(TestCase):
    """ Verify that a regular member cannot get the membership requests of somone else.
    
    Pre-Conditions:
    Valid Authentication Header
    Authenticated as regular member
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

class get_membership_requests_for_activity___regular_member(TestCase):
    """ Verify that a regular member can't access memberships requests for activity.

    Pre-Conditions:
    Valid Authentication Header
    Authenticated as regular member
    Expectations:
    Endpoint -- api/requests/activity/:id
    Expected Status Code -- 401 Unauthorized
    Expected Response Body -- Empty
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/requests/activity/' + activity_code

    def setup(self):
        # Report if there any current memberships for the Club to avoid false negatives.
        # If I am currently a director of the club, this test should fail.
        response = api.get(self.session, hostURL + 'api/memberships/student/' + str(my_id_number))
        try:
            for membership in response.json():
                if(membership['ActivityCode'] == activity_code and membership['Participation'] in LEADERSHIP_POSITIONS):
                    self.log_error('False Negative: This user is a leader for the activity we are testing.')
        except ValueError:
            self.log_error('We did not get a json response back during setup.')    

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            self.log_error('We did not get 401 Unauthorized.')
        if response.text:
            self.log_error('We got a non-empty response body.')


class post_valid_membership_request___regular_member(TestCase):
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
            except KeyError:
                self.log_error('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))

    def cleanup(self):
        # We try to delete the request we created
        if self.requestID < 0: # The creation wasn't successful
            self.log_error('Could not delete request with id of {0}'.format(self.requestID))
        else:
            d = api.delete(self.session, self.url + str(self.requestID))
            if not d.status_code == 200:
                self.log_error('There was a problem performing cleanup for {0}'.format(self.test_name))

class post_membership_request_for_someone_else___regular_member(TestCase):
    """ Verify that we can't create a membership request for someone else.

    Pre-conditions:
    Valid Authentication Header.
    Authenticated as Regular member.
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

# class put_membership_request___regular_member(TestCase):
#     """ Verify that regular member can't edit a membership request.

#     Pre-Conditions:
#     Valid Authorization Header.
#     Authenticated as regular member.
#     Expectations:
#     Endpoint -- api/requests/:id
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + '/api/requests/'
#         self.predata = {}
#         self.data = {}
#         self.requestID = -1

#     def setup(self):
#         self.predata = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'MEMBR',
#             'DATE_SENT' : '07/06/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         response = api.postAsJson(self.session, self.url, self.predata)
#         try:
#             self.requestID = response.json()['REQUEST_ID']
#             self.data = {
#                 'REQUEST_ID': self.requestID,
#                 'ACT_CDE': activity_code,
#                 'SESS_CDE' : '201501',
#                 'ID_NUM': my_id_number,
#                 'PART_CDE':'MEMBR',
#                 'DATE_SENT' : '07/06/2016',
#                 'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'            
#                 }            
#         except ValueError:
#             self.log_error('Error performing setup for {0}.'.format(self.test_name))


#     def test(self):
#         response = api.putAsJson(self.session, self.url + str(self.requestID), self.data)
#         if not response.status_code == 401:
#             self.log_error('We did not get a 401 Unauthorized.')
#         if response.text:
#             self.log_error('We got a non-empty response body.')

#     def cleanup(self):
#         d = api.delete(self.session, self.url + str(self.requestID))
#         if not d.status_code == 200:
#             self.log_error('There was error performing cleanup for {0}.'.format(self.test_name))


# class approve_my_request___regular_member(TestCase):
#     """ Verify that a regular member cannot approve his/her request

#     Pre-Conditions:
#     Valid Authentication Header
#     Authenticated as a regular member
#     Expectations:
#     Endpoints -- api/requests/:id/approve
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/requests/'
#         self.data = {}
#         self.requestID = -1

#     def setup(self):
#         #Create a memberships request for the trash club.
#         self.data = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'MEMBR',
#             'DATE_SENT' : '07/06/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         response = api.postAsJson(self.session, self.url, self.data)
#         if not response.status_code == 201:
#             self.log_error('Error in setup for {0}. Expected 201 Created, got {1}.'.format(self.test_name, response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Error in setup for {0}. Expected json response, got {1}.'.format(self.test_name, response.text))
#         else:
#             try:
#                 self.requestID = response.json()['REQUEST_ID']
#             except KeyError:
#                 self.log_error('Error in setup for {0}. Expected REQUEST_ID in response, got {1}.'.format(self.test_name, response.json()))
        
#     def test(self):
#         response = api.postAsJson(self.session, self.url + str(self.requestID) + '/approve', None)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))

#     def cleanup(self):
#         # We try to delete the request we made
#         if self.requestID < 0: # The request was not successful
#             self.log_error('Error in cleanup for {0}. Expected valid request ID, got {1}.'.format(self.test_name, self.requestID))
#         else:
#             d = api.delete(self.session, self.url + str(self.requestID))
#             if not d.status_code == 200:
#                 self.log_error('Error in cleanup for {0}. Expected 200 OK, got {1}.'.format(self.test_name, d.status_code))


# class deny_my_request___regular_member(TestCase):
#     """ Verify that a regular member cannot deny his/her request

#     Pre-Conditions:
#     Valid Authentication Header
#     Authenticated as a regular member
#     Expectations:
#     Endpoints -- api/requests/:id/deny
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/requests/'
#         self.data = {}
#         self.requestID = -1

#     def setup(self):
#         #Create a memberships request for the trash club.
#         self.data = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'MEMBR',
#             'DATE_SENT' : '07/06/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         response = api.postAsJson(self.session, self.url, self.data)
#         if not response.status_code == 201:
#             self.log_error('Error in setup for {0}. Expected 201 Created, got {1}.'.format(self.test_name, response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Error in setup for {0}. Expected json response, got {1}.'.format(self.test_name, response.text))
#         else:
#             try:
#                 self.requestID = response.json()['REQUEST_ID']
#             except KeyError:
#                 self.log_error('Error in setup for {0}. Expected REQUEST_ID in response, got {1}.'.format(self.test_name, response.json()))
        
#     def test(self):
#         response = api.postAsJson(self.session, self.url + str(self.requestID) + '/deny', None)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))

#     def cleanup(self):
#         # We try to delete the request we made
#         if self.requestID < 0: # The request was not successful
#             self.log_error('Error in cleanup for {0}. Expected valid request ID, got {1}.'.format(self.test_name, self.requestID))
#         else:
#             d = api.delete(self.session, self.url + str(self.requestID))
#             if not d.status_code == 200:
#                 self.log_error('Error in cleanup for {0}. Expected 200 OK, got {1}.'.format(self.test_name, d.status_code))




# class delete_my_membership_request___regular_member(TestCase):
#     """ Verify that regular member can delete his/her membership request

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/requests/:id
#     Expected Status Code -- 200 OK 
#     Expected Response Body -- Json object with deleted resource
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/requests/'
#         self.predata = {}
#         self.requestID = -1

#     def setup(self):
#         self.predata = {
#             'ACT_CDE': activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM': my_id_number,
#             'PART_CDE':'MEMBR',
#             'DATE_SENT' : '07/06/2016',
#             'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#             }
#         response = api.postAsJson(self.session, self.url, self.predata)
#         try:
#             self.requestID = response.json()['REQUEST_ID']
#         except ValueError:
#             self.log_error('Error on setup for {0}.'.format(self.test_name))

#     def test(self):
#         response = api.delete(self.session, self.url + str(self.requestID))
#         if not response.status_code == 200:
#             self.log_error('Expected Status Code 200, got {0}.'.format(response.status_code))
#         if not response.text:
#             self.log_error('We got an empty response body.')
#         try:
#             deletedData = response.json()
#             deletedDataID = deletedData['REQUEST_ID']
#         except ValueError:
#             self.log_error('We did not get a json response.')
#         else:
#             if not deletedDataID == self.requestID:
#                 self.log_error('The resource we deleted is different from the one we created.')
        

# # # # # # # # # # #
# # SUPERVISOR TESTS #
# # # # # # # # # # #


# class get_all_supervisors___regular_member(TestCase):
#     """ Verify that a regular member cannot view all supervisors.

#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as Regular member
#     Expectations:
#     Endpoint -- api/supervisors/
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/supervisors/'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 401:
#             self.log_error('Expected Status Code 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Got non-empty response body.')

# class get_supervisors_for_activity___regular_member(TestCase):
#     """ Verify that a regular member can get supervisors for activity

#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as regular member.
#     Expectations:
#     Endpoint -- api/supervisors/activity/:id
#     Expected Status Code -- 200 OK
#     Expected Response Body -- list of json objects
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/supervisors/activity/' + activity_code

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         if not response.text:
#             self.log_error('Got an empty response body.')
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Did not get a json response.')
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Did not get a list.')

# # class post_supervisor___regular_member(TestCase):
# #     """ verify that a regular member can't add a supervisor

# #     Pre-Conditions:
# #     Valid Authentication Header.
# #     Authenticated as a regular member
# #     Expectations:
# #     Endpoint -- api/supervisors/
# #     Expected Status Code -- 401 Unauthorized
# #     Expected Response Body -- Empty
# #     """
# #     def __init__(self , session=None):
# #         super().__init__(session)
# #         self.url = hostURL + 'api/supervisors/'
# #         self.data = {}
# #         self.supervisorID = -1

# #     def setup(self):
# #         self.data = {
# #             'ID_NUM' : my_id_number,
# #             'ACT_CDE' : activity_code,
# #             'SESS_CDE' : '201501'
# #             }

# #     def test(self):
# #         response = api.postAsJson(self.session, self.url, self.data)
# #         if not response.status_code == 401:
# #             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
# #         if response.text:
# #             self.log_error('Expected empty response body, got {0}.'.format(response.text))
# #         if response.status_code == 201:
# #             self.log_error('Delete the unauthorized resource...')
# #             try:
# #                 self.supervisorID = response.json()['SUP_ID']
# #             except ValueError:
# #                 self.log_error('Problem accessing compromised resource.')
        

# #     def cleanup(self):
# #         if self.supervisorID > 0: # Unauthorized creation took place
# #             d = api.delete(self.session, self.url + str(self.supervisorID))
# #             if response.status_code == 200: # Deletion was not successful
# #                 self.log_error('Unauthorized resource deleted.')
# #             else:
# #                 self.log_error('Error deleting Unauthorized resource.')

# class put_supervisor___regular_member(TestCase):
#     """ Verify that a regular member can't edit an existing supervisor

#     Pre-Conditions:
#     Valid Authentication Header.
#     Authenticated as regular member.
#     Expectations:
#     Endpoint -- api/supervisors/:id
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         # Trying to update a random supervisor
#         self.url = hostURL + 'api/supervisors/'

#         tempID = 0
#         response = api.get(self.session, self.url + str(tempID))
#         # Iterate until we find an actual supervisor.
#         while response.status_code == 404:
#             tempID = tempID + 1
#             response = api.get(self.session, self.url + str(tempID))

#         self.data = {
#             'SUP_ID' : tempID,
#             'ACT_CDE' : activity_code,
#             'SESS_CDE' : '201501',
#             'ID_NUM' : my_id_number
#         }

#         self.supervisorID = -1
        
#     def test(self):
#         response = api.putAsJson(self.session, self.url + str(self.data['SUP_ID']), self.data)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))
#         if response.status_code == 200: # If supervisor was updated
#             self.log_error('Unauthorized update. Attempting to delete...')
#             try:
#                 self.supervisorID = response.json()['SUP_ID']
#             except (ValueError, KeyError):
#                 self.log_error('Error accessing compromised supervisor.')
                        
#     def cleanup(self):
#         if self.supervisorID > 0: # The supervisor was updated.
#             d = api.delete(self.session, self.url)
#             if d.status_code == 200:
#                 self.log_error('Compromised resource deleted.')
#             else:
#                 self.log_error('Unable to delete compromised resource.')


# class delete_supervisor___regular_member(TestCase):
#     """ Verify that regular member can't delete a supervisor 

#     Pre-Conditions:
#     Valid Authentication Header
#     Authenticated as regular member
#     Expectations:
#     Endpoint -- api/supervisors/:id
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/supervisors/2'

#     def test(self):
#         response = api.delete(self.session, self.url)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty body, got {0}.'.format(response.text))


# # # # # # # # # #
# # ACTIVITY TESTS #
# # # # # # # # # #

# class get_all_activities___regular_member(TestCase):
#     """ Verify that a regular member can get all activities.

#     Pre-Conditions:
#     Valid Authentication Header.
#     Expectations:
#     Endpoint -- api/activities/
#     Expected Status Code -- 200 OK
#     Expected Response Body -- List of activities
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/activities/'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK , got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(respons.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Expected list, got {0}.'.format(response.json()))

# class get_one_activity___regular_member(TestCase):
#     """ Verify that a regular member can a single activity.

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/activities
#     Expected Status Code -- 200 OK
#     Expected Response Body -- Json object with activity resource
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/activities/' + activity_code

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             try:
#                 response.json()['ActivityCode']
#             except KeyError:
#                 self.log_error('Expected ACT_CDE in response, got {0}.'.format(response.json()))

# class get_activities_for_session___regular_member(TestCase):
#     """ Verify that a regular member can get all activities for specific session.

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/activities/session/:id
#     Expected Status Code -- 200 OK 
#     Expected Response Body -- list of activities
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/activities/session/201501'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.json()))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.json()))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Expected list, got {0}.'.format(response.json()))

# class update_activity___regular_member(TestCase):
#     """ Verify that a regular member cannot update activity information.

#     Pre-Conditions:
#     Valid Authentication Header
#     Authenticated as regular member
#     Expectations:
#     Endpoints -- api/activities/:id
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """

#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/activities/' + activity_code
#         self.data = {}

#     def setup(self):
#         # Report if there any current memberships for the Club to avoid false negatives.
#         # If I am currently a director of the club, this test should fail.
#         response = api.get(self.session, hostURL + 'api/memberships/student/' + str(my_id_number))
#         try:
#             for membership in response.json():
#                 if(membership['ActivityCode'] == activity_code and membership['Participation'] in LEADERSHIP_POSITIONS):
#                     self.log_error('False Negative: This user is a leader for the activity we are testing.')
#         except ValueError:
#             self.log_error('We did not get a json response back during setup.')
#         else:
#             self.data = {
#                 "ACT_CDE" : activity_code,
#                 "ACT_IMG" : "HACKING INTO SYSTEM AS REGULAR MEMBER",
#                 "ACT_BLURB" : "HACKING INTO SYSTEM AS REGULAR MEMBER",
#                 "ACT_URL" : "HACKING INTO SYSTEM AS REGULAR MEMBER"
#             }

#     def test(self):
#         response = api.putAsJson(self.session, self.url , self.data)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))

#     def cleanup(self):
#         # Don't delete activity even if it was updated. That's too drastic.
#         pass
    

#################
# PROFILE TESTS #
#################

class get_my_profile___regular_member(TestCase):
    """ Verify that a regular member can get a profile of the current user
    
    Pre-Conditions:
    Valid Authentication header
    Expectaions:
    Endpoint -- api/profiles/
    Expected Status Code -- 200 OK
    Expected Respones Body -- A json object of information on own profile
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/'
    def test(self):
         response = api.get(self.session, self.url)
         if not response.status_code == 200:
             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
         try:
             response.json()
         except ValueError:
             self.log_error('Expected Json response body, got {0}.'.format(response.text))


class get_profile_by_username___regular_member(TestCase):
    """ Verify that a regular member can get another person's profile, filtering private information
    Pre-Conditions:
    Valid Authentication Header
    Expectations:
    Endpoint -- api/profiles/:username
    Expected Status Code -- 200 Ok
    Expected Response Body -- list of information on the user without private info
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/' + username

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got{0}.'.format(response.text))


class get_college_role_by_username___regular_member(TestCase):
    """ Verify that a regular member can get a college role of the current user
    Pre-Conditions:
    Valid Authentication Header
    Expectations:
    Endpoint -- api/profiles/role/:username
    Expected Status Code -- 200 Ok
    Expected Response Body -- list of information on the user without private info
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/role/' + username

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got{0}.'.format(response.text))



class get_image___regular_member(TestCase):
    """ Verify that a regular member can get a profile image of the current user
    Pre-Conditions:
    Valid Authentication Header
    Expectations:
    Endpoint -- api/profiles/image
    Expected Status Code -- 200 Ok
    Expected Response Body -- image path of the current user
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/image/'

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got{0}.'.format(response.text))



class get_image_by_username___regular_member(TestCase):
    """ Verify that a regular member can get a profile image of someone else
    Pre-Conditions:
    Valid Authentication Header
    Expectations:
    Endpoint -- api/profiles/image/:username
    Expected Status Code -- 200 Ok
    Expected Response Body -- image path of another user
    """
    def __init__(self , session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/image/' + username

    def test(self):
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            self.log_error('Expected Json response body, got{0}.'.format(response.text))
        

# Often does not PASS due to permission issue to the image columns in WebSQL/CCT databases
class post_image___regular_member(TestCase):
    """ Verify that a user can upload a profile image
    Pre-Conditions:
    Authenticated as Regular member.
    Expectations:
    Endpoint -- api/profiles/image/
    Expected Status Code -- 200 OK 
    Expected Content -- 
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/image/'
        self.data = {
            'ID': my_id_number,
            'FILE_PATH': #File path of the image on the user's computer,
            'FILE_NAME': #Barcode ID of the user
        }
    
    def test(self):
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
            
    def cleanup(self):
        self.data = {
            'ID': my_id_number,
            'FILE_PATH': None,
            'FILE_NAME': None
        }
        d = api.post(self.session, self.url + 'reset/', self.data)
        if not d.status_code == 200:
            self.log_error('There was a problem performing cleanup for {0}'.format(self.test_name))



class post_reset_image___regular_member(TestCase):
    """ Verify that a user can reset a profile image
    Pre-Conditions:
    Authenticated as Regular member.
    Expectations:
    Endpoint -- api/profiles/image/reset/
    Expected Status Code -- 200 OK 
    Expected Content -- 
    """

    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/image/reset/'
        self.data = {
            'ID': my_id_number,
            'FILE_PATH': None,
            'FILE_NAME': None
        }
        self.requestID = -1

    def test(self):
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 Created, got {0}.'.format(response.status_code))
            


class put_social_media_links___regular_member(TestCase):
    """ Verify that a user can add and edit social media links
    Pre-Conditions:
    Authenticated as Regular member.
    Expectations:
    Endpoint -- api/profiles/:type
    Expected Status Code -- 200 OK
    Expected Content --
    """

    # Any other SNS names can be used to replace 'facebook' to test
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/facebook/'
        self.data = {
            'facebook': #'URL of any SNS including the domain name'
        }
        
    def test(self):
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        
    def cleanup(self):
        self.resetdata = {
            'facebook': ''
        }
        d = api.put(self.session, self.url, self.resetdata)
        if not d.status_code == 200:
            self.log_error('There was a problem performing cleanup for {0}'.format(self.test_name))



class put_mobile_privacy___regular_member(TestCase):
    """ Verify that a user can add and edit social media links
    Pre-Conditions:
    Authenticated as Regular member.
    Expectations:
    Endpoint -- api/profiles/mobile_privacy/:value (Y or N)
    Expected Status Code -- 200 OK
    Expected Content --
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/mobile_privacy/Y/'
        self.data = {
            'IsMobilePhonePrivate': 'Y'
        }
        
    def test(self):
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        
    def cleanup(self):
        self.resetdata = {
            'IsMobilePhonePrivate': 'N'
        }
        d = api.put(self.session, self.url, self.resetdata)
        if not d.status_code == 200:
            self.log_error('There was a problem performing cleanup for {0}'.format(self.test_name))


class put_image_privacy___regular_member(TestCase):
    """ Verify that a user can add and edit social media links
    Pre-Conditions:
    Authenticated as Regular member.
    Expectations:
    Endpoint -- api/profiles/image_privacy/:value (Y or N)
    Expected Status Code -- 200 OK
    Expected Content --
    """
    def __init__(self, session=None):
        super().__init__(session)
        self.url = hostURL + 'api/profiles/image_privacy/Y/'
        self.data = {
            'show_pic': 'Y'
        }
        
    def test(self):
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        
    def cleanup(self):
        self.resetdata = {
            'show_pic': 'N'
        }
        d = api.put(self.session, self.url, self.resetdata)
        if not d.status_code == 200:
            self.log_error('There was a problem performing cleanup for {0}'.format(self.test_name))



# # # # # # # # # # # # # 
# # PARTICIPATIONS TEST #
# # # # # # # # # # # # #


# class get_all_participations___regular_member(TestCase):
#     """ Verify that a regular member can get all participations

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/participations
#     Expected Status Code -- 200 OK
#     Expected Response Body -- List of all participations
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/participations'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Expected list, got {0}.'.format(response.json()))

# class get_one_participation___regular_member(TestCase):
#     """ Verify that a regular member can get a specific participation object

#     Pre-Conditions:
#     Valud Authentication Header
#     Expectations:
#     Endpoint -- api/participations
#     Expected Status Code -- 200 OK
#     Expected Response Body -- A participation object
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/participations/MEMBR'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueErrror:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             try:
#                 response.json()['ParticipationCode']
#             except KeyError:
#                 self.log_error('Expected ParticipationCode in response, got {0}.'.format(response.json()))


# # # # # # # # # #
# # SESSIONS TEST #
# # # # # # # # # #


# class get_all_sessions___regular_member(TestCase):
#     """ Verify that a regular member can get all session objects

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/sessions/
#     Expected Status Code -- 200 OK
#     Expected Response Body -- List of session resources
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/sessions/'


#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.json()))
#         else:
#             if not (type(response.json()) is list):
#                 self.log_error('Expected list, got {0}.'.format(response.json()))


# class get_one_session___regular_member(TestCase):
#     """ Verify that a regular member can get a session object

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/sessions/:id
#     Expected Status Code -- 200 OK
#     Expected Response Body -- A session resource.
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/sessions/201501'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             try:
#                 response.json()['SessionCode']
#             except KeyError:
#                 self.log_error('Expected SessionCode in response, got {0}.'.format(response.json()))

# # # # # # # # # #
# # STUDENTS TEST #
# # # # # # # # # #


# class get_all_students___regular_member(TestCase):
#     """ Verify that a regular member cannot list all students

#     Pre-Conditions:
#     Valid Authentications Header
#     Expectations:
#     Endpoint -- api/students
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/students/'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))

                
# class get_student_by_id___regular_member(TestCase):
#     """ Verify that a regular member can get a student resource

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/students/:id
#     Expected Status Code -- 200 Ok
#     Expected Response Body -- A json response with the student resource
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/students/' + str(random_id_number)

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got{0}.'.format(response.text))
#         else:
#             try:
#                 response.json()['StudentID']
#             except KeyError:
#                 self.log_error('Expected StudentID in response, got{0}.'.format(response.json()))
        

# class get_student_by_email___regular_member(TestCase):
#     """ Verify that a regular member cannot get a student resource by email.

#     Pre-Conditions:
#     Valid Authentication Header
#     Expectations:
#     Endpoint -- api/students/email/:email
#     Expected Status Code -- 200 OK
#     Expected Response Body -- A json response with the student resource
#     """

#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/students/email/ezeanyinabia.anyanwu@gordon.edu/'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got{0}.'.format(response.text))
#         else:
#             try:
#                 response.json()['StudentID']
#             except KeyError:
#                 self.log_error('Expected StudentID in response, got{0}.'.format(response.json()))

            
# # # # # # # # #
# # EMAIL  TEST #
# # # # # # # # #


# class get_emails_for_activity___regular_member(TestCase):
#     """ Verify that a regular member cannot get the emails for the members of an activity
    
#     Pre-conditions:
#     Valid Authentication Header
#     Authenticated as Regular Member
#     Expectations:
#     Endpoint -- api/emails/activity/:id
#     Expected Status Code -- 401 Unauthorized
#     Expected Response Body -- Empty
#     """
#     def __init__(self , session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/emails/activity/' + activity_code

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 401:
#             self.log_error('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#         if response.text:
#             self.log_error('Expected empty response body, got {0}.'.format(response.text))
    


# class get_emails_for_leaders___regular_member(TestCase):
#     """ Verify that a regular member can get the emails for any activity leader
    
#     Pre-Conditions:
#     Valid Authentication header
#     Expectaions:
#     Endpoint -- api/emails/activity/:id/leaders
#     Expected Status Code -- 200 OK
#     Expected Respones Body -- Json response with a list of emails
#     """
#     def __init__(self, session=None):
#         super().__init__(session)
#         self.url = hostURL + 'api/emails/activity/' + activity_code + '/leaders'

#     def test(self):
#         response = api.get(self.session, self.url)
#         if not response.status_code == 200:
#             self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
#         try:
#             response.json()
#         except ValueError:
#             self.log_error('Expected Json response body, got {0}.'.format(response.text))
#         else:
#             try:
#                 response.json()[0]['Email']
#             except KeyError:
#                 self.log_error('Expected Email in response, got{0}.'.format(response.json()))

if __name__ == '__main__':
    main()
