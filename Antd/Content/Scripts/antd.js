//terminal
$('#OpenTerminal').click(function () {
    $('#TerminalContainer').toggle();
});

$('input[data-role="open-console"]').click(function (event) {
    event.preventDefault();
    $('#TerminalContainer').toggle();
});

$('#TerminalClose').click(function () {
    $('#TerminalContainer').hide();
});

//cctable
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

$('#AddInputReference').click(function () {
    var label = UppercaseAllFirstLetters($('input[name="Label"]').val());
    label = label.replace(/ /g, '');
    if (label.length > 0) {
        var labelReference = '{New' + label + '}';
        var input = $('.add-to-this');
        input.val(input.val() + labelReference);
    }
});

function UppercaseAllFirstLetters(str) {
    str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
        return letter.toUpperCase();
    });
    return str;
}

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

$('tr[data-row-role="main"]').click(function () {
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
    var container = $('td[data-row-commands="list"]');
    var html = container.html();
    var set = container.find('p[data-command="set"]').text();
    var get = container.find('p[data-command="get"]').text();
    var cont = '<label style="display: inline-block;">Associated Command</label>' +
                '<input data-guid-set="' + guid + '" data-command="set" type="text" value="' + set + '"/>' +
                '<br />' +
                '<label style="display: inline-block;">Input Command</label>' +
                '<input data-guid-get="' + guid + '" data-command="get" type="text" value="' + get + '"/>';
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
                '<div class="span4">' +
                    '<input type="text" name="MapLabel" style="width: 90%; height: 25px;">' +
                '</div>' +
                '<div class="span4 droppable">' +
                    '<input type="text" name="MapLabelIndexText" style="width: 90%; height: 25px;">' +
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

///command management
$('#CmdMgmtButton').click(function () {
    $('#CommandManagementForm').toggle();
});

function CloseCommandMgmtPanel() {
    $('#CommandManagementForm').hide();
}

$(document).ready(function () {
    var main = $('input[name="Command"]');
    var layout = $('input[name="CommandLayout"]');
    main.keyup(function () {
        CopyInputLayout(main, layout);
    });

    $('input[type="text"]').not('input[name="Command"], input[name="CommandLayout"]').dblclick(function () {
        AddInputIDReference($(this), main, layout);
    });
});

function CopyInputLayout(main, layout) {
    var mainVal = '',
    layoutVal = '',
    mainLength = '',
    layoutLength = '',
    tmpVal = '';

    layout.val('');
    if (main.val().length === 0) {
        mainVal = '';
        layoutVal = '';
        mainLength = '';
        layoutLength = '';
        tmpVal = '';
        layout.val('');
    }
    if (mainVal.length === 0 && layoutVal.length === 0) {
        tmpVal = $('input[name="Command"]').val().replace(/_+/g, ' ');
        layout.val(tmpVal);
    } else {
        mainLength = mainVal.length;
        layoutLength = layoutVal.length;
        var mainString = main.val().substring(mainLength, main.val().length);
        var layoutString = layout.val().substring(layoutLength, layout.val().length);
        tmpVal = (layoutVal + mainString).replace(/_+/g, ' ');
        layout.val(tmpVal);
    }
}

function AddInputIDReference(self, main, layout) {
    var parameter = self;
    var label = parameter.attr('id');
    var value = parameter.val();
    layout.val(layout.val() + '{' + label + '}');
    main.val(main.val() + value);
    mainVal = main.val();
    layoutVal = layout.val();
    var hiddenInputId = $('input[name="InputID"]');
    hiddenInputId.val(label);
    main.focus();
}

/////////////////////

////////////////cookiecookiecookie///////////////////////////////////
$('#LockInput').click(function () {
    var value = cookie.get('_input');
    if (value == 'disabled') {
        EnableInputs();
        var d = cookie.get('_input');
        ChangeLockIcon(d);
    }
    else {
        DisableInputs();
        var e = cookie.get('_input');
        ChangeLockIcon(e);
    }
    return false;
});

var cookie = Cookies.noConflict();

$(document).ready(function () {
    var value = cookie.get('_input');
    if (value == undefined) {
        cookie.set('_input', 'enabled', { expires: 7 });
    }

    if (value == 'disabled') {
        ChangeLockIcon(value);
        DisableInputs();
    }
    else {
        ChangeLockIcon(value);
        EnableInputs();
    }
});

function ChangeLockIcon(value) {
    var icon = $('#LockInput').find('i');
    if (value == 'disabled') {
        icon.removeClass('icon-unlocked');
        icon.addClass('icon-locked');
    }
    else {
        icon.removeClass('icon-locked');
        icon.addClass('icon-unlocked');
    }
}

function EnableInputs() {
    $('input').each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).prop('disabled', false);
    });
    $('.button').each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).removeClass('disabled');
    });
    cookie.set('_input', 'enabled', { expires: 7 });
}

function DisableInputs() {
    $('input').each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).prop('disabled', true);
    });
    $('.button').each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).addClass('disabled');
    });
    cookie.set('_input', 'disabled', { expires: 7 });
}
///////////////////////////////////////////////////////////////////

$('[id^=Update]').click(function () {
    //$('p[id^=Update]').click(function () {
    var self = $(this);
    var id = self.attr('id');
    var inputID = id.replace('Update', 'New');
    var newValue = $('#' + inputID).val();
    if (newValue.length > 0) {
        jQuery.support.cors = true;
        $.ajax({
            url: '/command/mgmt/ex/' + inputID + '/' + newValue,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (data) {
                console.log(inputID + ': ' + data);
                alert('Value changed -> ' + inputID + ': ' + data);
                location.reload(true);
                return false;
            }
        });
    }
});

$('a.anchor').click(function (event) {
    event.preventDefault();
    var href = $(this).attr('data-scrollto');
    if (href == 'top') {
        $('html, body').animate({
            'scrollTop': 0
        }, 500);
        return false;
    }
    else {
        var scroll = $(href).offset().top - 40;
        $('html, body').animate({
            'scrollTop': scroll
        }, 500);
        return false;
    }
});

$(window).scroll(function () {
    if ($(window).scrollTop() > 150) {
        $('nav.navigation-bar.page-bar').css('position', 'fixed');
        $('nav.navigation-bar.page-bar').css('z-index', '999');
        $('nav.navigation-bar.page-bar').css('top', '0');
        $('nav.navigation-bar.page-bar').css('left', '0');
        $('nav.navigation-bar.page-bar').css('padding-left', '15px');
    }
    if ($(window).scrollTop() < 150) {
        $('nav.navigation-bar.page-bar').css('position', 'relative');
        $('nav.navigation-bar.page-bar').css('padding-left', '0');
    }
});

$(document).ready(function () {
    $('input:password').val('');
    $('input:text').attr('autocomplete', 'off');
    $('input:password').attr('autocomplete', 'off');
});

function Reset() {
    $('.item').hide();
    $('select').prop('selectedIndex', 0);
    $('.project-selectable').removeClass('picked');
    $('.group-selectable').removeClass('picked');
    $('.js-files').hide();
    $("input:text").each(function () {
        $(this).val("");
    });
    $('.file').remove();
    return false;
}

function SetCreate() {
    var button = $('#create-button');
    if (button != null) {
        button.toggleClass('fg-anthilla-green');
        button.toggleClass('no-overlay');
        button.toggleClass('fg-anthilla-gray');
        button.toggleClass('bg-anthilla-green');
    }
    $('#DashboardForm').toggle();
    return false;
}

function Quit() {
    Reset();
    SetCreate();
    return false;
}