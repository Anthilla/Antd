(function ($) {
    $.fn.CommandTemplate = function () {
        $(this).html('<input type="text" /><input type="submit" value="submit" />');
        $(this).after('<br /><textarea id="CommandResult" style="width: 45%;"></textarea>');
        return this;
    };
}(jQuery));

$('[data-role="command-set"]').CommandTemplate();

var WSCONNECTION;
function SetWebSocketConnection(port) {
    WSCONNECTION = new WebSocket("ws://" + location.host + ":" + port + "/cmd");
    WSCONNECTION.onopen = function () {
        console.log("websocket: connected on " + port);
    };
    WSCONNECTION.onclose = function () {
        console.log("websocket: disconnected");
    };
    WSCONNECTION.onerror = function () {
        console.log("websocket: error");
    };
    WSCONNECTION.onmessage = function (response) {
        console.log('onmessage');
        $("#CommandResult").text(response.data);
    };
    return false;
}

function CreateWebSocket(command) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/ws/post',
        type: 'POST',
        data: {
            Command: command
        },
        success: function (data) {
            console.log('connection /ws/post success');
            SetWebSocketConnection(data);
        }
    });
}

$('[data-role="command-set"]').find('input[type="submit"]').click(function () {
    var parent = $(this).parents('[data-role="command-set"]');
    var command = parent.attr("data-command") + " " + parent.find('input[type="text"]').val();
    CreateWebSocket(command);
});
