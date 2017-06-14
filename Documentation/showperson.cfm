<cfparam name="url.user" default="">
<cfparam name="building_name" default="">
<cfinclude template="/lib/auth/level3auth.cfm">
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
<cflock scope="session" timeout="20">
	<cfset accessList = session.access_list>
	<cfset mytype = session.type>
</cflock>

<cfparam name="viewer_type" default="Student">
<cfif mytype eq "FACULTY" or mytype eq "STAFF">
	<cfset viewer_type = "Facstaff">
</cfif>

<cfparam name="PublicSafety" type="boolean" default="false">
<cfif ListFindNoCase(accessList, "PUBSAF") GT 0>
	<cfset PublicSafety = true>
</cfif>

<cfparam name="EmergencyContactViewer" type="boolean" default="false">
<cfif ListFindNoCase(accessList, "ECV") GT 0>
	<cfset PublicSafety = true>
</cfif>

<cfparam name="AlumIDLookup" type="boolean" default="false">
<cfif ListFindNoCase(accessList, "AID") GT 0>
	<cfset AlumIDLookup = true>
</cfif>

<cfif isDefined("form.person")>
	<cfset variables.ad_username = form.person>
<cfelse>
	<cfset variables.ad_username = url.user>
</cfif>

<cfset show_pic = true>
<cfset officehrs = "">
<cfset pref_photo = "">
<cfset the_barcode = "">
<cfset account_id = "">
<!--- Check querystring to see if Active Directory username was passed or alumni id) --->
<cfif len(url.user) gt 0>
	<!--- Person is current student or recent graduate --->
	<cfquery name="getId" datasource="#request.db#">
	SELECT gordon_id as GcID, account_type, account.account_id, barcode, isnull(show_pic,'') as Show_Pic,
	       office_hours as officehours, isnull(Pref_Pic_Exists,'') as Pref_Pic_Exists
    FROM account 
	LEFT OUTER JOIN account_profile 
	ON account.account_id = account_profile.account_id 
	WHERE account.ad_username = '#variables.ad_username#' 
	</cfquery>
	<cfset gc_type = getid.account_type>
	<cfset gordon_id = getId.GcID>
	<cfset officehrs = getId.officehours>
	<cfset pref_photo = getId.Pref_Pic_Exists>
	<cfif  getId.Show_Pic eq "N"><cfset show_pic = false></cfif>
	<cfset the_barcode = getId.barcode>
	<cfset account_id = getId.account_id>
	<cfquery name="student" datasource="#request.mygordon#">
		select * from student where id = '#gordon_id#' <cfif not PublicSafety>and (keepprivate is null OR keepprivate = 'S')</cfif>
	</cfquery>
	<cfquery name="facstaff" datasource="#request.mygordon#">
		select f.*,privacylevel from facstaff f inner join websql.dbo.account a on f.id = a.gordon_id where id = '#gordon_id#'
	</cfquery>
	<cfquery name="alumni" datasource="#request.mygordon#">
		select * from alumni where id = '#gordon_id#'
	</cfquery>
<cfelseif IsDefined("url.aid") and viewer_type is "Facstaff">
	<!--- Person is legacy alumni --->
	<cfquery name="alumni" datasource="#request.mygordon#">
		select websql.dbo.account.account_id as account_id, isnull(Pref_Pic_Exists,'') as Pref_Pic_Exists, isnull(show_pic,'') as Show_Pic, alumni.* from alumni
		left outer join websql.dbo.account on alumni.id = websql.dbo.account.gordon_id
		left outer join websql.dbo.account_profile on websql.dbo.account.account_id = websql.dbo.account_profile.account_id
		where row_id = '#url.aid#'
	</cfquery>
	<cfset gc_type = "ALUMNI">
	<cfset gordon_id = alumni.id>
	<cfset the_barcode = alumni.barcode>
	<cfset account_id = alumni.account_id>
	<cfif  alumni.Show_Pic eq "N"><cfset show_pic = false></cfif>
	<cfset pref_photo = alumni.Pref_Pic_Exists>
	<cfset facstaff.recordcount = 0>
	<cfset student.recordcount = 0>
<cfelseif IsDefined("url.fid")>
	<cfquery name="facstaff" datasource="#request.mygordon#">
		select websql.dbo.account.account_id as account_id, isnull(Pref_Pic_Exists,'') as Pref_Pic_Exists, isnull(show_pic,'') as Show_Pic, PrivacyLevel,
        facstaff.* from facstaff
		left outer join websql.dbo.account on facstaff.id = websql.dbo.account.gordon_id
		left outer join websql.dbo.account_profile on websql.dbo.account.account_id = websql.dbo.account_profile.account_id
		/* left outer join (SELECT EMP,EmlEffDt FROM HRDATA.NuGDN.dbo.Eml EmlRet WHERE EmlChgRsn in ('TVRetired','TVEmeritus')) g ON facstaff.id = g.emp   */     
		where row_id = '#url.fid#'
	</cfquery>
	
	<cfset gc_type = ucase(facstaff.type)>
	<cfset gordon_id = facstaff.id>
	<cfset alumni.recordcount = 0>
	<cfset student.recordcount = 0>
	<cfset the_barcode = facstaff.barcode>
	<cfset account_id = facstaff.account_id>
	<cfif  facstaff.Show_Pic eq "N"><cfset show_pic = false></cfif>
	<cfset pref_photo = facstaff.Pref_Pic_Exists>
