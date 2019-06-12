import pytest
import warnings
# Regular Member Test Suite for Project Raymond
from pytest_components import requests

# Public configuration options
import test_config
# Private configuration options
import test_credentials
import pytest_components as api

# Constants
LEADERSHIP_POSITIONS = test_config.leadership_positions
REQUEST_STATUS_APPROVED = 'Approved'
REQUEST_STATUS_DENIED = 'Denied'
REQUEST_STATUS_PENDING = 'Pending'

# Configuration Details 
username = test_credentials.username
password = test_credentials.password
my_id_number = test_credentials.id_number
grant_payload = { 'username':username, 'password':password, 'grant_type':'password' }

leader_username = test_credentials.username_activity_leader
leader_password = test_credentials.password_activity_leader
leader_id_number = test_credentials.id_number_activity_leader
leader_grant_payload = { 'username':leader_username, 'password':leader_password, 'grant_type':'password' }

random_id_number = test_config.random_id_number
activity_code = test_config.activity_code
session_code = test_config.session_code
member_positions = test_config.member_positions
date = test_config.date
begin_date = test_config.begin_date
end_date = test_config.end_date
comments = test_config.comments
hostURL = test_config.hostURL
FILE_PATH = test_config.FILE_PATH
FILE_NAME = test_config.FILE_NAME

class testCase:
#   Parent Test Case class to be inherited
#  Describes attribute references common to all child classes.
#   Describtes Expectations for the Test Case.
    
    def createMemberAuthorizedSession(self):
        # Create an authorized session to test authorized calls.
        r = requests.post(hostURL+'token',grant_payload)
        access_token = r.json()["access_token"]
        authorization_header = "Bearer " + access_token
        authorized_session = requests.Session()
        authorized_session.verify = True
        authorized_session.headers.update({ "Authorization":authorization_header })
        return authorized_session

    def createLeaderAuthorizedSession(self):
        # Create an authorized session to test authorized calls.
        r = requests.post(hostURL+'token',leader_grant_payload)
        access_token = r.json()["access_token"]
        authorization_header = "Bearer " + access_token
        authorized_session = requests.Session()
        authorized_session.verify = True
        authorized_session.headers.update({ "Authorization":authorization_header })
        return authorized_session

class Test_allMemberTest(testCase):

# # # # # # # # # # # #
# AUTHENTICATION TESTS #
# # # # # # # # # # # #

#    Given valid credentials, verify that authentication is successful.
#
#    Expectations:
#    Endpoint --  token/
#    Expected Status code -- 200 Ok
#    Expected Content -- Json Object with access_token attribute.
    def test_authenticate_with_valid_credentials___regular_member(self):
        session = None
        self.session = requests.Session()
        self.url = hostURL + 'token'
        self.token_payload = { 'username':username, 'password':password, 'grant_type':'password' }
        response = api.post(self.session, self.url, self.token_payload)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        if not 'access_token' in response.json():
            pytest.fail('Expected access token in response, got {0}.'.format(response.json()))
    
# # # # # # # # # # # #
# EVENTS & CLAW TESTS #
# # # # # # # # # # # #

#    Verify that a regular member can get all events by type_ID
#    
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/25Live/type/:Event_OR_Type_ID
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources
#type_ID 10 does not return any data, but 14 does 
    def test_get_all_events___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        warnings.warn(UserWarning("Type_ID might not give any data"))
        self.url = hostURL + 'api/events/25Live/type/10'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a regular member can get all events by multiple type_ID
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/25Live/type/:Event_OR_Type_ID
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources
    def test_get_all_events_multiple___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        warnings.warn(UserWarning("Type_ID might not give any data"))
        self.url = hostURL + 'api/events/25Live/type/10$11$12$14'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a regular member can get all upcoming chapel events (category_ID = 85)
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/25Live/CLAW
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources
    def test_get_all_claw___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/events/25Live/CLAW'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a regular member can get all events in 25Live under predefined categories
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/25Live/All
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources

    def test_get_all_25Live___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/events/25Live/All'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a regular member can get information on specific event on 25Live
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/25Live/:Event_ID (2911 = Chapel)
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources

    def test_get_all_25Live_by_event_id___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/events/25Live/2911'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))
        assert response.json()[0]['Organization'] == "Chapel Office"

#    Verify that a regular member can get information on specific event on 25Live
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/25Live/All
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources
    
    def test_get_all_25Live_by_multiple_event_id___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/events/25Live/All/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#   Verify that a regular member can get information on chapel events attended    
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/chapel/:user_name
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources

    def test_get_all_chapel___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/events/chapel/sam.nguyen/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a regular member can get information on chapel events attended by specific school term
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/events/chapel/:user_name
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- list of all events resources
    def test_get_all_chapel_by_term___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/events/chapel/' + username + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

