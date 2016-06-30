import Ember from 'ember';
import config from './config/environment';

const Router = Ember.Router.extend({
  location: config.locationType
});

Router.map(function() {
  this.route('my-activities');
  this.route('specific-activity', { path: '/specific-activity/:ACT_CDE' });
  this.route('add-membership', { path: '/add-membership/:ACT_CDE' });
});

export default Router;
