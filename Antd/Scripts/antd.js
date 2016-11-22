var $jq = jQuery.noConflict();

$jq("#LockInput").on("click", function () {
    var value = cookie.get("_input");
    if (value === "disabled") {
        EnableInputs();
        var d = cookie.get("_input");
        ChangeLockIcon(d);
    }
    else {
        DisableInputs();
        var e = cookie.get("_input");
        ChangeLockIcon(e);
    }
    return false;
});

var cookie = Cookies.noConflict();

$jq(document).on("ready", function () {
    var value = cookie.get("_input");
    if (value == undefined) {
        cookie.set("_input", "enabled", { expires: 7 });
    }

    if (value === "disabled") {
        ChangeLockIcon(value);
        DisableInputs();
    }
    else {
        ChangeLockIcon(value);
        EnableInputs();
    }
});

function ChangeLockIcon(value) {
    var icon = $jq("#LockInput").find("i");
    if (value === "disabled") {
        icon.removeClass("icon-unlocked");
        icon.addClass("icon-locked");
    }
    else {
        icon.removeClass("icon-locked");
        icon.addClass("icon-unlocked");
    }
}

function EnableInputs() {
    $jq("input").each(function () {
        $jq(this).fadeOut(300).fadeIn(150);
        $jq(this).delay(455).prop("disabled", false);
    });
    $jq(".button").each(function () {
        $jq(this).fadeOut(300).fadeIn(150);
        $jq(this).delay(455).removeClass("disabled");
    });
    cookie.set("_input", "enabled", { expires: 7 });
}

function DisableInputs() {
    $jq("input").each(function () {
        $jq(this).fadeOut(300).fadeIn(150);
        $jq(this).delay(455).prop("disabled", true);
    });
    $jq(".button").each(function () {
        $jq(this).fadeOut(300).fadeIn(150);
        $jq(this).delay(455).addClass("disabled");
    });
    cookie.set("_input", "disabled", { expires: 7 });
}

$jq("a.anchor").on("click", function () {
    event.preventDefault();
    var href = $jq(this).attr("data-scrollto");
    if (href === "top") {
        $jq("html, body").animate({
            'scrollTop': 0
        }, 500);
        return false;
    }
    else {
        var scroll = $jq(href).offset().top - 60;
        $jq("html, body").animate({
            'scrollTop': scroll
        }, 500);
        return false;
    }
});

$jq(window).scroll(function () {
    if ($jq(window).scrollTop() > 150) {
        $jq("nav.navigation-bar.page-bar").css("position", "fixed");
        $jq("nav.navigation-bar.page-bar").css("z-index", "999");
        $jq("nav.navigation-bar.page-bar").css("top", "0");
        $jq("nav.navigation-bar.page-bar").css("left", "0");
        $jq("nav.navigation-bar.page-bar").css("padding-left", "15px");
    }
    if ($jq(window).scrollTop() < 150) {
        $jq("nav.navigation-bar.page-bar").css("position", "relative");
        $jq("nav.navigation-bar.page-bar").css("padding-left", "0");
    }
});

function Reset() {
    $jq(".item").hide();
    $jq("select").prop("selectedIndex", 0);
    $jq(".project-selectable").removeClass("picked");
    $jq(".group-selectable").removeClass("picked");
    $jq(".js-files").hide();
    $jq("input:text").each(function () {
        $jq(this).val("");
    });
    $jq(".file").remove();
    return false;
}

function SetCreate() {
    var button = $jq("#create-button");
    if (button != null) {
        button.toggleClass("fg-anthilla-green");
        button.toggleClass("no-overlay");
        button.toggleClass("fg-anthilla-gray");
        button.toggleClass("bg-anthilla-green");
    }
    $jq("#DashboardForm").toggle();
    return false;
}

function Quit() {
    Reset();
    SetCreate();
    return false;
}

$jq("#clock").clock({ "format": "24", "calendar": "false", "seconds": "false" });

$jq('select[name="Assignment"]').focusout(function () {
    var selectedoption = $jq(this).find("option:selected").val();
    if (selectedoption === "user") {
        $jq("#UserAssignmentRow").show();
        $jq("#ServiceAssignmentRow").hide();
    }
    if (selectedoption === "service") {
        $jq("#UserAssignmentRow").hide();
        $jq("#ServiceAssignmentRow").show();
    }
});

$jq('select[name="Assignment"]').on("click", function () {
    var selectedoption = $jq(this).find("option:selected").val();
    if (selectedoption === "user") {
        $jq("#UserAssignmentRow").show();
        $jq("#ServiceAssignmentRow").hide();
    }
    if (selectedoption === "service") {
        $jq("#UserAssignmentRow").hide();
        $jq("#ServiceAssignmentRow").show();
    }
});

$jq('[data-role="CertificateAuthoritySetup"]').on("click", function () {
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/ca/setup",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$jq('[data-role="CreateNewCertificate"]').on("click", function () {
    var name = $jq(this).prev('input[name="CertificateName"]').val();
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/ca/certificate/new",
        type: "POST",
        data: {
            Certificate: name
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$jq('[data-role="InvertSslOption"]').on("click", function () {
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/ca/ssl/toggle",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$jq('[data-role="ChangeCertificatePath"]').on("click", function () {
    var path = $jq(this).prev('input[name="CertificatePath"]').val();
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/ca/cert/set",
        type: "POST",
        data: {
            CertificatePath: path
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$jq("p.edit-conf").on("click", function () {
    var path = $jq(this).attr("data-path");
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/system/read/file/" + path,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (text) {
            var dashboard = $jq("#FileConfigDashboard");
            dashboard.find("#title").text("Editing: " + path);
            dashboard.find('input[name="FilePath"]').val(path);
            dashboard.find('textarea[name="FileContent"]').text(text);
            dashboard.show();
            return false;
        }
    });
    _requests.push(aj);
});

$jq("p.export-conf").on("click", function () {
    var p = $jq(this).attr("data-path");
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/system/export/file/" + p,
        type: "POST",
        data: JSON.stringify(p),
        contentType: "application/json;charset=utf-8",
        success: function () {
            location.reload(true);
            return false;
        }
    });
    _requests.push(aj);
});

$jq("input#close").on("click", function () {
    var form = $jq("form");
    form.find("input").val("");
    form.find("textarea").text("");
    form.hide();
});

$jq("input#clear").on("click", function () {
    var form = $jq("form");
    form.find("input").val("");
    form.find("textarea").text("");
});

$jq('[data-role="ToggleNextRow"]').on("click", function () {
    var id = $jq(this).attr("data-id");
    $jq('[data-hidden="' + id + '"]').toggle();
});

$jq(document).on("ready", function () {
    $jq('div[data-role="hide-selectize"]').find("#show-units").hide();
});

$jq(".search-field").keyup(function () {
    var context = $jq(this).attr("data-searchable");
    var queryString = $jq(this).val();
    var tableBody = $jq('.searchable[data-searchable="' + context + '"]').find("tbody");
    var row = tableBody.children("tr");
    row.each(function () {
        var thisRow = $jq(this);
        var queriedText = $jq(this).text();
        if (queriedText.indexOf(queryString) !== -1) {
            thisRow.show();
        }
        if (queriedText.indexOf(queryString) < 0) {
            thisRow.hide();
        }
    });
});

$jq("#ReloadSystemInfo").on("click", function () {
    jQuery.support.cors = true;
    var aj = $jq.ajax({
        url: "/system/import/info",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});