# # # # # # # # # # #
# MEMBERSHIP TESTS #
# # # # # # # # # # #

#    Verify that a regular member can fetch all leaders for a specific activity.
#
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as regular member.
#    Expectations:
#    Endpoint -- api/memberships/activity/:id/leaders
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json objects.
    def test_get_leader_memberships_for_an_activity___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/memberships/activity/' + activity_code + '/leaders'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
            assert response.json()[7]['Participation'] == "LEAD"
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')

# # # # # # # # # # # # # # #
# # MEMBERSHIP REQUEST TESTS #
# # # # # # # # # # # # # # #
#    Verify that a regular member cannot access all membership requests.
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as regular member.
#    Expectations:
#    Endpoint -- api/requests/
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Content -- Empty response content.

    def test_get_all_membership_requests___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            pytest.fail('Expected empty response body, got {0}.'.format(response.text))

#    Verify that a regular member cannot get the membership requests of somone else.
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as regular member
#    Expectations:
#    Endpoint -- api/requests/student/:id
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Body -- Empty
    def test_get_membership_requests_for_someone_else___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/requests/student/' + str(random_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            pytest.fail('Expected empty response bodty, got {0}.'.format(response.text))

#    Verify that a regular member can't access memberships requests for activity.
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as regular member
#    Expectations:
#    Endpoint -- api/requests/activity/:id
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Body -- Empty

#   Passed with activity code 'TRAS', but not AJG because studenttest is a leader for AJG
    def test_get_membership_requests_for_activity___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/requests/activity/' + 'TRAS'
        # Report if there any current memberships for the Club to avoid false negatives.
        # If I am currently a director of the club, this test should fail.
        response = api.get(self.session, hostURL + 'api/memberships/student/' + str(my_id_number))
        try:
            for membership in response.json():
                if(membership['ActivityCode'] == activity_code and membership['Participation'] in LEADERSHIP_POSITIONS):
                    pytest.fail('False Negative: This user is a leader for the activity we are testing.')
        except ValueError:
            pytest.fail('We did not get a json response back during setup.')    

        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('We did not get 401 Unauthorized.')
        if response.text:
            pytest.fail('We got a non-empty response body.')

#    Verify that we can create a membership request.
#    Pre-conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoints -- api/requests/
#    Expected Status Cpde -- 201 Created.
#    Expected Content -- A Json object with a REQUEST_ID attribute.
#    session code 201510 does not work 
    def test_post_valid_membership_request___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        # We will get the actual id when we post.
        # Setting it -1 to check later that we got an id from the post.
        self.requestID = -1

        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json response, got {0}.'.format(response.text))
        try:
            self.requestID = response.json()['REQUEST_ID']
            if self.requestID >= 0:
                api.delete(self.session, self.url + str(self.requestID))
        except KeyError:
            pytest.fail('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))


#    Verify that we can't create a membership request for someone else.
#    Pre-conditions:
#    Valid Authentication Header.
#    Authenticated as Regular member.
#    Expectations:
#    Endpoints -- api/requests/
#    Expected Status Code -- 401 Unauthorized.
#    Expected Response Content -- Empty Response.
#    look up for test_config.py for the data configuration

    def test_post_membership_request_for_someone_else___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number,
            'PART_CDE': member_positions,
            'DATE_SENT' : date,
            'COMMENT_TXT': comments
            }
        # We will get the actual id when we post.
        self.requestID = -1
        response = api.post(self.session, self.url, self.data)

        if response.status_code == 201:
            try:
                self.requestID = response.json()['REQUEST_ID']
                if self.requestID >=  0:
                    api.delete(self.session, self.url + str(self.requestID))
                    pytest.fail('Request {0} was created even though it was supposed to be unauthorized'.format(self.requestID))
            except (ValueError, KeyError):
                pytest.fail('Error in test')
        elif not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
    

#################
# PROFILE TESTS #
#################

#    Verify that a regular member can get a profile of the current user
#    
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/profiles/
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- A json object of information on own profile

    def test_get_my_profile___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            assert response.json()['AD_Username'] == '360.StudentTest'
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
    
#    Verify that a regular member can get another person's profile, filtering private information
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/profiles/:username
#    Expected Status Code -- 200 Ok
#    Expected Response Body -- list of information on the user without private info

    def test_get_profile_by_username___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/' + username +'/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            assert response.json()['AD_Username'] == '360.StudentTest'
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'.format(response.text))

#   Verify that a regular member can get a college role of the current user
#   Pre-Conditions:
#   Valid Authentication Header
#   Expectations:
#   Endpoint -- api/profiles/role/:username
#   Expected Status Code -- 200 Ok
#   Expected Response Body -- list of information on the user without private info

    def test_get_college_role_by_username___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/role/' + username + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            assert response.json()== 'student'
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'.format(response.text))

