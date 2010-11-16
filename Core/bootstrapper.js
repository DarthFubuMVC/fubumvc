$(document).ready(function() {
    $('#testEditor').testEditor(test);

    ST.pushChanges = function(json) {
        window.external.CaptureChanges(json);
    }
});
