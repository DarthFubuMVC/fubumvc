$(document).ready(function () {
    var url = $('#url').val();
    var count = 0;

    var source = new EventSource(url);
    source.addEventListener('message', function (e) {
        $('#last-message').html(e.data);

        count++;

        var text = count + '.) ' + e.data;

        if (e.lastEventId) {
            text += ' / ' + e.lastEventId;
        }

        $('<ol></ol>').html(text).appendTo('#all-messages');
    });
});