#    Verify that a regular member can get a profile image of the current user
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/profiles/image
#    Expected Status Code -- 200 Ok
#    Expected Response Body -- image path of the current user

    def test_get_image___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/image/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            #   Cannot assert because file name is too big
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'.format(response.text))

#    Verify that a regular member can get a profile image of someone else
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/profiles/image/:username
#    Expected Status Code -- 200 Ok
#    Expected Response Body -- image path of another user
    def test_get_image_by_username___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/image/' + username + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'.format(response.text))

#    Verify that a user can upload a profile image
#    Pre-Conditions:
#    Authenticated as Regular member.
#    Expectations:
#    Endpoint -- api/profiles/image/
#    Expected Status Code -- 200 OK 
#    Expected Content -- 
#    look up for test_config.py for the data configuration

    def test_post_image___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/image/'
        self.data = {
            'ID': my_id_number,
            'FILE_PATH': FILE_PATH, #File path of the image on the user's computer,
            'FILE_NAME': FILE_NAME  #Barcode ID of the user
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        self.data = {
            'ID': my_id_number,
            'FILE_PATH': FILE_PATH,
            'FILE_NAME': FILE_NAME
        }
        d = api.post(self.session, self.url + 'reset/', self.data)
        if not d.status_code == 200:
            pytest.fail('There was a problem performing cleanup')

#    Verify that a user can reset a profile image
#    Pre-Conditions:
#    Authenticated as Regular member.
#    Expectations:
#    Endpoint -- api/profiles/image/reset/
#    Expected Status Code -- 200 OK 
#    Expected Content -- 
#    look up for test_config.py for the data configuration

    def test_post_reset_image___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/image/reset/'
        self.data = {
            'ID': my_id_number,
            'FILE_PATH': FILE_PATH,
            'FILE_NAME': FILE_NAME
        }
        self.requestID = -1
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 Created, got {0}.'.format(response.status_code))
#    Verify that a user can add and edit social media links
#    Pre-Conditions:
#    Authenticated as Regular member.
#    Expectations:
#    Endpoint -- api/profiles/:type
#    Expected Status Code -- 200 OK
#    Expected Content --
#    look up for test_config.py for the data configuration

    def test_put_social_media_links___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/facebook/'
        self.data = {
            'facebook': 'https://www.facebook.com/360.studenttest' #'URL of any SNS including the domain name'
        }
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        self.resetdata = {
            'facebook': ''
        }
        d = api.put(self.session, self.url, self.resetdata)
        if not d.status_code == 200:
            pytest.fail('There was a problem performing cleanup')

#    Verify that a user can add and edit social media links
#    Pre-Conditions:
#    Authenticated as Regular member.
#    Expectations:
#    Endpoint -- api/profiles/mobile_privacy/:value (Y or N)
#    Expected Status Code -- 200 OK
#    Expected Content --

    def test_put_mobile_privacy___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/mobile_privacy/Y/'
        self.data = {
            'IsMobilePhonePrivate': 'Y'
        }
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        self.resetdata = {
            'IsMobilePhonePrivate': 'N'
        }
        d = api.put(self.session, self.url, self.resetdata)
        if not d.status_code == 200:
            pytest.fail('There was a problem performing cleanup')

#    Verify that a user can add and edit social media links
#    Pre-Conditions:
#    Authenticated as Regular member.
#    Expectations:
#    Endpoint -- api/profiles/image_privacy/:value (Y or N)
#    Expected Status Code -- 200 OK
#    Expected Content --
    def test_put_image_privacy___regular_member(self):
        self.session = self.createMemberAuthorizedSession()
        self.url = hostURL + 'api/profiles/image_privacy/Y/'
        self.data = {
            'show_pic': 'Y'
        }
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        self.resetdata = {
            'show_pic': 'N'
        }
        d = api.put(self.session, self.url, self.resetdata)
        if not d.status_code == 200:
            pytest.fail('There was a problem performing cleanup')

class Test_AllLeaderTest(testCase):
   
# # # # # # # # # # # #
# AUTHENTICATION TESTS #
# # # # # # # # # # # #

#     Given valid credentials, verify that authentication is successful.
#
#    Expectations:
#    Endpoint --  token/
#    Expected Status code -- 200 Ok
#    Expected Content -- Json Object with access_token attribute.

    def test_authenticate_with_valid_credentials___activity_leader(self):
        session = None
        self.session = requests.Session()
        self.url = hostURL + 'token'
        self.token_payload = { 'username':leader_username, 'password':leader_password, 'grant_type':'password' }
        response = api.post(self.session, self.url, self.token_payload)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        if not 'access_token' in response.json():
            pytest.fail('Expected access token in response, got {0}.'.format(response.json()))
    
# # # # # # # # # # #
# MEMBERSHIP TESTS #
# # # # # # # # # # #

#    Test retrieving all membership resources.
#
#    Pre-conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoint -- memberships/
#    Expected Status code -- 200 Ok
#    Expected Content -- List

    def test_get_all_memberships___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list')
    
#    Retrieve a specific membership resource.
#    Pre-conditions:
#    Valid Authentication header
#    Expectations:
#    Endpoint -- api/memberships/:id
#    Expected Status Code -- 200 OK
#    Expected Content -- A Json Object with a MembershipID attribute.

    def test_get_one_membership___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        self.membershipID = -1
        # Find a valid membership id
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Error in setup for {0}. Expected 200 OK, got {1}.'.format(self.test_name, response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Error in setup for {0}. Expected json response body, got {1}.'.format(self.test_name, response.text))
        try:
            self.membershipID = response.json()[0]['MembershipID']
            self.url = self.url + str(self.membershipID)
        except KeyError:
            pytest.fail('Error in setup for {0}. Expected MembershipID in response body, got {1}.'.format(self.test_name, self.response.json()))
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not ('MembershipID' in response.json()):
            pytest.fail('Expected MembershipID in jsob object, got {0}.'.format(response.json()))

