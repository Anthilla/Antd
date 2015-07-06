//$('#notify').on('click', function () {
//    var not = $.Notify({
//        caption: 'This is the title',
//        content: 'This is the content',
//        timeout: 10000,
//        style: { background: '#A7BD39', color: 'white' }
//    });
//});
var globalNotificationTimeout = 8000;

function InitializeCheck() {
    $('.metro.notify-container').on('click', '.notify', function () {
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

//$('.notify-info').on('click', function () {
//    var caption = $(this).attr('data-notify-caption');
//    var content = $(this).attr('data-notify-content');
//    var not = $.Notify({
//        caption: caption,
//        content: content,
//        timeout: globalNotificationTimeout,
//        style: { background: '#A7BD39', color: 'white' }
//    });
//    InitializeCheck();
//    CheckNotificationNumber();
//});

//$('input').keyup(function () {
//    var str = $(this).val();
//    var regex = /\W/;
//    //var regex = /^[a-zA-Z0-9- ]*$/;
//    if (regex.test(str) == false) {
//        var not = $.Notify({
//            caption: 'Error',
//            content: 'Your string contains illegal characters!',
//            timeout: globalNotificationTimeout,
//            style: { background: '#F06305', color: 'white' }
//        });
//        InitializeCheck();
//        CheckNotificationNumber();
//    }
//});

$('input[data-role="notify-howto-import"]').click(function (event) {
    event.preventDefault();
    $.Notify({
        caption: 'Import command and result: 1',
        content: 'Open the console-terminal',
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Import command and result: 2',
        content: 'You can launch and test any command you want',
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Import command and result: 3',
        content: "As soon as you've found the right command you can export it by appending '>>' at the end of the line",
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Import command and result: 4',
        content: "Then you can import both the command and its result in this form by pressing 'Import Data'",
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Import command and result: 5',
        content: "Done! Later you will be able to manipulate the result and map it as you want",
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    InitializeCheck();
    CheckNotificationNumber();
});

$('input[data-role="notify-howto-map"]').click(function (event) {
    event.preventDefault();
    $.Notify({
        caption: 'Map the result: 1',
        content: 'Open the console-terminal',
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Map the result: 2',
        content: 'You can launch and test any command you want',
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Map the result: 3',
        content: "As soon as you've found the right command you can export it by appending '>>' at the end of the line",
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Map the result: 4',
        content: "Then you can import both the command and its result in this form by pressing 'Import Data'",
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    $.Notify({
        caption: 'Map the result: 5',
        content: "Done! Later you will be able to manipulate the result and map it as you want",
        timeout: globalNotificationTimeout,
        style: { background: '#4390DF', color: 'white' }
    });
    InitializeCheck();
    CheckNotificationNumber();
});