<cfelseif IsDefined("url.sid")>
	<cfquery name="student" datasource="#request.mygordon#">
		select websql.dbo.account.account_id as account_id, isnull(Pref_Pic_Exists,'') as Pref_Pic_Exists, isnull(show_pic,'') as Show_Pic, student.* from student
		left outer join websql.dbo.account on student.id = websql.dbo.account.gordon_id
		left outer join websql.dbo.account_profile on websql.dbo.account.account_id = websql.dbo.account_profile.account_id
		where row_id = '#url.sid#'
	</cfquery>
	<cfset gc_type = "STUDENT">
	<cfset gordon_id = student.id>
	<cfset alumni.recordcount = 0>
	<cfset facstaff.recordcount = 0>
	<cfset the_barcode = student.barcode>
	<cfset account_id = student.account_id>
	<cfif  student.Show_Pic eq "N"><cfset show_pic = false></cfif>
	<cfset pref_photo = student.Pref_Pic_Exists>
<cfelse>
	<cfset gc_type = "">
	<cfset alumni.recordcount = 0>
	<cfset facstaff.recordcount = 0>
	<cfset student.recordcount = 0>
	<cfset the_barcode = "">
	<cfset account_id = "">
	<cfset pref_photo = "">
</cfif>

<!--- Determine the person type being displayed --->
<cfparam name="NotFound" default="false">
<cfif gc_type is "STUDENT" and student.recordcount gt 0>
	<cfset person_type = "Student">
	<cfparam name="SemiPrivate" default="false">	
	<cfparam name="FerpaProtected" default="false">
	<cfif student.KeepPrivate Contains "S">
		<cfset SemiPrivate = true>
	</cfif>
	<cfif student.KeepPrivate Contains "Y">
		<cfset FerpaProtected = true>
	</cfif>
	<cfif FerpaProtected and NOT PublicSafety>
		<cfset NotFound = true>
	</cfif>
	
	<cfstoredproc procedure="Select_Student_Cell_Phone_Privacy" datasource="#request.mygordon#">
	<cfprocparam type="IN" dbvarname="@id_num" cfsqltype="cf_sql_integer" value="#gordon_id#">
	<cfprocresult name="getcellphone" resultset="1">
	</cfstoredproc>
	
<cfelseif (gc_type is "FACULTY" or gc_type is "STAFF") and facstaff.recordcount gt 0>
	<cfset person_type = "Facstaff">
<cfelseif alumni.recordcount gt 0>
	<cfset person_type = "Alumni">
	<cfif alumni.HomeStreet2 Contains "deceased">
		<cfset NotFound = true>
	</cfif>
	<!--- Deny rights to see a person if they have not agreed to share their name... unless the user has id lookup rights --->
	<cfif Not AlumIDLookup And ucase(alumni.ShareName) neq "Y">
		<cfset NotFound = True>
	</cfif>
<cfelse>
	<cfset NotFound = true>
</cfif>

<h3>Who's Who on Campus</h3>
<br>
<cfif (student.recordcount is 0 AND facstaff.recordcount is 0 AND alumni.recordcount is 0) or NotFound>
	<table width="100%" cellpadding="0" cellspacing="0" border="0">
	<tr>
	<td align="center"><h3>No Matching Person Found</h3></td>
	</tr>
	</table>
	
