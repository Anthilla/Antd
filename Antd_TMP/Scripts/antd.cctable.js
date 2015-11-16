$('#CCTAbleSettingShowMoreButton').click(function () {
    $('#CCTAbleSettingMore').toggle();
});

$('select[data-role="CommandBooleanSelector"]').windowed();

$('input[data-sumbit-type]').click(function () {
    var self = $(this);
    var rowGuid = self.attr('id');
    var type = self.attr('data-sumbit-type');
    var newValue = $('input[type="text"]#' + rowGuid).val();
    console.log(newValue);
    var boolSelected = $('select#' + rowGuid).find(':selected').val();
    console.log(boolSelected);
    jQuery.support.cors = true;
    $.ajax({
        url: '/cctable/launch/',
        type: 'POST',
        data: {
            Type: type,
            RowGuid: rowGuid,
            NewValue: newValue,
            BoolSelected: boolSelected
        },
        success: function (data) {
            location.reload(true);
            return false;
        }
    });
});

$('select[name="InputType"]').windowed({
    change: function (event, selected) {
        $('input[name="CCTableCommand"]').val('');
        var selected = $(selected);
        if (selected.val() == 'hidden') {
            $('#NoneCommand').show();
            $('#TextCommand').hide();
            $('#BooleanCommand').hide();
        }
        if (selected.val() == 'text') {
            $('#TextCommand').show();
            $('#NoneCommand').hide();
            $('#BooleanCommand').hide();
        }
        if (selected.val() == 'checkbox') {
            $('#BooleanCommand').show();
            $('#NoneCommand').hide();
            $('#TextCommand').hide();
        }
    }
});

$('select[name="FlagOSI"]').windowed();

$('select[name="FlagFunction"]').windowed();

$('input#AddInputReference').click(function () {
    //var label = UppercaseAllFirstLetters($('input[name="Label"]').val());
    //label = label.replace(/ /g, '');
    var labelReference = '{Value}';
    var input = $('textarea[name="CCTableCommandText"]');
    var readText = input.val();
    var editText = input.val() + labelReference;
    input.val(editText.replace(/\s{2,}/g, ' '));
});

//function UppercaseAllFirstLetters(str) {
//    str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
//        return letter.toUpperCase();
//    });
//    return str;
//}

$('input[data-create-input-layout]').click(function () {
    var type = $(this).attr('data-create-input-layout');
    var input = $('#InputLayoutText');
    var a = input.val();
    input.val('');
    if (a == '') {
        input.val(type);
    }
    else {
        input.val(a + ', ' + type);
    }
});

$('input[data-cctable-role="add-row"]').click(function () {
    var guid = $(this).attr('data-table-guid');
    var form = $('form[data-table-form="' + guid + '"]');
    form.toggle();
});

$('input[data-cctable-role="add-column"]').click(function () {
    var guid = $(this).attr('data-table-guid');
    var container = $('.further-commands[data-table="' + guid + '"]');
    var content = '<div class="row" data-table="' + guid + '">' +
                     '<div class="span3">' +
                         '<label>Command </label>' +
                     '</div>' +
                     '<div class="span5">' +
                         '<input type="text" name="Command" style="width: 90%; height: 25px;">' +
                     '</div>' +
                     '<div class="span5">' +
                         '<input type="text" name="Result" style="width: 90%; height: 25px;">' +
                     '</div>' +
                     '<div class="span2">' +
                         '<input class="bg-darkTeal" data-role="import-data" type="button" value="ImportData">' +
                     '</div>' +
                     '<div class="span1">' +
                        '<input class="bg-anthilla-orange" data-table="' + guid + '" type="button" data-role="RemoveThisRow" value="x">' +
                     '</div>' +
                 '</div>';
    container.append(content);
    RemoveCommandRow();
    ImportDataFromClipboard();
});

$(document).ready(function () {
    ImportDataFromClipboard();
});

function ImportDataFromClipboard() {
    $('input[data-role="import-data"]').click(function () {
        var self = $(this);
        self.parents('div.row').find('input[name="Command"]').val($('input#Clipboard0').val());
        self.parents('div.row').find('input[name="Result"]').val($('input#Clipboard1').val());
    });
}

function RemoveCommandRow() {
    $('input[data-role="RemoveThisRow"]').click(function () {
        var guid = $(this).attr('data-table');
        $(this).parents('.row[data-table="' + guid + '"]').remove();
    });
}

$('input[data-cctable-role="delete-table"]').click(function () {

    var guid = $(this).attr('data-table-guid');
    jQuery.support.cors = true;
    $.ajax({
        url: '/cctable/delete/table/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            location.reload(true);
        }
    });
});

function ShowDashboard() {
    $('#CCTableDashboard').toggle();
}

$('i[data-row-role="main"]').click(function () {
    var guid = $(this).attr('data-row-guid');
    $('tr[data-row-role="' + guid + '"]').toggle();
});

$('input.delete-command').click(function () {
    var guid = $(this).attr('data-cmd-guid');
    jQuery.support.cors = true;
    $.ajax({
        url: '/cctable/delete/row/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            location.reload(true);
        }
    });
});

