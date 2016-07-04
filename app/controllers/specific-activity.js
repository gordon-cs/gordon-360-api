import Ember from 'ember';

export default Ember.Controller.extend({
    actions: {
        toggleFollow() {
            var activityCode = this.get('model').activity.ActivityCode.trim();
            var membershipID = this.get('model').membershipID;
            if (this.get('model').following) {
                Ember.$.ajax({
                    type: "DELETE",
                    url: "https://ccttrain.gordon.edu/api/memberships/" + membershipID,
                    contentType: "application/json",
                    success: function(data) {
                        console.log(JSON.stringify(data));
                    },
                    error: function(errorThrown) {
                        console.log(JSON.stringify(errorThrown));
                    }
                });
            }
            else {
                var membership = {
                    "ACT_CDE": activityCode,
                    "SESSION_CDE": "201501",
                    "ID_NUM": "50154997",
                    "PART_LVL": "GUEST",
                    "BEGIN_DTE": "1/1/2016",
                    "END_DTE": "2/2/2016",
                    "DESCRIPTION": "Basic Follower",
                    "USER_NAME": null,
                    "JOB_NAME": null,
                    "JOB_TIME": null
                };
                var newMembershipID = null;
                Ember.$.ajax({
                    type: "POST",
                    url: "https://ccttrain.gordon.edu/api/memberships",
                    data: JSON.stringify(membership),
                    contentType: "application/json",
                    async: false,
                    success: function(data) {
                        newMembershipID = data.MembershipID;
                    },
                    error: function(errorThrown) {
                        alert(JSON.stringify(errorThrown));
                    }
                });
                this.set('model.membershipID', newMembershipID);
            }
            this.set('model.following', !this.get('model').following);
        }
    }
});
