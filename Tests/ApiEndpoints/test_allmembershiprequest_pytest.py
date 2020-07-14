import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllMembershipRequestTest(control.testCase):
# # # # # # # # # # # # # #
# MEMBERSHIP REQUEST TEST #
# # # # # # # # # # # # # #

#    Verify that a regular member cannot access all membership requests.
#    Endpoint -- api/requests/
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Content -- Empty response content.
    def test_not_get_all_membership_requests(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/requests/'
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized, got {0}.'\
                .format(response.status_code))
        if response.text:
            pytest.fail('Expected empty response body, got {0}.'\
                .format(response.text))


#    Verify that a regular member cannot get the membership requests of somone
#    else.
#    Endpoint -- api/requests/student/:id
#    Expected Status Code -- 404 Not Found
#    Expected Response Body -- Empty
    def test_not_get_membership_requests_for_someone_else(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/requests/student/' + str(control.valid_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'.format(response.status_code))
            

#    Verify that a regular member can't access memberships requests for
#    activity.
#    Endpoint -- api/requests/activity/:id
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Body -- Empty
#    Passed with activity code 'TRAS', but not AJG because studenttest is a 
#    leader for AJG
    def test_not_get_membership_requests_for_activity(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/requests/activity/' + 'TRAS'
        # Report if there any current memberships for the Club to avoid false
        # negatives.
        # If I am currently a director of the club, this test should fail.
        response = api.get(self.session, control.hostURL + 'api/memberships/student/' \
            + str(control.my_id_number))
        try:
            for membership in response.json():
                if(membership['ActivityCode'] == control.activity_code_AJG and \
                    membership['Participation'] in control.LEADERSHIP_POSITIONS):
                    pytest.fail('False Negative: This user is a leader for' + \
                        'the activity we are testing.')
        except ValueError:
            pytest.fail('We did not get a json response back during setup.')

        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('We did not get 401 Unauthorized.')
        if response.text:
            pytest.fail('We got a non-empty response body.')


#    Verify that we can create a membership request.
#    Precondition -- unknown
#    Endpoints -- api/requests/
#    Expected Status Cpde -- 201 Created.
#    Expected Content -- A Json object with a REQUEST_ID attribute.
#    session code 201510 does not work
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "409 Error")
    def test_post_valid_membership_request__as_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '06/27/2019',
            'COMMENT_TXT': control.comments
            }
        # We will get the actual id when we post.
        # Setting it -1 to check later that we got an id from the post.
        self.requestID = -1
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json response, got {0}.'\
                .format(response.text))
     
        #checking if the correctness of post\
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        getResponse = api.get(self.session, control.hostURL + \
            'api/requests/activity/' + str(control.activity_code_AJG))       
        self.requestID = response.json()['REQUEST_ID']
        req = getResponse.json()
        found = False
        for dic in req:
            reqID = dic['RequestID']
            if (reqID == self.requestID):
                found = True      
                try:
                    assert dic['ActivityCode'] == control.activity_code_AJG
                    assert dic['SessionCode'] == control.session_code
                    assert dic['IDNumber'] == control.my_id_number
                except ValueError:
                    pytest.fail('Expected Json response body, got{0}.'\
                        .format(getResponse.json))
        if not found:
            pytest.fail('requestID not found:', self.requestID)

        #delete the test post
        try:
            self.requestID = response.json()['REQUEST_ID']
            print(self.requestID)
            if self.requestID >= 0:
               api.delete(self.session, self.url + str(self.requestID))
        except KeyError:
            pytest.fail('Expected REQUEST_ID in response body, got {0}.'\
                .format(response.json()))


