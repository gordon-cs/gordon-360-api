import Ember from 'ember';

export default Ember.Route.extend({
    model() {
        return Ember.$.getJSON('https://ccttrain.gordon.edu/api/activities');
    }
});
