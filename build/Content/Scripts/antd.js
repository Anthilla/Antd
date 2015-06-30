//terminal
$('#OpenTerminal').click(function () {
    $('#Terminal').toggle();
    $('#TerminalContent').toggle();
});

jQuery(function ($) {
    $('#TerminalContent').terminal(function (cmd, term) {
        if (cmd == 'help') {
            term.echo("available commands are mysql, js, test");
        } else if (cmd == 'test') {
            term.push(function (cmd, term) {
                if (command == 'help') {
                    term.echo('type "ping" it will display "pong"');
                } else if (cmd == 'ping') {
                    term.echo('pong');
                } else {
                    term.echo('unknown command "' + cmd + '"');
                }
            }, {
                prompt: 'test> ',
                name: 'test'
            });
        } else if (command == "js") {
            term.push(function (command, term) {
                var result = window.eval(command);
                if (result != undefined) {
                    term.echo(String(result));
                }
            }, {
                name: 'js',
                prompt: 'js> '
            });
        } else if (command == 'mysql') {
            term.push(function (command, term) {
                term.pause();
                //$.jrpc is helper function which
                //creates json-rpc request
                $.jrpc("mysql-rpc-demo.php",
                  "query",
                  [command],
                  function (data) {
                      term.resume();
                      if (data.error) {
                          term.error(data.error.message);
                      } else {
                          if (typeof data.result == 'boolean') {
                              term.echo(data.result ?
                                        'success' :
                                        'fail');
                          } else {
                              var len = data.result.length;
                              for (var i = 0; i < len; ++i) {
                                  term.echo(data.result[i].join(' | '));
                              }
                          }
                      }
                  },
                  function (xhr, status, error) {
                      term.error('[AJAX] ' + status +
                                 ' - Server reponse is: \n' +
                                 xhr.responseText);
                      term.resume();
                  }); // rpc call
            }, {
                greetings: "This is example of using mysql" +
                  " from terminal\n you are allowed to exe" +
                  "cute: select, insert, update and delete" +
                  " from/to table:\n   table test(integer_" +
                  "value integer, varchar_value varchar(255))",
                prompt: "mysql> "
            });
        } else {
            term.echo("unknow command " + command);
        }
    }, {
        greetings: "multiply terminals demo use help" +
              " to see available commands"
    });
});

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