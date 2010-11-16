ST.testEditor = function (editor, test) {
    editor.isLatched = true;
    editor.inner = $('.container', editor).get(0);
    editor.test = test;

    //$(editor.inner).clear();

    editor.markDirty = function () {
        if (editor.isLatched) return true;

        editor.isLatched = true;

        editor.inner.update();
        var json = JSON.stringify(editor.test);
        ST.pushChanges(json);

        editor.isLatched = false;
    }

    $('.testName', editor).html(test.Name);

    var metadata = $(editor.inner).metadata();

    $(editor.inner).sectionEditor(metadata, test);
    editor.getStepNames = function () {
        return editor.inner.getStepNames();
    }

    editor.isLatched = false;

    return editor;
}

function applyChanges() {
    $('#testEditor').markDirty();
}

ST.pushChanges = function(json) {
    // nothing
}

$.fn.testEditor = function(test) {
    return this.each(function(i, div) {
        ST.testEditor(div, test);
    });
}


