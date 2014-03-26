$(document).ready(function () {
    $('#chain-summary').click(function () {
        $('#routeModalBody').load($('#chainUrl').val());
        $('#routeModalBody .accordion').collapse();
        $('#routeModal').modal('show');
    });
});