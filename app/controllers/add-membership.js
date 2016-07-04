import Ember from 'ember';

export default Ember.Controller.extend({
    actions: {

        //selectSession: function(session) {

        //},
        //selectRole: function(role) {

        //},
        post: function() {
            //var a = this.get('student-id');
            //var b = this.get('student-id');
            //var c = this.get('student-id');
            //var d = this.get('student-id');
            alert("hello");
        }
        /*post: function(activityCode, studentId, session, role, beginDate, endDate, comments) {
            var data = {
                //"MEMBERSHIP_ID"
                "ACT_CDE": activityCode,
                "SESSION_CDE": session,
                "ID_NUM": studentId,
                "PART_LVL": role,
                "BEGIN_DTE": beginDate,
                "END_DTE": endDate,
                "DESCRIPTION": comments
                //"USER_NAME"
                //"JOB_NAME"
                //"JOB_TIME"
            };
            Ember.$.ajax({
                type: "POST",
                url: 'https://ccttrain.gordon.edu/api/activities/' + param.ACT_CDE,
                data, data,
                success: function(data) {
                    alert("success");
                },
                dataType: "json"
            });
        }*/
    }
});
