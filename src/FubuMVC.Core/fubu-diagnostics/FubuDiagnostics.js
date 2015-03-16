var FubuDiagnostics = {
    

    cache: {},

    get: function (key, params, callback) {
        var url = this.toUrl(key, params);

        $.get(url, callback);
    },

    toUrl: function (key, params) {
        var route = this.routes[key];
        var url = route.url;
        _.each(route.params, function (param) {
            url = url.replace('{' + param + '}', params[param]);
        });

        return url;
    },

    // TODO -- add cached ability

}