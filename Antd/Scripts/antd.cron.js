var $cron = jQuery.noConflict();

$cron("#ShowCronInfo").on("click", function () {
    $cron("#CronDashboard").show();
});

$cron("#HideCronInfo").on("click", function () {
    $cron("#CronDashboard").hide();
});

$cron('[data-role="EditCronJob"]').on("click", function () {
    var guid = $cron(this).attr("data-guid");
    var command = $cron(this).parents("td").find("textarea").val();
    jQuery.support.cors = true;
    var aj = $cron.ajax({
        url: "/scheduler/edit",
        type: "POST",
        data: {
            Guid: guid,
            Command: command
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$cron('[data-role="DeleteCronJob"]').on("click", function () {
    var guid = $cron(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $cron.ajax({
        url: "/scheduler/delete",
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

$cron('[data-role="DisableCronJob"]').on("click", function () {
    var guid = $cron(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $cron.ajax({
        url: "/scheduler/disable",
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

$cron('[data-role="EnableCronJob"]').on("click", function () {
    var guid = $cron(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $cron.ajax({
        url: "/scheduler/enable",
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

$cron('[data-role="LaunchCronJob"]').on("click", function () {
    var guid = $cron(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $cron.ajax({
        url: "/scheduler/launch",
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