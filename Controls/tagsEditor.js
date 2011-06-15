ST.tagsEditor = function (div, metadata, TestTags) {
    div.TestTags = TestTags;

    div.textbox = $('.editor', div);
    div.editorHolder = $('.tags-editor-holder', div);
    div.previewHolder = $('.tags-text-holder', div);
    div.preview = $('.tags-text', div);

    $('.deleteStep', div).removable();

    div.showPreview = function () {
        div.editorHolder.hide();
        div.previewHolder.show();
    }

    $(div.textbox).blur(div.showPreview);

    div.edit = function () {
        div.editorHolder.show();
        div.previewHolder.hide();
    }

    div.update = function () {
        div.textbox.change();
        return div.TestTags;
    }

    $('.tags-editor', div).click(function () {
        div.edit();
    });

    $('.tags-closer', div).click(function () {
        div.showPreview();
    });

    div.textbox.change(function () {
        TestTags.text(div.textbox.val());
        div.preview.html(div.textbox.val());
        $(div).markDirty();
    });

    var shouldEdit = false;
    if (!TestTags.text()) {
        shouldEdit = true;
    }

    div.textbox.val(TestTags.text());
    div.preview.html(TestTags.text());

    if (shouldEdit) {
        div.edit();
        div.textbox.focus();
        div.textbox.select();
    }
    else {
        div.showPreview();
    }


}

ST.registerGrammar('.TestTags', ST.tagsEditor);

$.fn.tagsEditor = function(TestTags) {
    return this.each(function(i, div) {
        ST.tagsEditor(div, {}, TestTags);
    });
}
