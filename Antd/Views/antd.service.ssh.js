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