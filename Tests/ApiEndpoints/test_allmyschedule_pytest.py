import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllMyScheduleTest(control.testCase):
# # # # # # # # # # # 
# MYSCHEDULE TESTS  #
# # # # # # # # # # #

#    Get all custom events from the schedule of the currently logged in user.
#    Endpoint -- api/myschedule/
#    Expected Status code -- 200 Ok
#    Expected Content -- all custom events of the currently logged in user.
    def test_get_all_myschedule_objects_of_current_user(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/myschedule/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        assert response.json()[0]["EVENT_ID"] == control.event_id
        assert response.json()[0]["LOCATION"] == control.location
        assert response.json()[0]["DESCRIPTION"] == control.put_description
        assert response.json()[0]["BEGIN_TIME"] == control.begintime
        assert response.json()[0]["END_TIME"] == control.endtime
        
#    Get all custom events of a user with username `username` as a parameter.
#    Endpoint -- api/myschedule/{username}
#    Expected Status code -- 200 Ok
#    Expected Content -- all custom events of a user with username `username`
#    as a parameter
    def test_get_all_myschedule_objects_of_user(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/myschedule/' + control.leader_username + '/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        assert response.json()[0]["EVENT_ID"] == '1100'
        assert response.json()[0]["GORDON_ID"] == str(control.leader_id_number)
        assert response.json()[0]["LOCATION"] == control.location
        
#    Get a specific custom event of the currently logged in user with `eventId` 
#    as a parameter
#    Endpoint --  api/myschedule/event/{eventID}
#    Expected Status code -- 200 Ok
#    Expected Content -- a specific custom event of the currently logged in user
#    with `eventId`  as a parameter
    def test_get_myschedule_objects_of_id(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/myschedule/event/' + control.event_id + '/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        assert response.json()["EVENT_ID"] == control.event_id
        assert response.json()["LOCATION"] == control.location
        assert response.json()["DESCRIPTION"] == control.put_description
        assert response.json()["BEGIN_TIME"] == control.begintime
        assert response.json()["END_TIME"] == control.endtime

# Create a custom event of the currently logged in user.
# Expectations:
# Endpoints -- api/myschedule/
# Expected Status Code -- 201 Created.
# Expected Content -- a custom event with the data in the test
    def test_myschedule_post(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/myschedule/'
        self.data = {
            'GORDON_ID' : str(control.my_id_number),
            'LOCATION' : control.location,
            'DESCRIPTION' : control.description,
            'TUE_CDE' : 'T',
            'IS_ALLDAY' : 1,
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
        assert response.json()["GORDON_ID"] == str(control.my_id_number)
        assert response.json()["LOCATION"] == control.location
        assert response.json()["DESCRIPTION"] == control.description

        # delete the test post
        # Expected Status Code -- 200 OK.
        try:
            self.GordonID = response.json()["GORDON_ID"]
            if self.GordonID == str(control.my_id_number):
                response = api.delete(self.session, self.url + \
                    str(response.json()["EVENT_ID"]))
        except KeyError:
            pytest.fail('Expected REQUEST_ID in response body, got {0}.'\
                .format(response.json()))
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

#    Update a custom event of the currently logged in user.
#    Endpoints -- api/myschedule/
#    Expected Status Code -- 200 OK.
#    Expected Content -- The Json object (custom event) with a 
#    GORDON_ID attribute.
    def test_myschedule_put(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/myschedule/'
        try:
            self.data = {
            'EVENT_ID' : control.event_id,
            'GORDON_ID' : str(control.my_id_number),
            'LOCATION' : control.location,
            'DESCRIPTION' : control.put_description,
            'MON_CDE' : 'M',
            'TUE_CDE' : 'T',
            'WED_CDE' : None, # Showing the options of the value
            'THU_CDE' : 'R',
            'FRI_CDE' : 'F',
            'IS_ALLDAY' : 0,
            'BEGIN_TIME' : control.begintime,
            'END_TIME' : control.endtime,
            }
        except (KeyError, ValueError):
            pytest.fail('Error in setup.')
        response = api.putAsJson(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()["GORDON_ID"] == str(control.my_id_number)
        assert response.json()["LOCATION"] == control.location
        assert response.json()["DESCRIPTION"] == control.put_description
        assert response.json()["BEGIN_TIME"] == control.begintime
        assert response.json()["END_TIME"] == control.endtime