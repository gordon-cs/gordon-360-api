<cfinclude template="/lib/auth/level3auth.cfm">
<cfparam name="form.studentcriteria" default="">
<cfparam name="form.facultycriteria" default="">
<cfparam name="form.alumnicriteria" default="">
<cfparam name="IncludeAlumni" default="N">
<cfparam name="ShowWarning" default="false">
<html><!-- InstanceBegin template="/Templates/community.dwt" codeOutsideHTMLIsLocked="false" -->
<!-- InstanceParam name="DocTitle" type="text" value="Gordon College: People Search " -->
<!-- InstanceParam name="Keywords" type="text" value="Gordon, People, Search" -->
<!-- InstanceParam name="OptionalColumn1" type="boolean" value="true" -->
<head>
    <title>Gordon College: People Search </title>
<meta http-equiv="Content-Type" content="text/html;">
<meta name="keywords" content="Gordon, People, Search"><link rel="stylesheet" type="text/css" href="/lib/css/gosite.css">
<script language="JavaScript">
if (parent != window)
	parent.location = window.location;
</script>
<script language="Javascript" type="text/javascript" src="/lib/js/gordon.js"></script>
</head>
   <body bottommargin="0" topmargin="0" rightmargin="0" leftmargin="0" background="/images/bg_image.gif">
   	<table border="0" cellpadding="0" cellspacing="0" width="800">
   		<tr class="dark"><td><img src="/images/gologo.gif" alt="Go.Gordon.edu" width="160" height="84" border="0"></td><td align="right"><cf_banner></td></tr>
   		<tr class="dark"><td><img src="/images/gologo_shade.gif" alt="" width="160" height="6" border="0"></td>	<td><img src="/images/banner_shade.gif" alt="" width="640" height="6" border="0"></td></tr>
   		<tr><td>                
            <!--- left nav table 2--->
            <table border="0" cellpadding="0" cellspacing="0" width="160">  
                                <tr ><td class="medium"  valign="bottom"  height="44"><cfinclude template="/lib/inc/user_info.inc"></td></tr>
                                <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="1" height="1" border="0"></td></tr>
                                <tr><td><img src="/images/navhdr_categories.gif" alt="Categories" border="0" width="160" height="44"></td></tr>
                                <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="1" height="1" border="0"></td></tr>
                                <tr><td><img src="/images/btn_community_off.gif" alt="Community" border="0" class="leftnav" width="160" height="22"   name="community"  onMouseOut="MM_swapImgRestore()" onMouseOver="MM_swapImage('community','','/images/btn_community_on.gif',1)" onClick="newlocation('/')"></td></tr>
                                <!--- <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="160" height="1" border="0"></td></tr> --->
								<tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="openWin('http://www.gordon.edu/bot','Win2');">- Board of Trustees</td></tr>
                                <tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/chat/');">- Chat</td></tr>   
                                <tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/communities/featured/');">- Featured Speakers</td></tr>
								<tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="openWin('/forums/','Win2');">- Forums</td></tr>
								<tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/communities/gbay/');">- GBAY</td></tr>
                                <tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/communities/gcsa/');">- GCSA</td></tr>
								<tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="openWin('http://www.gordon.edu/webcam','Win2');">- Live Webcam</td></tr>					
                                <tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/communities/localinfo.cfm');">- Local Info</td></tr>
								<tr><td  class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/people/');">- People Search</td></tr> 			 
                                <tr><td class="leftnav" onMouseOver="style.backgroundColor='#66CCFF'" onMouseOut="style.backgroundColor='#3399cc'" onClick="newlocation('/communities/transportation/index.cfm');">- Transportation</td></tr> 
                                <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="1" height="1" border="0"></td></tr>         
                                <cfinclude template="/lib/inc/user_links.inc">              
                                <tr><td><img src="/images/btn_academics_off.gif" alt="Academics" border="0" class="leftnav" width="160" height="22"  name="academics" onMouseOut="MM_swapImgRestore()" onMouseOver="MM_swapImage('academics','','/images/btn_academics_on.gif',1)"  onClick="newlocation('/academics/')"></td></tr>        
                                <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="1" height="1" border="0"></td></tr>
                                <tr><td><img src="/images/btn_departments_off.gif" alt="Departments" border="0" class="leftnav" width="160" height="22"  name="departments" onMouseOut="MM_swapImgRestore()" onMouseOver="MM_swapImage('departments','','/images/btn_departments_on.gif',1)"  onClick="newlocation('/departments/')"></td></tr> 
                                <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="1" height="1" border="0"></td></tr>
                                <tr><td><img src="/images/btn_campuslife_off.gif" alt="Campus Life" border="0" class="leftnav" width="160" height="22"   name="campuslife"  onMouseOut="MM_swapImgRestore()" onMouseOver="MM_swapImage('campuslife','','/images/btn_campuslife_on.gif',1)" onClick="newlocation('/campuslife/')"></td></tr>
                                <tr><td height="1"><img src="/images/blue_pixel.gif" alt="" width="1" height="1" border="0"></td></tr>
 
                                <tr><td><img src="/images/btn_news_off.gif" alt="News and Events" border="0" class="leftnav" width="160" height="22"  name="campus_news" onMouseOut="MM_swapImgRestore()" onMouseOver="MM_swapImage('campus_news','','/images/btn_news_on.gif',1)" onClick="newlocation('/news/')"></td></tr>
                                <tr><td><img src="/images/spacer.gif" alt="" border="0"  width="160" height="44"></td></tr>
                                <tr><td><img src="/images/navhdr_emergency.gif" alt="Emergency - dial 3333. Public Safety - dial 4444." border="0" width="160" height="44"></td></tr>
                                <tr><td><img src="/images/spacer.gif" alt="" border="0"  width="160" height="24"></td></tr>
          </table>
                        <!--- end left nav table 2 --->
   			</td>
   			<td valign="top">	<!--- table 3 top nav area--->
   				        <table border="0" cellpadding="0" cellspacing="0" width="640">
   					            <tr>
                                <td valign="top">	
                                    <!--- table 4 top nav buttons--->
                                    <table border="0" cellpadding="0" cellspacing="0" width="309" >
                                                <cfinclude template="/lib/inc/hdr_buttons.inc">
                                                <tr>
                                                <td><img src="/images/spacer.gif" alt="" border="0" width="5" height="1" ></td>
                                                <td colspan="7"><img src="/images/blue_pixel.gif" alt="" border="0" width="303" height="1" ></td>
                                                <td><img src="/images/blue_pixel.gif" alt="" border="0" width="1" height="1"></td>  
                                                </tr>  
                                                <cfinclude template="/lib/inc/top_nav.inc">
                                                <tr>
                                                <td><img src="/images/spacer.gif" alt="" border="0" width="5" height="1" ></td>
                                                <td colspan="7"><img src="/images/blue_pixel.gif" alt="" border="0" width="303" height="1" ></td>
                                                <td><img src="/images/blue_pixel.gif" alt="" border="0" width="1" height="1"></td>  
                                                </tr>  
                                    </table>
                                    <!--- end table 4 top nav buttons--->
 
   						        </td>
   						        <td valign="top" align="left">	
                                    <!--- table 5  Location and search--->
                                    <table width="330" cellspacing="0" cellpadding="0">
                                                <tr><td align="left"  width="318" height="22" style="vertical-align:middle;">&nbsp;&nbsp;Location: <a href="/">Home</a> / <a href="/communities/">Community</a></td><td><img src="/images/blue_pixel.gif" alt="" border="0" width="1" height="22"></td></tr>
                                                <tr><td align="left"><img src="/images/blue_pixel.gif" alt="" border="0" width="330" height="1"></td><td><img src="/images/blue_pixel.gif" alt="" border="0" width="1" height="1"></td></tr>
                                                <tr><td align="right" width="318" height="22" class="medium"><cfinclude template="/lib/inc/search_input.inc">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td><td><img src="/images/blue_pixel.gif" alt="" border="0" width="1" height="22"></td></tr>
                                                <tr><td align="left"><img src="/images/blue_pixel.gif" alt="" border="0" width="330" height="1"></td><td><img src="/images/blue_pixel.gif" alt="" border="0" width="1" height="1"></td></tr>   
                                    </table>
                                    <!--- end table 5 Location and search--->
   						        </td>
   					            </tr>
   				        </table>	
                        <!--- end table 3 top nav area--->
                        <!--- Content Table --->
                        <table cellpadding="2" cellspacing="2" width="100%" border="0">
                                    <tr><td><img src="/images/spacer.gif" alt="" width="5" height="1" border="0"></td>
                                    <td>
                                      <!-- InstanceBeginEditable name="Editable Content" -->
