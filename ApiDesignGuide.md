
# Gordon 360: API Design Guide

<div style="background-color:#eee;padding:10px 20px">
<strong>This is currently an incomplete guide</strong>
<br>Created Summer 2020. Note that I (Cameron) hope to continue working on this design and documentation as I hopefully continue to work on 360 in the future.</div>

<br>

<div style="background-color:#eee;padding:10px 20px"><strong>Good people to contact about this subject</strong> are Cameron Abbot (author), Matt Felgate (Entity Framework expert), Eze Anyanwu (original creator of API), Jay Whitehouse (knowledge of Stored Procedures), and Dr. Tuck (good neutral party in the EF vs Stored Procedures debate)</div>

## Overview
There are two major approaches to developing this API. The first is to use Entity Framework according to the original standards of .NET 4.x, while the second is to use a hybrid of this framework while using Stored Procedures to access the database.

## Detailed Explanations
(currently just notes)

#### STORED PROCEDURES:

Good examples to look at in current API: Pretty much anywhere that calls a RawSql query in the file, also the news endpoints that aren't Delete or Edit as of now.

#### ENTITY FRAMEWORK ".NET 4.X":
This is a video suggested by Matt Felgate on how EF (Entity Framework) works: https://youtu.be/oycRolZUiLs

Good examples to look at in current API: 'session' API service created by Eze and also the Delete/Edit news endpoints.

## Pros/Cons Comparison

<table>
<tr>
    <th>STORED PROCEDURES</th>
    <th>ASP.NET 4.X WEB API</th>
<tr>
<tr>
    <td>Slight performance advantage (precompiled) - may be 3x faster</td>
    <td>How the API was intended</td>
</tr>
<tr>
    <td>Easy to test SQL procedures</td>
    <td>Simple development as project grows with simple CRUD operations, more manageable growth</td>
</tr>
<tr>
    <td>Very easy to understand, learn, and fix</td>
    <td>Consistent error checking</td>
</tr>
<tr>
    <td>Consistent in that all our queries can be in one place</td>
    <td>Repo is way easier once set up</td>
</tr>
<tr>
    <td>Allows centralized permissions control (ex. prevent developers from accessing timesheets sql)</td>
    <td>Can use views to allow centralized permissions control</td>
</tr>
<tr>
    <td>Complex queries might be easier</td>
    <td></td>
</tr>
</table>

See <a href="https://docs.google.com/document/d/1Rp_Aru5QGDH6SZ9k3MG86y7d7vXWeJXXwQSGMcN_7vk/edit?usp=sharing">this document</a> for the rest of my thoughts thus far on comparing design philosophy pros and cons. This also includes a look at .NET Core which is the latest version of .NET (we are using .NET 4.x).


## Workflow Comparison
See <a href="https://docs.google.com/document/d/1FYO9KAYmXBzHP7rfmyD-ZQ2EmW8urS_Qrotki9DmRw8/edit?usp=sharing">this document</a> for an incomplete walk-through of using each design philosophy