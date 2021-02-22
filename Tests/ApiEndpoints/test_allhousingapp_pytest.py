import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllHousingAppTest(control.testCase):
# # # # # # # # # # 
# HOUSING APP TEST#
# # # # # # # # # # 

#    Verify that a student can get their current information
#    Endpoint -- api/housing/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of one dictionary with user answer
    def test_get_all_for_apartment_app(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartmentInfo'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json respone body, got {0}.'\
                .format(response.text))
        assert response.json()[0]["OnOffCampus"] == None
        assert response.json()[0]["OnCampusRoom"] == '210'

#    Verify that a user can submit a housing application
#    Endpoint -- 'api/housing/putApartmentApplication'
#    Expected Status Code -- 200 OK
#    Expected Content --
    def test_put_apartment_application(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/putApartmentApplication'
        self.data = {
            'ID': control.my_id_number
        }
        self.requestID = -1
        response = api.put(self.session, self.url, self.data)
        if not response.status_code == 200:
            pytest.fail('Expected 200 Created, got {0}.'\
                .format(response.status_code))

#   Verify that the editor (primary applicant) can save the application
#   Endpoint -- 'api/housing/apartment/save'
#   Expected Status Code -- 201 Created
#   Expected Content --
    def test_post_save_apartment_application_as_editor(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/save'
        self.data = {
            'ID': control.my_id_number
        }
        self.requestID = -1
        response = api.post(self.session, self.url, self.data)

        #
        #   PLACE HOLDER
        #   Put pytest codes here
        #

        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))

#   Verify that the editor (primary applicant) can edit the application
#   Endpoint -- 'api/housing/apartment/save'
#   Expected Status Code -- 201 Created
#   Expected Content --
    def test_put_edit_apartment_application_as_editor(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/save'
        self.data = {
            'ID': control.my_id_number
        }
        self.requestID = -1
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))