<script language="JavaScript1.2">
function reSort(col)
{
	switch(col)
	{
		case "LastName":
			document.reSortForm.stuSortByCol.value = "LastName";
			document.reSortForm.facSortByCol.value = "LastName";
			document.reSortForm.alumSortByCol.value = "LastName";
			break;
		case "FirstName":
			document.reSortForm.stuSortByCol.value = "FirstName";
			document.reSortForm.facSortByCol.value = "FirstName";
			document.reSortForm.alumSortByCol.value = "FirstName";
			break;
		case "StuFac":
			document.reSortForm.stuSortByCol.value = "StuFac";
			document.reSortForm.facSortByCol.value = "StuFac";
			document.reSortForm.alumSortByCol.value = "StuFac";
			break;
		case "ClassJob":
			document.reSortForm.stuSortByCol.value = "Class";
			document.reSortForm.facSortByCol.value = "JobTitle";
			document.reSortForm.alumSortByCol.value = "PreferredClassYear";
			break;
		case "Email":
			document.reSortForm.stuSortByCol.value = "Email";
			document.reSortForm.facSortByCol.value = "Email";
			document.reSortForm.alumSortByCol.value = "HomeEmail";
			break;
	}
	document.reSortForm.submit();
}
function personSelected_old(ad_username)
{
	//document.personSubmit.person.value = ad_username.replace(/\|/g,"'");
	//document.personSubmit.submit();
	ad_username = ad_username.replace(/\|/g,"'");
    myurl = 'showperson.cfm?user=' + ad_username;
    openWin(myurl,'Win2');
}
function personSelected(ad_username, row_id, person_type)
{
	//document.personSubmit.person.value = ad_username.replace(/\|/g,"'");
	//document.personSubmit.submit();
	ad_username = ad_username.replace(/\|/g,"'");
	if (ad_username == "") {
		if  (person_type == "Alumni")   myurl = 'showperson.cfm?aid=' + row_id;
		if ((person_type == "Faculty") || (person_type == "Staff")) myurl = 'showperson.cfm?fid=' + row_id;
		if  (person_type == "Student")  myurl = 'showperson.cfm?sid=' + row_id;
	}
	else {myurl = 'showperson.cfm?user=' + ad_username;}
    openWin(myurl,'Win2');
}
</script>
<cfoutput>
<form name="reSortForm" id="reSortForm" method="post">
	<input type="hidden" name="stuSortByCol" id="stuSortByCol">
	<input type="hidden" name="facSortByCol" id="facSortByCol">
	<input type="hidden" name="alumSortByCol" id="alumSortByCol">
	<input type="hidden" name="studentCriteria" id="studentCriteria" value="#form.studentCriteria#">
	<input type="hidden" name="facultyCriteria" id="facultyCriteria" value="#form.facultyCriteria#">
	<input type="hidden" name="alumniCriteria" id="alumniCriteria" value="#form.alumniCriteria#">
	<input type="hidden" name="IncludeAlumni" id="IncludeAlummni" value="#IncludeAlumni#">
