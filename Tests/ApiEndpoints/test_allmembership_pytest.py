import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllMembershipTest(control.testCase):
# # # # # # # # # # #
# MEMBERSHIP TESTS  #
# # # # # # # # # # #

#    Test retrieving all membership resources as a leader
#    Endpoint -- memberships/
#    Expected Status code -- 200 Ok
#    Expected Content -- List of all memberships
    def test_get_all_memberships___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list')
        assert response.json()[0]["ActivityCode"] == control.activity_code_360
        assert response.json()[0]["ActivityDescription"] == \
            control.activity_description_360

#    Test retrieving all membership resources as a member
#    Endpoint -- memberships/
#    Expected Status code -- 401 Unauthorized
    def test_get_all_memberships___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'\
                .format(response.status_code))

#    Retrieve a specific membership resource as a leader
#    Endpoint -- api/memberships/:id
#    Expected Status Code -- 200 OK
#    Expected Content -- A Json Object with a MembershipID attribute.
    def test_get_one_membership___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.membershipID = -1
        # Find a valid membership id
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json response body, got {0}.'\
                .format(response.text))
        if "IDNumber" in response.json()[0]:
            warnings.warn("Security fault, Gordon ID leak")

#    Verify that a leader can fetch memberships for an activity.
#    Endpoint -- api/memberships/activity/{activityId}
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json Objects.
    def test_get_memberships_for_an_activity___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')
        assert response.json()[0]["ActivityCode"] == control.activity_code_AJG 
        if "IDNumber" in response.json()[0]:
            warnings.warn("Security fault, Gordon ID leak")

#    Verify that a member can fetch memberships for an activity.
#    Endpoint -- api/memberships/activity/{activityId}
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json Objects.
    def test_get_memberships_for_an_activity___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            warnings.warn("Security fault")
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a member can get all group admins
#    Endpoint -- api/memberships/activity/{activityId}/group-admin
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json Objects.
    def test_get_admins_for_an_activity___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + \
            '/group-admin/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')
        assert response.json()[0]["ActivityCode"] == control.activity_code_AJG 
        if "IDNumber" in response.json()[0]:
            warnings.warn("Security fault, Gordon ID leak")

#    Verify that a regular member can fetch all leaders for a specific activity.
#    Endpoint -- api/memberships/activity/:id/leaders
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json objects.
    def test_get_leader_memberships_for_an_activity___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + \
            '/leaders/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
            assert response.json()[0]['Participation'] == "LEAD"
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')
        if "IDNumber" in response.json()[0]:
            warnings.warn("Security fault, Gordon ID leak")

#    Verify that a regular member can fetch all advisors for a specific activity.
#    Endpoint -- api/memberships/activity/:id/advisors
#    Expected Status Code -- 200 OK
#    Expected Response Content -- A list of json objects.
    def test_get_advisors_memberships_for_an_activity___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + \
            '/advisors/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')
        if "IDNumber" in response.json()[0]:
            warnings.warn("Security fault, Gordon ID leak")

#    Verify that a regular member can fetch number of followers for a specific 
#    activity.
#    Endpoint -- api/memberships/activity/:id/advisors
#    Expected Status Code -- 200 OK
#    Expected Response Content -- An integer
    def test_get_followers_memberships_for_an_activity___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + \
            '/followers/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))

#    Verify that a regular member can fetch number of followers for a specific
#    activity in a given session
#    Endpoint -- api/memberships/activity/:id/advisors
#    Expected Status Code -- 200 OK
#    Expected Response Content -- An integer
    def test_get_followers_memberships_for_an_activity_session___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/AJG/followers/201809/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json() == 0

#    Verify that a regular member can fetch number of members for a specific
#    activity in a given session
#    Endpoint -- api/memberships/activity/:id/advisors
#    Expected Status Code -- 200 OK
#    Expected Response Content -- An integer
    def test_get_members_for_an_activity_session___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/AJG/members/201809/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json() == 3

#    Verify that a regular member can fetch number of members for a 
#    specific activity.
#    Endpoint -- api/memberships/activity/:id/members
#    Expected Status Code -- 200 OK
#    Expected Response Content -- An integer
    def test_get_members_for_an_activity___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/memberships/activity/' + control.activity_code_AJG + \
            '/members/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json() == 88

#    Verify that a leader can fetch memberships associated with them.
#    Endpoints -- api/memberships/student/:id
#    Expected Status Code -- 200 OK
#    Expected Reponse Content -- A list of json objects
    def test_get_all_my_memberships___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = \
            control.hostURL + 'api/memberships/student/' + str(control.my_id_number) + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')
        if control.my_id_number != response.json()[0]["IDNumber"]:
            warnings.warn("Security fault, not the user's Gordon ID")

