import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllWellnessCheckTest(control.testCase):
# # # # # # # # # # 
# HOUSING APP TEST#
# # # # # # # # # # 

#    Verify that a student can get their current information
#    Endpoint -- api/housing/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of one dictionary with user answer
    def test_get_all_for_apartment_app(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json respone body, got {0}.'\
                .format(response.text))
        assert response.json()[0]["OnOffCampus"] == null
        assert response.json()[0]["OnCampusRoom"] == 210 
