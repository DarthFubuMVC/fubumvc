(function () {
    var jasmineEnv = jasmine.getEnv();
    jasmineEnv.updateInterval = 1000;

    var trivialReporter = new jasmine.TrivialReporter();
    var teamcityReporter = new jasmine.TeamcityReporter();
  
    jasmineEnv.addReporter(trivialReporter);
    jasmineEnv.addReporter(teamcityReporter);
    jasmineEnv.specFilter = function (spec) {
        return trivialReporter.specFilter(spec);
    };

    var currentWindowOnload = window.onload;

    window.onload = function () {
        if (currentWindowOnload) {
            currentWindowOnload();
        }
        execJasmine();
    };

    function execJasmine() {
        jasmineEnv.execute();
    }

})();