#    Verify that a member can fetch memberships based on username
#    Endpoints -- api/memberships/student/:id
#    Expected Status Code -- 200 OK
#    Expected Reponse Content -- A list of json objects
    def test_get_memberships_username___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = \
            control.hostURL + 'api/memberships/student/username/' + control.username + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list.')
        if control.my_id_number != response.json()[0]["IDNumber"]:
            warnings.warn("Security fault, not the user's ID")

#    Verify that leader can fetch someone else's memberships.
#    Endpoint -- api/memberships/student/:id
#    Expected Status Code -- 200 OK.
#    Expected Response Content --  A list of json objects.
    def test_get_all_memberships_for_someone_else___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/student/' + str(control.valid_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Response was not a list')
        if control.valid_id_number != response.json()[0]["IDNumber"]:
            warnings.warn("Security fault, not the user's ID")

#    Verify that an activity leader can create a Guest membership for someone.
#    Precondition -- unknown
#    Endpoints -- api/memberships/
#    Expected Status Code -- 201 Created.
#    Expected Content -- A Json object with a MEMBERSHIP_ID attribute.
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "409 Error")
    def test_post_new_guest_membership_for_someone_else__activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE': control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE': 'GUEST',
            'BEGIN_DTE': '06/10/2016',
            'END_DTE': '07/16/2016',
            'COMMENT_TXT': control.comments
           }
       # We will get the actual id when we post.
       # Setting it -1 to check later that we got an id from the post.
        self.createdMembershipID = -1
        response = api.postAsJson(self.session, self.url, self.data)
        if response.status_code == 201:
            if not ('MEMBERSHIP_ID' in response.json()):
                pytest.fail('Expected MEMBERSHIP_ID in json response, got {0}.'\
                    .format(response.json()))
            else:
                self.createdMembershipID = response.json()['MEMBERSHIP_ID']
        else:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()['PART_CDE'] =='GUEST'
        api.delete(self.session, self.url + str(self.createdMembershipID))

#    Verify that an activity leader can create a membership for someone.
#    Precondition -- unknown
#    Endpoint -- api/memberships/
#    Expected Status Code -- 200 OK
#    Expected Content -- A json response with the created membership
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "409 Error")
    def test_post_new_membership_for_someone___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.createdMembershipID = -1
        # Add a new participant
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE': control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE': 'MEMBR',
            'BEGIN_DTE': '06/10/2016',
            'END_DTE': '07/16/2016',
            'COMMENT_TXT': control.comments
            }

        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            self.createdMembershipID = response.json()['MEMBERSHIP_ID']
        except KeyError:
            pytest.fail('Expected MEMBERSHIP ID in response, got {0}.'\
                .format(response.json()))

        #checking the correctness of post
        getResponse = api.get(self.session, control.hostURL + \
            'api/memberships/activity/' + str(control.activity_code_AJG))       
        self.membershipID = response.json()['MEMBERSHIP_ID']
        req = getResponse.json()
        found = False
        for dic in req:
            reqID = dic['MembershipID']
            if (reqID == self.membershipID):
                found = True      
                try:
                    assert dic['ActivityCode'] == control.activity_code_AJG
                    assert dic['SessionCode'] == control.session_code
                    assert dic['IDNumber'] == control.valid_id_number
                except ValueError:
                    pytest.fail('Expected Json response body, got{0}.'\
                        .format(getResponse.json))
        if not found:
            pytest.fail('requestID not found: {0}.'.format(response.json()))

        if self.createdMembershipID >= 0:
            api.delete(self.session, self.url + str(self.createdMembershipID))

#    Verify that an activity leader can assign a new leader
#    Precondition -- unknown
#    Endpoint -- api/memberships/
#    Expected Status Code -- 200 OK
#    Expected Content -- A json response with the created membership
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "409 Error")
    def test_post_new_leader_membership_for_someone___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.createdMembershipID = -1
        # Add a new leader
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE': control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE':'LEAD',
            'BEGIN_DTE': control.begin_date,
            'END_DTE': control.end_date,
            'COMMENT_TXT': control.comments
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            self.createdMembershipID = response.json()['MEMBERSHIP_ID']
            if self.createdMembershipID < 0: # The creation was not successful
                pytest.fail('Expected valid memberhsip ID, got {0}.'\
                    .format(self.createdMembershipID))
            else:
                 #checking if the correctness of post
                getResponse = api.get(self.session, control.hostURL + \
                    'api/memberships/activity/' + str(control.activity_code_AJG))       
                self.membershipID = response.json()['MEMBERSHIP_ID']
                req = getResponse.json()
                found = False
                for dic in req:
                    reqID = dic['MembershipID']
                    if (reqID == self.membershipID):
                        found = True      
                        try:
                            assert dic['ActivityCode'] == control.activity_code_AJG
                            assert dic['SessionCode'] == control.session_code
                            assert dic['IDNumber'] == control.valid_id_number
                        except ValueError:
                            pytest.fail('Expected Json response body, got{0}.'\
                                .format(getResponse.json))
                if not found:
                    pytest.fail('MembershipID not found:', self.membershipID)
                d = api.delete(self.session, self.url + \
                    str(self.createdMembershipID))
                if not d.status_code == 200:
                    pytest.fail('Error in cleanup. Expected , got {0}.'\
                        .format(d.status_code))
        except KeyError:
            pytest.fail('Expected MEMBERSHIP ID in response, got {0}.'\
                .format(response.json()))

        

