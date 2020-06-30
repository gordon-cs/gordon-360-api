import pytest
import warnings
import string
from pytest_components import requests
from datetime import datetime

import pytest_components as api
import test_gordon360_pytest as control

class Test_AllWellnessCheckTest(control.testCase):
# # # # # # # # # # # #
# WELLNESS CHECK TEST #
# # # # # # # # # # # #

#    Verify that a student can get their current status
#    Endpoint -- api/wellness/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of one dictionary with user answer
    def test_get_wellness_student_asymptomatic_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/wellness/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json respone body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        assert response.json()[0]["answerValid"] == True
        assert response.json()[0]["userAnswer"] == True or \
            response.json()[0]["userAnswer"] == False

#    Verify that a student can get the wellness check question
#    Endpoint -- api/wellness/Question
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of a dictionary of questions
#    and prompts.
    def test_get_wellness_question_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/wellness/question/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        assert response.json()[0].has_key('yesPrompt')
        assert response.json()[0].has_key('noPrompt')
        assert response.json()[0].has_key('question')

#    Verify that a student can answer if they are symptomatic (true)
#    Endpoint -- api/wellness/
#    Expected Status Code -- 201 Created
#    Expected Response Body -- none
    def test_post_wellness_symptomatic_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/wellness/'
        self.data = {
            'answerValid': True,
            'timestamp': datetime,
            'userAnswer': True
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a student can answer if they are asymptomatic (false)
#    Endpoint -- api/wellness/
#    Expected Status Code -- 201 Created
#    Expected Response Body -- none
    def test_post_wellness_asymptomatic_student(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/wellness/'
        self.data = {
            'answerValid': True,
            'timestamp': datetime,
            'userAnswer': False
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a student can't change the questions
#    Endpoint -- api/wellness/question
#    Expected Status Code -- 404 Method Not Allowed
#    Expected Response Body -- A method not allowed error message
    def test_post_wellness_question_session(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/wellness/question'
        self.data = {
            'yesPrompt': 'Trying to change',
            'question': 'No questions',
            'noPrompt': 'Have a nice day of testing'
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 405:
            pytest.fail('Expected 405 Not Found, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == \
                "The requested resource does not support http method 'POST'."
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))

#    Verify that a faculty user can get their current status
#    Endpoint -- api/wellness/
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of one dictionary with user answer
    def test_get_wellness_student_asymptomatic_faculty(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/wellness/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        try:
            response.json()
        except ValueError:
            pytest.fail('Expected Json respone body, got {0}.'\
                .format(response.text))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        assert response.json()[0]["answerValid"] == True
        assert response.json()[0]["userAnswer"] == True or \
            response.json()[0]["userAnswer"] == False

#    Verify that a faculty user can get the wellness check question
#    Endpoint -- api/wellness/Question
#    Expected Status Code -- 200 OK
#    Expected Response Body -- A list of a dictionary of questions
#    and prompts.
    def test_get_wellness_question_faculty(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/wellness/question/'
        response = api.get(self.session, self.url)

        if not response.status_code == 200:
            pytest.fail('Expected 200 OK, got {0}.'\
                .format(response.status_code))
        if not (type(response.json()) is list):
            warnings.warn("Response is not a list.")
        assert response.json()[0].has_key('yesPrompt')
        assert response.json()[0].has_key('noPrompt')
        assert response.json()[0].has_key('question')

#    Verify that a faculty user can answer if they are symptomatic (true)
#    Endpoint -- api/wellness/
#    Expected Status Code -- 201 Created
#    Expected Response Body -- none
    def test_post_wellness_symptomatic_faculty(self):
        self.session = self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/wellness/'
        self.data = {
            'answerValid': True,
            'timestamp': datetime,
            'userAnswer': True
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a faculty user can answer if they are asymptomatic (false)
#    Endpoint -- api/wellness/
#    Expected Status Code -- 201 Created
#    Expected Response Body -- none
    def test_post_wellness_asymptomatic_faculty(self):
        self.session = self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/wellness/'
        self.data = {
            'answerValid': True,
            'timestamp': datetime,
            'userAnswer': False
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 201:
            pytest.fail('Expected 201 OK, got {0}.'\
                .format(response.status_code))

#    Verify that a faculty user can't change the questions
#    Endpoint -- api/wellness/question
#    Expected Status Code -- 405 Method Not Allowed
#    Expected Response Body -- A method not allowed error message
    def test_post_wellness_question_faculty(self):
        self.session = \
            self.createAuthorizedSession(control.leader_username, control.leader_password)
        self.url = control.hostURL + 'api/wellness/question'
        self.data = {
            'yesPrompt': 'Trying to change',
            'question': 'No questions',
            'noPrompt': 'Have a nice day of testing'
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 405:
            pytest.fail('Expected 405 Not Found, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == \
                "The requested resource does not support http method 'POST'."
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))
    
#    Verify that a guest can't get their current status
#    Endpoint -- api/wellness/
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_wellness_student_asymptomatic_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/wellness/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a guest can't get the wellness check question
#    Endpoint -- api/wellness/Question
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_get_wellness_question_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/wellness/question/'
        response = api.get(self.session, self.url)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))

#    Verify that a guest can't answer if they are symptomatic (true)
#    Endpoint -- api/wellness/
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_post_wellness_symptomatic_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/wellness/'
        self.data = {
            'answerValid': True,
            'timestamp': datetime,
            'userAnswer': True
        }
        response = api.post(self.session, self.url, self.data)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))


#    Verify that a guest can't answer if they are asymptomatic (false)
#    Endpoint -- api/wellness/
#    Expected Status Code -- 401 Unauthorized Error
#    Expected Response Body -- An authorization denied error
    def test_post_wellness_asymptomatic_guest(self):
        self.session = self.createGuestSession()
        self.url = control.hostURL + 'api/wellness/'
        self.data = {
            'answerValid': True,
            'timestamp': datetime,
            'userAnswer': False
        }
        response = api.post(self.session, self.url, self.data)

        if not response.status_code == 401:
            pytest.fail('Expected 401 Unauthorized Error, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == control.AUTHORIZATION_DENIED
        except ValueError:
            pytest.fail('Expected Json response body, got{0}.'\
                .format(response.text))


#    Verify that a guest can't change the questions
#    Endpoint -- api/wellness/question
#    Expected Status Code -- 404 Method Not Allowed
#    Expected Response Body -- A method not allowed error message
    def test_post_wellness_question_guest(self):
        self.session = self.createAuthorizedSession(control.username, control.password)
        self.url = control.hostURL + 'api/wellness/question'
        self.data = {
            'yesPrompt': 'Trying to change',
            'question': 'No questions',
            'noPrompt': 'Have a nice day of testing'
        }
        response = api.post(self.session, self.url, self.data)
        if not response.status_code == 405:
            pytest.fail('Expected 405 Not Found, got {0}.'\
                .format(response.status_code))
        try:
            assert response.json()['Message'] == \
                "The requested resource does not support http method 'POST'."
        except ValueError:
            pytest.fail('Expected Json response body, got {0}.'\
                .format(response.text))