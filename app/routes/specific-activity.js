import Ember from 'ember';

export default Ember.Route.extend({
    model(param) {
        var mdl = {"following": false};
        Ember.$.ajax({
            type: "GET",
            url: 'https://ccttrain.gordon.edu/api/activities/' + param.activity_code,
            async: false,
            success: function(data) {
                mdl.activity = data;
            },
            error: function(errorThrown) {
                console.log(JSON.stringify(errorThrown));
            }
        });
        Ember.$.ajax({
            type: "GET",
            url: 'https://ccttrain.gordon.edu/api/students/50154997/memberships',
            async: false,
            success: function(data) {
                for (var i = 0; i < data.length; i ++) {
                    if (data[i].ActivityCode === param.activity_code && data[i].Participation === "GUEST") {
                        mdl.following = true;
                        mdl.membershipID = data[i].MembershipID;
                    }
                }
            },
            error: function(errorThrown) {
                console.log(JSON.stringify(errorThrown));
            }
        });
        console.log(JSON.stringify(mdl));
        return mdl;
    }
});
