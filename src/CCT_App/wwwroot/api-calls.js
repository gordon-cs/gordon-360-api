$testUrl = "http://ipinfo.io/json";
$test2Url = "http://ccttrain.gordon.edu";
// Base API Url
//$apiUrl = "http://localhost:5000/api"
$apiUrl = "http://ccttrain.gordon.edu/api";
// Acitivity Url
$activityUrl = $apiUrl + "/activities";
// Membershup Url
$membershipUrl = $apiUrl + "/memberships";
// Login
$loginUrl = $apiUrl + "/authentication";

$username = null;

// Show table of data
function populateTable(data) {
    data = JSON.parse(data);

    document.getElementById("tableBody").innerHTML = "";
    var tableBody = document.getElementById("tableBody");

    if (data.length == undefined) {
        tableHeaderRow = document.createElement("tr");
        for (var key in data) {
            var tableHeader = document.createElement("th");
            tableHeader.appendChild(document.createTextNode(key));
            tableHeaderRow.appendChild(tableHeader);
        }
        tableBody.appendChild(tableHeaderRow);
        var tableRow = document.createElement("tr");
        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                var item = data[key];

                var tableItem = document.createElement("td");
                tableItem.appendChild(document.createTextNode(item));

                tableRow.appendChild(tableItem);
            }
        }
        tableBody.appendChild(tableRow);
    }
    else {
        tableHeaderRow = document.createElement("tr");
        for (var key in data[0]) {
            var tableHeader = document.createElement("th");
            tableHeader.appendChild(document.createTextNode(key));
            tableHeaderRow.appendChild(tableHeader);
        }
        tableBody.appendChild(tableHeaderRow);
        for (var i = 0; i < data.length; i ++) {
            var tableRow = document.createElement("tr");
            for (var key in data[i]) {
                if (data[i].hasOwnProperty(key)) {
                    var item = data[i][key];

                    var tableItem = document.createElement("td");
                    tableItem.appendChild(document.createTextNode(item));

                    tableRow.appendChild(tableItem);
                }
            }
            tableBody.appendChild(tableRow);
        }
    }
}

// Test Get Request
function test() {
    sendGetRequest($testUrl);
}

// Login
function login(username) {
    $username = username;

    sendPostRequest($loginUrl, username);

    document.getElementById("username").style.display = "none";
    document.getElementById("loginButton").style.display = "none";
    document.getElementById("name").innerHTML = "Hello, " + username;
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
    sendGetRequest($activityUrl + "/" + activityId);
    document.getElementById("activityGetId").value = "";
}

// Get Single Membership
function getMembership(membershipId) {
    sendGetRequest($membershipUrl + "/" + membershipId);
    document.getElementById("membershipGetId").value = "";
}

// Post Single Activity
function postActivity(activityName, activityAdvisor, activityDescription) {
    var activity = {
        "activity_name": activityName,
        "activity_advisor": activityAdvisor,
        "activity_description": activityDescription
    }
    sendPostRequest($activityUrl, JSON.stringify(activity));
    document.getElementById("activityPostName").value = "";
    document.getElementById("activityPostAdvisor").value = "";
    document.getElementById("activityPostDescription").value = "";
}

// Post Single Membership
function postMembership(studentId, activityId, membershipLevel) {
    var membership = {
        "student_id": studentId,
        "activity_id": activityId,
        "membership_level": membershipLevel
    }
    sendPostRequest($membershipUrl, JSON.stringify(membership));
    document.getElementById("membershipPostStudentId").value = "";
    document.getElementById("membershipPostActivityId").value = "";
    document.getElementById("membershipPostLevel").value = "";
}

// Get Request
function sendGetRequest(url) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200)
            //show(xhr.responseText);
            populateTable(xhr.responseText);
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
    xhr.setRequestHeader("Content-type", "application/json");
    xhr.send(data);
}
