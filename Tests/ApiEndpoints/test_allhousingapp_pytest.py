import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllHousingAppTest(control.testCase):
# # # # # # # # # # #
# HOUSING APP TESTS #
# # # # # # # # # # #

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

#    Verify that a user who is on the admin whitelist gets the OK to access staff features
#    Endpoint -- 'api/housing/admin'
#    Expected Status Code -- 200 OK
#    Expected Content -- Empty response content
    def test_is_on_whitelist(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/admin'
        # add test user to whitelist
        self.token_payload = { 'username':control.username, 'password':control.password, \
            'grant_type':'password' }
        api.post(self.session, self.url + '/' + str(control.my_id_number) + '/', self.token_payload)
        # check that user is on the whitelist
        response = api.get(self.session, self.url)
        # remove 
        api.delete(self.session, self.url + '/' + str(control.my_id_number) + '/')

        if not response.status_code == 200:
            pytest.fail('Expected 200 Created, got {0}.'\
                .format(response.status_code))

#    Verify that a user who is not on the admin whitelist gets the response Not Found
#    Endpoint -- 'api/housing/admin'
#    Expected Status Code -- 404 Not Found
#    Expected Content -- Empty Response content
    def test_not_on_whitelist(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        # the test user should not be an admin unless it is added in one of these tests
        self.url = control.hostURL + 'api/housing/admin'
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Created, got {0}.'\
                .format(response.status_code))

#    Verify that nothing is found if the current user is not on an application 
#    Endpoint -- 'api/housing/apartment'
#    Expected Status Code -- 404 Not Found
#    Expected Content -- 
    def test_get_application_user_not_found(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment'
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Created, got {0}.'\
                .format(response.status_code))

#    Verify that nothing is found if a given id is not on an application 
#    (Not necessarily the current user's id)
#    Endpoint -- 'api/housing/apartment/{username}'
#    Expected Status Code -- 404 Not Found
#    Expected Content -- 
    def test_get_application_id_not_found(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment' + "/" + str(control.leader_username)
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Created, got {0}.'\
                .format(response.status_code))
        

#   Verify that the editor (primary applicant) can save the application
#   Endpoint -- 'api/housing/apartment/save'
#   Expected Status Code -- 201 Created
#   Expected Content --
    # def test_post_save_apartment_application__editor(self):
    #     self.session = self.createAuthorizedSession(control.username, control.password)
    #     self.url = control.hostURL + 'api/housing/apartment/save'
    #     self.data = {
    #         'ID': control.my_id_number
    #     }
    #     self.requestID = -1
    #     response = api.post(self.session, self.url, self.data)

    #     #
    #     #   PLACE HOLDER
    #     #   Put pytest codes here
    #     #

    #     if not response.status_code == 201:
    #         pytest.fail('Expected 201 Created, got {0}.'\
    #             .format(response.status_code))

#   Verify that the editor (primary applicant) can edit the application
#   Endpoint -- 'api/housing/apartment/save'
#   Expected Status Code -- 201 Created
#   Expected Content --
    def test_put_edit_apartment_application__editor(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/save'
        self.data = {
            'ID': control.my_id_number
        }

        response = api.put(self.session, self.url, self.data)
        # Sudo code
        # First, create an authorized session and check if the student is the editor
        # If editor, call the save api to save the application. (accesses the database)
        # No additional applicant or hall preference is added.

        if not response.status_code == 201:
            pytest.fail('Expected 201 Created, got {0}.'\
                .format(response.status_code))

