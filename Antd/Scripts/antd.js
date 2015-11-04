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

//users
function SetCreateUser() {
    $('#UserCreateDashboard').toggle();
}

$('select[name="UserType"]').windowed({
    change: function (event, selected) {
        var selected = $(selected);
        if (selected.val() == 'app') {
            $('#AppUsersDBoard').hide();
            $('#SysUsersDBoard').hide();
            $('#AppUsersDBoard').show();
        }
        if (selected.val() == 'sys') {
            $('#AppUsersDBoard').hide();
            $('#SysUsersDBoard').hide();
            $('#SysUsersDBoard').show();
        }
    }
});

//volumes

$('i[id="ReloadVolumes"]').mouseover(function (event) {
    $(this).addClass('fg-anthilla-green');
});

$('i[id="ReloadVolumes"]').mouseout(function (event) {
    $(this).removeClass('fg-anthilla-green');
});

$('i[id="ReloadVolumes"]').click(function (event) {
    event.preventDefault();
    jQuery.support.cors = true;
    $.ajax({
        url: '/storage/reload/volumes/',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            location.reload(true);
            return false;
        }
    });
});

//2fa,,
$('#Disable2FA').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/auth/disable/',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            location.reload(true);
            return false;
        }
    });
});

$('#Enable2FA').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/auth/enable/',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            location.reload(true);
            return false;
        }
    });
});

//command management
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

//////////////

$('i[data-role="show-mount"]').click(function () {
    var self = $(this);
    var i = '<input type="text" data-role="value-mount" data-folder="' + self.attr('data-folder') + '"/>' +
        '<i class="icon-plus fg-green" data-role="submit-mount" data-folder="' + self.attr('data-folder') + '"></i>';
    self.after(i);
    InitSubmitMount();
});

function InitSubmitMount() {
    $('i[data-role="submit-mount"]').click(function () {
        var g = $(this).attr('data-folder');
        var m = $('input[data-folder="' + g + '"]').val();
        alert(g);
        alert(m);
        jQuery.support.cors = true;
        $.ajax({
            url: '/apps/mount',
            type: 'POST',
            data: {
                Folder: g,
                Mount: m
            },
            success: function (data) {
                location.reload(true);
            }
        });
    });
}
///////////cookiecookiecookie////////////////////////
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