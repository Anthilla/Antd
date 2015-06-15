$('p[id^=Update]').click(function () {
    var self = $(this);
    var id = self.attr('id');
    var inputID = id.replace('Update', 'New');
    var newValue = $('#' + inputID).val();
    if (newValue.length > 0) {
        jQuery.support.cors = true;
        $.ajax({
            url: '/command/mgmt/ex/' + inputID + '/' + newValue,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (data) {
                console.log(data);
                return false;
            }
        });
    }
});

$('a.anchor').click(function (event) {
    event.preventDefault();
    var href = $(this).attr('data-scrollto');
    var scroll = $(href).offset().top - 40;
    $('html, body').animate({
        'scrollTop': scroll
    }, 500);
    return false;
});

$(window).scroll(function () {
    if ($(window).scrollTop() > 150) {
        $('nav.navigation-bar.page-bar').css('position', 'fixed');
        $('nav.navigation-bar.page-bar').css('z-index', '999');
        $('nav.navigation-bar.page-bar').css('top', '0');
        $('nav.navigation-bar.page-bar').css('left', '0');
        $('nav.navigation-bar.page-bar').css('padding-left', '15px');
    }
    if ($(window).scrollTop() < 150) {
        $('nav.navigation-bar.page-bar').css('position', 'relative');
        $('nav.navigation-bar.page-bar').css('padding-left', '0');
    }
});

$(document).ready(function () {
    $('input:password').val('');
    $('input:text').attr('autocomplete', 'off');
    $('input:password').attr('autocomplete', 'off');
});

function Reset() {
    $('.item').hide();
    $('select').prop('selectedIndex', 0);
    $('.project-selectable').removeClass('picked');
    $('.group-selectable').removeClass('picked');
    $('.js-files').hide();
    $("input:text").each(function () {
        $(this).val("");
    });
    $('.file').remove();
    return false;
}

function SetCreate() {
    var button = $('#create-button');
    if (button != null) {
        button.toggleClass('fg-anthilla-green');
        button.toggleClass('no-overlay');
        button.toggleClass('fg-anthilla-gray');
        button.toggleClass('bg-anthilla-green');
    }
    $('#DashboardForm').toggle();
    return false;
}

function Quit() {
    Reset();
    SetCreate();
    return false;
}