$("#StopSshd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/sshd/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadSshd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/sshd/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableSshd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/sshd/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableSshd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/sshd/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigSshd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/sshd/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});
