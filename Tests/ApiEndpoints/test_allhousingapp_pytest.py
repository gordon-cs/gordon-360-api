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

    # Create test data

    COMPLETE_APPLICANT1_JSON = {
            'Profile' : {
                'AD_Username' : control.username,
                'Class' : 'Sophomore',
            },
            'OffCampusProgram' : 'Chemistry'
        }

    MINIMAL_APPLICANT1_JSON = {
            'Profile' : {
                'AD_Username' : control.username,
            },
        }

    COMPLETE_APPLICANT2_JSON = {
            'Profile' : {
                'AD_Username' : control.leader_username,
                'Class' : 'Senior',
            },
            'OffCampusProgram' : 'Center for Entrepreneurial Leadership'
        }

    MINIMAL_APPLICANT2_JSON = {
            'Profile' : {
                'AD_Username' : control.leader_username,
            },
        }

    HALL1_RANK1 = {
            'HallRank' : 1,
            'HallName' : 'Bromley'
        }

    HALL1_RANK2 = {
            'HallRank' : 2,
            'HallName' : 'Bromley'
        }

    HALL2_RANK1 = {
            'HallRank' : 1,
            'HallName' : 'Tavilla'
        }

    NEW_APPLICATION_JSON = {  
            'ApplicationID' : -1, 
            'EditorProfile' : MINIMAL_APPLICANT1_JSON,  
            'Applicants' : [
                COMPLETE_APPLICANT1_JSON,
                COMPLETE_APPLICANT2_JSON
            ],
            'ApartmentChoices' : [
                HALL2_RANK1,
                HALL1_RANK2
            ],
        }

    NEW_APP_MINIMAL_JSON = {  
            'ApplicationID' : -1, 
            'EditorProfile' : MINIMAL_APPLICANT1_JSON,  
            'Applicants' : [
                MINIMAL_APPLICANT1_JSON,
            ],
            'ApartmentChoices' : [],
        }

    def create_application_json(appID="", dSubmitted="", dModified="", editorP="", applicants="", apartChoices=""):
        return {
            'ApplicationID' : appID,
            'DateSubmitted' : dSubmitted,
            'DateModified' : dModified,
            'EditorProfile' : editorP,
            'Applicants' : applicants,
            'ApartmentChoices' : apartChoices
        }

    # TEST!

