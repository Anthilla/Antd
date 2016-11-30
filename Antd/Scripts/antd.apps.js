$('[data-role="ApplicationSetup"]').on("click", function () {
    var app = $(this).attr("data-app-name");
    jQuery.support.cors = true;
    $.ajax({
        url: "/apps/setup",
        type: "POST",
        data: {
            AppName: app
        },
        success: function () {
            location.reload(true);
        }
    });
});

$('[data-role="ApplicationRestart"]').on("click", function () {
    var app = $(this).attr("data-app-name");
    jQuery.support.cors = true;
    $.ajax({
        url: "/apps/restart",
        type: "POST",
        data: {
            Name: app
        },
        success: function () {
            location.reload(true);
        }
    });
});

$('[data-role="ApplicationStop"]').on("click", function () {
    var app = $(this).attr("data-app-name");
    jQuery.support.cors = true;
    $.ajax({
        url: "/apps/stop",
        type: "POST",
        data: {
            Name: app
        },
        success: function () {
            location.reload(true);
        }
    });
});

$(document).on("ready", function () {
    $('[data-role="ApplicationStatus"]').each(function () {
        var container = $(this);
        var app = container.attr("data-app-name");
        jQuery.support.cors = true;
        $.ajax({
            url: "/apps/status/" + app,
            type: "GET",
            success: function (data) {
                container.text(data);
            }
        });
    });
});

$(document).on("ready", function () {
    $('[data-role="ApplicationActivity"]').each(function () {
        var container = $(this);
        var app = container.attr("data-app-name");
        jQuery.support.cors = true;
        $.ajax({
            url: "/apps/active/" + app,
            type: "GET",
            success: function (data) {
                if (data === "active") {
                    container.html('<i class="icon-checkmark fg-anthilla-green"></i>');
                    return;
                }
                if (data === "inactive") {
                    container.html('<i class="icon-cancel-2 fg-red"></i>');
                    return;
                }
                container.text(data);
            }
        });
    });
});
