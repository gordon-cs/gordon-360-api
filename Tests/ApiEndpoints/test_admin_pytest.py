import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AdminTest(control.testCase):
  
# # # # # # # #
# ADMIN  TEST #
# # # # # # # #

#    Verify that a super admin get information of a specific admin via GordonId.
#    Endpoint -- api/admins
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with the student resource
    def test_get_all_admin_as_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/admins/' 
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            admins = response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(admins) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json))
        print(admins)
        assert [admin for admin in admins if admin['EMAIL'] == "360.facultytest@gordon.edu"]
        assert [admin for admin in admins if admin['EMAIL'] == "Chris.Carlson@gordon.edu" and admin['ADMIN_ID'] == 1 and admin['USER_NAME'] == "Chris.Carlson"]

#    Verify that a guest can't get information of a specific admin via GordonId.
#    Endpoint -- api/admins
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_all_admin_as_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/admins/'
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a student can't get information of a specific admin via
#    GordonId.
#    Pre-condition -- unknown
#    Endpoint -- api/admins
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    @pytest.mark.skipif(not control.unknownPrecondition, reason = \
        "Hanging on get request")
    def test_get_all_admin_as_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/admins/'
        response = api.get(self.session, self.url)
        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))
            
#    Verify that a super admin get information of all admins.
#    Endpoint -- api/admin/_id
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with the student resource
    def test_get_admin(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/admins/8330171/' 
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()['EMAIL'] == "Chris.Carlson@gordon.edu"
        assert response.json()['ADMIN_ID'] == 1
        assert response.json()['ID_NUM'] == 8330171
        assert response.json()['USER_NAME'] == "Chris.Carlson"
        assert response.json()['EMAIL'] == "Chris.Carlson@gordon.edu"
        assert response.json()['SUPER_ADMIN'] == True

#    Verify that a guest can't get information of all admins.
#    Endpoint -- api/admin/_id
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_guest_admin(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/admins/8330171/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a student can't get information of all admins.
#    Pre-condition -- unknown
#    Endpoint -- api/admin/_id
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    @pytest.mark.skipif(not control.unknownPrecondition, reason = \
        "Hanging on get request")
    def test_get_student_admin(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/admins/8330171'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))        