#    Verify that a user who is on the admin whitelist gets the OK to access staff features
#    Endpoint -- 'api/housing/admin'
#    Expected Status Code -- 200 OK
#    Expected Content -- Empty response content
    def test_is_admin(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/admin'
        # add test user to whitelist
        self.data = { }
        api.post(self.session, self.url + '/' + str(control.my_id_number) + '/', self.data)
        # check that user is on the whitelist
        response = api.get(self.session, self.url)
        # remove 
        api.delete(self.session, self.url + '/' + str(control.my_id_number) + '/')

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a user who is not on the admin whitelist gets the response Not Found
#    Endpoint -- 'api/housing/admin'
#    Expected Status Code -- 404 Not Found
#    Expected Content -- Empty Response content
    def test_not_admin(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        # the test user should not be an admin unless it is added in one of these tests
        self.url = control.hostURL + 'api/housing/admin'
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that an application and all rows that reference are deleted successfully
#    Endpoint -- 'api/housing/apartment/applications/{applicationID}'
#    Expected Status Code -- 200 OK and 404 Not Found
#    Expected Content -- Empty Response content
    def test_application_deleted(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/applications'
        self.data = {
            'ApplicationID' : -1,
            'EditorProfile' : {
                'AD_Username' : control.leader_username,
            },   
            'Applicants' : [
                {
                    'Profile' : {
                       'AD_Username' : control.leader_username,
                       'Class' : 'Junior',
                    },
                },
                {
                    'Profile' : {
                       'AD_Username' : control.username,
                       'Class' : 'Senior',
                    },
                },
            ],
            'ApartmentChoices' : [
                {
                    'HallRank' : 1,
                    'HallName' : 'Tavilla'
                },
                {
                    'HallRank' : 2,
                    'HallName' : 'Conrad'
                },
                {
                    'HallRank' : 3,
                    'HallName' : 'Hilton'
                }
            ],
        }   
        appIDResponse = api.postAsJson(self.session, self.url, self.data)

        appID = appIDResponse.content

        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID)
        response = api.delete(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

        # make sure the referenced rows have been deleted too
        # (no endpoint exists to just get a list of hall choices, 
        # it is not verified here that the application hall choices are deleted)

        self.url = control.hostURL + 'api/housing/apartment/' + control.leader_username
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))

        self.url = control.hostURL + 'api/housing/apartment/' + control.username
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that the list of apartment-style halls is retrieved correctly
#    Endpoint -- 'api/housing/halls'
#    Expected Status Code -- 200 OK
#    Expected Content -- 
    def test_get_apartment_halls(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/halls/apartments'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

#    Verify that an application is found if the current user is on an application 
#    Endpoint -- 'api/housing/apartment'
#    Expected Status Code -- 200 OK 
#    Expected Content -- 
    def test_get_application_is_found(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/applications'
        # the user in this context is 360.studenttest because we create the authorized session with control.username, 
        # so we are creating an application with that user on it to get an OK from 'api/housing/apartment'
        self.data = NEW_APPLICATION_JSON 
        appIDResponse = api.postAsJson(self.session, self.url, self.data)

        self.url = control.hostURL + 'api/housing/apartment'
        response = api.get(self.session, self.url)

        # clean up
        appID = appIDResponse.content
        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID)
        api.delete(self.session, self.url) # delete has not yet been implemented in this branch

        if not response.status_code == 200:
            pytest.fail('Expected 200 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that nothing is found if the current user is not on an application 
#    Endpoint -- 'api/housing/apartment'
#    Expected Status Code -- 404 Not Found
#    Expected Content -- 
    def test_get_application_not_found(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment'
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that an application is found if the given user is on an application 
#    Endpoint -- 'api/housing/apartment'
#    Expected Status Code -- 200 OK 
#    Expected Content -- 
    def test_get_user_application_is_found(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/applications'
        self.data = NEW_APPLICATION_JSON 
        appIDResponse = api.postAsJson(self.session, self.url, self.data)

        self.url = control.hostURL + 'api/housing/apartment/' + str(control.leader_username)
        response = api.get(self.session, self.url)

        # clean up
        appID = appIDResponse.content
        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID)
        api.delete(self.session, self.url) # delete has not yet been implemented in this branch

        if not response.status_code == 200:
            pytest.fail('Expected 200 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that nothing is found if a given user is not on an application 
#    (Not necessarily the current user's id)
#    Endpoint -- 'api/housing/apartment/{username}'
#    Expected Status Code -- 404 Not Found
#    Expected Content -- 
    def test_get_user_application_not_found(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/' + str(control.leader_username)
        response = api.get(self.session, self.url)

        if not response.status_code == 404:
            pytest.fail('Expected 404 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that an application is saved successfully
#    Endpoint -- 'api/housing/apartment/applications'
#    Expected Status Code -- 200 OK
#    Expected Content -- 
    def test_application_saved(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/applications'
        self.data = NEW_APPLICATION_JSON
        appIDResponse = api.postAsJson(self.session, self.url, self.data)

        # clean up
        appID = appIDResponse.content
        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID)
        api.delete(self.session, self.url) # delete has not yet been implemented in this branch

        if not appIDResponse.status_code == 200:
            pytest.fail('Expected 200 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that an application is edited successfully when an applicant is added 
#    Endpoint -- 'api/housing/apartment/applications/{applicationID}'
#    Expected Status Code -- 200 OK
#    Expected Content -- 
    def test_application_edited_applicant(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/applications'
        self.data = NEW_APPLICATION_JSON
        appIDResponse = api.postAsJson(self.session, self.url, self.data)

        appID = appIDResponse.content
        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID)
        self.data = NEW_APPLICATION_JSON #TODO

        # clean up
        api.delete(self.session, self.url) # delete has not yet been implemented in this branch

        if not response.status_code == 200:
            pytest.fail('Expected 200 Not Found, got {0}.'\
                .format(response.status_code))

#    Verify that an application's editor is changed if the new editor is on the 
#    application
#    Endpoint -- 'api/housing/apartment/applications/{applicationID}/editor'
#    Expected Status Code -- 200 OK
#    Expected Content -- 
    def test_application_editor_changed(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/housing/apartment/applications'
        self.data = NEW_APPLICATION_JSON
        appIDResponse = api.postAsJson(self.session, self.url, self.data)

        appID = appIDResponse.content

        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID) + '/editor'
        self.data = { 
            'ApplicationID' : appID, 
            'EditorProfile' : {
                'AD_Username' : control.leader_username,
            },  
            'Applicants' : [
                {
                    'Profile' : {
                       'AD_Username' : control.username,
                       'Class' : 'Junior',
                    },
                },
                {
                    'Profile' : {
                       'AD_Username' : control.leader_username,
                       'Class' : 'Senior',
                    },
                },
            ],
            'ApartmentChoices' : [
                {
                    'HallRank' : 1,
                    'HallName' : 'Tavilla'
                },
            ],
        }

        response = api.putAsJson(self.session, self.url, self.data)

        # clean up
        self.url = control.hostURL + 'api/housing/apartment/applications/' + str(appID)
        api.delete(self.session, self.url) # delete has not yet been implemented in this branch

        if not response.status_code == 200:
            pytest.fail('Expected 200 Not Found, got {0}.'\
                .format(response.status_code))  





