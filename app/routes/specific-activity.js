import Ember from 'ember';

export default Ember.Route.extend({
    model(param) {
        return Ember.$.getJSON('https://ccttrain.gordon.edu/api/activities/' + param.ACT_CDE);
    }
});
