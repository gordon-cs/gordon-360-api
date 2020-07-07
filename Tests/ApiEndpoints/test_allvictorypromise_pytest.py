import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllVictoryPromiseTest(control.testCase):

# # # # # # # # # # # # #
# VICTORY PROMISE TEST  #
# # # # # # # # # # # # #

#    Verify that a student user can get their own victory promise information
#    Endpoint -- api/vpscore/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with victory promise points
    def test_victory_promise(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/vpscore/'
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
            pytest.fail('Expected list, got {0}.'.format(response.json))
        assert response.json()[0]["TOTAL_VP_IM_SCORE"] == 0 
        assert response.json()[0]["TOTAL_VP_CC_SCORE"] == 0 
        assert response.json()[0]["TOTAL_VP_LS_SCORE"] == 0 
        assert response.json()[0]["TOTAL_VP_LW_SCORE"] == 0

#    Verify that a guest can't get victory promise information
#    Endpoint -- api/vpscore/
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied message
    def test_guest_victory_promise(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/vpscore/'
        response = api.get(self.session, self.url)
        
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a faculty user's victory promise information is always 0.
#    Endpoint -- api/vpscore/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with victory promise points all 0
    def test_faculty_victory_promise(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/vpscore/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json))
        assert response.json()[0]["TOTAL_VP_IM_SCORE"] == 0 
        assert response.json()[0]["TOTAL_VP_CC_SCORE"] == 0 
        assert response.json()[0]["TOTAL_VP_LS_SCORE"] == 0 
        assert response.json()[0]["TOTAL_VP_LW_SCORE"] == 0