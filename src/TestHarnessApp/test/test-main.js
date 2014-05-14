var tests = [];
for (var file in window.__karma__.files) {
  if (window.__karma__.files.hasOwnProperty(file)) {
    if (/Spec\.js$/.test(file)) {
      tests.push(file);
    }
  }
}

require({
  baseUrl: '/base/public/javascripts',
  paths: {
    jquery: 'vendor/jquery/jquery',
    angular: 'vendor/angular/angular',
    angularMocks: 'vendor/angular-mocks/angular-mocks'
  },
  shim: {
    'angular': {
      deps:['jquery'],
      'exports': 'angular'
    },
    'angularMocks': {
      deps:["angular"],
      exports: 'angular.mock'
    }
  },
  // ask Require.js to load these files (all our tests)
  deps: tests,

  // start test run, once Require.js is done
  callback: window.__karma__.start
});