#    Verify that a leader can fetch memberships associated with them.
#    Pre-Conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoints -- api/memberships/student/:id
#    Expected Status Code -- 200 OK
#    Expected Reponse Content -- A list of json objects

    def test_get_all_my_memberships___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/student/' + str(my_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')

#    Verify that leader can fetch someone else's memberships.
#    
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as regular member.
#    Expectations
#    Endpoint -- api/memberships/student/:id
#    Expected Status Code -- 200 OK.
#    Expected Response Content --  A list of json objects.

    def test_get_all_memberships_for_someone_else___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/student/' + str(random_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list')

#    Verify that a leader can fetch memberships for an activity.
#    Pre-Conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoint -- api/memberships/activity/:id
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json Objects.

    def test_get_memberships_for_an_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/activity/' + activity_code
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')

#    Verify that a leader can fetch all leaders for a specific activity.
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as regular member.
#    Expectations:
#    Endpoint -- api/memberships/activity/:id/leaders
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json objects.

    def test_get_leader_memberships_for_an_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/activity/' + activity_code + '/leaders'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')

#    Verify that an activity leader can create a Guest membership for someone.
#    Pre-conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoints -- api/memberships/
#    Expected Statis Cpde -- 201 Created.
#    Expected Content -- A Json object with a MEMBERSHIP_ID attribute.
#    Session code is too old so it returns error 
    def test_post_new_guest_membership_for_someone_else__activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()

        self.url = hostURL + 'api/memberships/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number,
            'PART_CDE':'GUEST',
            'BEGIN_DTE':'06/10/2016',
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
           }
       # We will get the actual id when we post.
       # Setting it -1 to check later that we got an id from the post.
        self.createdMembershipID = -1

        response = api.postAsJson(self.session, self.url, self.data)
        if response.status_code == 201:
            if not ('MEMBERSHIP_ID' in response.json()):
                pytest.fail('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
            else:
                self.createdMembershipID = response.json()['MEMBERSHIP_ID']
        else:
            pytest.fail('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        api.delete(self.session, self.url + str(self.createdMembershipID))


#    Verify that an activity leader can create a membership for someone.
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as Activity Leader
#    Expectations:
#    Endpoint -- api/memberships/
#    Expected Status Code -- 200 OK
#    Expected Content -- A json response with the created membership

    def test_post_new_membership_for_someone___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        self.createdMembershipID = -1
        # Add a new participant
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number, 
            'PART_CDE':'MEMBR',
            'BEGIN_DTE':'06/10/2016',
            'END_DTE':'07/16/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }

        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            self.createdMembershipID = response.json()['MEMBERSHIP_ID']
        except KeyError:
            pytest.fail('Expected MEMBERSHIP ID in response, got {0}.'.format(response.json()))
            
        if self.createdMembershipID >= 0: 
            api.delete(self.session, self.url + str(self.createdMembershipID))

#    Verify that an activity leader can assign a new leader
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as Activity Leader
#    Expectations:
#    Endpoint -- api/memberships/
#    Expected Status Code -- 200 OK
#    Expected Content -- A json response with the created membership
    def test_post_new_leader_membership_for_someone___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        self.createdMembershipID = -1
        # Add a new leader
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE':session_code,
            'ID_NUM': random_id_number, 
            'PART_CDE':'LEAD',
            'BEGIN_DTE':begin_date,
            'END_DTE':end_date,
            'COMMENT_TXT':comments
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            self.createdMembershipID = response.json()['MEMBERSHIP_ID']
            if self.createdMembershipID < 0: # The creation was not successful
                pytest.fail('Expected valid memberhsip ID, got {0}.'.format(self.createdMembershipID))
            else:
                d = api.delete(self.session, self.url + str(self.createdMembershipID))
                if not d.status_code == 200:             
                    pytest.fail('Error in cleanup. Expected , got {0}.'.format(d.status_code))
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in response, got {0}.'.format(response.json()))

#    Verify that an activity leader can upgrade a normal membership to leader status.
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as Activity Leader
#    Expectations:
#    Endpoint -- api/memberships/:id
#    Expected Status Code -- 200 OK
#    Expected Content -- A json object with a MEMBERSHIP_ID attribute.

    def test_put_edited_membership_member_to_leader___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        self.createdMembershipID = -1
                            
        # The membership to modify
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number,
            'PART_CDE':'MEMBR', # Is a participant at first.
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
                'SESS_CDE' : session_code,
                'ID_NUM': random_id_number,
                'PART_CDE':'LEAD', # Upgrade him to director.
                'BEGIN_DTE':'02/10/2016', # New start date
                'END_DTE':'07/16/2016',
                'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
                }
        except (KeyError, ValueError):
            pytest.fail('Error in setup.')
        response = api.putAsJson(self.session, self.url + str(self.createdMembershipID), self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            response.json()['MEMBERSHIP_ID']
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
        if self.createdMembershipID >= 0: 
            api.delete(self.session, self.url + str(self.createdMembershipID))

#    Verify that an activity leader can demote a leader membership.
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as Activity Leader
#    Expectations:
#    Endpoint -- api/memberships/:id
#    Expected Status Code -- 200 OK
#    Expected Content -- A json object with a MEMBERSHIP_ID attribute.
 
    def test_put_edited_membership_leader_to_member___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        self.createdMembershipID = -1                           
        # The membership to modify
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number,
            'PART_CDE':'LEAD', # Is a leader at first
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
                'SESS_CDE' : session_code,
                'ID_NUM': random_id_number,
                'PART_CDE':'MEMBR', # Demote him to member
                'BEGIN_DTE':'02/10/2016', # New start date
                'END_DTE':'07/16/2016',
                'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
                }
        except (KeyError, ValueError):
            pytest.fail('Error in setup.')
        response = api.putAsJson(self.session, self.url + str(self.createdMembershipID), self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:                
            response.json()['MEMBERSHIP_ID']
            if self.createdMembershipID < 0: # The Creation wasn't successfull. ID is still -1.
                pytest.fail('Expected valid membership ID, got {0}.'.format(self.createdMembershipID))
            else:
                d = api.delete(self.session, self.url + str(self.createdMembershipID))
                if not d.status_code == 200:
                    api.delete(self.session, self.url)
                    pytest.fail('Error in cleanup')
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in json response, got {0}.'.format(response.json()))
                
#    Verify that a an activity leader can delete someone else's membership.
#    Pre-Conditions:
#    Valid Authentication header.
#    Authenticated as regular member for the activity.
#    Expectations:
#    Endpoint -- api/memberships/
#    Expected Status Code -- 200 OK
#    Expected Response Content -- The membership resource that wad delteed.

    def test_delete_valid_membership___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/memberships/'
        self.createdMembershipID = -1

        # Create a Memerships that we'll eventually delete
        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number,
            'PART_CDE':'MEMBR',
            'BEGIN_DTE':begin_date,
            'END_DTE':end_date,
            'COMMENT_TXT':comments
        }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:
            self.createdMembershipID = r.json()['MEMBERSHIP_ID']
        except (ValueError, KeyError):
            pytest.fail('Error doing setup')
        response = api.delete(self.session, self.url + str(self.createdMembershipID))
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not ('MEMBERSHIP_ID' in response.json()):
            pytest.fail('Expected MEMBERSHIP_ID in response, got {0}.'.format(response.json()))
# # # # # # # # # # # # # #
# MEMBERSHIP REQUEST TESTS #
# # # # # # # # # # # # # #

#    Verify that an activity leader cannot access all membership requests.
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as activity leader.
#    Expectations:
#    Endpoint -- api/requests/
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Content -- Empty response content.

    def test_get_all_membership_requests___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            pytest.fail('Expected empty response body, got {0}.'.format(response.text))

#    Verify that the activity leader can get requests to join the activity he/she is leading
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as activity leader.
#    Expectations:
#    Endpoint -- api/requests/activity/:id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- List of json objects representing the membership requests.

    def test_get_membership_requests_for_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/activity/' + activity_code + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list in response body, got {0}.'.format(response.json()))

