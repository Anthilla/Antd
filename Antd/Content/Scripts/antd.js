//page-system-general
$('#NewHostname').keyup(function () {
    var value = $('#NewHostname').val();
    if (value != "" || val != " ") {
        $('#UpdateHostname').show();
    }
});

$('#UpdateHostname').click(function () {
    jQuery.support.cors = true;
    var newHostName = $('#NewHostname').val();
    $.ajax({
        url: '/system/update/hostname/' + newHostName,
        type: 'POST',
        data: JSON.stringify(newHostName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewDomainname').keyup(function () {
    var value = $('#NewDomainname').val();
    if (value != "" || val != " ") {
        $('#UpdateDomainname').show();
    }
});

$('#UpdateDomainname').click(function () {
    jQuery.support.cors = true;
    var newDomainName = $('#NewDomainname').val();
    $.ajax({
        url: '/system/update/domainname/' + newDomainName,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTimezone').change(function () {
    $('#UpdateTimezone').show();
});

$('#UpdateTimezone').click(function () {
    jQuery.support.cors = true;
    var newTimezone = $('#NewTimezone>option:selected').val();
    $.ajax({
        url: '/system/update/timezone/' + newTimezone,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTimeserver').keyup(function () {
    var value = $('#NewTimeserver').val();
    if (value != "" || val != " ") {
        $('#UpdateTimeserver').show();
    }
});

$('#UpdateTimeserver').click(function () {
    jQuery.support.cors = true;
    var newTimeserver = $('#NewTimeserver').val();
    $.ajax({
        url: '/system/update/timeserver/' + newTimeserver,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewLanguage').change(function () {
    $('#UpdateLanguage').show();
});

$('#UpdateLanguage').click(function () {
    jQuery.support.cors = true;
    var newLanguage = $('#NewLanguage>option:selected').val();
    $.ajax({
        url: '/system/update/language/' + newLanguage,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#SaveSystemGeneral').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/systemgeneral/',
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

//page-system-advanced

$('#UpdateProtocol').click(function () {
    var newProtocol = $('input[name="WebConfiguratorProtocol"]:checked').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/protocol/' + newProtocol,
        type: 'POST',
        data: JSON.stringify(newProtocol),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTCPport').keyup(function () {
    var value = $('#NewTCPport').val();
    if (value != "" || val != " ") {
        $('#UpdateTCPport').show();
    }
});

$('#UpdateTCPport').click(function () {
    var newTCPport = $('#NewTCPport').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/tcpport/' + newTCPport,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewMaxProcesses').keyup(function () {
    var value = $('#NewMaxProcesses').val();
    if (value != "" || val != " ") {
        $('#UpdateMaxProcesses').show();
    }
});

$('#UpdateMaxProcesses').click(function () {
    var newMaxProcesses = $('#NewMaxProcesses').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/maxprocs/' + newMaxProcesses,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateWebGUIRedirects').click(function () {
    var newWebGUIRedirects = $('#NewWebGUIRedirects').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/webguiredirects/' + newWebGUIRedirects,
        type: 'POST',
        data: JSON.stringify(newWebGUIRedirects),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateWebGUILoginAutocomplete').click(function () {
    var newWebGUILoginAutocomplete = $('#NewWebGUILoginAutocomplete').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/webguiloginautocomplete/' + newWebGUILoginAutocomplete,
        type: 'POST',
        data: JSON.stringify(newWebGUILoginAutocomplete),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateWebGUIloginmessages').click(function () {
    var newWebGUIloginmessages = $('#NewWebGUIloginmessages').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/webguiloginmessages/' + newWebGUIloginmessages,
        type: 'POST',
        data: JSON.stringify(newWebGUIloginmessages),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateAntiLockout').click(function () {
    var newAntiLockout = $('#NewAntiLockout').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/antilockout/' + newAntiLockout,
        type: 'POST',
        data: JSON.stringify(newAntiLockout),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDNSRebindCheck').click(function () {
    var newDNSRebindCheck = $('#NewDNSRebindCheck').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/dnsrebindcheck/' + newDNSRebindCheck,
        type: 'POST',
        data: JSON.stringify(newDNSRebindCheck),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewAlternateHostnames').keyup(function () {
    var value = $('#NewAlternateHostnames').val();
    if (value != "" || val != " ") {
        $('#UpdateAlternateHostnames').show();
    }
});

$('#UpdateAlternateHostnames').click(function () {
    var newAlternateHostnames = $('#NewAlternateHostnames').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/alternatehostnames/' + newAlternateHostnames,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateHTTP_REFERERenforcement').click(function () {
    var newHTTP_REFERERenforcement = $('#NewHTTP_REFERERenforcement').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/refererenforcement/' + newHTTP_REFERERenforcement,
        type: 'POST',
        data: JSON.stringify(newHTTP_REFERERenforcement),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateTabText').click(function () {
    var newTabText = $('#NewTabText').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/tabtext/' + newTabText,
        type: 'POST',
        data: JSON.stringify(newTabText),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateEnableSecureShell').click(function () {
    var newEnableSecureShell = $('#NewEnableSecureShell').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/enablesecureshell/' + newEnableSecureShell,
        type: 'POST',
        data: JSON.stringify(newEnableSecureShell),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateAuthenticationMethod').click(function () {
    var newAuthenticationMethod = $('#NewAuthenticationMethod').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/authenticationmethod/' + newAuthenticationMethod,
        type: 'POST',
        data: JSON.stringify(newAuthenticationMethod),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewSSHport').keyup(function () {
    var value = $('#NewSSHport').val();
    if (value != "" || val != " ") {
        $('#UpdateSSHport').show();
    }
});

$('#UpdateSSHport').click(function () {
    var newSSHport = $('#NewSSHport').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/sshport/' + newSSHport,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateSerialTerminal').click(function () {
    var newSerialTerminal = $('#NewSerialTerminal').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/serialterminal/' + newSerialTerminal,
        type: 'POST',
        data: JSON.stringify(newSerialTerminal),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewSerialSpeed').change(function () {
    $('#UpdateSerialSpeed').show();
});

$('#UpdateSerialSpeed').click(function () {
    jQuery.support.cors = true;
    var newSerialSpeed = $('#NewSerialSpeed>option:selected').val();
    $.ajax({
        url: '/system/update/serialspeed/' + newSerialSpeed,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewPrimaryConsole').change(function () {
    $('#UpdatePrimaryConsole').show();
});

$('#UpdatePrimaryConsole').click(function () {
    jQuery.support.cors = true;
    var newPrimaryConsole = $('#NewPrimaryConsole>option:selected').val();
    $.ajax({
        url: '/system/update/primaryconsole/' + newPrimaryConsole,
        type: 'POST',
        data: JSON.stringify(newDomainName),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateConsoleMenu').click(function () {
    var newConsoleMenu = $('#NewConsoleMenu').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/consolemenu/' + newConsoleMenu,
        type: 'POST',
        data: JSON.stringify(newConsoleMenu),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});