import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllAccountTest(control.testCase):
# # # # # # # # #
# ACCOUNT TESTS #
# # # # # # # # #

#    Verify that a user can get account by email
#    Endpoint -- api/accounts/email/{email}
#    Expected Status Code -- 200 OK
#    Expected Response Body -- profile of the email person
    def test_get_student_by_email(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/accounts/email/' + control._email + '/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()["FirstName"] == control.activity_code_360
        assert response.json()["LastName"] == "StudentTest"
        assert response.json()["Email"] == "360.StudentTest@gordon.edu"
        assert response.json()["ADUserName"] == "360.StudentTest"
        assert response.json()["AccountType"] == "STUDENT"
        assert response.json()["Barcode"] == "21607000485992"
        assert response.json()["show_pic"] == 0
        assert response.json()["ReadOnly"] == 0
        assert response.json()["account_id"] == 30578
        if "GordonID" in response.json():
            warnings.warn("Security fault, Gordon ID leak")

#    Verify that a user can search someone by a word 
#    Endpoint -- api/accounts/search/:word
#    Expected Status Code -- 200 OK
#    Expected Response Body -- any info that has the word
    def test_get_search_by_string(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/accounts/search/' + control.searchString + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()[0]["FirstName"].lower() == control.searchString.lower()

#    Verify that a user can search someone by two words 
#    Endpoint -- api/accounts/search/:word/:word2
#    Expected Status Code -- 200 OK
#    Expected Response Body -- any info that has both of words 
    def test_get_search_by_two_string(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/accounts/search/' + control.searchString + '/' + \
            control.searchString2 + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert control.searchString in response.json()[0]["FirstName"].lower()
        assert control.searchString2 in response.json()[0]["LastName"].lower()

#    Verify that a user can search by username 
#    Endpoint -- api/accounts/username/{username}
#    Expected Status Code -- 200 OK
#    Expected Response Body -- profile info of {username}
    def test_get_search_by_username(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/accounts/username/' + control.leader_username + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()["ADUserName"].lower() == control.leader_username.lower()
        assert response.json()["FirstName"] == control.activity_code_360
        assert response.json()["LastName"] == control.activity_code_360
        assert response.json()["Email"] == "360.FacultyTest@gordon.edu"
        assert response.json()["ADUserName"] == "360.FacultyTest"
        assert response.json()["AccountType"] == "FACULTY"
        assert response.json()["Barcode"] == "21607000486016"
        assert response.json()["show_pic"] == 1
        assert response.json()["ReadOnly"] == 0
        assert response.json()["account_id"] == 30580
        if "GordonID" in response.json():
            warnings.warn("Security fault, Gordon ID leak")