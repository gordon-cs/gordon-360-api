import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllAuthenticationTest(control.testCase):
# # # # # # # # # # # # #
# AUTHENTICATION TESTS  #
# # # # # # # # # # # # #

#    Given valid credentials, verify that authentication is successful for a 
#    student/member.
#    Endpoint -- token/
#    Expected Status code -- 200 Ok
#    Expected Content -- Json Object with access_token attribute.
    def test_authenticate_with_valid_credentials_as_student(self):
        self.session = requests.Session()
        self.url = control.hostURL + 'token'
        self.token_payload = { 'username':control.username, 'password':control.password, \
            'grant_type':'password' }
        response = api.post(self.session, self.url, self.token_payload)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        if not 'access_token' in response.json():
            pytest.fail('Expected access token in response, got {0}.'\
                .format(response.json()))
        assert response.json()["token_type"] == "bearer"

#    Given valid credentials, verify that authentication is successful for a 
#    faculty/leader/god.
#    Endpoint --  token/
#    Expected Status code -- 200 Ok
#    Expected Content -- Json Object with access_token attribute.
    def test_authenticate_with_valid_credentials___activity_leader(self):
        self.session = requests.Session()
        self.url = control.hostURL + 'token'
        self.token_payload = { 'username':control.leader_username, \
            'password':control.leader_password, 'grant_type':'password' }
        response = api.post(self.session, self.url, self.token_payload)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        if not 'access_token' in response.json():
            pytest.fail('Expected access token in response, got {0}.'\
                .format(response.json()))
        assert response.json()["token_type"] == "bearer"