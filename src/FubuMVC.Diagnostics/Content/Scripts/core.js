$(document).ready(function () {
    var toggleHover = function () {
        $(this).toggleClass('ui-state-hover');
    };
    $('.ui-button').hover(toggleHover, toggleHover);
});