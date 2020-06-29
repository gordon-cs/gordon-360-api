import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllEventsTest(control.testCase):
# # # # # # # # # # # #
# EVENTS & CLAW TESTS #
# # # # # # # # # # # #

#    Verify that a student can get all their own chapel events
#    Endpoint -- api/events/chapel
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all chapel events attended by the user 
    def test_get_all_chapel_events(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/chapel/'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a user can get all chapel events in specific term (ex: FA16)
#    Endpoint -- api/events/chapel/:term
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all chapel events attended by the user
#    during term
    def test_get_all_chapel_events_during_term(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/chapel/' + control.term_code + '/'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a 404 Not Found Error message will be shown when an invalid
#    term code is used
#    Precondition -- Will still give 200 OK when term code is fake
#    Endpoint -- api/events/chapel/{term}
#    Expected Status Code -- 404 Not Found
#    Expected Response Body -- Not Found error message
    @pytest.mark.skipif(not control.unknownPrecondition, reason = "Still returns" + \
        "200 OK regardless of term code")
    def test_get_all_chapel_events_fake_term(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/chapel/thisIsAFakeTermCode/'
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))
        #try: Not sure of error message yet

#    Verify that a user can get all events by type_ID
#    Endpoint -- api/events/25Live/type/:Event_OR_Type_ID
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all events resources
    def test_get_all_events___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/25Live/type/10'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a user can get all events by multiple type_ID
#    Endpoint -- api/events/25Live/type/:Event_OR_Type_ID
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all events resources
    def test_get_all_events_multiple(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/25Live/type/10$11$12$14'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a regular member can get all upcoming chapel events 
#    (category_ID = 85)
#    Endpoint -- api/events/25Live/CLAW
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all events resources
    def test_get_all_claw(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/25Live/CLAW'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a user can get all events in 25Live under predefined 
#    categories
#    Endpoint -- api/events/25Live/All
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all events resources
    def test_get_all_25Live(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/25Live/All'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))

#    Verify that a user can get information on specific event on 25Live
#    Endpoint -- api/events/25Live/:Event_ID (2911 = Chapel)
#    Expected Status Code -- 200 OK
#    Expected Response Body -- list of all events resources
#    This endpoint doesn't seem to be active
    def test_get_all_25Live_by_event_id(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/events/25Live/2911'
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
            pytest.fail('Expected list, got {0}.'.format(response.json()))
        assert response.json()[0]['Organization'] == "Chapel Office"
        assert response.json()[0]['Event_ID'] == '2911'
        assert response.json()[0]['Event_Name'] == 'Chapel'
        assert response.json()[0]['Event_Title'] == 'Chapel: David Kirika'
        assert response.json()[0]['Event_Type_Name'] == 'Chapel/Worship'

#     Verify that a Guest can only get the public events on 25Live
#     Endpoint -- api/events/25Live/Public
#     Expected Status Code -- 200 OK
#     Expected Response Body -- list of all guest events resources
    def test_get_all_public_events(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/events/25Live/Public'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expect 200 OK, got {0}.'.format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            pytest.fail('Expected list, got {0}.'.format(response.json()))
        for i in range(len(response.json())):
            assert response.json()[i]['Requirement_Id'] == '3'