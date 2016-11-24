var $asset = jQuery.noConflict();

$asset('[data-role="ToggleAssetContent"]').click(function () {
    console.log(0);
    var self = $asset(this);
    self.toggleClass("icon-arrow-right-5");
    self.toggleClass("icon-arrow-down-5");
    var cont = self.parents("div.container").find("div.content");
    cont.toggle();
});

$asset('[data-role="ShareSshKeys"]').click(function () {
    var ip = $asset(this).attr("data-ip");
    var port = $asset(this).attr("data-port");
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
    console.log(1);
    var btn = $asset(this);
    var ip = $asset(this).attr("data-ip");
    var ttt = $asset(this).parents("div.container");
    var cont = ttt.find('[data-role="NmapResult"]');
    ttt.find("div.content").show();
    var arr = ttt.find('[data-role="ToggleAssetContent"]');
    arr.removeClass("icon-arrow-right-5");
    arr.addClass("icon-arrow-down-5");
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
            cont.parents("table").show();
            btn.hide();
        }
    });
    _requests.push(aj);
});

$asset('[data-role="Wol"]').click(function () {
    var mac = $asset(this).attr("data-mac");
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
    var host = $asset("#NoVncHost").val();
    var port = $asset("#NoVncPort").val();
    $asset("#NoVncConnect").attr("href", "?host=" + host + "&port=" + port);
}

$asset('input[data-role="novnc-setup"]').keyup(function () {
    SetConnectionLink();
});