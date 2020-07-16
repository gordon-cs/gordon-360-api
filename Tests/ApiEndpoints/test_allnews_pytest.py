import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllNewsTest(control.testCase):
# # # # # # #
# NEWS TEST #
# # # # # # #

#    Verify that a student can get the full list of category names, category ids
#    , and sort order of student news.
#    Endpoint -- api/news/category
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of category names, ids and sort order
    def test_get_news_category_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/news/category/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a student can get the list of news that hasn't expired
#    Endpoint -- api/news/not-expired
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of student news entries that have not 
#    expired
    def test_get_news_not_expired_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/news/not-expired/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        if response.json():
            assert response.json()[0].has_key('categoryName')
            assert response.json()[0].has_key('ManualExpirationDate')
            assert response.json()[0].has_key('SortOrder')

#    Verify that a student can get student news entries that have been accepted
#    and not expired, and is new since 10am the day before.
#    Endpoint -- api/news/new
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of student news entries that have been 
#    accepted, are not expired, and are new since 10 am the day before.
    def test_get_news_new_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/news/new/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        if not response.json():
            print("Find out the content to add")

#    Verify that a faculty user can get the full list of category names, category ids
#    , and sort order of student news.
#    Endpoint -- api/news/category
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of category names, ids and sort order
    def test_get_news_category_faculty(self):
        self.session = self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/news/category/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a faculty user can get the list of news that hasn't expired
#    Endpoint -- api/news/not-expired
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of student news entries that have not 
#    expired
    def test_get_news_not_expired_faculty(self):
        self.session = self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/news/not-expired/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        if response.json():
            assert response.json()[0].has_key('categoryName')
            assert response.json()[0].has_key('ManualExpirationDate')
            assert response.json()[0].has_key('SortOrder')

#    Verify that a faculty user can get student news entries that have been accepted
#    and not expired, and is new since 10am the day before.
#    Endpoint -- api/news/new
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of student news entries that have been 
#    accepted, are not expired, and are new since 10 am the day before.
    def test_get_news_new_faculty(self):
        self.session = self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/news/new/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        if not response.json():
            print("Find out the content to add")

#    Verify that a guest can't get the full list of category names, category ids
#    , and sort order of student news.
#    Endpoint -- api/news/category
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_news_category_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/news/category/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a guest can't get the list of news that hasn't expired
#    Endpoint -- api/news/not-expired
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_news_not_expired_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/news/not-expired/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a guest can't get student news entries that have been accepted
#    and not expired, and is new since 10am the day before.
#    Endpoint -- api/news/new
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_news_new_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/news/new/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))
