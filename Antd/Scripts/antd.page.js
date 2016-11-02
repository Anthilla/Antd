$('[data-role="load-page"]').on("click", function () {
    AbortAllAjaxRequests();
    var page = $(this).attr("data-page");
    var ico = $(this).find("i");
    $('[data-icon="load"]').each(function () {
        $(this).hide();
    });
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/load/" + page,
        type: "GET",
        beforeSend: function () {
            ico.show();
        },
        success: function (data) {
            $('[data-role="page-container"]').html(data);
            ico.hide();
            ReloadJS();
        }
    });
    _requests.push(aj);
});

$(document).on("ready", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/storage",
        type: "GET",
        success: function (data) {
            $('[data-render="part-storage"]').html(data);
        }
    });
    _requests.push(aj);
});

$(document).on("ready", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/info/losetup",
        type: "GET",
        success: function (data) {
            $('[data-render="part-info-losetup"]').html(data);
        }
    });
    _requests.push(aj);
});

$(document).on("ready", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/scheduler",
        type: "GET",
        success: function (data) {
            $('[data-render="part-scheduler"]').html(data);
        }
    });
    _requests.push(aj);
});

$(document).on("ready", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/ssh",
        type: "GET",
        success: function (data) {
            $('[data-render="part-ssh"]').html(data);
        }
    });
    _requests.push(aj);
});