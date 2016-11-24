var $asset = jQuery.noConflict();

$asset('[data-role="ShareSshKeys"]').click(function () {
    var ip = $asset(this).attr('data-ip');
    var port = $asset(this).attr('data-port');
    jQuery.support.cors = true;
    var aj = $asset.ajax({
        url: "/asset/handshake?host=" + ip + "&port=" + port,
        type: "GET",
        success: function () {
            alert("Key shared successfully!");
        }
    });
    _requests.push(aj);
});

$asset('[data-role="ShowNmap"]').click(function () {
    var btn = $asset(this);
    var ip = $asset(this).attr('data-ip');
    var cont = $asset(this).parents('.panel-content').find('[data-role="NmapResult"]');
    jQuery.support.cors = true;
    var aj = $asset.ajax({
        url: "/asset/nmap/" + ip,
        type: "GET",
        success: function (data) {
            $asset.each(data, function (k, v) {
                cont.append("<tr>" + "<td class=\"bg-gray\">" + v.protocol + "</td>" +
                    "<td class=\"bg-gray\">" + v.status + "</td>" +
                    "<td class=\"bg-gray\">" + v.type + "</td>" + "</tr>");
            });
            cont.parents('table').show();
            btn.hide();
        }
    });
    _requests.push(aj);
});

$asset('[data-role="Wol"]').click(function () {
    var mac = $asset(this).attr('data-mac');
    jQuery.support.cors = true;
    var aj = $asset.ajax({
        url: "/asset/wol/" + mac,
        type: "GET",
        success: function () {
        }
    });
    _requests.push(aj);
});

function SetConnectionLink() {
    var host = $asset('#NoVncHost').val();
    var port = $asset('#NoVncPort').val();
    $asset("#NoVncConnect").attr("href", "?host=" + host + "&port=" + port);
}

$asset('input[data-role="novnc-setup"]').keyup(function () {
    SetConnectionLink();
});