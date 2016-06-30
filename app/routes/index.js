import Ember from 'ember';

export default Ember.Route.extend({
    model() {
        //return Ember.$.getJSON('https://ccttrain.gordon.edu/api/activities');
    },
    activate() {
        Ember.$.ajax({
            type: "POST",
            dataType: "application/x-www-form-urlencoded",
            url: 'https://ccttrain.gordon.edu/Home/Login',
            data: "username=James.Kempf, password=August24",
            success: function() {
                alert("success");
            },
            error: function(errorThrown) {
                alert(JSON.stringify(errorThrown));
            }
        });
    }
});
