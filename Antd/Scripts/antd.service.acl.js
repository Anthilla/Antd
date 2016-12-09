$("#StopAcl").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/acl/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadAcl").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/acl/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableAcl").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/acl/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableAcl").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/acl/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="RemoveAcl"]').click(function () {
    var self = $(this);
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/acl/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            self.parents('[data-click="ShowAclRules"]').remove();
        }
    });
    _requests.push(aj);
});

$('[data-role="ApplyAcl"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/acl/apply",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function (data) {
            if (data.length < 1) {
                alert("Acls applied!!");
            } else {
                alert(data);
            }
        }
    });
    _requests.push(aj);
});