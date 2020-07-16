import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllDiningTest(control.testCase):
# # # # # # # # #
# DINING  TESTS #
# # # # # # # # #

#    Verify that a student user can get meal plan data.
#    Endpoint -- api/dining/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with dict containing the
#    student mealplan data
    def test_dining_plan_for_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/dining/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json() == "0"

#    Verify that a faculty user can get meal plan data.
#    Endpoint -- api/dining/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with dict containing the
#    student mealplan data
    def test_dining_plan_for_faculty(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/dining/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json() == "0"

#    Verify that a guest user can't get meal plan data.
#    Endpoint -- api/dining/
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied message
    def test_dining_plan_for_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/dining/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))