#    Verify that we can't create a membership request for someone else as a 
#    member.
#    Endpoints -- api/requests/
#    Expected Status Code -- 401 Unauthorized.
#    Expected Response Content -- Empty Response.
#    look up for configuration.py for the data configuration
    def test_not_post_membership_request_for_someone_else(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE': control.member_positions,
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT': control.comments
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
            pytest.fail('Expected 401 Unauthorized, got {0}.'\
                .format(response.status_code))
            

#    Verify that an activity leader can access all membership requests.
#    Endpoint -- api/requests/
#    Expected Status Code -- 200 OK
#    Expected Response Content -- List of json objects representing the
#    membership requests for all.
    def test_get_all_membership_requests(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()[0]['ActivityCode'] == control.activity_code_360
            assert response.json()[1]['ActivityCode'] == "BADM"
            assert response.json()[2]['ActivityCode'] == "ACS"
            assert response.json()[3]['ActivityCode'] == "SCOTTIE"
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list in response body, got {0}.'\
                .format(response.json()))


#    Verify that the activity leader can get requests to join the activity
#    he/she is leading
#    Endpoint -- api/requests/activity/:id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- List of json objects representing the membership
#    requests for an activity.
    def test_get_membership_requests_for_activity(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/activity/' + control.activity_code_AJG + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()[0]['ActivityCode'] == control.activity_code_AJG
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list in response body, got {0}.'\
                .format(response.json()))
        if control.activity_code_AJG != response.json()[0]["ActivityCode"]:
            warnings.warn("Security fault, wrong activity")


#    Verify that an activity leader cannot get the membership requests of
#    someone else.
#    Endpoint -- api/requests/student/:id
#    Expected Status Code -- 404 Not Found
#    Expected Response Body -- Empty
    def test_get_membership_requests_for_someone_else___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/student/' + str(control.valid_id_number)
        response = api.get(self.session, self.url)
        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))


#    Verify that we can create a membership request as leader.
#    Precondition -- unknown
#    Endpoints -- api/requests/
#    Expected Status Cpde -- 201 Created.
#    Expected Content -- A Json object with a REQUEST_ID attribute.
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "409 Error")
    def test_post_valid_membership_request__as_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT': control.comments
                    }
        # We will get the actual id when we post.
        # Setting it -1 to check later that we got an id from the post.
        self.requestID = -1

        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json response, got {0}.'\
                .format(response.text))

        #checking if the correctness of post\
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        getResponse = api.get(self.session, control.hostURL + \
            'api/requests/activity/' + str(control.activity_code_AJG))       
        self.requestID = response.json()['REQUEST_ID']
        req = getResponse.json()
        found = False
        for dic in req:
            reqID = dic['RequestID']
            if (reqID == self.requestID):
                found = True      
                try:
                    assert dic['ActivityCode'] == control.activity_code_AJG
                    assert dic['SessionCode'] == control.session_code
                    assert dic['IDNumber'] == control.my_id_number
                except ValueError:
                    pytest.fail('Expected Json response body, got{0}.'\
                        .format(getResponse.json()))
        if not found:
            pytest.fail('requestID not found:', self.requestID)
        
        #try:
        #    self.requestID = response.json()['REQUEST_ID']
        #    if not response.json()['STATUS'] == REQUEST_STATUS_PENDING:
        #    pytest.fail('Expected Pending status , got {0}.'.format(resposne.json()))
        #    except KeyError:
        #    pytest.fail('Expected REQUEST_ID in response body, got {0}.'.format(response.json()))
        #    We try to delete the request we created
        if self.requestID >= 0:
            api.delete(self.session, self.url + str(self.requestID))

#    Verify that we can create a membership request for someone else as leader.
#    Precondition -- unknown
#    Endpoints -- api/requests/
#    Expected Status Code -- 401 Unauthorized.
#    Expected Response Content -- Empty Response.
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "get request")
    def test_post_membership_request_for_someone_else(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/'
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.valid_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT': control.comments
            }
        # We will get the actual id when we post.
        self.requestID = -1
        response = api.post(self.session, self.url, self.data)
        if response.status_code == 201:
            try:
                self.requestID = response.json()['REQUEST_ID']
            except (ValueError, KeyError):
                pytest.fail('Error in test')

        #checking if the correctness of post\
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        getResponse = api.get(self.session, control.hostURL + \
            'api/requests/activity/' + str(control.activity_code_AJG))       
        self.requestID = response.json()['REQUEST_ID']
        req = getResponse.json()
        found = False
        for dic in req:
            reqID = dic['RequestID']
            if (reqID == self.requestID):
                found = True      
                try:
                    assert dic['ActivityCode'] == control.activity_code_AJG
                    assert dic['SessionCode'] == control.session_code
                    assert dic['IDNumber'] == control.valid_id_number
                except ValueError:
                    pytest.fail('Expected Json response body, got{0}.'\
                        .format(getResponse.json()))
        if not found:
            pytest.fail('requestID not found:', self.requestID)


        #delete the test post
        d = api.delete(self.session, self.url + str(self.requestID))
        if  d.status_code != 200:
            pytest.fail('Unauthorized resource not deleted.')


#    Verify that an activity leader can't edit a membership request through a
#    put request.
#    Pre-Conditions:
#    Valid Authorization Header.
#    Authenticated as activity leader.
#    Expectations:
#    Endpoint -- api/requests/:id
#    Expected Status Code -- 401 Unauthorized
#    Expected Response Body -- Empty
#
#    def test_put_membership_request___activity_leader(self):
#        self.session = \
#           self.createAuthorizedSession(control.leader_username, control.leader_password)
#        self.url = control.hostURL + '/api/requests/'
#        self.requestID = -1
#
#        self.predata = {
#            'ACT_CDE': control.activity_code_AJG,
#            'SESS_CDE' : control.session_code,
#            'ID_NUM': control.my_id_number,
#            'PART_CDE':'MEMBR',
#            'DATE_SENT' : '07/06/2016',
#            'COMMENT_TXT': control.comments
#            }
#        response = api.postAsJson(self.session, self.url, self.predata)
#        try:
#            self.requestID = response.json()['REQUEST_ID']
#            self.data = {
#                'REQUEST_ID': self.requestID,
#                'ACT_CDE': control.activity_code_AJG,
#                'SESS_CDE' : '201501',
#                'ID_NUM': control.valid_id_number, #Changing values to emulate attacker muhahah
#                'PART_CDE':'PART',
#                'DATE_SENT' : '07/06/2016',
#                'COMMENT_TXT': control.comments
#                }
#        except ValueError:
#            pytest.fail('Error performing setup')
#
#        response = api.putAsJson(self.session, self.url + \
#           str(self.requestID), self.data)
#        if not response.status_code == 401:
#            pytest.fail('Expected 401 Unauthorized, got {0}.'\
#               .format(response.status_code))
#        if response.text:
#            pytest.fail('Expected empty response body, got {0}.'
#               .format(response.text))
#        api.delete(self.session, self.url + str(self.requestID))


