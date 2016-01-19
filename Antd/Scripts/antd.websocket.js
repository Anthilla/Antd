(function ($) {
    $.fn.CommandTemplate = function () {
        $(this).html('<input type="text" /><input type="submit" value="submit" />');
        $(this).after('<br /><textarea id="CommandResult" style="width: 45%;"></textarea>');
        return this;
    };
}(jQuery));

$('[data-role="command-set"]').CommandTemplate();

$('[data-role="command-set"]').find('input[type="submit"]').click(function () {
    var wsport;
    var wsconnection;
    var parent = $(this).parents('[data-role="command-set"]');
    var command = parent.attr("data-command") + " " + parent.find('input[type="text"]').val();
    jQuery.support.cors = true;
    $.ajax({
        url: "/ws/post",
        type: "POST",
        success: function (port) {
            wsport = port;
            wsconnection = new WebSocket("ws://" + location.host + ":" + wsport + "/cmd");
            wsconnection.onopen = function () {
                wsconnection.send(command);
            };
            wsconnection.onclose = function () {
                console.log("websocket connection @ " + wsport + " closed");
            };
            wsconnection.onerror = function () {
                console.log("websocket connection @ " + wsport + " failed due to an error");
            };
            wsconnection.onmessage = function (response) {
                $("#CommandResult").text(response.data);
            };
        }
    });
});
