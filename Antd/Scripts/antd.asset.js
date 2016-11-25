var $asset = jQuery.noConflict();

$asset('[data-role="ApplyNetscanSettingSubnet"]').on("click", function () {
    var Subnet = $asset('[data-role="NetscanSettingSubnet"]').val();
    jQuery.support.cors = true;
    var aj = $asset.ajax({
        url: "/netscan/setsubnet",
        type: "POST",
        data: {
            Subnet: Subnet
        },
        success: function () {
            alert('Subnet Saved');
        }
    });
    _requests.push(aj);
});

$asset('[data-role="SaveNetscanSetting"]').on("click", function () {
    $asset('[data-role="NetscanSettingTable"]').find('[data-name="NetscanLabel"]').each(function () {
        var self = $asset(this);
        var label = self.val();
        var letter = self.attr("data-l");
        var number = self.attr("data-nl");
        jQuery.support.cors = true;
        var aj = $asset.ajax({
            url: "/netscan/setlabel",
            type: "POST",
            data: {
                Letter: letter,
                Number: number,
                Label: label
            },
            success: function () {
                alert('Settings Saved');
            }
        });
        _requests.push(aj);
    });
});

$asset('[data-role="ToggleAssetContent"]').on("click", function () {
    var self = $asset(this);
    self.toggleClass("icon-arrow-right-5");
    self.toggleClass("icon-arrow-down-5");
    var cont = self.parents("div.container").find("div.content");
    cont.toggle();
});

$asset('[data-role="ShareSshKeys"]').on("click", function () {
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

$asset('[data-role="ShowNmap"]').on("click", function () {
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

$asset('[data-role="Wol"]').on("click", function () {
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

$asset('input[data-role="novnc-setup"]').on("keyup", function () {
    SetConnectionLink();
});