import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllSessionTest(control.testCase): 
 
# # # # # # # # # 
# SESSIONS TEST # 
# # # # # # # # # 

#    Verify that an activity leader can get all session objects 
#    Endpoint -- api/sessions/ 
#    Expected Status Code -- 200 OK 
#    Expected Response Body -- List of all session resources 
    def test_get_all_sessions(self): 
        self.session = self.createAuthorizedSession(control.username, control.password) 
        self.url = control.hostURL + 'api/sessions/' 
        response = api.get(self.session, self.url) 
        if not response.status_code == 200: 
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code)) 
        try: 
            response.json() 
        except ValueError: 
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.json())) 
        if not (type(response.json()) is list): 
            pytest.fail('Expected list, got {0}.'.format(response.json())) 
        assert response.json()[0]["SessionCode"] == "201209" 
        assert response.json()[0]["SessionDescription"] == \
            "Fall 12-13 Academic Year" 
        assert response.json()[0]["SessionBeginDate"] == "2012-08-29T00:00:00" 
        assert response.json()[0]["SessionEndDate"] == "2012-12-21T00:00:00" 

        self.url = control.hostURL + 'api/sessions/current/' 
        current = api.get(self.session, self.url) 
        assert response.json()[-2]["SessionCode"] == \
            current.json()["SessionCode"] 
        assert response.json()[-2]["SessionDescription"] == \
            current.json()["SessionDescription"] 
        assert response.json()[-2]["SessionBeginDate"] == \
            current.json()["SessionBeginDate"] 
        assert response.json()[-2]["SessionEndDate"] == \
            current.json()["SessionEndDate"]

#    Verify that an activity leader can get a session object 
#    Endpoint -- api/sessions/:id 
#    Expected Status Code -- 200 OK 
#    Expected Response Body -- A session resource. 
    def test_get_one_session(self): 
        self.session = self.createAuthorizedSession(control.username, control.password) 
        self.url = control.hostURL + 'api/sessions/' + control.session_code + '/' 

        response = api.get(self.session, self.url) 
        if not response.status_code == 200: 
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code)) 
        try: 
            response.json() 
        except ValueError: 
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text)) 
        try: 
            response.json()['SessionCode'] 
        except KeyError: 
            pytest.fail('Expected SessionCode in response, got {0}.'\
                .format(response.json())) 
        assert response.json()['SessionCode'] == control.session_code 

#    Verify that a user can get the current session 
#    Endpoint -- api/sessions/current/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- the current session 
    def test_get_current_session(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/sessions/current/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            response.json()['SessionCode']
        except KeyError:
            pytest.fail('Expected SessionCode in response, got {0}.'\
                .format(response.json()))

#    Verify that a user can get the days left of the session
#    Endpoint -- api/sessions/daysLeft/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- numbers of days left 
    def test_get_daysLeft_session(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/sessions/daysLeft/'

        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        try:
            response.json()
        except KeyError:
            pytest.fail('Expected SessionCode in response, got {0}.'\
                .format(response.json()))