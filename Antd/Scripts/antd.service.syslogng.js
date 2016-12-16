$("#StopSyslogNg").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syslogng/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadSyslogNg").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syslogng/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableSyslogNg").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syslogng/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableSyslogNg").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syslogng/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigSyslogNg").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syslogng/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});
