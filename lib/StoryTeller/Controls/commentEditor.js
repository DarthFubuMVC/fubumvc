ST.commentEditor = function (div, metadata, comment) {
    div.comment = comment;

    div.textbox = $('.editor', div);
    div.editorHolder = $('.comment-editor-holder', div);
    div.previewHolder = $('.comment-text-holder', div);
    div.preview = $('.comment-text', div);

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
        return div.comment;
    }

    $('.comment-editor', div).click(function () {
        div.edit();
    });

    $('.comment-closer', div).click(function () {
        div.showPreview();
    });

    div.textbox.change(function () {
        comment.text(div.textbox.val());
        div.preview.html(div.textbox.val());
        $(div).markDirty();
    });

    var shouldEdit = false;
    if (!comment.text()) {
        shouldEdit = true;
    }

    div.textbox.val(comment.text());
    div.preview.html(comment.text());

    if (shouldEdit) {
        div.edit();
        div.textbox.focus();
        div.textbox.select();
    }
    else {
        div.showPreview();
    }


}

ST.registerGrammar('.Comment', ST.commentEditor);

$.fn.commentEditor = function(comment) {
    return this.each(function(i, div) {
        ST.commentEditor(div, {}, comment);
    });
}
