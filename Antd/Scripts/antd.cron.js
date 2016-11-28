$("#ShowCronInfo").on("click", function () {
    $("#CronDashboard").show();
});

$("#HideCronInfo").on("click", function () {
    $("#CronDashboard").hide();
});

$('[data-role="EditCronJob"]').on("click", function () {
    var guid = $(this).attr("data-guid");
    var command = $(this).parents("td").find("textarea").val();
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$('[data-role="DeleteCronJob"]').on("click", function () {
    var guid = $(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$('[data-role="DisableCronJob"]').on("click", function () {
    var guid = $(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$('[data-role="EnableCronJob"]').on("click", function () {
    var guid = $(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$('[data-role="LaunchCronJob"]').on("click", function () {
    var guid = $(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
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