#    Verify that an activity leader can delete a membership request for his 
#    activity
#    Endpoints -- api/requests/:id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- The request that was deleted
    def test_delete_membership_request(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/'
        self.predata = {}
        self.requestID = -1

        self.predata = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE': control.session_code,
            'PART_CDE': 'MEMBR',
            'ID_NUM': control.leader_id_number,
            'DATE_SENT': '07/19/2016',
            'COMMENT_TXT': control.comments
            }
        response = api.postAsJson(self.session, self.url, self.predata)
        if not response.status_code == 201:
            pytest.fail('Error in setup. Expected 201 Created, got {0}.'\
                .format(response.status_code))
        else:
            self.requestID = response.json()['REQUEST_ID']
        response = \
            api.delete(self.session, self.url + '/' + str(self.requestID))
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            response.json()['REQUEST_ID']
        except KeyError:
            pytest.fail('Expected REQUEST_ID in response body, got {0}.'\
                .format(response.json()))

#    Verify that the activity leader can accept a request directed at their 
#    activity.
#    Endpoints -- api/requests/:id/approve
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with the request that was accepted.
    def Test_Allow_someone_to_join_my_activity___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/'
        self.requestID = -1
        self.membershipID = -1

        #Create a memberships request for the trash club.
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT': control.comments
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Error in setup. Expected 201 Created, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Error in setup. Expected json response, got {0}.'\
                .format(response.text))
        try:
            self.requestID = response.json()['REQUEST_ID']
        except KeyError:
            pytest.fail('Error in setup. Expected REQUEST_ID in response, ' + \
                'got {0}.'.format(response.json()))

        response = api.postAsJson(self.session, self.url + \
            str(self.requestID) + '/approve', None)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            self.membershipID = response.json()['MEMBERSHIP_ID']
            if self.requestID < 0:
                pytest.fail('Error in cleanup for {0}. Expected valid ' + \
                    'request ID, got {1}.'.format(self.requestID))
            else:
                d = api.delete(self.session, self.url + str(self.requestID))
                if not d.status_code == 200:
                    pytest.fail('Error in cleanup for {0}. Expected 200 OK ' + \
                        'when deleting request, got {1}.'\
                            .format(d.status_code))
            if self.membershipID < 0: # membership creation was not successful
                pytest.fail('Error in cleanup. Expected valid membership ID' + \
                    ', got {0}.'.format(self.membershipID))
            else:
                api.delete(self.session, control.hostURL + 'api/memberships/' + \
                    str(self.membershipID))
                if not d.status_code == 200:
                    pytest.fail('Error in cleanup. Expected 200 OK when ' + \
                        'deleting membership, got {0}.'.format(d.status_code))
        except KeyError:
            pytest.fail('Expected MEMBERSHIP_ID in response bady, got {0}.'\
                .format(response.json()))

#    Verify that the activity leader can deny a request directed at their
#    activity.
#    Precondition -- unknown
#    Endpoints -- api/requests/:id/deny
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with the request that was denied
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "409 Error")
    def test_deny_someone_joining_my_activity(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/requests/'
        self.requestID = -1

        #Create a memberships request for the trash club.
        self.data = {
            'ACT_CDE': control.activity_code_AJG,
            'SESS_CDE' : control.session_code,
            'ID_NUM': control.my_id_number,
            'PART_CDE':'MEMBR',
            'DATE_SENT' : '07/06/2016',
            'COMMENT_TXT': control.comments
            }
        response = api.postAsJson(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected json response, got {0}.'\
                .format(response.text))
        else:
            try:
                self.requestID = response.json()['REQUEST_ID']
            except KeyError:
                pytest.fail('Error in setup. Expected REQUEST_ID in response' + \
                    ', got {0}.'.format(response.json()))
        response = api.postAsJson(self.session, self.url + \
            str(self.requestID) + '/deny', None)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        else:
            try:
                if not response.json()['STATUS'] == control.REQUEST_STATUS_DENIED:
                    pytest.fail('Expected approved request, got {0}.'\
                        .format(response.json()))
            except KeyError:
                pytest.fail('Expected STATUS in response bady, got {0}.'\
                    .format(response.json()))
        api.delete(self.session, self.url + str(self.requestID))