</form>
<form name="personSubmit" id="personSumit" method="post" action="showperson.cfm" target="Win2">
	<input type="hidden" name="person" id="person">
</form>
</cfoutput>
<h3>Search Results</h3>

<cfset sqlstudent = form.studentCriteria>
<!--- <cfoutput>#sqlstudent#</cfoutput> --->

<cflock scope="session" timeout="20">
	<cfset accessList = session.access_list>
</cflock>
<cfparam name="PublicSafety" type="boolean" default="false">
<cfif ListFindNoCase(accessList, "PUBSAF") GT 0>
	<cfset PublicSafety = true>
</cfif>
<cfparam name="AlumIDLookup" type="boolean" default="false">
<cfif ListFindNoCase(accessList, "AID") GT 0>
	<cfset AlumIDLookup = true>
</cfif>

<cfif trim(sqlstudent) neq "">
	<cfoutput>
	<cfif PublicSafety>
		<cfquery name="student" datasource="#request.mygordon#">
			SELECT *, LastName+' '+Firstname as Name FROM student WHERE #PreserveSingleQuotes(sqlstudent)# AND id <> ''
			ORDER BY  LastName, FirstName 
		</cfquery>
	<cfelse>
		<cfquery name="student" datasource="#request.mygordon#"> 
	 	SELECT *, LastName+' '+Firstname as Name FROM student WHERE #PreserveSingleQuotes(sqlstudent)# AND id <> '' AND (keepprivate is null OR keepprivate = 'S')
		ORDER BY  LastName, FirstName 
		</cfquery>
	</cfif>
	</cfoutput>
</cfif>
				  
