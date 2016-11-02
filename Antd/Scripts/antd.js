$("#LockInput").on("click", function () {
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

$(document).on("ready", function () {
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
    var icon = $("#LockInput").find("i");
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
    $("input").each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).prop("disabled", false);
    });
    $(".button").each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).removeClass("disabled");
    });
    cookie.set("_input", "enabled", { expires: 7 });
}

function DisableInputs() {
    $("input").each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).prop("disabled", true);
    });
    $(".button").each(function () {
        $(this).fadeOut(300).fadeIn(150);
        $(this).delay(455).addClass("disabled");
    });
    cookie.set("_input", "disabled", { expires: 7 });
}

$("a.anchor").on("click", function () {
    event.preventDefault();
    var href = $(this).attr("data-scrollto");
    if (href === "top") {
        $("html, body").animate({
            'scrollTop': 0
        }, 500);
        return false;
    }
    else {
        var scroll = $(href).offset().top - 60;
        $("html, body").animate({
            'scrollTop': scroll
        }, 500);
        return false;
    }
});

$(window).scroll(function () {
    if ($(window).scrollTop() > 150) {
        $("nav.navigation-bar.page-bar").css("position", "fixed");
        $("nav.navigation-bar.page-bar").css("z-index", "999");
        $("nav.navigation-bar.page-bar").css("top", "0");
        $("nav.navigation-bar.page-bar").css("left", "0");
        $("nav.navigation-bar.page-bar").css("padding-left", "15px");
    }
    if ($(window).scrollTop() < 150) {
        $("nav.navigation-bar.page-bar").css("position", "relative");
        $("nav.navigation-bar.page-bar").css("padding-left", "0");
    }
});

$(document).on("ready", function () {
    $("input:password").val("");
    $("input:text").attr("autocomplete", "off");
    $("input:password").attr("autocomplete", "off");
});

function Reset() {
    $(".item").hide();
    $("select").prop("selectedIndex", 0);
    $(".project-selectable").removeClass("picked");
    $(".group-selectable").removeClass("picked");
    $(".js-files").hide();
    $("input:text").each(function () {
        $(this).val("");
    });
    $(".file").remove();
    return false;
}

function SetCreate() {
    var button = $("#create-button");
    if (button != null) {
        button.toggleClass("fg-anthilla-green");
        button.toggleClass("no-overlay");
        button.toggleClass("fg-anthilla-gray");
        button.toggleClass("bg-anthilla-green");
    }
    $("#DashboardForm").toggle();
    return false;
}

function Quit() {
    Reset();
    SetCreate();
    return false;
}

$("#clock").clock({ "format": "24", "calendar": "false", "seconds": "false" });

$('select[name="Assignment"]').focusout(function () {
    var selectedoption = $(this).find("option:selected").val();
    if (selectedoption === "user") {
        $("#UserAssignmentRow").show();
        $("#ServiceAssignmentRow").hide();
    }
    if (selectedoption === "service") {
        $("#UserAssignmentRow").hide();
        $("#ServiceAssignmentRow").show();
    }
});

$('select[name="Assignment"]').on("click", function () {
    var selectedoption = $(this).find("option:selected").val();
    if (selectedoption === "user") {
        $("#UserAssignmentRow").show();
        $("#ServiceAssignmentRow").hide();
    }
    if (selectedoption === "service") {
        $("#UserAssignmentRow").hide();
        $("#ServiceAssignmentRow").show();
    }
});

$('[data-role="CertificateAuthoritySetup"]').on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/ca/setup",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="CreateNewCertificate"]').on("click", function () {
    var name = $(this).prev('input[name="CertificateName"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$('[data-role="InvertSslOption"]').on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/ca/ssl/toggle",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="ChangeCertificatePath"]').on("click", function () {
    var path = $(this).prev('input[name="CertificatePath"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$("p.edit-conf").on("click", function () {
    var path = $(this).attr("data-path");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/system/read/file/" + path,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (text) {
            var dashboard = $("#FileConfigDashboard");
            dashboard.find("#title").text("Editing: " + path);
            dashboard.find('input[name="FilePath"]').val(path);
            dashboard.find('textarea[name="FileContent"]').text(text);
            dashboard.show();
            return false;
        }
    });
    _requests.push(aj);
});

$("p.export-conf").on("click", function () {
    var p = $(this).attr("data-path");
    jQuery.support.cors = true;
    var aj = $.ajax({
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

$("input#close").on("click", function () {
    var form = $("form");
    form.find("input").val("");
    form.find("textarea").text("");
    form.hide();
});

$("input#clear").on("click", function () {
    var form = $("form");
    form.find("input").val("");
    form.find("textarea").text("");
});

$('[data-role="ToggleNextRow"]').on("click", function () {
    var id = $(this).attr("data-id");
    $('[data-hidden="' + id + '"]').toggle();
});

$(document).on("ready", function () {
    $('div[data-role="hide-selectize"]').find("#show-units").hide();
});

$(".search-field").keyup(function () {
    var context = $(this).attr("data-searchable");
    var queryString = $(this).val();
    var tableBody = $('.searchable[data-searchable="' + context + '"]').find("tbody");
    var row = tableBody.children("tr");
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

$("#ReloadSystemInfo").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/system/import/info",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});