$(document).ready(function() {
    var arrow = $('#arrow').html();
    $('#nodes > li:not(:last)').each(function() {
        $(this).append('<li class="arrow">' + arrow  + '</li>');
    });
});