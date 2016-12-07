$("#StopRsync").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/rsync/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadRsync").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/rsync/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableRsync").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/rsync/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableRsync").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/rsync/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigRsync").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/rsync/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="RemoveRsyncDirectory"]').click(function () {
    var guid = $(this).attr("data-id");
    var par = $(this).parents("tr");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/rsync/directory/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            par.hide();
        }
    });
    _requests.push(aj);
});