#    Verify that an activity leader cannot get the membership requests of someone else.
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as activity leader
#    Expectations:
#    Endpoint -- api/requests/student/:id
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Body -- Empty

    def test_get_membership_requests_for_someone_else___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/student/' + str(random_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        if response.text:
            pytest.fail('Expected empty response bodty, got {0}.'.format(response.text))

#    Verify that an activity leader can retrieve all requests belonging to them. 
#    Pre-Conditions:
#    Valid Authentication Header.
#    Authenticated as regular member.
#    Expectations:
#    Endpoint -- api/requests/student/:id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of membership requests 

    def test_get_all_my_membership_requests___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/student/' + str(leader_id_number) + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            self.log_error('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that we can create a membership request.
#    Pre-conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoints -- api/requests/
#    Expected Status Cpde -- 201 Created.
#    Expected Content -- A Json object with a REQUEST_ID attribute.

    def test_post_valid_membership_request___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
                    }
        # We will get the actual id when we post.
        # Setting it -1 to check later that we got an id from the post.
        self.requestID = -1

        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json response, got {0}.'.format(response.text))
        try:
            self.requestID = response.json()['REQUEST_ID']
            if not response.json()['STATUS'] == REQUEST_STATUS_PENDING:
                pytest.fail('Expected Pending status , got {0}.'.format(resposne.json()))
        except KeyError:
            pytest.fail('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))

        # We try to delete the request we created
        if self.requestID >= 0:
            api.delete(self.session, self.url + str(self.requestID))

#    Verify that we can't create a membership request for someone else.
#    Pre-conditions:
#    Valid Authentication Header.
#    Authenticated as Activity Leader member.
#    Expectations:
#    Endpoints -- api/requests/
#    Expected Status Code -- 401 Unauthorized.
#    Expected Response Content -- Empty Response.
    def test_post_membership_request_for_someone_else___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': random_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        # We will get the actual id when we post.
        self.requestID = -1
        response = api.post(self.session, self.url, self.data)
        if response.status_code == 201:
            try:
                self.requestID = response.json()['REQUEST_ID']
            except (ValueError, KeyError):
                pytest.fail('Error in test')
            d = api.delete(self.session, self.url + str(self.requestID))
            if  d.status_code != 200:            
                pytest.fail('Unauthorized resource not deleted.')
        elif not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
        elif response.text:
            pytest.fail('Expected empty response, got {0}.'.format(response.text))
                

#    Verify that an activity leader can't edit a membership request through a put request.
#    Pre-Conditions:
#    Valid Authorization Header.
#    Authenticated as activity leader.
#    Expectations:
#    Endpoint -- api/requests/:id
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Body -- Empty
#
#    def test_put_membership_request___activity_leader(self):
#        self.session = self.createLeaderAuthorizedSession()
#        self.url = hostURL + '/api/requests/'
#        self.requestID = -1
#
#        self.predata = {
#            'ACT_CDE': activity_code,
#            'SESS_CDE' : session_code,
#            'ID_NUM': my_id_number,
#            'PART_CDE':'MEMBR',
#            'DATE_SENT' : '07/06/2016',
#            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
#            }
#        response = api.postAsJson(self.session, self.url, self.predata)
#        try:
#            self.requestID = response.json()['REQUEST_ID']
#            self.data = {
#                'REQUEST_ID': self.requestID,
#                'ACT_CDE': activity_code,
#                'SESS_CDE' : '201501',
#                'ID_NUM': random_id_number, #Changing values to emulate attacker muhahah
#                'PART_CDE':'PART',
#                'DATE_SENT' : '07/06/2016',
#                'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'            
#                }            
#        except ValueError:
#            pytest.fail('Error performing setup')
#
#        response = api.putAsJson(self.session, self.url + str(self.requestID), self.data)
#        if not response.status_code == 401:
#            pytest.fail('Expected 401 Unauthorized, got {0}.'.format(response.status_code))
#        if response.text:
#            pytest.fail('Expected empty response body, got {0}.'.format(response.text))
#        api.delete(self.session, self.url + str(self.requestID))


#    Verify that an activity leader can delete a membership request for his activity
#    Pre-Conditions:
#    Expectations:
#    Endpoints -- api/requests/:id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- The request that was deleted
    def test_delete_membership_request___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.predata = {}
        self.requestID = -1

        self.predata = {
            'ACT_CDE': activity_code,
            'SESS_CDE': session_code,
            'PART_CDE': 'MEMBR',
            'ID_NUM': leader_id_number,
            'DATE_SENT': '07/19/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.predata)
        if not response.status_code == 201:
            pytest.fail('Error in setup. Expected 201 Created, got {0}.'.format(response.status_code))
        else:
            self.requestID = response.json()['REQUEST_ID']
        response = api.delete(self.session, self.url + '/' + str(self.requestID))
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            response.json()['REQUEST_ID']
        except KeyError:
            pytest.fail('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))

#    Verify that the activity leader can accept a request directed at their activity.
#
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as Activity Leader
#    Expectations:
#    Endpoints -- api/requests/:id/approve
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with the request that was accepted.
    def test_allow_someone_to_join_my_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.requestID = -1
        self.membershipID = -1

        #Create a memberships request for the trash club.
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Error in setup. Expected 201 Created, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Error in setup. Expected json response, got {0}.'.format(response.text))
        try:
            self.requestID = response.json()['REQUEST_ID']
        except KeyError:
            pytest.fail('Error in setup. Expected REQUEST_ID in response, got {0}.'.format(response.json()))
        
        response = api.postAsJson(self.session, self.url + str(self.requestID) + '/approve', None)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            self.membershipID = response.json()['MEMBERSHIP_ID']
            if self.requestID < 0:
                pytest.fail('Error in cleanup for {0}. Expected valid request ID, got {1}.'.format(self.test_name, self.requestID))
            else:
                d = api.delete(self.session, self.url + str(self.requestID))
                if not d.status_code == 200:
                    pytest.fail('Error in cleanup for {0}. Expected 200 OK when deleting request, got {1}.'.format(self.test_name, d.status_code))
            if self.membershipID < 0: # membership creatino was not successful
                pytest.fail('Error in cleanup. Expected valid membership ID, got {0}.'.format(self.membershipID))
            else:
                api.delete(self.session, hostURL + 'api/memberships/' + str(self.membershipID))
                if not d.status_code == 200:
                    pytest.fail('Error in cleanup. Expected 200 OK when deleting membership, got {0}.'.format(d.status_code))
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in response bady, got {0}.'.format(response.json()))
                    
#    Verify that the activity leader can deny a request directed at their activity.
#
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as Activity Leader
#    Expectations:
#    Endpoints -- api/requests/:id/deny
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with the request that was denied
    def test_deny_someone_joining_my_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/requests/'
        self.requestID = -1

        #Create a memberships request for the trash club.
        self.data = {
            'ACT_CDE': activity_code,
            'SESS_CDE' : session_code,
            'ID_NUM': my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT':'Generated by the Raymond Test Suite. IGNORE'
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Error in setup for {0}. Expected 201 Created, got {1}.'.format(self.test_name, response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Error in setup for {0}. Expected json response, got {1}.'.format(self.test_name, response.text))
        else:
            try:
                self.requestID = response.json()['REQUEST_ID']
            except KeyError:
                pytest.fail('Error in setup. Expected REQUEST_ID in response, got {0}.'.format(response.json()))
        response = api.postAsJson(self.session, self.url + str(self.requestID) + '/deny', None)
        if not response.status_code == 200:
            pytest.failr('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        else:
            try:
                if not response.json()['STATUS'] == REQUEST_STATUS_DENIED:
                    pytest.fail('Expected approved request, got {0}.'.format(response.json()))
            except KeyError:
                pytest.fail('Expected STATUS in response bady, got {0}.'.format(response.json()))

        api.delete(self.session, self.url + str(self.requestID))

# # # # # # # # #
# ACTIVITY TESTS #
# # # # # # # # #

#    Verify that an activity leader can get all activities.
#
#    Pre-Conditions:
#    Valid Authentication Header.
#    Expectations:
#    Endpoint -- api/activities/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- List of activities
    def test_get_all_activities___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/activities/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK , got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(respons.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that an activity leader can a single activity.
#
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/activities
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json object with activity resource

    def test_get_one_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/activities/' + activity_code + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            response.json()['ActivityCode']
        except KeyError:
            pytest.fail('Expected ACT_CDE in response, got {0}.'.format(response.json()))
#    Verify that an activity leader can get all activities for specific session.
#
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/activities/session/:id
#    Expected Status Code -- 200 OK 
#    Expected Response Body -- list of activities
    def test_get_activities_for_session___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/activities/session/'
        self.sessionID = -1

        response = api.get(self.session, hostURL + 'api/sessions/current')
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Error in setup. Expected json in response body, got {0}.'.format(response.text))
        try:
            self.sessionID = response.json()['SessionCode']
        except KeyError:
            pytest.fail('Error in setup. Expected SessionCode in response body, got {0}.'.format(response.json()))
                        
        response = api.get(self.session, self.url + str(self.sessionID))
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.json()))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.json()))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that an activity leader can update activity information.
#
#    Pre-Conditions:
#    Valid Authentication Header
#    Authenticated as activity leader
#    Expectations:
#    Endpoints -- api/activities/:id
#    Expected Status Code -- 200 Ok
#    Expected Response Body -- Updated activity information

    def test_update_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/activities/' + activity_code + '/'
        self.data = {
            "ACT_CDE" : activity_code,
            "ACT_BLURB" : 'BLURB GENERATED BY THE TEST SUITE.IGNORE',
            "ACT_URL" : 'http://www.lolcats.com/'
        }

        response = api.putAsJson(self.session, self.url , self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json in response body, got {0}.'.format(response.text))
        try:
            response.json()['ACT_CDE']
        except ValueError:
            pytest.fail('Expected ACT_CDE in response body, got {0}.'.format(response.json()))
# # # # # # # # # # # #
# STUDENTEMPLOYMENT TEST #
# # # # # # # # # # # # 

    def test_get_all_student_employment___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/studentemployment/' + str(my_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        else:
            if not (type(response.json()) is list):
                pytest.fail('Expected list, got {0}.'.format(response.json()))
                
# # # # # # # # # # # # 
# PARTICIPATIONS TEST #
# # # # # # # # # # # #

#    Verify that an activity leader can get all participations
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/participations
#    Expected Status Code -- 200 OK
#    Expected Response Body -- List of all participations
    def test_get_all_participations___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/participations/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))