<cfset sqlfaculty = form.facultyCriteria>
<cfif trim(sqlfaculty) neq "">
	<cfoutput>
	<cfquery name="faculty" datasource="#request.mygordon#">
			
	<!--- Modified 10/05/2015 by HLG per CTS-85057 to omit Non-Active retired faculty with Permanent GoSite Access from PeopleSearch --->  
		SELECT Distinct * ,LastName+' '+Firstname as Name FROM facstaff 
        LEFT OUTER JOIN (SELECT Distinct Eml.Emp,Eml.EmlEffDt, Emp.PrmGoSiteAcc FROM HRDATA.NuGDN.dbo.Eml Eml 
						 INNER JOIN HRDATA.NuGDN.dbo.Emp Emp on Eml.Emp = Emp.Emp 
						 WHERE Eml.EmlChgRsn in ('TVRetired','TVEmeritus')) g ON facstaff.id = g.emp and PrmGoSiteAcc <> 1
		WHERE #PreserveSingleQuotes(sqlfaculty)# 
		AND ActiveAccount = 1
	 	ORDER BY  LastName, FirstName ;
	<!------------------------------------------------------------------------------------------------------------------->
	
	 
	</cfquery>
	</cfoutput>
</cfif>

<cfset sqlalumni = form.alumniCriteria>
<cfif (trim(sqlalumni) neq "") and (IncludeAlumni IS "Y")>
	<cfoutput>
	<cfif AlumIDLookup>
		<cfquery name="alumni" datasource="#request.mygordon#">
			SELECT *, LastName+' '+Firstname as Name FROM alumni WHERE #PreserveSingleQuotes(sqlalumni)# 
			AND id <> ''
			ORDER BY  LastName, FirstName 
		</cfquery>
	<cfelse>
		<cfquery name="alumni" datasource="#request.mygordon#">
			SELECT *, LastName+' '+Firstname as Name FROM alumni WHERE #PreserveSingleQuotes(sqlalumni)# 
			AND id <> '' AND upper(sharename) = 'Y'
			ORDER BY  LastName, FirstName 
		</cfquery>
	</cfif>
	
	</cfoutput>
</cfif>


<cfif isDefined("form.stuSortByCol")>
	<cfset stuSortByCol = form.stuSortByCol>
<cfelse>
	<cfset stuSortByCol = "Name">
</cfif>
<cfif isDefined("form.facSortByCol")>
	<cfset facSortByCol = form.facSortByCol>
<cfelse>
	<cfset facSortByCol = "Name">
</cfif>
<cfif isDefined("form.alumSortByCol")>
	<cfset alumSortByCol = form.alumSortByCol>
<cfelse>
	<cfset alumSortByCol = "Name">
