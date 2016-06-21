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

// Show Table of Data
function populateTable(data) {
    data = JSON.parse(data);
    document.getElementById("tableBody").innerHTML = "";
    var tableHead = document.getElementById("tableHead");
    var tableBody = document.getElementById("tableBody");
    // Add the Header Row to the Table
    function addHeader(item) {
        var tableRow = document.createElement("tr");
        for (var key in item) {
            var tableItem = document.createElement("th");
            tableItem.appendChild(document.createTextNode(key));
            tableRow.appendChild(tableItem);
        }
        tableBody.appendChild(tableRow);
    }
    // Add a Single Row to the Table
    function addItem(item) {
        var tableRow = document.createElement("tr");
        for (var key in item) {
            var value = item[key];
            var tableItem = document.createElement("td");
            tableItem.appendChild(document.createTextNode(value));
            tableRow.appendChild(tableItem);
        }
        tableBody.appendChild(tableRow);
    }
    // Function Called With a Single JSON Object
    if (data.length == undefined) {
        addHeader(data);
        addItem(data);
    }
    // Function Called With an array of JSON Objects
    else if (data.length > 0) {
        addHeader(data[0]);
        for (var i = 0; i < data.length; i ++) {
            addItem(data[i]);
        }
    }
    // Function Called with Empty Array
    else {
        addItem({"Response": "Nothing Found"});
    }
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
    if (activityId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($activityUrl + "/" + activityId);
        document.getElementById("activityGetId").value = "";
    }
}

// Get Single Membership
function getMembership(membershipId) {
    if (membershipId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($membershipUrl + "/" + membershipId);
        document.getElementById("membershipGetId").value = "";
    }
}

// Post Single Activity
function postActivity(activityName, activityAdvisor, activityDescription) {
    if (activityName == "" || activityAdvisor == "" || activityDescription == "") {
        alert("Please enter all fields!");
    }
    else {
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
}

// Post Single Membership
function postMembership(studentId, activityId, membershipLevel, sessionId, comments) {
    if (studentId == "" || activityId == "" || membershiplevel == "" || sessionId == "") {
        alert("Please enter all fields! (comments optional)");
    }
    else {
        var membership = {
            "student_id": studentId,
            "activity_id": activityId,
            "membership_level": membershipLevel,
            "session_id": sessionId,
            "comments": comments
        }
        sendPostRequest($membershipUrl, JSON.stringify(membership));
        document.getElementById("membershipPostStudentId").value = "";
        document.getElementById("membershipPostActivityId").value = "";
        document.getElementById("membershipPostLevel").value = "";
        document.getElementById("membershipPostSessionId").value = "";
        document.getElementById("membershipPostComments").value = "";
    }
}

// Get Request
function sendGetRequest(url) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200)
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
