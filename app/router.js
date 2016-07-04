import Ember from 'ember';
import config from './config/environment';

const Router = Ember.Router.extend({
  location: config.locationType
});

Router.map(function() {
  this.route('my-activities');
  this.route('specific-activity', { path: '/specific-activity/:activity_code' });
  this.route('add-membership', { path: '/add-membership/:activity_code' });
});

export default Router;
