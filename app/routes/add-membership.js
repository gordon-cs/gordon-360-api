import Ember from 'ember';

export default Ember.Route.extend({
    model(param) {
        var response = {};
        Ember.$.ajax({
            dataType: "json",
            url: 'https://ccttrain.gordon.edu/api/activities/' + param.activity_code,
            async: false,
            success: function(data) {
                response.activity = data;
            }
        });
        Ember.$.ajax({
            dataType: "json",
            url: 'https://ccttrain.gordon.edu/api/sessions',
            async: false,
            success: function(data) {
                response.sessions = data;
            }
        });
        Ember.$.ajax({
            dataType: "json",
            url: 'https://ccttrain.gordon.edu/api/participations',
            async: false,
            success: function(data) {
                response.roles = data;
            }
        });
        return response;
    }
});
