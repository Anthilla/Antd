$(document).ready(function () {
    $('[data="CmdGet"]').each(function () {
        var container = $(this).parents('[data-role="CmdContainer"]');
        LaunchCommand(container);
    });
});

$('[data-role="CmdLaunch"]').click(function () {
    var container = $(this).parents('[data-role="CmdContainer"]');
    LaunchCommand(container);
});

function LaunchCommand(container) {
    var button = container.find('[data-role="CmdLaunch"]');
    var command = button.attr("data-name");
    var values = "";
    var par = button.attr("data-par");
    var res = par.split(",");
    $.each(res, function (i, v) {
        var app = v + ":";
        var val = container.find('[data="' + v + '"]').val();
        app += val;
        app += ";";
        if (val.length > 1) {
            values += app;
        }
    });
    jQuery.support.cors = true;
    $.ajax({
        url: "/cmd/launch",
        type: "POST",
        data: {
            Command: command,
            Matches: values
        },
        success: function (data) {
            container.find('[data-role="CmdResult"]').val(data);
        }
    });
}