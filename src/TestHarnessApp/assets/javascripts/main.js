(function() {
  'use strict';

  window.name = 'NG_DEFER_BOOTSTRAP!';

  require({
    urlArgs: "b=" + ((new Date()).getTime()),
    paths: {
      jquery: 'vendor/jquery/jquery',
      angular: 'vendor/angular/angular',
      bootstrap: 'vendor/bootstrap/bootstrap'
    },
    shim: {
      'angular' : {
        deps: ['jquery'],
        exports: 'angular'
      },
      'bootstrap': {
        deps: ['jquery']
      }
    },
    deps: ['angular']
  }, ['jquery','angular', 'app/app'], function($, angular, app) {
    $(document).ready(function () {
      angular.resumeBootstrap();
    });
  });

}).call(this);
