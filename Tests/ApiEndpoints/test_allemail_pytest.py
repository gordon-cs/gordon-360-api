import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllEmailTest(control.testCase):
# # # # # # # #
# EMAIL TEST  #
# # # # # # # #

#    Verify that a student member can get a list of the emails for all members 
#    in the activity.
#    Endpoint -- api/emails/activity/{activity_ID}
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with the student resource
    def test_get_list_of_emails(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/emails/activity/' + control.activity_code_AJG + '/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))
        assert response.json()[0]["FirstName"] == "Christopher"
        assert response.json()[0]["LastName"] == "Carlson"
        assert response.json()[0]["Email"] == "Chris.Carlson@gordon.edu"

#   Verify that an activity leader can get the emails for the members of an 
#   activity in specific session code 
#   Endpoint -- api/emails/activity/:id
#   Expected Status Code -- 200 OK
#   Expected Response Body -- A list of json objects
    def test_get_emails_for_activity___activity_leader(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/emails/activity/' + control.activity_code_AJG + \
            '/session/201809/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json in response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json))
        assert response.json()[0]['Email'] == "Chris.Carlson@gordon.edu"
        assert response.json()[1]['Email'] == "Emmy.Short@gordon.edu"

#    Verify that a supervisor can get the emails for any activity leader based 
#    on a session code
#    Endpoint -- api/emails/activity/:id/leaders/session/:sessionCode
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with a list of emails
    def test_get_emails_for_leaders___supervisor(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/emails/activity/' + control.activity_code_AJG + \
            '/leaders/session/201809/'

        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()[0]['Email'] == "Emmy.Short@gordon.edu"
        assert response.json()[0]['FirstName'] == "Emmy"
        assert response.json()[0]['LastName'] == "Short"

#    Verify that a leader can get the advisor for a student's involvement based
#    on activity code and session code.
#    Endpoint -- api/emails/activity/AJG/advisors/session/201809
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with the student resource
    def test_get_student_by_email___advisor(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/emails/activity/' + control.activity_code_AJG + \
            '/advisors/session/201809'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))
        assert response.json()[0]['Email'] == "Chris.Carlson@gordon.edu"
        assert response.json()[0]['FirstName'] == "Christopher"
        assert response.json()[0]['LastName'] == "Carlson"

#    Verify that a supervisor can get the emails for any advisor
#    Endpoint -- api/emails/activity/:id/advisor
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with a list of emails
    def test_get_all_advisor_emails___supervisor(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/emails/activity/' + control.activity_code_AJG + \
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
        assert response.json()[0]['Email'] == "Chris.Carlson@gordon.edu"
        assert response.json()[0]['FirstName'] == "Christopher"
        assert response.json()[0]['LastName'] == "Carlson"

#    Verify that a supervisor can get the emails for any advisors based on 
#    session code
#    Endpoint -- api/emails/activity/:id/advisors/session/{sessioncode}
#    Expected Status Code -- 200 OK
#    Expected Response Body -- Json response with a list of emails
    def test_get_emails_for_group_admin___supervisor(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/emails/activity/' + control.activity_code_AJG + \
            '/advisors/session/201809/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        assert response.json()[0]['Email'] == "Chris.Carlson@gordon.edu"
        assert response.json()[0]['FirstName'] == "Christopher"
        assert response.json()[0]['LastName'] == "Carlson"

#    Verify that a 404 Not Found error message will be returned based on a
#    bad session code
#    Precondition -- Shouldn't return anything if activity id isn't valid
#    Endpoint -- api/emails/activity/:id
#    Expected Status Code -- 404 Not Found
#    Expected Response Body -- Not Found error message
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "Shouldn't allow access"\
        " because the activity id doesn't exist")
    def test_get_emails_for_activity_404(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/emails/activity/thisIsATest'
        response = api.get(self.session, self.url)
        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))
        #try: Don't know exact error message yet