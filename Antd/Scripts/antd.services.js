//START SAMBA
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
//END SAMBA

//START BIND
$("#StopBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="RemoveBindZone"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/zone/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});
//END BIND

//START DHCP
$("#StopDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="DeleteDhcpdPool"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/pool/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="DeleteDhcpdClass"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/class/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="DeleteDhcpdReservation"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/reservation/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});
//END DHCP

//START SSH
$("#ActivateSsh").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/activate/ssh",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#RefreshSsh").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/refresh/ssh",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadSsh").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/reloadconfig/ssh",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ShowSshStructure").mousedown(function () {
    $("#SshStructure").show();
}).mouseup(function () {
    $("#SshStructure").hide();
});;

$(document).on("ready", function () {
    $("#SshForm").find('tr[data-object="ssh-parameter"]').each(function (index) {
        var dataKey = $(this).find('[name="DataKey"]');
        dataKey.attr("name", dataKey.attr("name") + "_" + index);
        var dataValue = $(this).find('[name="DataValue"]');
        dataValue.attr("name", dataValue.attr("name") + "_" + index);
        var dataFile = $(this).find('[name="DataFilePath"]');
        dataFile.attr("name", dataFile.attr("name") + "_" + index);
    });
});

$(document).on("ready", function () {
    $("[data-key-form]").each(function () {
        $(this).find('[data-object="key-parameter"]').each(function (index) {
            var dataKey = $(this).find('[name="DataKey"]');
            dataKey.attr("name", dataKey.attr("name") + "_" + index);
            var dataValue = $(this).find('[name="DataValue"]');
            dataValue.attr("name", dataValue.attr("name") + "_" + index);
            var dataFile = $(this).find('[name="DataFilePath"]');
            dataFile.attr("name", dataFile.attr("name") + "_" + index);
        });
    });
});

$("#AddNewParameterSsh").on("click", function () {
    $("#NewParameterSshDashboard").toggle();
});

$("#AddNewKey").on("click", function () {
    $("#NewSshKey").toggle();
});