$('input.set-edit-command').click(function () {
    var guid = $(this).attr('data-cmd-guid');
    var container = $(this).parents('tr').find('td[data-row-commands="list"]')
    var cont = "";
    container.find('p[data-command]').each(function (i) {
        var thisItem = $(this);
        var text = thisItem.text();
        var type = thisItem.attr('data-command');
        cont += '<label style="display: inline-block;">' + type + '</label>' +
            '<input data-guid-set="' + guid + '" data-command="' + type + '" type="text" value="' + text + '"/>' +
            '<br />';
    });
    //var html = container.html();
    //var set = container.find('p[data-command="set"]').text();
    //var get = container.find('p[data-command="get"]').text();
    //var cont = '<label style="display: inline-block;">Associated Command</label>' +
    //            '<input data-guid-set="' + guid + '" data-command="set" type="text" value="' + set + '"/>' +
    //            '<br />' +
    //            '<label style="display: inline-block;">Input Command</label>' +
    //            '<input data-guid-get="' + guid + '" data-command="get" type="text" value="' + get + '"/>';
    container.html(cont);
    var parent = $(this).parent('td');
    var newButton = '<input data-cmd-guid="' + guid + '" type="button" value="Update" class="bg-anthilla-violet edit-command" style="width: 100% !important"/>';
    parent.html(newButton);
    ConfirmEdit();
});

function ConfirmEdit() {
    $('input.edit-command').click(function () {
        var guid = $(this).attr('data-cmd-guid');
        var cmd = $('input[data-guid-set="' + guid + '"]').val();
        jQuery.support.cors = true;
        $.ajax({
            url: '/cctable/edit/row/' + guid + '/' + cmd,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (data) {
                location.reload(true);
            }
        });
    });
}

$('input[data-role="map-result"]').click(function () {
    var self = $(this);
    var guid = self.attr('data-row-guid');
    $('tr[data-row-map-guid="' + guid + '"]').toggle();
});

$('input[data-role="add-mapping-row"]').click(function () {
    var self = $(this);
    var guid = self.attr('data-row-guid');

    var row = '<div class="row">' +
                '<div class="span5">' +
                    '<input type="text" name="MapLabel" style="width: 90%; height: 25px;" placeholder="Map Label">' +
                '</div>' +
                '<div class="span5 droppable">' +
                    '<input type="text" name="MapLabelIndexText" style="width: 90%; height: 25px;" placeholder="Map Data">' +
                    '<input type="hidden" name="MapLabelIndex">' +
                '</div>' +
                '<div class="span1">' +
                    '<input class="bg-darkOrange" data-role="remove-mapping-row" type="button" value="x" style="width: 90%;">' +
                '</div>' +
            '</div>';
    self.parents('div.grid').find('.further-result-map').append(row);
    RemoveMappingRow();
    InitializeDragAndDrop();
});

function RemoveMappingRow() {
    $('input[data-role="remove-mapping-row"]').click(function () {
        $(this).parents('div.row').remove();
    });
}

$('input[data-role="refresh-result"]').click(function (event) {
    event.preventDefault();
    var g = $(this).attr('data-row-guid');
    jQuery.support.cors = true;
    $.ajax({
        url: '/cctable/row/refresh',
        type: 'POST',
        data: {
            Guid: g
        },
        success: function (data) {
            location.reload(true);
        }
    });
});

//dnd per mappatura
$(document).ready(function () {
    InitializeDragAndDrop();
});

function InitializeDragAndDrop() {
    $(function () {
        $('.result-part-item').draggable({
            helper: 'clone'
        });

        $('.droppable').droppable({
            accept: '.result-part-item',
            drop: function (event, ui) {
                var data = ui.draggable.attr('data-part');
                var dataIndex = ui.draggable.attr('data-position');
                AddData($(this), data);
                AddDataIndex($(this), dataIndex);
            }
        });
    });

    function AddData(droppable, value) {
        var input = droppable.find('input[name="MapLabelIndexText"]');
        var exValue = input.val();
        if (exValue == undefined || exValue.length == 0) {
            input.val(value);
        }
        else {
            input.val(exValue + ' ' + value);
        }
    }

    function AddDataIndex(droppable, value) {
        var input = droppable.find('input[name="MapLabelIndex"]');
        var exValue = input.val();
        if (exValue == undefined || exValue.length == 0) {
            input.val(value);
        }
        else {
            input.val(exValue + ';' + value);
        }
    }
}

//selectize
$(document).ready(function () {
    $.when(
        LoadEtcConfs()
    ).then(
        //console.log('.')
    );
});

function Callback(callback, url) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        data: {
            s: ' '
        },
        error: function () {
            callback();
            return false;
        },
        success: function (data) {
            callback(data);
        }
    });
}

var SelectizerOptions = function () {
    return {
        load: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
                data: {
                    s: query
                },
                error: function (input) {
                    callback();
                },
                success: function (data) {
                    callback(data);
                }
            });
        },
        render: function (data, escape) {
            var icon = 'bug';
            if (escape(data.type) == 0) {
                icon = 'file';
            }
            else {
                icon = 'folder';
            }
            return '<div><span data-path="' + escape(data.path) + '" class="button name bg-anthilla-violet">' +
                '<i class="icon-' + icon + ' on-left"></i>'
                + escape(data.name) + '</span></div>';
        }
    };
}();

var $etcConfsSelectizer = $('#show-etc-confs').selectize({
    maxItems: 1,
    valueField: 'path',
    labelField: 'name',
    searchField: ['name', 'path'],
    create: false,
    render: { option: SelectizerOptions.render },
    remoteUrl: '/cctable/conf/files',
    load: SelectizerOptions.load
});

function LoadEtcConfs() {
    if ($('#show-etc-confs').size() > 0) {
        var etcConfsSelectizer = $etcConfsSelectizer[0].selectize;
        etcConfsSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
            $('#show-etc-confs').hide();
        });
    }
}

$(document).ready(function () {
    var table = $('tbody[data-role="file-rows"]');
    table.find('tr').each(function (index) {
        $(this).find('input[name="LineNumber"]').val(index);
        $(this).find('.LineNumber').text(index);
    });
});

$('select[name="LineType"]').windowed();