</cfif>

	<cfset searchStruct = StructNew()>
	
	<cfset i = 1>
	<cfoutput>
	<cfif isDefined("student")>
	<cfloop query="student">
		<cfif stuSortByCol eq "StuFac">
			<cfset sortByCol = "student">
		<cfelseif Evaluate("student.#stuSortByCol#") eq "">
			<cfset sortByCol = " -- ">
		<cfelse>
			<cfset sortByCol = Evaluate("student.#stuSortByCol#")>
		</cfif>
		<cfswitch expression="#student.Class#">
			<cfcase value="1"><cfset className = "Freshman"></cfcase>
			<cfcase value="2"><cfset className = "Sophomore"></cfcase>
			<cfcase value="3"><cfset className = "Junior"></cfcase>
			<cfcase value="4"><cfset className = "Senior"></cfcase>
			<cfcase value="5"><cfset className = "Graduate Student"></cfcase>
			<cfcase value="6"><cfset className = "Undergraduate Conferred"></cfcase>
			<cfcase value="7"><cfset className = "Graduate Conferred"></cfcase>
			<cfdefaultcase>
				<cfset className = " -- ">
			</cfdefaultcase>
		</cfswitch>
		<cfset Fname = student.FirstName>
		<cfif trim(student.NickName) neq "" and student.NickName neq Student.FirstName>
			<cfset Fname = Fname & " (" & student.Nickname & ")">
		</cfif>
		<cfset searchStruct["order#i#"] = sortByCol & ";" & student.LastName & ";" & student.id & ";" & student.LastName & ";" & Fname & ";Student;" & className & " ;" & LCase(student.email) & " ;" & student.KeepPrivate & " ;" & student.row_ID & " ;"> 
		<cfset i = i + 1>
	</cfloop>
	</cfif>
	</cfoutput>

	<cfoutput>
	<cfif isDefined("faculty")>
	<cfloop query="faculty">
		<cfif facSortByCol eq "StuFac">
			<cfset sortByCol = "faculty">
		<cfelseif Evaluate("faculty.#facSortByCol#") eq "">
			<cfset sortByCol = " -- ">
		<cfelse>
			<cfset sortByCol = Evaluate("faculty.#facSortByCol#")>
		</cfif>
        <!--- <cfif ucase(left(faculty.id,1)) eq "F"><cfset facstafftype = "Faculty"><cfelse><cfset facstafftype = "Staff"></cfif> --->
		<cfset facstafftype = faculty.type>
		<cfset Fname = faculty.FirstName>
		<cfif trim(faculty.NickName) neq "" and faculty.NickName neq faculty.FirstName>
			<cfset Fname = Fname & " (" & faculty.Nickname & ")">
		</cfif>
		<cfif Len(faculty.EmlEffDt) gt 0 >
            <cfset myJobTitle = 'Retired'>
        <cfelse>
            <cfset myJobTitle = faculty.JobTitle>
        </cfif> 
		<cfset searchStruct["order#i#"] = sortByCol & ";" & faculty.LastName & ";" & faculty.id & ";" & faculty.LastName & ";" & Fname & ";" & facstafftype & ";" & myJobTitle & " ;" & LCase(faculty.email) & " ;" & faculty.KeepPrivate & " ;" & faculty.row_ID & " ;">
		<cfset i = i + 1>
	</cfloop>
	</cfif>
	</cfoutput>

	<cfoutput>
	<cfif isDefined("alumni")>
		<cfloop query="alumni">
			<cfif alumSortByCol eq "StuFac">
				<cfset sortByCol = "alumni">
			<cfelseif Evaluate("alumni.#alumSortByCol#") eq "">
				<cfset sortByCol = " -- ">
			<cfelse>
				<cfset sortByCol = Evaluate("alumni.#alumSortByCol#")>
			</cfif>
			<cfset Fname = alumni.FirstName>
			<cfif trim(alumni.NickName) neq "" and alumni.NickName neq alumni.FirstName>
				<cfset Fname = Fname & " (" & alumni.Nickname & ")">
			</cfif>
			<cfif trim(alumni.PreferredClassYear neq "")>
				<cfset gradyear = "Class of " & left(alumni.PreferredClassYear, 4)>
			<cfelseif trim(alumni.ClassYear neq "")>
				<cfset gradyear = "Class of " & alumni.ClassYear>
			<cfelse>
				<cfset gradyear = "Grad Year Unavailable">	
			</cfif>
			<cfset searchStruct["order#i#"] = sortByCol & ";" & alumni.LastName & ";" & alumni.id & ";" & alumni.LastName & ";" & Fname & ";Alumni;" & gradyear & " ;" & LCase(alumni.homeemail) & " ;" & " ;" & alumni.row_id & " ;">
			<cfset i = i + 1>
		</cfloop>
	</cfif>
	</cfoutput>

	<cfset sortStruct = ArrayToList(StructSort(searchStruct,"textnocase","ASC"),";")>
	<cfoutput>
	<table border="0" cellpadding="5" cellspacing="0">
		<tr>
			<td colspan="5">Please click a name to see more information on the selected person.</td>
		</tr>	