#    Verify that an activity leader can get a specific participation object
#    Pre-Conditions:
#    Valud Authentication Header
#    Expectations:
#    Endpoint -- api/participations
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A participation object
#
#    def test_get_one_participation___activity_leader(self):
#        self.session = self.createLeaderAuthorizedSession()

#   no MEMBR for participation
#        self.url = hostURL + 'api/participations/MEMBR/'
#
#        response = api.get(self.session, self.url)
#        if not response.status_code == 200:
#            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
#        try:
#            response.json()
#        except ValueErrror:
#            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
#        else:
#            try:
#                response.json()['ParticipationCode']
#            except KeyError:
#                pytest.fail('Expected ParticipationCode in response, got {0}.'.format(response.json()))
# # # # # # # # #
# SESSIONS TEST #
# # # # # # # # #

#    Verify that an activity leader can get all session objects
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/sessions/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- List of session resources
#
    #Can't do api/sessions/
#    def test_get_all_sessions___activity_leader(self):
#        self.url = hostURL + 'api/sessions/'
#
#        response = api.get(self.session, self.url)
#        if not response.status_code == 200:
#            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
#        try:
#            response.json()
#        except ValueError:
#            pytest.fail('Expected Json response body, got {0}.'.format(response.json()))
#        else:
#            if not (type(response.json()) is list):
#                pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that an activity leader can get a session object
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/sessions/:id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A session resource.
    def test_get_one_session___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/sessions/' + session_code + '/'

        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            response.json()['SessionCode']
        except KeyError:
            pytest.fail('Expected SessionCode in response, got {0}.'.format(response.json()))

