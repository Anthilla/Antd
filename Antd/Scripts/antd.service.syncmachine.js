$("#StopSyncMachine").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syncmachine/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadSyncMachine").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syncmachine/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableSyncMachine").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syncmachine/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableSyncMachine").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syncmachine/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="AddSyncMachineResource"]').click(function () {
    var machineAddress = $(this).attr("data-address");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syncmachine/machine",
        type: "POST",
        data: {
            MachineAddress: machineAddress
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="RemoveSyncMachineResource"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/syncmachine/machine/del",
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
