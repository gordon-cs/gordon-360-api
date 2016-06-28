// Api Urls
//$apiUrl = "http://localhost:49645/api"
$apiUrl = "https://ccttrain.gordon.edu/api";
$activityUrl = $apiUrl + "/activities";
$membershipUrl = $apiUrl + "/memberships";
$studentUrl = $apiUrl + "/students";
$roleUrl = $apiUrl + "/roles";
$sessionUrl = $apiUrl + "/sessions";
$loginUrl = $apiUrl + "/authentication";

// Messages to Display In Table
$responseSuccess = '{"Response": "Success"}';
$responseError = '{"Response": "Error"}';
$responseNothingFound = '{"Response" : "Nothing Found"}';

$username = null;

window.onload = function() {
    populateSession();
    populateRole();
}

//////////////////////////////////////////////////////
// Utilities
//////////////////////////////////////////////////////

// Show Table of Data
function populateTable(data) {
    data = JSON.parse(data);
    document.getElementById("tableHead").innerHTML = "";
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
        tableHead.appendChild(tableRow);
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
        populateTable($responseNothingFound);
    }
    location.href = "#table";
}

// Populates Activty ID Select With All Activities for Post Membership
function populateActivities(sessionId) {
    url = $sessionUrl + "/" + sessionId.trim() + "/activities";
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            var select = document.getElementById('membershipPostActivityCode');
            select.innerHTML = "";
            var option = document.createElement("option");
            option.appendChild(document.createTextNode("Activity"));
            select.appendChild(option);
            for (var i = 0; i < data.length; i ++) {
                var option = document.createElement("option");
                option.appendChild(document.createTextNode(data[i].ACT_DESC));
                option.value = data[i].ACT_CDE;
                select.appendChild(option);
            }
        }
    }
    xhr.open("GET", url, true);
    xhr.send();
}

// Populates Role Select With All Roles for Post Membership
function populateRole() {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            var select = document.getElementById('membershipPostLevel');
            for (var i = 0; i < data.length; i ++) {
                var option  = document.createElement("option");
                option.appendChild(document.createTextNode(data[i].PART_DESC));
                option.value = data[i].PART_CDE;
                select.appendChild(option);
            }
        }
    }
    xhr.open("GET", $roleUrl, true);
    xhr.send();
}

// Populates Session Select With All Sessions for Post Membership
function populateSession() {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            var select = document.getElementById('membershipPostSessionCode');
            for (var i = 0; i < data.length; i ++) {
                var option  = document.createElement("option");
                option.appendChild(document.createTextNode(data[i].SESS_DESC));
                option.value = data[i].SESS_CDE;
                select.appendChild(option);
            }
        }
    }
    xhr.open("GET", $sessionUrl, true);
    xhr.send();
}

// Login
function login(username) {
    if (username == "") {
        alert("Please enter all fields!");
    }
    else {
        $username = username;
        sendPostRequest($loginUrl, username);
        document.getElementById("username").style.display = "none";
        document.getElementById("loginButton").style.display = "none";
        document.getElementById("name").innerHTML = "Hello, " + username;
    }
}

//////////////////////////////////////////////////////
// Activities
//////////////////////////////////////////////////////

// Get All Activities
function getActivities() {
    sendGetRequest($activityUrl);
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

//////////////////////////////////////////////////////
// Memberships
//////////////////////////////////////////////////////

// Get All Memberships
function getMemberships() {
    sendGetRequest($membershipUrl);
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

// Delete a Single Mebership
function deleteMembership(membershipId) {
    if (membershipId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendDeleteRequest($membershipUrl + "/" + membershipId);
        document.getElementById("membershipDeleteId").value = "";
    }
}

// Post Single Membership
function postMembership(activityCode, sessionCode, participationLevel, studentId, comments, beginDate, endDate) {
    if (activityCode == "" || studentId == "" || participationLevel == "" || sessionCode == "") {
        alert("Please enter all fields! (comments optional)");
    }
    else {
        var membership = {
            //"MEMBERSHIP_ID": null,
            "ACT_CDE": activityCode,
            "SESSION_CDE": sessionCode,
            "ID_NUM": studentId,
            "PART_LVL": participationLevel,
            "BEGIN_DTE": beginDate,
            "END_DTE": endDate,
            "DESCRIPTION": comments,
            "USER_NAME": null,
            "JOB_NAME": null,
            "JOB_TIME": null,
        }
        alert(JSON.stringify(membership));
        sendPostRequest($membershipUrl, JSON.stringify(membership));
        document.getElementById("membershipPostStudentId").value = "";
        document.getElementById("membershipPostComments").value = "";
        document.getElementById("membershipPostBeginDate").value = "";
        document.getElementById("membershipPostEndDate").value = "";
    }
}

//////////////////////////////////////////////////////
// Roles
//////////////////////////////////////////////////////

// Get All Roles
function getRoles() {
    sendGetRequest($roleUrl);
}

// Get Single Role
function getRole(roleId) {
    if (roleId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($roleUrl + "/" + roleId);
        document.getElementById("roleGetId").value = "";
    }
}

//////////////////////////////////////////////////////
// Students
//////////////////////////////////////////////////////

// Get All Students
function getStudents() {
    sendGetRequest($studentUrl);
}

// Get Single Student
function getStudent(studentId) {
    if (studentId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($studentUrl + "/" + studentId);
        document.getElementById("studentGetId").value = "";
    }
}

// Get All Memberships a Student is In
function getMembershipsByStudent(studentId) {
    if (studentId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($studentUrl + "/" + studentId + "/memberships");
        document.getElementById("membershipsByStudentGetId").value = "";
    }
}

//////////////////////////////////////////////////////
// Sessions
//////////////////////////////////////////////////////

// Get All Sessions
function getSessions() {
    sendGetRequest($sessionUrl);
}

// Get All Activities In Session
function getActivitiesInSession(sessionId) {
    if (sessionId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($sessionUrl + "/" + sessionId + "/activities");
        document.getElementById("activitiesInSessionGetId").value = "";
    }
}

// Get Single Session
function getSession(sessionId) {
    if (sessionId == "") {
        alert("Please enter all fields!");
    }
    else {
        sendGetRequest($sessionUrl + "/" + sessionId);
        document.getElementById("sessionGetId").value = "";
    }
}

//////////////////////////////////////////////////////
// Requests
//////////////////////////////////////////////////////

// Get Request
function sendGetRequest(url) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                populateTable(xhr.responseText);
            }
            else {
                populateTable($responseError);
            }
        }
    }
    xhr.open("GET", url, true);
    xhr.send();
}

// Post Request
function sendPostRequest(url, data) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) {
            if (xhr.status == 201) {
                populateTable($responseSuccess);
            }
            else {
                populateTable($responseError);
            }
        }
    }
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-type", "application/json");
    xhr.send(data);
}

// Delete Request
function sendDeleteRequest(url, data) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) {
            if (xhr.status == 204) {
                populateTable($responseSuccess);
            }
            else {
                populateTable($responseError);
            }
        }
    }
    xhr.open("DELETE", url, true);
    xhr.send(data);
}