#    Verify that an activity leader can upgrade a normal membership to leader 
#    status.
#    Precondition -- unknown
#    Endpoint -- api/memberships/:id
#    Expected Status Code -- 200 OK
#    Expected Content -- A json object with a MEMBERSHIP_ID attribute.
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "Error in setup")
    def test_put_edited_membership_member_to_leader___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.createdMembershipID = -1

        # The membership to modify
        self.predata = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE':'MEMBR', # Is a participant at first.
            'BEGIN_DTE':'06/10/2016', # Old start date
            'END_DTE':'07/16/2016',
            'COMMENT_TXT': control.comments
            }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:
            self.createdMembershipID = r.json()["MEMBERSHIP_ID"]
            # Updated Data
            self.data = {
                'MEMBERSHIP_ID' : self.createdMembershipID,
                'ACT_CDE': control.activity_code_AJG,
                'SESS_CDE' : control.session_code,
                'ID_NUM': control.valid_id_number,
                'PART_CDE':'LEAD', # Upgrade him to director.
                'BEGIN_DTE':'02/10/2016', # New start date
                'END_DTE':'07/16/2016',
                'COMMENT_TXT': control.comments
                }
        except (KeyError, ValueError):
            pytest.fail('Error in setup.')
        response = api.putAsJson(self.session, self.url + \
            str(self.createdMembershipID), self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            self.createdMembershipID = response.json()['MEMBERSHIP_ID']
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in json response, got {0}.'\
                .format(response.json()))
        assert response.json()['PART_CDE'] == 'LEAD'
        if self.createdMembershipID >= 0:
            api.delete(self.session, self.url + str(self.createdMembershipID))

#    Verify that an activity leader can demote a leader membership.
#    Precondition -- unknown
#    Endpoint -- api/memberships/:id
#    Expected Status Code -- 200 OK
#    Expected Content -- A json object with a MEMBERSHIP_ID attribute.
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "Error in setup.")
    def test_put_edited_membership_leader_to_member___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.createdMembershipID = -1
        # The membership to modify
        self.predata = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE':'LEAD', # Is a leader at first
            'BEGIN_DTE':'06/10/2016', # Old start date
            'END_DTE':'07/16/2016',
            'COMMENT_TXT': control.comments
            }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:
            self.createdMembershipID = r.json()["MEMBERSHIP_ID"]
            # Updated Data
            self.data = {
                'MEMBERSHIP_ID' : self.createdMembershipID,
                'ACT_CDE': control.activity_code_AJG,
                'SESS_CDE' : control.session_code,
                'ID_NUM': control.valid_id_number,
                'PART_CDE':'MEMBR', # Demote him to member
                'BEGIN_DTE':'02/10/2016', # New start date
                'END_DTE':'07/16/2016',
                'COMMENT_TXT': control.comments
                }
        except (KeyError, ValueError):
            pytest.fail('Error in setup.')
        response = api.putAsJson(self.session, self.url + \
            str(self.createdMembershipID), self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            self.createdMembershipID = response.json()['MEMBERSHIP_ID']
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in json response, got {0}.'\
                .format(response.json()))
        assert response.json()['PART_CDE'] == 'MEMBR'
        if self.createdMembershipID >= 0:
            api.delete(self.session, self.url + str(self.createdMembershipID))

#    Verify that an activity leader can delete someone else's membership.
#    Predcondition -- unknown
#    Endpoint -- api/memberships/
#    Expected Status Code -- 200 OK
#    Expected Response Content -- The membership resource that wad deleted.
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "Error doing setup.")
    def test_delete_valid_membership___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/memberships/'
        self.createdMembershipID = -1

        # Create a Memerships that we'll eventually delete
        self.predata = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE':'MEMBR',
            'BEGIN_DTE': control.begin_date,
            'END_DTE': control.end_date,
            'COMMENT_TXT': control.comments
        }
        r = api.postAsJson(self.session, self.url, self.predata)
        try:
            self.createdMembershipID = r.json()['MEMBERSHIP_ID']
        except (ValueError, KeyError):
            pytest.fail('Error doing setup')
        response = \
            api.delete(self.session, self.url + str(self.createdMembershipID))
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not ('MEMBERSHIP_ID' in response.json()):
            pytest.fail('Expected MEMBERSHIP_ID in response, got {0}.'\
                .format(response.json()))
