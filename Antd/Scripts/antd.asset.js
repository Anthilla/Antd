var $asset = jQuery.noConflict();

function RemoveNetscanSettingObject() {
    $asset('[data-role="RemoveNetscanSettingObject"]').on("click", function () {
        var cont = $asset('[data-role="NetscanSettingTable"]');
        var id = cont.find('[data-name="id"]').val();
        jQuery.support.cors = true;
        var aj = $asset.ajax({
            url: "/netscan/remove",
            type: "POST",
            data: {
                Id: id
            },
            success: function () {
                cont.remove();
            }
        });
        _requests.push(aj);
    });
}

$asset('[data-role="SaveNetscanSetting"]').on("click", function () {
    $asset('[data-role="NetscanSettingTable"]').find('[data-role="NetscanSettingObject"]').each(function () {
        var self = $asset(this);
        var id = self.find('[data-name="id"]').val();
        var start = self.find('[data-name="start"]').val();
        var end = self.find('[data-name="end"]').val();
        var lbl = self.find('[data-name="label"]').val();
        jQuery.support.cors = true;
        var aj = $asset.ajax({
            url: "/netscan/add",
            type: "POST",
            data: {
                Id: id,
                Start: start,
                End: end,
                Label: lbl
            },
            success: function () {
            }
        });
        _requests.push(aj);
    });
});

$asset('[data-role="AddNetscanSettingObject"]').on("click", function () {
    var cont = $asset('[data-role="NetscanSettingTable"]');
    var html = "<tr data-role=\"NetscanSettingObject\"> <td> <input type=\"text\" value=\"\" data-name=\"id\" /></td> <td> <input type=\"text\" value=\"\" data-name=\"start\" /></td> <td> <input type=\"text\" value=\"\" data-name=\"end\" /></td> <td> <input type=\"text\" value=\"\" data-name=\"label\" /></td> <td> <input data-role=\"RemoveNetscanSettingObject\" value=\"X\" type=\"button\" /> </td></tr>";
    cont.append(html);
    RemoveNetscanSettingObject();
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