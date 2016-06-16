$testUrl = "http://ipinfo.io/json";
$test2Url = "http://ccttrain.gordon.edu";
// Base API Url
$apiUrl = "http://ccttrain.gordon.edu/api";
// Acitivity Url
$activityUrl = $apiUrl + "/activities";
// Membershup Url
$membershipUrl = $apiUrl + "/memberships";

// Get All Activities
var getActivities = function() {
    sendGetRequest($activityUrl);
}

// Get All Memberships
var getMemberships = function() {
    sendGetRequest($membershipUrl);
}

// Get Single Acitivty
var getActivity = function(activityId) {
    alert ($activityUrl + "/" + activityId);
    sendGetRequest($activityUrl + "/" + activityId);
}

// Get Single Membership
var getMembership = function(membershipId) {
    alert($membershipUrl + "/" + membershipId);
    sendGetRequest($membershipUrl + "/" + membershipId);
}

// Post Single Activity
var postActivity = function(activityName, activityAdvisor, activityDescription) {
    var activity = {
        "activity_name": activityName,
        "activity_advisor": activityAdvisor,
        "activity_description": activityDescription
    }
    alert(JSON.stringify(activity));
    sendPostRequest($activityUrl, activity);
}

// Post Aingle Membership
var postMembership = function(studentId, activityId, membershipLevel) {
    var membership = {
        "student_id": studentId,
        "activity_id": activityId,
        "membership_level": membershipLevel
    }
    alert(JSON.stringify(membership));
    sendPostRequest($membershipUrl, membership);
}

// Get Request
// Returns JSON response
var sendGetRequest = function(url) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200)
            var response = JSON.parse(xhr.responseText);
            document.getElementById("p1").innerHTML = xhr.responseText;
            return response;
    }
    xhr.open("GET", url, true);
    xhr.send();
}

// Post Request
var sendPostRequest = function(url, data) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200)
            document.getElementById("p1").innerHTML = xhr.responseText;
    }
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.send(data);
}
