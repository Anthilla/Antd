var $sh = jQuery.noConflict();

$sh("#ActivateSsh").on("click", function () {
    jQuery.support.cors = true;
    var aj = $sh.ajax({
        url: "/services/activate/ssh",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$sh("#RefreshSsh").on("click", function () {
    jQuery.support.cors = true;
    var aj = $sh.ajax({
        url: "/services/refresh/ssh",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$sh("#ReloadSsh").on("click", function () {
    jQuery.support.cors = true;
    var aj = $sh.ajax({
        url: "/services/reloadconfig/ssh",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$sh("#ShowSshStructure").mousedown(function () {
    $sh("#SshStructure").show();
}).mouseup(function () {
    $sh("#SshStructure").hide();
});;

$sh(document).on("ready", function () {
    $sh("#SshForm").find('tr[data-object="ssh-parameter"]').each(function (index) {
        var dataKey = $sh(this).find('[name="DataKey"]');
        dataKey.attr("name", dataKey.attr("name") + "_" + index);
        var dataValue = $sh(this).find('[name="DataValue"]');
        dataValue.attr("name", dataValue.attr("name") + "_" + index);
        var dataFile = $sh(this).find('[name="DataFilePath"]');
        dataFile.attr("name", dataFile.attr("name") + "_" + index);
    });
});

$sh(document).on("ready", function () {
    $sh("[data-key-form]").each(function () {
        $sh(this).find('[data-object="key-parameter"]').each(function (index) {
            var dataKey = $sh(this).find('[name="DataKey"]');
            dataKey.attr("name", dataKey.attr("name") + "_" + index);
            var dataValue = $sh(this).find('[name="DataValue"]');
            dataValue.attr("name", dataValue.attr("name") + "_" + index);
            var dataFile = $sh(this).find('[name="DataFilePath"]');
            dataFile.attr("name", dataFile.attr("name") + "_" + index);
        });
    });
});

$sh("#AddNewParameterSsh").on("click", function () {
    $sh("#NewParameterSshDashboard").toggle();
});

$sh("#AddNewKey").on("click", function () {
    $sh("#NewSshKey").toggle();
});