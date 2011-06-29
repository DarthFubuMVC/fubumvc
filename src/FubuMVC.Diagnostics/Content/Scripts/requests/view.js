$(document).ready(function () {

    var resetDottedLines = function() {
        if($(this).find('.children:visible').size() != 0) {
            $(this).addClass('with-children');
        }
        else {
            $(this).removeClass('with-children');
        }
    };

    $('#nodes').treeview({
        collapsed: true,
		animated: 'fast',
        prerendered: true,
        toggle: function() {
            resetDottedLines.call(this);
        }
    });

    $('#nodes > li').each(function() {
        resetDottedLines.call(this);
    });

    $('.console > .message > ul').each(function() {
        $(this).find('li.binding-detail:last').each(function() {
            $(this).addClass('last');
        });
    });

    $('.output').each(function() {
        var metadata = $(this).metadata();
        if(metadata.type && metadata.type == 'application/json') {
            var self = $(this);
            var json = eval('(' + self.find('code').html() + ')');
            self.html('');
            self.append(prettyPrint(json));
        }
    });
});