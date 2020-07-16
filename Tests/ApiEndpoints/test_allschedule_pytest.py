import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllScheduleTest(control.testCase):
# # # # # # # # # # 
# SCHEDULE TESTS  #
# # # # # # # # # #

#    Get all schedule objects of the currently logged in user.
#    Pre-condition -- student must be enrolled in summer practicum
#    Endpoint --  api/schedule/:username
#    Expected Status code -- 200 Ok
#    Expected Content -- all schedule objects of the currently logged in user.
    @pytest.mark.skipif(not control.enrolledInPracticum, reason = \
        "Student not enrolled in Practicum")
    def test_get_all_schedule_objects_of_current_user(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/schedule/' + control.username + '/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        assert response.json()[0]["CRS_TITLE"] == \
            "SUMMER PRACTICUM COMPUTER SCIENCE"

#    Get all schedule objects of a user with username `username` as a parameter.
#    Pre-condition -- unknown
#    Endpoint --  api/schedule/:username
#    Expected Status code -- 200 Ok
#    Expected Content -- all schedule objects of a user with username `username`
#    as a parameter
#    Normal faculty works, student's don't work, facultytest doesn't work
    @pytest.mark.skipif(not control.unknownPrecondition, reason = \
        "Unknown reason for error")
    def test_get_all_schedule_objects_of_user(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/schedule/' + control.leader_username + '/'
        response = api.get(self.session, self.url)
    
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json, got {0}.'.format(response.text))
        assert response.json()[0]["GORDON_ID"] == str(control.leader_id_number)