$testUrl = "http://ipinfo.io/json";
$test2Url = "http://ccttrain.gordon.edu";
// Base API Url
$apiUrl = "http://ccttrain.gordon.edu/api";
// Acitivity Url
$activityUrl = $apiUrl + "/activities";
// Membershup Url
$membershipUrl = $apiUrl + "/memberships";

// Displays Text On Page
function show(text) {
    document.getElementById("p1").innerHTML = text;
}

function test() {
    sendGetRequest($testUrl);
}

// Get All Activities
function getActivities() {
    sendGetRequest($activityUrl);
}

// Get All Memberships
function getMemberships() {
    sendGetRequest($membershipUrl);
}

// Get Single Acitivty
function getActivity(activityId) {
    alert ($activityUrl + "/" + activityId);
    sendGetRequest($activityUrl + "/" + activityId);
}

// Get Single Membership
function getMembership(membershipId) {
    alert($membershipUrl + "/" + membershipId);
    sendGetRequest($membershipUrl + "/" + membershipId);
}

// Post Single Activity
function postActivity(activityName, activityAdvisor, activityDescription) {
    var activity = {
        "activity_name": activityName,
        "activity_advisor": activityAdvisor,
        "activity_description": activityDescription
    }
    alert(JSON.stringify(activity));
    sendPostRequest($activityUrl, activity);
}

// Post Aingle Membership
function postMembership(studentId, activityId, membershipLevel) {
    var membership = {
        "student_id": studentId,
        "activity_id": activityId,
        "membership_level": membershipLevel
    }
    alert(JSON.stringify(membership));
    sendPostRequest($membershipUrl, membership);
}

// Get Request
function sendGetRequest(url) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200)
            show(xhr.responseText);
    }
    xhr.open("GET", url, true);
    xhr.send();
}

// Post Request
function sendPostRequest(url, data) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200)
            show(xhr.responseText);
    }
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.send(data);
}
