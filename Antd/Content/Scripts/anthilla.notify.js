//$('#notify').on('click', function () {
//    var not = $.Notify({
//        caption: 'This is the title',
//        content: 'This is the content',
//        timeout: 10000,
//        style: { background: '#A7BD39', color: 'white' }
//    });
//});
var globalNotificationTimeout = 20000;

function InitializeCheck() {
    $('.metro.notify-container').on('click', '.notify', function () {
        console.log('nyet');
        $(this).remove();
    });
}

function CheckNotificationNumber() {
    var n = $('.notify').length;
    if (n > 5) {
        HowToHideNotification();
    }
}

function HowToHideNotification() {
    var not = $.Notify({
        caption: 'Info',
        content: 'Click on a notification to hide',
        timeout: globalNotificationTimeout,
        style: { background: '#60A1E4', color: 'white' }
    });
}

$('.notify-info').on('click', function () {
    var caption = $(this).attr('data-notify-caption');
    var content = $(this).attr('data-notify-content');
    var not = $.Notify({
        caption: caption,
        content: content,
        timeout: globalNotificationTimeout,
        style: { background: '#A7BD39', color: 'white' }
    });
    InitializeCheck();
    CheckNotificationNumber();
});

$('input').keyup(function () {
    var str = $(this).val();
    var regex = /\W/;
    //var regex = /^[a-zA-Z0-9- ]*$/;
    if (regex.test(str) == false) {
        var not = $.Notify({
            caption: 'Error',
            content: 'Your string contains illegal characters!',
            timeout: globalNotificationTimeout,
            style: { background: '#F06305', color: 'white' }
        });
        InitializeCheck();
        CheckNotificationNumber();
    }
});