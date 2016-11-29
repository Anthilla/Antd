$("#StopSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/samba/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/samba/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/samba/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/samba/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/samba/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="RemoveSambaResource"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/samba/resource/del",
        type: "POST",
        data: {
            Guid : guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});