<!---	 <cfoutput>SELECT * FROM student WHERE #PreserveSingleQuotes(sqlstudent)# AND id != '' AND keepprivate = '' ORDER BY  LastName, FirstName ;</cfoutput> --->
		<tr>
			<td><a href="javascript: reSort('LastName');">Last Name</a></td>
			<td><a href="javascript: reSort('FirstName');">First Name (NickName)</a></td>
			<td><a href="javascript: reSort('StuFac');">Type</a></td>
			<td><a href="javascript: reSort('ClassJob');">Class / Job Title</a></td>
			<td><a href="javascript: reSort('Email');">Email</a></td>
		</tr>
		<tr>
			<td colspan="5"><hr size="1"></td>
		</tr>

	<cfif form.studentcriteria neq "" or form.facultycriteria neq "" or form.alumnicriteria neq "">
	
	<!--- Don't dispaly results if just selecting on HomeCountry = 'US' ---> 
	<cfif form.studentcriteria is "isnull(student.HomeCountry,'US') = 'US'" or 
	      form.facultycriteria is "isnull(facstaff.KeepPrivate,0)<>1 and isnull(facstaff.HomeCountry,'US') = 'US'" or
		  form.alumnicriteria is "isnull(alumni.HomeCountry,'US') = 'US' and alumni.HomeStreet2 not like '%Deceased%'">
			<tr><td colspan="5">Only criteria selected is <b>HomeCountry = 'United States'</b>.
			<br>Please try again with additional crtieria when selecting U.S. as the Home Country.</td></tr>
	<cfelse>
		<!--- Loop through all people found --->
		<cfset even = true>
		<cfset rowNum = 0>
		<cfloop list="#sortStruct#" delimiters=";" index="person">
			<!--- verify that this person should be displayed --->

			<cfset gordon_id = ListGetAt(#Evaluate("searchStruct.#person#")#,3,";")>
			<cfset person_type = ListGetAt(#Evaluate("searchStruct.#person#")#,6,";")>
			<cfset keep_private = ListGetAt(#Evaluate("searchStruct.#person#")#,9,";")>
			<cfset row_id = ListGetAt(#Evaluate("searchStruct.#person#")#,10,";")>
			
			<cfset FerpaProtected = false>
			<cfif person_type eq "Student" and keep_private contains "Y">
				<cfset FerpaProtected = true>
				<cfset ShowWarning = true>
			</cfif>
			
			<cfstoredproc datasource="#request.db#" procedure="account_get">
              <cfprocparam type="in" value="#gordon_id#" cfsqltype="cf_sql_varchar" dbvarname="@gordon_id">
              <cfprocresult name="getUser" maxrows="1">
            </cfstoredproc>
			
			<!--- <cfif (person_type eq "Alumni") OR (getUser.recordcount eq 1 and getUser.ad_username neq "" and getUser.disabled eq "N")> --->
            	<cfset my_ad_username=replacenocase(getUser.ad_username,"'","|")>
				<!--- Display the person in the row --->
				<cfset rowNum = rowNum + 1>
				<cfif even eq true><cfset colStyle = "alt1"><cfelse><cfset colStyle = "alt2"></cfif>
			    <tr class="#colStyle#" style="cursor:pointer;" onClick="personSelected('#my_ad_username#', '#row_id#', '#person_type#')" onMouseOver="this.className='medium'" onMouseOut="this.className='#colStyle#'"> 
				<a href="##">
 				<cfset colNum = 1>
				<cfloop list='#Evaluate("searchStruct.#person#")#' delimiters=";" index="element">
					<!--- cells of the row: lastname, firstname, type, class/job title --->
					<cfif colNum gt 3>
						<cfif FerpaProtected>
							<td style="color:##FF0000">
						<cfelse>
							<td>
						</cfif>
						<cfif Len("#element#") lte 1 or colNum ge 9>
							&nbsp;
						<cfelse>
							#element#
						</cfif>
					</td>					</cfif>
					<cfset colNum = colNum + 1>		
				</cfloop>
				</a>
				</tr>
	
				<cfif even eq true><cfset even = false><cfelse><cfset even = true></cfif>
			<!--- </cfif> --->
		</cfloop>
		<cfif ShowWarning>
			<tr><td colspan="5"><hr size="1"></td></tr>
			<tr><td style="color:##FF0000" colspan="5">
			<div class="alert">* - DO NOT DISCLOSE THE EXISTENCE OF THESE STUDENT(S)!</div> 
				
				The above student(s) are protected under the Federal Educational 
				Rights and Privacy Act (FERPA - 20 U.S.C. § 1232g) . Disclosure of this information 
				to any unauthorized party under any circumstances without previous written consent from 
				the student or his/her parents is a violation of federal law. For any further questions or 
				information contact the Registrar's Office at: (978) 867-4243.
			</td></tr>
		</cfif>
	  </cfif>
	<cfelse>
		<!--- Display no criteria message --->
		<tr colspan="4"><td>No search criteria supplied.</td></tr>
	</cfif>
	</table>
	<center><form action="index.cfm"><input type="submit" value="New Search"></form></center>
	</cfoutput>
	 <!-- InstanceEndEditable -->
	 								</td>
                                    </tr>
                                    <!--- footer --->
                                    <tr><td><img src="/images/spacer.gif" alt="" width="5" height="1" border="0"></td>
                                    <td align="center">
                                    <cfinclude template="/lib/inc/footer.inc">
                                    </td>
                                    </tr>
                        </table>
                        <!--- end Content table ---> 
   			</td></tr>
   	</table>
   </body>
<!-- InstanceEnd -->
</html>
