$(".search-field").keyup(function () {
    var tableReference = $(this).attr("data-table");
    var searchIn = $('table[data-search="' + tableReference + '"]').find("tbody");
    var queryString = $(this).val();
    var row = searchIn.children("tr");
    row.each(function () {
        var thisRow = $(this);
        var queriedText = $(this).text();
        if (queriedText.indexOf(queryString) !== -1) {
            thisRow.show();
        }
        if (queriedText.indexOf(queryString) < 0) {
            thisRow.hide();
        }
    });
});

//START SAMBA
$("#ActivateSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/activate/samba",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#RefreshSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/refresh/samba",
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
        url: "/services/reloadconfig/samba",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ShowSambaStructure").mousedown(function () {
    $("#SambaStructure").show();
}).mouseup(function () {
    $("#SambaStructure").hide();
});;

$(document).on("ready", function () {
    $("#SambaForm").find('tr[data-object="samba-parameter"]').each(function (index) {
        var dataKey = $(this).find('[name="DataKey"]');
        dataKey.attr("name", dataKey.attr("name") + "_" + index);
        var dataValue = $(this).find('[name="DataValue"]');
        dataValue.attr("name", dataValue.attr("name") + "_" + index);
        var dataFile = $(this).find('[name="DataFilePath"]');
        dataFile.attr("name", dataFile.attr("name") + "_" + index);
    });
});

$(document).on("ready", function () {
    $("[data-share-form]").each(function () {
        $(this).find('[data-object="share-parameter"]').each(function (index) {
            var dataKey = $(this).find('[name="DataKey"]');
            dataKey.attr("name", dataKey.attr("name") + "_" + index);
            var dataValue = $(this).find('[name="DataValue"]');
            dataValue.attr("name", dataValue.attr("name") + "_" + index);
            var dataFile = $(this).find('[name="DataFilePath"]');
            dataFile.attr("name", dataFile.attr("name") + "_" + index);
        });
    });
});

$("#AddNewParameterSamba").on("click", function () {
    $("#NewParameterSambaDashboard").toggle();
});

$("#AddNewShare").on("click", function () {
    $("#NewSambaShare").toggle();
});
//END SAMBA

//START BIND
$("#ActivateBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/activate/bind",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#RefreshBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/refresh/bind",
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
        url: "/services/reloadconfig/bind",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$(document).on("ready", function () {
    $.when(
        $('input[data-array="bind"]').selectize({
            delimiter: ";",
            persist: false,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            }
        })
    ).done(function () {
        $(".selectize-input.items").find('div.item[data-Value=""]').remove();
        $('input[data-array="bind"]').hide();
    });
});

$(document).on("ready", function () {
    $('form[data-role="bind-section-update"]').each(function () {
        $(this).find('[data-object="bind-parameter"]').each(function (index) {
            var dataKey = $(this).find('[name="DataKey"]');
            dataKey.attr("name", dataKey.attr("name") + "_" + index);
            var dataValue = $(this).find('[name="DataValue"]');
            dataValue.attr("name", dataValue.attr("name") + "_" + index);
            var dataFile = $(this).find('[name="DataFilePath"]');
            dataFile.attr("name", dataFile.attr("name") + "_" + index);
        });
    });
});

$("#AddNewAclBind").on("click", function () {
    $("#NewAclBindDashboard").toggle();
});

$("#AddNewKeyBind").on("click", function () {
    $("#NewKeyBindDashboard").toggle();
});

$("#AddNewMastersBind").on("click", function () {
    $("#NewMastersBindDashboard").toggle();
});

$("#AddNewServerBind").on("click", function () {
    $("#NewServerBindDashboard").toggle();
});

$("#AddNewViewBind").on("click", function () {
    $("#NewViewBindDashboard").toggle();
});

$("#AddNewZoneBind").on("click", function () {
    $("#NewZoneBindDashboard").toggle();
});
//END BIND

//START DHCP
$("#ActivateDhcp").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/activate/dhcp",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#RefreshDhcp").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/refresh/dhcp",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadDhcp").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/reloadconfig/dhcp",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ShowDhcpStructure").mousedown(function () {
    $("#DhcpStructure").show();
}).mouseup(function () {
    $("#DhcpStructure").hide();
});;

$(document).on("ready", function () {
    $("#DhcpForm").find('tr[data-object="dhcp-parameter"]').each(function (index) {
        var dataKey = $(this).find('[name="DataKey"]');
        dataKey.attr("name", dataKey.attr("name") + "_" + index);
        var dataValue = $(this).find('[name="DataValue"]');
        dataValue.attr("name", dataValue.attr("name") + "_" + index);
        var dataFile = $(this).find('[name="DataFilePath"]');
        dataFile.attr("name", dataFile.attr("name") + "_" + index);
    });
});

$(document).on("ready", function () {
    $("[data-share-form]").each(function () {
        $(this).find('[data-object="share-parameter"]').each(function (index) {
            var dataKey = $(this).find('[name="DataKey"]');
            dataKey.attr("name", dataKey.attr("name") + "_" + index);
            var dataValue = $(this).find('[name="DataValue"]');
            dataValue.attr("name", dataValue.attr("name") + "_" + index);
            var dataFile = $(this).find('[name="DataFilePath"]');
            dataFile.attr("name", dataFile.attr("name") + "_" + index);
        });
    });
});

$("#AddNewParameterDhcp").on("click", function () {
    $("#NewParameterDhcpDashboard").toggle();
});

$("#AddNewShare").on("click", function () {
    $("#NewDhcpShare").toggle();
});

$(document).on("ready", function () {
    $.when(
        $('input[data-array="dhcp"]').selectize({
            delimiter: ";",
            persist: false,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            }
        })
    ).done(function () {
        $(".selectize-input.items").find('div.item[data-Value=""]').remove();
        $('input[data-array="dhcp"]').hide();
    });
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