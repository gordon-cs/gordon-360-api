import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllStudentEmploymentTest(control.testCase):

# # # # # # # # # # # # # #
# STUDENT EMPLOYMENT TEST #
# # # # # # # # # # # # # #

#    Verify that a student user can get their own student employment information
#    Pre-Conditions: Need to be logged in as cct.service in visual studio 
#    Endpoint -- api/studentemployment/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A json response with student employment info
    @pytest.mark.skipif(not control.cctService, reason = \
        "Not logged in as cct.service.")
    def test_student_employment___regular_member(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/studentemployment/'
        response = api.get(self.session, self.url)
        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is dict):
            pytest.fail('Expected dict, got {0}.'.format(response.json()))