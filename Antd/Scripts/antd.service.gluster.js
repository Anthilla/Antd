$("#StopGluster").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/gluster/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadGluster").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/gluster/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableGluster").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/gluster/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableGluster").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/gluster/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigGluster").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/gluster/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="AddGlusterConfigNewNode"]').on("click", function () {
    var html = '<div class="input-control text" data-role="input-control"><input name="GlusterNode" type="text" style="width: 90%;"></div>';
    $('[data-role="GlusterConfigNewNodes"]').append(html);
});

$('[data-role="AddGlusterConfigNewVolume"]').on("click", function () {
    var html = '<div class="input-control text border-anthilla-gray" data-role="input-control"><input name="GlusterVolumeName" type="text" style="width: 90%;"><input name="GlusterVolumeBrick" type="text" style="width: 90%;"><input name="GlusterVolumeMountPoint" type="text" style="width: 90%;"></div>';
    $('[data-role="GlusterConfigNewVolumes"]').append(html);
});