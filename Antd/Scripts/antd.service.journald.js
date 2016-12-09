$("#StopJournald").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/journald/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadJournald").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/journald/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableJournald").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/journald/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableJournald").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/journald/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigJournald").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/journald/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});