# # # # # # # #
# EMAIL  TEST #
# # # # # # # #

#    Verify that an advisor get a student resource by email.
#
#    Pre-Conditions:
#    Valid Authentication Header
#    Expectations:
#    Endpoint -- api/emails/activity/AJG/advisors/session/201809
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with the student resource
    def test_get_student_by_email___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/emails/activity/AJG/advisors/session/201809'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'.format(response.text))
        try:
            assert response.json()[0]['Email'] == "Chris.Carlson@gordon.edu"
        except KeyError:
            pytest.fail('Expected StudentID in response, got{0}.'.format(response.json()))

#   Verify that an activity leader can get the emails for the members of an activity
#   
#   Pre-conditions:
#   Valid Authentication Header
#   Authenticated as Activity leader
#   Expectations:
#   Endpoint -- api/emails/activity/:id
#   Expected Status Code -- 200 OK
#   Expected Response Body -- A list of json objects 
    def test_get_emails_for_activity___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/emails/activity/' + activity_code + '/session/201809/'

        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            assert response.json()[0]['Email'] == "Chris.Carlson@gordon.edu"
            assert response.json()[1]['Email'] == "Emmy.Short@gordon.edu"
        except ValueError:
            pytest.fail('Expected Json in response body, got {0}.'.format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json))
#     Verify that a supervisor can get the emails for any activity leader
#    
#    Pre-Conditions:
#    Valid Authentication header
#    Expectaions:
#    Endpoint -- api/emails/activity/:id/leaders
#    Expected Status Code -- 200 OK
#    Expected Respones Body -- Json response with a list of emails
    def test_get_emails_for_leaders___activity_leader(self):
        self.session = self.createLeaderAuthorizedSession()
        self.url = hostURL + 'api/emails/activity/' + activity_code + '/leaders/session/201809/'

        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'.format(response.text))
        try:
            assert response.json()[0]['Email'] == "Emmy.Short@gordon.edu"
            assert response.json()[0]['FirstName'] == "Emmy"
            assert response.json()[0]['LastName'] == "Short"
        except KeyError:
            pytest.fail('Expected Email in response, got{0}.'.format(response.json()))
