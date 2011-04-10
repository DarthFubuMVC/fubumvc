$(document).ready(function () {
    $('#nodes').treeview({
        collapsed: true,
		animated: 'fast',
        prerendered: true
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