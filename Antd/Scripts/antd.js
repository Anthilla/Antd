//users
function SetCreateUser() {
    $("#UserCreateDashboard").toggle();
}

$('select[name="UserType"]').windowed({
    change: function (event, selected) {
        selected = $(selected);
        if (selected.val() === "app") {
            $("#AppUsersDBoard").hide();
            $("#SysUsersDBoard").hide();
            $("#AppUsersDBoard").show();
        }
        if (selected.val() === "sys") {
            $("#AppUsersDBoard").hide();
            $("#SysUsersDBoard").hide();
            $("#SysUsersDBoard").show();
        }
    }
});

//volumes

$('[id="ReloadVolumes"]').on("mouseover", function () {
    //$('i[id="ReloadVolumes"]').mouseover(function () {
    $(this).addClass("fg-anthilla-green");
});

$('[id="ReloadVolumes"').on("mouseout", function () {
    //$('i[id="ReloadVolumes"]').mouseout(function () {
    $(this).removeClass("fg-anthilla-green");
});

$('[id="ReloadVolumes"').on("click", function () {
    //$('i[id="ReloadVolumes"]').click(function (event) {
    event.preventDefault();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/storage/reload/volumes/",
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function () {
            location.reload(true);
            return false;
        }
    });
    _requests.push(aj);
});

$('[data-role="show-mount"').on("click", function () {
    //$('i[data-role="show-Mount"]').click(function () {
    var self = $(this);
    var i = '<input type="text" data-role="value-Mount" data-folder="' + self.attr("data-folder") + '"/>' +
        '<i class="icon-plus fg-green" data-role="submit-Mount" data-folder="' + self.attr("data-folder") + '"></i>';
    self.after(i);
    InitSubmitMount();
});

function InitSubmitMount() {
    $('[data-role="submit-mount"').on("click", function () {
        //$('i[data-role="submit-Mount"]').click(function () {
        var g = $(this).attr("data-folder");
        var m = $('input[data-folder="' + g + '"]').val();
        alert(g);
        alert(m);
        jQuery.support.cors = true;
        var aj = $.ajax({
            url: "/apps/Mount",
            type: "POST",
            data: {
                Folder: g,
                Mount: m
            },
            success: function () {
                location.reload(true);
            }
        });
        _requests.push(aj);
    });
}
///////////cookiecookiecookie////////////////////////
$("#LockInput").on("click", function () {
    //$("#LockInput").click(function () {
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
    //$(document).ready(function () {
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
    //$("a.anchor").click(function (event) {
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
    //$(document).ready(function () {
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