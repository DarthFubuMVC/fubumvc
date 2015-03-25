$(document).ready(function () {
    $('#all-specs-node').treeview();
    $('#jasmine-reporter').hide();
    jasmine.getGlobal().console.log = function (message) {
        $('#jasmine-reporter').append($('<li class="jasmine-reporter-item">' + message + '</li>'));
    };
});