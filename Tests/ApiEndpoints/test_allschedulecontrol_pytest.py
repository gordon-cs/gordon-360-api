import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllScheduleControlTest(control.testCase):
# # # # # # # # # # # # #
# SCHEDULECONTROL TESTS #
# # # # # # # # # # # # #

#    Get the privacy status and description of the currently logged in user.
#    Endpoint -- api/schedulecontrol
#    Expected Status code -- 200 Ok
#    Expected Content -- all schedule objects of the currently logged in user.
    def test_get_all_schedulecontrol_objects_of_current_user(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/schedulecontrol/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'\
                .format(response.text))
        assert response.json()['IsSchedulePrivate'] == True
        assert response.json()['Description'] == control.put_description
        
#    Get the privacy, description, and ID of a user's schedule with username 
#    `leader_username` as a parameter.
#    Endpoint -- api/schedulecontrol/{username}
#    Expected Status code -- 200 Ok
#    Expected Content -- all schedule objects of the currently logged in user.
    def test_get_all_schedulecontrol_objects_of_user(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/schedulecontrol/' + control.leader_username + '/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        assert response.json()['IsSchedulePrivate'] == False
        assert response.json()['Description'] == \
            'httpsCoLnSlShSlShgithubdOTcomSlSh'

#    Update schedule privacy of the currently logged in user to 1.
#    Endpoint -- api/schedulecontrol/privacy/{value}
#    Expected Status code -- 200 Ok
#    Expected Content -- schedule privacy of the currently logged in user.
    def test_schedulecontrol_put_privacy(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/schedulecontrol/privacy/N/'
        response = api.put(self.session, self.url, 'N')

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

        self.url = control.hostURL + 'api/schedulecontrol/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        assert response.json()["IsSchedulePrivate"] == False

        self.url = self.url + 'privacy/Y/'
        response = api.put(self.session, self.url, 'Y')

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        
        self.url = control.hostURL + 'api/schedulecontrol/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        assert response.json()["IsSchedulePrivate"] == True

#    Update the schedule description of the currently logged in user.
#    Endpoint -- api/schedulecontrol/description/{value}
#    Expected Status code -- 200 Ok
#    Expected Content -- schedule description of the currently logged in user.
    def test_schedulecontrol_put_description(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/schedulecontrol/description/' + \
            control.put_description + '/'
        response = api.put(self.session, self.url, control.put_description)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        self.url = control.hostURL + 'api/schedulecontrol/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        assert response.json()["Description"] == control.put_description