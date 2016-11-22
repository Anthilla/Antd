var $ws = jQuery.noConflict();

(function ($ws) {
    $ws.fn.CommandTemplate = function () {
        var options = $ws(this).attr("data-options");
        if (str.toLowerCase().indexOf("button-only") >= 0) {
            var myRegexp = /button-only:([a-zAA-Z 0-9]*);/g;
            var match = myRegexp.exec(options);
            var buttonValue = match[1];
            $ws(this).html('<input type="submit" value="' + buttonValue + '" />');
        }
        else {
            $ws(this).html('<input type="text" /><input type="submit" value="submit" />');
            $ws(this).after('<br /><textarea id="CommandResult" style="width: 45%;"></textarea>');
        }
        return this;
    };
}(jQuery));

$ws('[data-role="command-set"]').CommandTemplate();

$ws('[data-role="command-set"]').find('input[type="submit"]').on("click", function () {
    var wsport;
    var wsconnection;
    var parent = $ws(this).parents('[data-role="command-set"]');
    var command = parent.attr("data-command") + " " + parent.find('input[type="text"]').val();
    jQuery.support.cors = true;
    var aj = $ws.ajax({
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
                $ws("#CommandResult").text(response.data);
            };
        }
    });
    _requests.push(aj);
});
