
ST.tableEditor = function (div, metadata, step) {
    div.step = step;
    div.columns = new TableColumns($('table.templates', div).get(0));
    div.selector = new ColumnSelector(div);
    div.thead = $('table.editor > thead', div);
    div.tbody = $('table.editor > tbody', div);
    div.leaf = step.childFor(metadata.leafName);



    $('.deleteStep').removable();

    div.addColumn = function (column) {
        div.update();

        div.columns.addColumn(column);
        div.selector.hideColumn(column);

        div.rebuildTable();

        $(div).markDirty();
    }

    div.removeColumn = function (column) {
        div.update();

        $(div.leaf.children).each(function (i, step) {
            step.remove(column);
        });

        div.columns.removeColumn(column);
        div.selector.showColumn(column);

        div.rebuildTable();

        $(div).markDirty();
    }

    div.rebuildTable = function () {
        div.thead.empty();
        var header = div.columns.headerRow();
        div.thead.append(header);

        div.tbody.empty();
        $(div.leaf.children).each(function (i, step) {
            div.add(step);
        });

        var columnCount = header.children.length;
        $('tfoot > tr > td', div).attr('colspan', columnCount);

        if (this.columns.hasInactiveColumns()) {
            div.selector.show();
        }
        else {
            div.selector.hide();
        }
    }

    div.update = function () {
        div.leaf.children = [];

        $('tr', div.tbody).each(function (i, tr) {
            var step = tr.update();
            div.leaf.children.push(step);
        });

        return div.step;
    }

    div.add = function (step) {
        if (step == null || !step.isStep) {
            step = new Step('row');
        }

        var row = div.columns.bodyRow(step);
        div.tbody.append(row);

        $('.cloner', div).show();

        $(div).markDirty();

        return false;
    }

    div.cloneLast = function () {
        var lastRow = $('tr:last', div.tbody).get(0);
        var newStep = lastRow.step.simpleClone();
        div.add(newStep);
        $('.cloner', div).show();

        return false;
    }

    $(div)
        .bind("keydown", "ctrl+1", div.add)
        .bind("keydown", "ctrl+2", div.cloneLast);


    div.rowRemoved = function () {
        if (div.tbody.children().length == 0) {
            $('.cloner', div).hide();
        }

        $(div).markDirty();
    }

    $('.adder', div).click(div.add);
    $('.cloner', div).click(div.cloneLast);

    div.columns.chooseColumns(div.leaf);   // decide which columns should be visible
    div.selector.showColumns(div.columns); // set the initial state of the columns
    div.rebuildTable();                    // rebuild the initial state of the row/columns
    div.rowRemoved();

    // If this is a new table, add at least one row
    if (div.leaf.children.length == 0) {
        div.add();
    }

    return div;
}

ST.registerGrammar('.table-editor', ST.tableEditor);



function ColumnSelector(div){
    var self = this;
    self.container = $('.column-selector', div);

    $('a.column-adder', div).click(function(){
        var column = $(this).metadata().key;
        $(this).closest('.table-editor').get(0).addColumn(column);
        
        return false;
    });

    self.showColumns = function (tableColumns) {
        $('a.column-adder', div).hide();
        tableColumns.columns.forHiddenCells(function (column) {
            self.showColumn(column);
        });
    }
    
    self.findColumn = function(column){
        var search = 'a.add' + column;
        return $(search, div);
    }
    
    self.hideColumn = function(column){
        self.findColumn(column).hide();
    }
    
    self.showColumn = function(column){
        self.findColumn(column).show();
    }

    self.hide = function () {
        self.container.hide();
    }

    self.show = function () {
        self.container.show();
    }

    
    return self;
}



function ColumnList(){
    var self = this;
    
    self.readStep = function(i, step){
        $(step.propertyNames()).each(function(ind, prop){
            self[prop] = true;
        });
    }
    
    self.forCells = function(callback){
        for (prop in self){
            var value = self[prop];
            if (value == true){
                callback(prop);
            }
        }
    }
    
    self.forHiddenCells = function(callback){
        for (prop in self){
            var value = self[prop];
            if (value == false){
                callback(prop);
            }
        }
    }

    self.hasInactiveColumns = function () {
        for (prop in self) {
            if (self[prop] == false) {
                return true;
            }
        }

        return false;
    }

    return self;
}

$.fn.tableColumns = function(){
    return this.each(function(i, table){
        TableColumns(table);
    });
}

function TableColumns(table) {
    table.columns = new ColumnList();

    table.chooseColumns = function(leaf){
        // first pass, loop through columns
        $('th', table).each(function(i, header){
            var metadata = $(header).metadata();  
            table.columns[metadata.key] = metadata.mandatory;
        });
        
        $(leaf.children).each(table.columns.readStep);
        
        table.rebuildTemplates();
    }

    table.removeColumn = function (columnName) {
        table.columns[columnName] = false;
        table.rebuildTemplates();
    }
    
    table.addColumn = function(columnName){
        table.columns[columnName] = true;
        table.rebuildTemplates();
    }

    table.rebuildTemplates = function () {
        table.header = $('<tr></tr>').get(0);
        table.body = $('<tr></tr>').get(0);

        $('th.command', table).clone().appendTo(table.header);
        $('td.command', table).clone().appendTo(table.body);

        table.columns.forCells(function (column) {
            $('th.' + column, table).clone().appendTo(table.header);
            $('td.' + column, table).clone().appendTo(table.body);
        });
    }
    
    table.headerRow = function(){
        return $(table.header).clone().headerRow();
    }
    
    table.bodyRow = function(step){
        return $(table.body).clone().bodyRow(step);
    }


    table.hasInactiveColumns = function () {
        return table.columns.hasInactiveColumns();
    }
    
    return table;
}

$.fn.headerRow = function () {
    var tr = this.get(0);

    tr.getCell = function (cellName) {
        return $('th.' + cellName, tr).get(0);
    }

    $('.column-remover', tr).click(function () {
        var column = $(this).metadata().key;
        $(this).closest('.table-editor').get(0).removeColumn(column);
        return false;
    });

    return tr;
}

$.fn.bodyRow = function(step){
    if (!step){
        step = new Step('row');
    }
    
    var tr = this.get(0);
    
    ST.cellMatchers.applyToAllDescendents(tr, step);
    ST.sentence(tr, {}, step);

    tr.getCell = function(cellName){
        return $('td.' + cellName, tr).get(0);
    }
    
    $('.remover', tr).click(function(){
        var editor = $(tr).closest('.table-editor').get(0);
        $(tr).remove();
        editor.rowRemoved();
        
        return false;
    });

    return tr;
}
