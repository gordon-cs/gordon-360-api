<cfinclude template="/lib/auth/level3auth.cfm">
<cfparam name="mylogin_type" default="">
<cflock timeout="20">
	<cfset mylogin_type = session.type>
</cflock>
<html><!-- InstanceBegin template="/Templates/community.dwt" codeOutsideHTMLIsLocked="false" -->
<!-- InstanceParam name="DocTitle" type="text" value="Gordon College: People Search " -->
<!-- InstanceParam name="Keywords" type="text" value="Gordon, People, Search" -->
<!-- InstanceParam name="OptionalColumn1" type="boolean" value="true" -->
<head>
    <title>Gordon College: People Search </title>
<meta http-equiv="Content-Type" content="text/html;">
<meta name="keywords" content="Gordon, People, Search"><link rel="stylesheet" type="text/css" href="/lib/css/gosite.css">
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
								<tr><td class="leftnav" onmouseover="style.backgroundColor='#66CCFF'" onmouseout="style.backgroundColor='#3399cc'" onclick="openWin('http://www.gordon.edu/bot','Win2');">- Board of Trustees</td></tr>
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

<script type="text/javascript" language="JavaScript">
function buildSQL()
{
	var sqlString, fac_sqlString ;
	sqlString = "";
	fac_sqlString = "";
	alum_sqlString = "";
	
	if(document.searchForm.lNameInput.value != "")
	{
		switch(document.searchForm.lNameParam.value)
		{	case "begins with":
				sqlString = sqlString + " and student.LastName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%'";
				fac_sqlString = fac_sqlString + " and facstaff.LastName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " and (alumni.LastName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " or alumni.MaidenName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%')";
				break;
			case "equals":
				sqlString = sqlString + " and student.LastName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " and facstaff.LastName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " and (alumni.LastName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " or alumni.MaidenName like '" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "')";
				break;
			case "contains":
				sqlString = sqlString + " and student.LastName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%'"
				fac_sqlString = fac_sqlString + " and facstaff.LastName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " and (alumni.LastName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " or alumni.MaidenName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "%')";
				break;
			case "ends with":
				sqlString = sqlString + " and student.LastName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " and facstaff.LastName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " and (alumni.LastName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " or alumni.MaidenName like '%" + document.searchForm.lNameInput.value.replace(/\'/g,"''") + "')";
				break;
		}
	}
	if(document.searchForm.fNameInput.value != "")
	{
		switch(document.searchForm.fNameParam.value)
		{	case "begins with":
				sqlString = sqlString + " and (student.FirstName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%'";
				sqlString = sqlString + " or student.NickName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%')";
				fac_sqlString = fac_sqlString + " and (facstaff.FirstName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%'";
				fac_sqlString = fac_sqlString + " or facstaff.NickName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%')";
				alum_sqlString = alum_sqlString + " and (alumni.FirstName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " or alumni.NickName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%')";
				break;
			case "equals":
				sqlString = sqlString + " and (student.FirstName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "'";
				sqlString = sqlString + " or student.NickName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "')";
				fac_sqlString = fac_sqlString + " and (facstaff.FirstName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " or facstaff.NickName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "')";
				alum_sqlString = alum_sqlString + " and (alumni.FirstName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " or alumni.NickName like '" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "')";
				break;
			case "contains":
				sqlString = sqlString + " and (student.FirstName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%'";
				sqlString = sqlString + " or student.NickName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%')";
				fac_sqlString = fac_sqlString + " and (facstaff.FirstName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%'";
				fac_sqlString = fac_sqlString + " or facstaff.NickName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%')";
				alum_sqlString = alum_sqlString + " and (alumni.FirstName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " or alumni.NickName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "%')";
				break;
			case "ends with":
				sqlString = sqlString + " and (student.FirstName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "'";
				sqlString = sqlString + " or student.NickName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "')";
				fac_sqlString = fac_sqlString + " and (facstaff.FirstName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " or facstaff.NickName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "')";
				alum_sqlString = alum_sqlString + " and (alumni.FirstName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " or alumni.NickName like '%" + document.searchForm.fNameInput.value.replace(/\'/g,"''") + "')";
				break;
		}
	}
	
	if(document.searchForm.Hall.value != "" && document.searchForm.Hall.value != '[Commuters]')
	{
		sqlString = sqlString + " and student.OnCampusBuilding like '" + document.searchForm.Hall.value.replace(/\'/g,"''") + "%'";
				fac_sqlString = fac_sqlString + " and facstaff.ID = '0'";  // exclude faculty/staff
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
	}
	if(document.searchForm.Hall.value != "" && document.searchForm.Hall.value == '[Commuters]')
	{
		sqlString = sqlString + " and student.OnOffCampus = 'O'";
				fac_sqlString = fac_sqlString + " and facstaff.ID = '0'";  // exclude faculty/staff
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
	}
	if(document.searchForm.RoomNumberInput.value != "" && document.searchForm.Hall.value != '[Commuters]')
	{
		sqlString = sqlString + " and student.OnCampusRoom = '" + document.searchForm.RoomNumberInput.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " and facstaff.ID = '0'";  // exclude faculty/staff
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
	}
    <!--- DRA - 08/05/2013 Room phone extensions will no longer be available Fall 2013, so there's no point in displaying invalid data.
		if(document.searchForm.PhoneInput.value != "")
			sqlString = sqlString + " and student.OnCampusPhone = '" + document.searchForm.PhoneInput.value.replace(/\'/g,"''") + "'"; 
	--->
	if(document.searchForm.Major.value != "")
	{
		sqlString = sqlString + "  and (student.Major in " + document.searchForm.Major.value + " or student.Major2 in " + document.searchForm.Major.value + " or student.Major3 in " + document.searchForm.Major.value + ")";
				fac_sqlString = fac_sqlString + " and facstaff.ID = '0'";  // exclude faculty/staff
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
	}
	if(document.searchForm.Minor.value != "")
	{
		sqlString = sqlString + " and (student.Minor1 in " + document.searchForm.Minor.value + " or student.Minor2 in " + document.searchForm.Minor.value + " or student.Minor3 in " + document.searchForm.Minor.value + ")";
				fac_sqlString = fac_sqlString + " and facstaff.ID = '0'";  // exclude faculty/staff
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
	}
	if(document.searchForm.Class.value != "")
	{
				fac_sqlString = fac_sqlString + " and facstaff.ID = '0'";  // exclude faculty/staff
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
		if(document.searchForm.Class.value == "5")
			sqlString = sqlString + " and (student.Class = '5' and student.Grad_Student = 'Y')";
		else
			if(document.searchForm.Class.value == "7")
				sqlString = sqlString + " and (student.Class = '7' and student.Grad_Student = 'Y')";
			else
				sqlString = sqlString + " and (student.Class = '" + document.searchForm.Class.value.replace(/\'/g,"''") + "')";
	}
	if(document.searchForm.Hometown.value != "")
	{
		switch(document.searchForm.HometownParam.value)
		{	case "begins with":
				sqlString = sqlString + " and student.HomeCity like '" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "%'";
				fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and facstaff.HomeCity like '" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " and alumni.HomeCity like '" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "%'";
				break;
			case "equals":
				sqlString = sqlString + " and student.HomeCity like '" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and facstaff.HomeCity like '" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " and alumni.HomeCity like '" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "'";
				break;
			case "contains":
				sqlString = sqlString + " and student.HomeCity like '%" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "%'";
				fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and facstaff.HomeCity like '%" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "%'";
				alum_sqlString = alum_sqlString + " and alumni.HomeCity like '%" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "%'";
				break;
			case "ends with":
				sqlString = sqlString + " and student.HomeCity like '%" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "'";
				fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and facstaff.HomeCity like '%" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "'";
				alum_sqlString = alum_sqlString + " and alumni.HomeCity like '%" + document.searchForm.Hometown.value.replace(/\'/g,"''") + "'";
				break;
		}
		
	}
	if(document.searchForm.HomeState.value != "") {
		sqlString = sqlString + " and student.HomeState = '" + document.searchForm.HomeState.value.replace(/\'/g,"''") + "'";
		fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and facstaff.HomeState = '" + document.searchForm.HomeState.value.replace(/\'/g,"''") + "'";
		alum_sqlString = alum_sqlString + " and alumni.HomeState = '" + document.searchForm.HomeState.value.replace(/\'/g,"''") + "'";
	}
	if(document.searchForm.HomeZip.value != "") {
		sqlString = sqlString + " and student.HomePostalCode = '" + document.searchForm.HomeZip.value.replace(/\'/g,"''") + "'";
		fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and facstaff.HomePostalCode = '" + document.searchForm.HomeZip.value.replace(/\'/g,"''") + "'";
		alum_sqlString = alum_sqlString + " and alumni.HomePostalCode = '" + document.searchForm.HomeZip.value.replace(/\'/g,"''") + "'";
	}
	if(document.searchForm.HomeCountry.value != "") {
		sqlString = sqlString + " and isnull(student.HomeCountry,'US') = '" + document.searchForm.HomeCountry.value.replace(/\'/g,"''") + "'";
		fac_sqlString = fac_sqlString + " and isnull(facstaff.KeepPrivate,0)<>1 and isnull(facstaff.HomeCountry,'US') = '" + document.searchForm.HomeCountry.value.replace(/\'/g,"''") + "'";
		alum_sqlString = alum_sqlString + " and isnull(alumni.HomeCountry,'US') = '" + document.searchForm.HomeCountry.value.replace(/\'/g,"''") + "'";
	}
	
	if(document.searchForm.Office.value != "")
	{
		fac_sqlString = fac_sqlString + " and facstaff.OnCampusDepartment like '" + document.searchForm.Office.value.replace(/\'/g,"''") + "'";
				sqlString = sqlString + " and student.ID = '0'";           // exclude students
				alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
		
	}
		
	if(document.searchForm.Building.value != "")
	{
		fac_sqlString = fac_sqlString + " and facstaff.OnCampusBuilding = '" + document.searchForm.Building.value.replace(/\'/g,"''") + "'";
			sqlString = sqlString + " and student.ID = '0'";           // exclude students
			alum_sqlString = alum_sqlString + " and alumni.ID = '0'";  // exclude alumni
	}
	
	if(sqlString != "")
		sqlString = sqlString.substring(5,sqlString.length);
	if(fac_sqlString != "")
		fac_sqlString = fac_sqlString.substring(5,fac_sqlString.length);
	if(alum_sqlString != "")
	{
		alum_sqlString = alum_sqlString + " and alumni.HomeStreet2 not like '%Deceased%'";
		alum_sqlString = alum_sqlString.substring(5,alum_sqlString.length);
	}
	
	document.getElementById("studentCriteria").value = sqlString;
	document.getElementById("facultyCriteria").value = fac_sqlString;
	document.getElementById("alumniCriteria").value = alum_sqlString;
	
	
	<!--- APRIL FOOLS 2005 --->
	document.getElementById("studentRelationship").value = document.searchForm.Relationship.value;
	<!--- APRIL FOOLS 2005 --->
	
	
}	

</script>									  
<form name="searchForm" id="searchForm" action="searchresults.cfm" method="post">
<h2>People Search</h2>
<table cellpadding="0" cellspacing="0">
<tr><td>
<br>
<table width="410" cellpadding="0" cellspacing="0">
    <tr><td colspan="3"><h4>Name Information</h4></td></tr>
	<tr>
		    <td width="100">Last Name:</td>
		    <td width="100">
				<select name="lNameParam" id="lNameParam" size="1">
					<option value="begins with" selected>Begins With</option>
					<option value="contains">Contains</option>
					<option value="equals">Equals</option>
					<option value="ends with">Ends With</option>
				</select>
            </td>
		              <td> 
                        <input type="text" name="lNameInput" id="lNameInput">
                      </td>
	</tr>
	<tr>
		<td>First Name:<br>(or NickName)</td>
		<td><select name="fNameParam" id="fNameParam" size="1">
				<option value="begins with" selected>Begins With</option>
				<option value="contains">Contains</option>
				<option value="equals">Equals</option>
				<option value="ends with">Ends With</option>
			</select>
		</td>
		<td><input type="text" name="fNameInput" id="fNameInput"></td>
	</tr>
<!--- Is the user a staff or faculty member? --->
<CFIF (mylogin_type IS "STAFF") OR (mylogin_type IS "FACULTY")>
	<!--- If yes, include a checkbox that allows an alumni search. --->
	<tr>
	<td colspan="3"><br>
	<input type="checkbox" name="IncludeAlumni" id="IncludeAlumni" value="Y"> Include alumni in search</td>
	</tr>
</CFIF>
</table>


<!--- APRIL FOOLS 2005 --->
<cfif mylogin_type eq "STUDENT" and (month(now()) eq 4 and day(now()) eq 1)>
<br><table width="100%" border="0"><tr><td height="1" class="module"></td></tr></table><br>
<table width="410" cellpadding="0" cellspacing="0">
                    <tr> 
                      <td colspan="4"><h4>Relationship Status</h4></td>
                    </tr>
					<tr>
					  <td colspan="4"><span style="color:#ac5ebd;"><b>*Now Available*</b></span></td>
					</tr>
                    <tr> 
                      <td width="100">Current Status:</td>
                      <td colspan="3" > <select name="Relationship"  id="Relationship" size="1">
                          <option value="" selected>Any</option>
						  <option value="Single - Available">Single - Available</option>
						  <option value="Single - Unavailable">Single - Unavailable</option>
						  <option value="Undefined">Undefined</option>
						  <option value="Taken">Taken</option>						
						  <option value="Married">Married</option>
                        </select> (Combine with additional criteria)</td>
                    </tr>					
                  </table>
<cfelse>
	<input type="hidden" name="Relationship" value="">
</cfif>
<!--- APRIL FOOLS 2005 --->


<br><table width="100%" border="0"><tr><td height="1" class="module"></td></tr></table><br>
<table width="410" cellpadding="0" cellspacing="0">
    <tr><td colspan="3"><h4>Dorm Room Information</h4></td></tr>
	<tr>
       <td width="100">Dorm:</td>
       <td> 
	   <select name="Hall" id="Hall" size="1">
			<option value="">Select A Dorm</option>
			<option value="[Commuters]">[Commuters]</option>
			<cftry>
    		<cfquery name="halls" datasource="#request.mygordon#">
    			SELECT BuildingID, BuildingDescription FROM ActiveDorms where BuildingID <> 'TEMP'
    		</cfquery>
    		<cfcatch>Database currently unaccessible. Please check back later.<cfabort></cfcatch>
    		</cftry>
    		<cfoutput query="halls"><option value="#halls.BuildingID#">#halls.BuildingDescription#</option></cfoutput>
		</select>
	   </td>
	</tr>
	<tr>
		<td>Room Number:</td>
		<td><input type="text" name="RoomNumberInput" id="RoomNumberInput" size="5" maxlength="4"></td>
	</tr>
	<!--- DRA - 08/05/2013 Room phone extensions will no longer be available Fall 2013, so there's no point in displaying invalid data.
	<tr>
		              <td>Phone Extension:</td>
		<td><input type="text" name="PhoneInput" id="PhoneInput" size="5" maxlength="4"></td>
	</tr> --->
</table>
<br><table width="100%" border="0"><tr><td height="1" class="module"></td></tr></table><br>
<table width="410" cellpadding="0" cellspacing="0">
    <tr><td colspan="3"><h4>Academic Information</h4></td></tr>
	<tr>
	  <td width="100">Major:</td>
		<td>
		<select name="Major" size="1">
			<option value="">Select A Major</option>
			<cftry>
    		<cfquery name="majors" datasource="#request.mygordon#">
    			SELECT MajorCode, MajorDescription FROM Majors_Unique
    		</cfquery>
    		<cfcatch>Database currently unaccessible. Please check back later.<cfabort></cfcatch>
    		</cftry>
    		<cfoutput query="majors"><option value="#majors.MajorCode#">#majors.MajorDescription#</option></cfoutput>
		</select>&nbsp;
		</td>
	</tr>
	<tr>
	  <td width="100">Minor:</td>
		<td>
		<select name="Minor" size="1">
			<option value="">Select A Minor</option>
			<cftry>
    		<cfquery name="minors" datasource="#request.mygordon#">
    			SELECT MinorCode, MinorDescription FROM Minors_Unique
    		</cfquery>
    		<cfcatch>Database currently unaccessible. Please check back later.<cfabort></cfcatch>
    		</cftry>
    		<cfoutput query="minors"><option value="#minors.MinorCode#">#minors.MinorDescription#</option></cfoutput>
		</select>&nbsp;
		</td>
	</tr>
	<tr>
	  <td width="100">Class:</td>
		<td>
			<select name="Class" size="1">
				<option selected value="">Select A Class
				<option value="1">Freshman
				<option value="2">Sophomore
				<option value="3">Junior
				<option value="4">Senior
				<option value="5">Graduate Student
				<option value="6">Undergraduate Conferred
				<option value="7">Graduate Conferred
				<option value="0">Unassigned
			</select>
		</td>
	</tr>
</table>

<br><table width="100%" border="0"><tr><td height="1" class="module"></td></tr></table><br>
<table width="410" cellpadding="0" cellspacing="0">
                    <tr><td colspan="3"><h4>Hometown Information</h4></td></tr>
                    <tr> 
                      <td width="100">Hometown:</td>
                      <td width="100"> 
					  <select name="HometownParam"  id="HometownParam" size="1">
                          <option value="begins with" selected>Begins With</option>
                          <option value="contains">Contains</option>
                          <option value="equals">Equals</option>
                          <option value="ends with">Ends With</option>
                      </select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
					  </td>
                      <td><input type="text" name="Hometown" id="Hometown"></td>
                    </tr>
                    <tr>
                      <td>Home State:</td>
					  <td>&nbsp;</td>
                      <td><input name="HomeState" type="text" id="HomeState2" size="2" maxlength="2"></td>
                    </tr>
                    <tr> 
                      <td>Home Zip Code:</td>
					  <td>&nbsp;</td>
                      <td><input name="HomeZip" type="text" id="HomeZip4" size="5" maxlength="5"></td>
                    </tr>
					<tr>
						<td>Home Country</td>
						<td>&nbsp;</td>
		    			<td>
						<select name="HomeCountry" id="HomeCountry" size="1">
						<option value="">Select A Country</option>
						<cftry>
						<!--- Only display country codes for Students or Alumni that come from that country.  --->
	    				<cfquery name="HomeCountry" datasource="#request.mygordon#">
	    					select distinct homecountry as country_code, table_desc as country_desc
							from mygordon.dbo.student s
							inner join tmseprd.dbo.table_detail t on t.column_name='country' and t.table_value = s.homecountry
							union
							select distinct homecountry as country_code, table_desc as country_desc
							from mygordon.dbo.alumni a
							inner join tmseprd.dbo.table_detail t on t.column_name='country' and t.table_value = a.homecountry
							order by table_desc
	    				</cfquery>
						<cfoutput query="HomeCountry"><option value="#HomeCountry.country_code#">#left(HomeCountry.country_desc,32)#</option></cfoutput>
    					<cfcatch></cfcatch>
    					</cftry>
						</select>
		    			</td>
	      		  </tr>
                  </table>
<br><table width="100%" border="0"><tr><td height="1" class="module"></td></tr></table>
<br><table width="500" cellpadding="0" cellspacing="0">
	<tr><td><h4>Office / Department <em>(Faculty/Staff Only)</em></h4></td></tr>
	<tr>
		  <td valign="top"> 
              <table border=0 cellspacing=1 cellpadding=0>
          <tr>
			<td width="100"> Office: </td>
		    <td align="left">
			<select name="Office" id="Office" size="1">
			<option value="">Select A Department</option>
			<cftry>
				<!--- PSB, 2006-12-07; changed to look to NuView for complete department list
				
						Was: SELECT code, description FROM codelists WHERE type='Dept' ORDER BY description
				--->
	    		<cfquery name="office" datasource="#request.mygordon#">
	    			SELECT code, description FROM vwDepartments ORDER BY description
	    		</cfquery>
				<cfoutput query="office"><option value="#office.description#">#office.description#</option></cfoutput>
    		<cfcatch></cfcatch>
    		</cftry>
			</select>
		    </td>
	      </tr>	  
		  <tr>
			<td width="100"> Building: </td>
		    <td align="left">
			<select name="Building" id="Building" size="1">
			<option value="">Select A Building</option>
			<cftry>
	    		<cfquery name="building" datasource="#request.db#">
	    			SELECT RTRIM(BLDG_CDE) AS BLDG_CDE, RTRIM(BUILDING_DESC) AS BUILDING_DESC, * FROM TmsEPrd.dbo.BUILDING_MASTER B
					WHERE BLDG_TYPE <> 'SR' and BLDG_CDE IN (select distinct building from websql.dbo.account) ORDER BY B.BUILDING_DESC
	    		</cfquery>
				<cfoutput query="building"><option value="#building.bldg_cde#">#building.building_desc#</option></cfoutput>
    		<cfcatch></cfcatch>
    		</cftry>
			</select>
		    </td>
	      </tr>
		  
		</table>		
		</td>
	</tr>
</table>

</td>
</tr>
</table>
<table width="360" cellpadding="5" cellspacing="0">
<tr><td align="center">
	<input type="hidden" name="facultyCriteria" id="facultyCriteria" value="">
	<input type="hidden" name="studentCriteria" id="studentCriteria" value="">
	
	
	<!--- APRIL FOOLS 2005 --->
	<input type="hidden" name="studentRelationship" id="studentRelationship" value="">
	<!--- APRIL FOOLS 2005 --->
	
	
	<input type="hidden" name="alumniCriteria" id="alumniCriteria" value="">
	<input type="hidden" name="searchStudents" id="searchStudents" value="False">
    <input type="hidden" name="searchFaculty" id="searchFaculty" value="False">
	<br><br><input type="submit" onClick="buildSQL()" value="Search">
</td></tr>
</table>
</form>
<script language="javascript" type="text/javascript">
document.searchForm.lNameInput.focus();
</script>

									  <!-- InstanceEndEditable --></td>
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
<!-- InstanceEnd --></html>