<cfelse>

	<cfoutput>
   
	<!--- See what the person type is --->
	<cfif person_type is "Student">
		<!--- Person type is student --->
		<cfparam name="OnCampus" default=true>
		<cfif student.OnOffCampus neq "">
			<cfset OnCampus = false>
		</cfif>
		<table width="100%" cellspacing="0" cellpadding="0" border="0">
		<tr>
		<td align="center">
		<h3>
			<!--- <cfif Len(student.Title) gt 1>#student.Title#</cfif> --->
			<cfswitch expression="#student.Title#">
			<cfcase value="MR">Mr.</cfcase>
			<cfcase value="MS">Ms.</cfcase>
			<cfcase value="MIS">Ms.</cfcase>
			<cfcase value="MRS">Mrs.</cfcase>
			<cfdefaultcase>#student.Title#</cfdefaultcase>
			</cfswitch>
			<cfif Len(student.FirstName) gt 1>#student.FirstName#</cfif>
			<cfif Len(student.NickName) gt 1 AND student.NickName neq student.FirstName>(#student.NickName#)</cfif> 	
			<cfif Len(student.LastName) gt 1>#student.LastName#</cfif>
			<cfif Len(student.Suffix) gt 1>#student.Suffix#</cfif>
		</h3>
		</td>
		</tr>
		<tr>
		<td align="center">
		<h3>
		<cfswitch expression="#student.Class#">
		<cfcase value="1">Freshman</cfcase>
		<cfcase value="2">Sophomore</cfcase>
		<cfcase value="3">Junior</cfcase>
		<cfcase value="4">Senior</cfcase>
		<cfcase value="5">Graduate Student</cfcase>
		<cfcase value="6">Undergraduate Conferred</cfcase>
		<cfcase value="7">Graduate Conferred</cfcase>
		</cfswitch>
		</h3>
		</td>
		</tr>
		
		<cfif PublicSafety AND FerpaProtected>
		<tr>
		<td>
		<table border="0" cellpadding="2" cellspacing="0">
		<tr><td bordercolor="white">&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>
		<tr>
		<td class="alert" style="border: solid 2px ##FF0000">
		NOTICE: This student is protected under the Federal Educational Rights and Privacy
		Act (FERPA - 20 U.S.C. § 1232g) . Disclosure of this information to any unauthorized party
		under any circumstances without previous written consent from the student or his/her parents
		is a violation of federal law. For any further questions or information
		contact the Registrar's Office
		 at: (978) 867-4243.</td>    
		</tr>
		</table></td>
		</tr>
		<tr><td bordercolor="white">&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>
		</cfif> 
		
		<tr>
		<td align="center">
			<table width="530" cellpadding="0" cellspacing="0" border="0">
			<cfinclude template="/people/picture_inc.cfm">
	 		<tr>
			<cfset major_desc = "">
			<cfif len(student.Major) gt 0>
				<cftry>
					<cfquery name="major1" datasource="#request.db#">
					select major_minor_desc from TmsEPrd.dbo.major_minor_def
					where major_cde = '#student.Major#'
					</cfquery>
					<cfcatch><cfset major_desc = Trim(student.Major)></cfcatch>
				</cftry>
				<cfif major1.recordcount eq 1>
				    <cfset major_desc = Trim(major1.major_minor_desc)>
				</cfif>
				<cfif len(student.Major2) gt 0>
					<cftry>
						<cfquery name="major2" datasource="#request.db#">
						select major_minor_desc from TmsEPrd.dbo.major_minor_def
						where major_cde = '#student.Major2#'
						</cfquery>
						<cfcatch><cfset major_desc = major_desc & ', ' & Trim(student.Major2)></cfcatch>
					</cftry>
					<cfif major2.recordcount eq 1>
					    <cfset major_desc = major_desc & ', ' & Trim(major2.major_minor_desc)>
					</cfif>
					<cfif len(student.Major3) gt 0>
						<cftry>
							<cfquery name="major3" datasource="#request.db#">
							select major_minor_desc from TmsEPrd.dbo.major_minor_def
							where major_cde = '#student.Major3#'
							</cfquery>
							<cfcatch><cfset major_desc = major_desc & ', ' & Trim(student.Major3)></cfcatch>
						</cftry>
						<cfif major3.recordcount eq 1>
						    <cfset major_desc = major_desc & ', ' & Trim(major3.major_minor_desc)>
						</cfif>
					</cfif>
				</cfif>						
			</cfif>
					
			<cfif OnCampus>
			
				<cfset building_name = '#student.OnCampusBuilding#'>
				<!--- Show 'Unassigned Building, Room 100' as 'TBD' --->
				<cfif building_name eq 'Temp'>
				   <cfset building_name = 'TBD'>
				   <cfset student.OnCampusRoom = ''>
				<cfelse>
					<cftry>
						<cfquery name="building" datasource="#request.db#">
						select building_desc from TmsEPrd.dbo.building_master
						where bldg_cde = '#student.OnCampusBuilding#'
					</cfquery>
						<cfcatch><cfset building_name = '#student.OnCampusBuilding#'></cfcatch>
					</cftry>
					<cfif building.recordcount eq 1><cfset building_name = building.building_desc></cfif>
				</cfif>
				<td>
				<table width="260" border="0" cellpadding="1" cellspacing="0">
				<tr><td colspan="3"><b>Campus Address</b></td></tr>
				
				<!-- HLG - 7/1/2016 CTS-92457 - The student room assignments were visible on Go site even though they are set to be invisible from 6/1/ - 8/1 in SQL. -->
				<cfif #DateFormat(Now(), "m")#  neq "6" AND #DateFormat(Now(), "m")#  neq "7" >
					<tr><td>Hall:</td><td rowspan="99" width="10">&nbsp;</td><td width="170">#rtrim(building_name)#</td></tr>
					<tr><td>Room:</td><td>#student.OnCampusRoom#</td></tr>
				</cfif>
					
				<!--- DRA - 08/05/2013 Room phone extensions will no longer be available Fall 2013, so there's no point in displaying invalid data.
				<tr><td>Room Phone:</td><td>#student.OnCampusPhone#</td></tr> --->
				<!--- <tr><td>Major:</td><td>#student.Major#<cfif len(trim(#student.Major2#))>, #student.Major2#</cfif><cfif len(trim(#student.Major3#))>, #student.Major3#</cfif></td></tr> --->
				<tr><td>Major:</td><td>#major_desc#</td></tr>
				<tr><td>Email:</td>
				<cfif student.Email eq "">
					<td>No email available</td>
				<cfelse>
					<cfset email = "#lcase(student.Email)#">
					<td><a href="mailto:#email#">#email#</a></td>
				</cfif></tr>
				</table>
				</td>
			<cfelse>
				<td>
				<table width="260" border="0" cellpadding="1" cellspacing="0">
				<tr><td colspan="3"><b>Off-Campus Address</b></td></tr>
				<tr><td>Street:</td><td rowspan="99" width="10">&nbsp;</td>
				<cfif #student.OffCampusStreet1# eq "">
					<td>#student.OffCampusStreet2#</td>
				<cfelse>
					<td>#student.OffCampusStreet1#</td>
				</cfif></tr>
				<cfif #student.OffCampusStreet1# neq "" and #student.OffCampusStreet2# neq "">
				<tr><td></td><td>#student.OffCampusStreet2#</td></tr>
				</cfif>
				<tr><td>City:</td>
				<td>#student.OffCampusCity#<cfif #student.OffCampusCity# neq "">,</cfif>
				&nbsp;#student.OffCampusState#&nbsp;#student.OffCampusPostalCode#&nbsp;#student.OffCampusCountry#</td></tr>				
				<tr><td>Phone:</td><td>
				<cfif len(student.OffCampusPhone) eq 10 and isNumeric(student.OffCampusPhone)>
				    (#Left(student.OffCampusPhone, 3)#) #Mid(student.OffCampusPhone, 4,3)#-#Right(student.OffCampusPhone, 4)#
				<cfelse>
				    #student.OffCampusPhone#
				</cfif></td></tr>			
				<!--- <tr><td>Major:</td><td>#student.Major#<cfif len(trim(#student.Major2#))>, #student.Major2#</cfif><cfif len(trim(#student.Major3#))>, #student.Major3#</cfif></td></tr> --->
				<tr><td>Major:</td><td>#major_desc#</td></tr>
				<tr><td>Email:</td>
				<cfif student.Email eq "">
					<td>No email available</td>
				<cfelse>
					<cfset email = "#lcase(student.Email)#">
					<td><a href="mailto:#email#">#email#</a></td>
				</cfif></tr>
				</table>
				</td>
			</cfif>
			<td rowspan="99" width="10">
			<!--- Home Address table --->
			<td>
				<table width="260" border="0" cellpadding="1" cellspacing="0">
				<tr><td colspan="3"><b>Home Address</b></td></tr>
				<tr><td width="70">Street:</td><td rowspan="99" width="10">&nbsp;</td><td>
				<cfif PublicSafety or not(SemiPrivate)>		
					<cfif #student.HomeStreet1# eq "">
						#student.HomeStreet2#
					<cfelse>
						#student.HomeStreet1#
					</cfif></tr>
				<cfelse>
					Kept private as requested.
				</cfif></tr>
				<cfif PublicSafety or not(SemiPrivate)>					
					<cfif #student.HomeStreet1# neq "" and #student.HomeStreet2# neq "">
					<tr><td></td><td>#student.HomeStreet2#</td></tr>
					</cfif>
				</cfif>
				<tr><td>City:</td><td>
				<cfif PublicSafety or not(SemiPrivate)>
				    #student.HomeCity#<cfif #student.HomeCity# neq "">,</cfif>&nbsp;
				    #student.HomeState#&nbsp;#student.HomePostalCode#&nbsp;
					<cfif len(student.HomeCountry) gt 0 and student.HomeCountry neq 'US'>
						<cftry>
							<cfquery name="country" datasource="#request.db#">
							select table_desc from TmsEPrd.dbo.table_detail
							where column_name='country' and table_value = '#student.HomeCountry#'
							</cfquery>
							<cfcatch><cfset country_desc = '#student.HomeCountry#'></cfcatch>
						</cftry>
						<cfif country.recordcount eq 1><cfif country.table_desc neq 'United States'><cfset country_desc = country.table_desc></cfif></cfif>
						#country_desc#
					</cfif>
				</cfif></td></tr>
				<tr><td>Home Phone:</td><td>
				<cfif PublicSafety or not(SemiPrivate)>
					<cfif len(student.HomePhone) eq 10 and isNumeric(student.HomePhone)>
				    (#Left(student.HomePhone, 3)#) #Mid(student.HomePhone, 4,3)#-#Right(student.HomePhone, 4)#
				    <cfelse>
				    #student.HomePhone#
				    </cfif>
				</cfif></td></tr>
				<tr><td>Cell Phone:</td>
				<td>
				<cfif #student.MobilePhone# neq "">
					<cfif getcellphone.mobile_phone_priva eq "N" or mytype eq "FACULTY" or mytype eq "STAFF">
						<cfif len(student.MobilePhone) eq 10 and isNumeric(student.MobilePhone)>
					    (#Left(student.MobilePhone, 3)#) #Mid(student.MobilePhone, 4,3)#-#Right(student.MobilePhone, 4)#
						<cfelse>
						#rtrim(student.MobilePhone)#
						</cfif>
					<cfelse>
					Kept private as requested.
					</cfif>
				</cfif>
				</td></tr>
				</table>
			</td>
			</tr>
			<!--- </table> --->	 	
	</cfif>

	<cfif person_type is "Facstaff">
	<!--- Person type is faculty/staff --->
		<cfparam name="InfoPublic" default=true>
		<cfif facstaff.PrivacyLevel neq "">
			<cfset InfoPublic = false>
			<!--- <cfset show_pic = false> --->
		</cfif>
		<table width="100%" cellpadding="0" cellspacing="0" border="0">
		<tr>
		<td align="center">	
		<!--
		<h3>
			<cfif Len(facstaff.Title) gt 1>#facstaff.Title#   </cfif>
			<cfif Len(facstaff.FirstName) gt 1>#facstaff.FirstName# </cfif>
			<cfif Len(facstaff.NickName) gt 1 AND facstaff.NickName neq facstaff.FirstName>(#facstaff.NickName#) </cfif>
			<cfif Len(facstaff.MiddleName) gt 1 AND facstaff.MiddleName neq facstaff.NickName>(#facstaff.MiddleName#) </cfif>
			<cfif Len(facstaff.LastName) gt 1>#facstaff.LastName#</cfif>
			<cfif Len(facstaff.Suffix) gt 1>#facstaff.Suffix#</cfif>
		</h3>
		-->
		<h3>
			<!--- <cfif Len(facstaff.Title) gt 1>#facstaff.Title#</cfif> --->
			<cfswitch expression="#facstaff.Title#">
			<cfcase value="DR">Dr.</cfcase>
			<cfcase value="MR">Mr.</cfcase>
			<cfcase value="MS">Ms.</cfcase>
			<cfcase value="MIS">Ms.</cfcase>
			<cfcase value="MRS">Mrs.</cfcase>
			<cfdefaultcase>#facstaff.Title#</cfdefaultcase>
			</cfswitch>
			<cfif Len(facstaff.NickName) gt 1 AND facstaff.NickName neq facstaff.FirstName>#facstaff.NickName#<cfelse><cfif Len(facstaff.FirstName) gt 1>#facstaff.FirstName# </cfif></cfif> 
			<cfif Len(facstaff.MiddleName) gt 1 AND facstaff.MiddleName neq facstaff.NickName>#facstaff.MiddleName# </cfif>
			<cfif Len(facstaff.LastName) gt 1>#facstaff.LastName#</cfif>
			<cfif Len(facstaff.Suffix) gt 1>#facstaff.Suffix#</cfif>
		</h3>
		<cfif Len(facstaff.NickName) gt 1 AND facstaff.NickName neq facstaff.FirstName>
			<h5 style="font-style:italic;font-size:9pt;color:gray;">
			#facstaff.FirstName#<cfif Len(facstaff.MiddleName) gt 1> #facstaff.MiddleName#</cfif> <cfif Len(facstaff.LastName) gt 1>#facstaff.LastName#</cfif>
			</h5>
		</cfif>
		</td>
		</tr>
		<tr>
		<td align="center">
			<table width="440" cellspacing="0" cellpadding="0" border="0">
			<cfinclude template="/people/picture_inc.cfm">
			<tr>
		    <td colspan="2"><b>Campus Address</b></td>
			<td rowspan="6">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
			<td colspan="2"><b>Home Address</b></td>
			</tr>

			<cfif isdefined("facstaff.id")>
            	<tr>
				<td> Job Title:</td>
				<td>
            	<cfquery datasource="#request.mygordon#" name="getRetired">           
            	SELECT EmlRet.EMP,EmlEffDt FROM HRDATA.NuGDN.dbo.Eml EmlRet 
                   WHERE EmlChgRsn in 	('TVRetired','TVEmeritus') 
				   
				   	AND (Not EXISTS(SELECT EmlRet2.EMP,EmlRet2.EmlEffDt FROM HRDATA.NuGDN.dbo.Eml EmlRet2
					INNER JOIN HRDATA.NuGDN.dbo.Emp EmpRet on EmlRet2.Emp = EmpRet.Emp
					WHERE RetireDt IS NOT NULL and (EmlRet2.EmlEfdDt Is NULL and EmlRet2.EmpSts = 'Active') and EmlRet.Emp = EmlRet2.Emp))
				   
				   and EmlRet.emp =  #facstaff.id#
            	</cfquery>	
				<cfif getRetired.recordcount gt 0 and Len(getRetired.EmlEffDt) gt 0 >
            		Retired
            	<cfelse>
            		#facstaff.JobTitle#
             	</cfif> 
             	</td>
				<cfif InfoPublic>
					<td width=40>Street:</td>
					<td width=120>#facstaff.HomeStreet1#</td>
				<cfelse>
					<td width=160><nobr>Kept private as requested.</nobr></td>
				</cfif>
				</tr>
			</cfif>
            <tr>
			<td width=40>Department:&nbsp;&nbsp;&nbsp;</td>
			<td>#facstaff.OnCampusDepartment#</td>
			<td width=40></td>
			<td><cfif InfoPublic>#facstaff.HomeStreet2#</cfif></td>
			</tr>
			<tr>
			<td>Office:</td>
			<td>
			<cftry>
				<cfquery name="building" datasource="#request.db#">
					select rtrim(building_desc) as building_desc from TmsEPrd.dbo.building_master
					where rtrim(bldg_cde) = '#facstaff.OnCampusBuilding#'
				</cfquery>
				<cfcatch>
					<cfset building_name = '#facstaff.OnCampusBuilding#'>
				</cfcatch>
			</cftry>
			<cfif building.recordcount eq 1><cfset building_name = building.building_desc></cfif>
			#building_name#&nbsp;#facstaff.OnCampusRoom#</td>
			<cfif InfoPublic>
				<td width=40>City:</td>
				<td>#facstaff.HomeCity#,&nbsp;#facstaff.HomeState#&nbsp;#facstaff.HomePostalCode#&nbsp;
				<cfif len(facstaff.HomeCountry) gt 0 and facstaff.HomeCountry neq 'US'>
						<cftry>
							<cfquery name="country" datasource="#request.db#">
							select table_desc from TmsEPrd.dbo.table_detail
							where column_name='country' and table_value = '#facstaff.HomeCountry#'
							</cfquery>
							<cfcatch><cfset country_desc = '#facstaff.HomeCountry#'></cfcatch>
						</cftry>
						<cfif country.recordcount eq 1><cfif country.table_desc neq 'United States'><cfset country_desc = country.table_desc></cfif></cfif>
						#country_desc#
				</cfif>
				</td>
			</cfif>
			</tr>
			<tr>
			<td>Phone:</td>
			<td><cfif len(facstaff.OnCampusPhone) eq 10 and isNumeric(facstaff.OnCampusPhone)>
			(#Left(facstaff.OnCampusPhone, 3)#) #Mid(facstaff.OnCampusPhone, 4,3)#-#Right(facstaff.OnCampusPhone, 4)#
			<cfelse>
			#facstaff.OnCampusPhone#
			</cfif>
			</td>
			<cfif InfoPublic>
				<td width=70>Home Phone:</td>
				<td><cfif len(facstaff.HomePhone) eq 10 and isNumeric(facstaff.HomePhone)>
				(#Left(facstaff.HomePhone, 3)#) #Mid(facstaff.HomePhone, 4,3)#-#Right(facstaff.HomePhone, 4)#
				<cfelse>
				#facstaff.HomePhone#
				</cfif>
				</td>
			</cfif>
			</tr>
			<tr>
			<td>Email:</td>
			<cfif facstaff.Email eq "">
				<td>No email available</td>
			<cfelse>
				<cfset email = "#LCase(facstaff.Email)#">
				<td><a href="mailto:#email#">#email#</a></td>
			</cfif>
			<cfif InfoPublic>
				<td width="40"><cfif len(#facstaff.SpouseName#) gt 0>Spouse:</cfif>&nbsp;</td>
				<td>#facstaff.SpouseName#</td>
			</cfif>
			<cfif officehrs neq "">
				<tr>
				<td>Office hrs:</td>
				<td colspan="4">#officehrs#</td>
				</tr>
			</cfif>		
	</cfif>

	<cfif person_type is "Alumni">
	<!--- Person type is alumni --->
		<cfif trim(alumni.PreferredClassYear neq "")>
			<cfset gradyear = "Class of " & left(alumni.PreferredClassYear, 4)>
		<cfelseif trim(alumni.ClassYear neq "")>
			<cfset gradyear = "Class of " & alumni.ClassYear>
		<cfelse>
			<cfset gradyear = "Grad Year Unavailable">	
		</cfif>
		
		<table width="100%" cellspacing="0" cellpadding="0" border="0">
		<tr>
		<td align="center">
		<h3>
			<cfif Len(alumni.Title) gt 1>#alumni.Title#</cfif>
			<cfif Len(alumni.FirstName) gt 1>#alumni.FirstName#</cfif>
			<cfif Len(alumni.NickName) gt 1 AND alumni.NickName neq alumni.FirstName>(#alumni.NickName#)</cfif> 	
			<cfif Len(alumni.LastName) gt 1>#alumni.LastName#</cfif>
			<cfif Len(alumni.Suffix) gt 1>#alumni.Suffix#</cfif>
		</h3>
		</td>
		</tr>
		<tr>
		<td align="center">
		<h3>
		#gradyear#
		</h3>
		</td>
		</tr>
		<tr>
		<td>&nbsp;</td>
		</tr>
		<tr>
		<td align="center">
			<table width="400" cellpadding="0" cellspacing="0" border="0">
			<cfinclude template="/people/picture_inc.cfm">
			<tr>
			<td colspan="2"><b>General Information</b></td>
			<td rowspan="5">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
			<td colspan="2"><b>Home Address</b></td>
			</tr>
			<tr>
			<td>Class:</td>
			<td>#gradyear#</td>
			<td width=40>Street:</td>
		<!--- See if alumnus wants address shared --->
		<cfif ucase(alumni.ShareAddress) is not "Y">
			<!--- Don't share address --->
			<td>N/A	</td>
		<cfelse>
			<!--- Share address --->
			<cfparam name="city" default="">
			<cfif FindNoCase("Lost Addr", alumni.HomeStreet2) is not 0>
				<cfset street = "N/A">
				<cfset city = "N/A">
			<cfelse>
				<cfif len(alumni.HomeStreet2) gt 0><cfset street = alumni.HomeStreet2></cfif>
				<cfif (len(alumni.HomeStreet2)) gt 0 and (len(alumni.HomeStreet1) gt 0)><cfset street = street & "<br>"></cfif>
				<cfif len(alumni.HomeStreet1) gt 0><cfset street= street & alumni.HomeStreet1></cfif>
			</cfif>
			<td>#street#</td>
		</cfif>
			</tr>	
			<tr>
			<td>Major:</td>
			<td>#alumni.Major1#
		<!-- If there's a second major, add it on a second line within the same cell --->
		<cfif len(trim(#alumni.Major2#)) gt 0><br>#alumni.Major2#</cfif>
			</td>
			<td>City:</td>
		<!--- See if alumnus wants address shared --->
		<cfif ucase(alumni.ShareAddress) is not "Y">
			<td>N/A	</td>
		<cfelse>
			<!--- Share city state and zip if they are present --->
			<cfif city is not "N/A">
				<cfset city=alumni.HomeCity>
				<cfif len(alumni.HomeState) gt 0>
					<cfset city=city & ", " & alumni.HomeState>
				</cfif> 
				<cfset city = city & " " & alumni.HomePostalCode & " "> 
				<cfif len(alumni.HomeCountry) gt 0 and alumni.HomeCountry neq 'US'>
						<cftry>
							<cfquery name="country" datasource="#request.db#">
							select table_desc from TmsEPrd.dbo.table_detail
							where column_name='country' and table_value = '#alumni.HomeCountry#'
							</cfquery>
							<cfcatch><cfset country_desc = '#alumni.HomeCountry#'></cfcatch>
						</cftry>
						<cfif country.recordcount eq 1><cfif country.table_desc neq 'United States'><cfset country_desc = country.table_desc></cfif></cfif>
						<cfset city = city & country_desc>
				</cfif>
			</cfif>
			<td>#city#</td>
		</cfif>
			</tr>	
			<tr>
			<td>Email:</td>
			<cfparam name="email1" default="">
			<cfparam name="email2" default="">
			<!--- set the person's gordon email (if present) to email1 --->
			<cfif alumni.Email neq "">
				<cfset email1 = "#LCase(alumni.Email)#">
			</cfif>
			<!--- Set the person's home email (if present) to email2 --->
			<cfif alumni.HomeEmail neq "">
				<cfset email2 = LCase(alumni.HomeEmail)>
			</cfif>
			<!-- if email2 matches email1, set email2 to an empty string --->
			<cfif email1 eq email2>
				<cfset email2 = "">
			</cfif>
			<!-- Set up email links based on which email addresses are present -->
			<cfif ucase(alumni.ShareAddress) is not "Y">
				<td>N/A	</td>
			<cfelseif email1 neq "" and email2 neq "">
				<!--- both addresses are present --->
				<td><a href="mailto:#email1#">#email1#</a><br>
				<a href="mailto:#email2#">#email2#</a></td>
			<cfelseif email1 neq "" and email2 eq "">
				<!--- only Gordon email is present --->
				<td><a href="mailto:#email1#">#email1#</a></td>
			<cfelseif email1 eq "" and email2 neq "">
				<!--- only home email is present --->
				<td><a href="mailto:#email2#">#email2#</a></td>
			<cfelse>
				<td>No email available</td>
			</cfif>		
			<td>Phone:</td>
		<cfif ucase(alumni.ShareAddress) is not "Y">
			<td>N/A	</td>
		<cfelse>
			<td>#alumni.HomePhone#</td>
		</cfif>
			</tr>
			<!--- Show maiden name if it exists --->
			<tr>
			<td nowrap><cfif len(alumni.MaidenName) gt 0>Maiden Name:&nbsp;</cfif></td>
			<td><cfif len(alumni.MaidenName) gt 0>#alumni.MaidenName#</cfif></td>
			<!---<cfif AlumIDLookup>
			<td>ID:</td>
			<td>#gordon_id#</td>
			<cfelse>
			<td colspan="2">&nbsp;</td>
			</cfif> --->
			</tr>
	</cfif>
	
	<cfif viewer_type eq "Student">
			<tr>
			<td valign="top" align="left" colspan="5">
			<br>
			<font color="##FF0000">
			<strong><br>NOTE: </strong><br>
			<big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>To prevent your picture or your cell phone number from displaying, click <a href="https://go.gordon.edu/general/myaccount.cfm">here</a>.<br>
			<big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>To update your On Campus Address, contact <a href="mailto:housing@gordon.edu">Housing</a> (x4263).<br>
			<big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>For all other changes or to partially/fully prevent your data from displaying,<br>
	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;contact the <a href="mailto:registrar@gordon.edu">Registrar's Office</a> (x4242).<br>
			</font>
			</td>
			</tr>
			</table>
		</td>
		</tr>
		</table>
	</cfif>
	
	<cfif viewer_type eq "Facstaff">
			<tr valign="top">
			<td valign="top" align="left" colspan="5">
			<cfif not PublicSafety>
			<br>
			<font color="##FF0000">
			<strong><br>NOTE: </strong><br>
			<big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>To update your office location or hours, click <a href="https://go.gordon.edu/general/myaccount.cfm">here</a>.<br>
			<big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>To prevent your picture or home address from displaying click <a href="https://go.gordon.edu/general/myaccount.cfm">here</a>.<br>
			<!--- <cfif mytype is "FACULTY"><big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>To update office hours click <a href="https://go.gordon.edu/general/myaccount.cfm">here</a>.<br></cfif> --->
			<big>&nbsp;&nbsp;&nbsp;&bull;&nbsp;</big>To update your data contact <a href="mailto:hr@gordon.edu">Human Resources</a> (x4828).<br>
			</font>
			<cfelse>
			
			<cfstoredproc procedure="Select_Emergency_Contacts" datasource="#request.mygordon#">
			<cfprocparam type="IN" dbvarname="@id_num" cfsqltype="cf_sql_integer" value="#gordon_id#">
			<cfprocresult name="getcontact" resultset="1">
			</cfstoredproc>
			<br>
			<div style="color:red"><b>Emergency Contacts</b></div>
			<table cellpadding="0" cellspacing="0" border="0">
			<tr><th align="left">Contact&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp</th><th align="left">Relationship&nbsp;&nbsp;&nbsp&nbsp;&nbsp</th><th align="left">Home Phone&nbsp;&nbsp;&nbsp&nbsp;&nbsp</th><th align="left">Cell Phone&nbsp;&nbsp;&nbsp&nbsp;&nbsp</th><th align="left">Work Phone&nbsp;&nbsp;&nbsp&nbsp;&nbsp	</th></tr>
			<cfloop query="getcontact">
			<tr><td>#Contact_First_Name# #Contact_Last_Name#</td><td>#Relationship#</td>
			<td><cfif len(Home_Phone) eq 10 and isNumeric(Home_Phone)>(#Left(Home_Phone, 3)#) #Mid(Home_Phone, 4,3)#-#Right(Home_Phone, 4)#<cfelse>#Home_Phone#</cfif></td>
			<td><cfif len(Cell_Phone) eq 10 and isNumeric(Cell_Phone)>(#Left(Cell_Phone, 3)#) #Mid(Cell_Phone, 4,3)#-#Right(Cell_Phone, 4)#<cfelse>#Cell_Phone#</cfif></td>
			<td><cfif len(Work_Phone) eq 10 and isNumeric(Work_Phone)>(#Left(Work_Phone, 3)#) #Mid(Work_Phone, 4,3)#-#Right(Work_Phone, 4)#<cfelse>#Work_Phone#</cfif></td>
			</cfloop>
			</table>
			</cfif>
			</td>
			</tr>
			</table>
		</td>
		</tr>
		</table>
	</cfif>
		
	</cfoutput>	
</cfif>
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
<!-- InstanceEnd -->
</html>
