$('[data-role="ApplyNetscanSettingSubnet"]').on("click", function () {
    var subnet = $('[data-role="NetscanSettingSubnet"]').val();
    var label = $('[data-role="NetscanSettingSubnetLabel"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/netscan/setsubnet",
        type: "POST",
        data: {
            Subnet: subnet,
            Label: label
        },
        success: function () {
            alert("Subnet Saved");
        }
    });
    _requests.push(aj);
});

$('[data-role="SaveNetscanSetting"]').on("click", function () {
    $('[data-role="NetscanSettingTable"]').find('[data-name="NetscanLabel"]').each(function () {
        var self = $(this);
        var label = self.val();
        var letter = self.attr("data-l");
        var number = self.attr("data-n");
        jQuery.support.cors = true;
        var aj = $.ajax({
            url: "/netscan/setlabel",
            type: "POST",
            data: {
                Letter: letter,
                Number: number,
                Label: label
            },
            success: function () {
                alert("Settings Saved");
            }
        });
        _requests.push(aj);
    });
});

$('[data-role="ToggleAssetContent"]').on("click", function () {
    var self = $(this);
    self.toggleClass("icon-arrow-right-5");
    self.toggleClass("icon-arrow-down-5");
    var cont = self.parents("div.container").find("div.content");
    cont.toggle();
});

$('[data-role="ShareSshKeys"]').on("click", function () {
    var ip = $(this).attr("data-ip");
    var port = $(this).attr("data-port");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/asset/handshake?host=" + ip + "&port=" + port,
        type: "GET",
        success: function () {
            alert("Key shared successfully!");
        }
    });
    _requests.push(aj);
});

$('[data-role="ShowNmap"]').on("click", function () {
    var ip = $(this).attr("data-ip");
    var ttt = $(this).parents("div.container");
    var cont = ttt.find('[data-role="NmapResult"]');
    ttt.find("div.content").show();
    var arr = ttt.find('[data-role="ToggleAssetContent"]');
    arr.removeClass("icon-arrow-right-5");
    arr.addClass("icon-arrow-down-5");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/asset/nmap/" + ip,
        type: "GET",
        success: function (data) {
            cont.html("");
            $.each(data, function (k, v) {
                cont.append("<tr>" + "<td class=\"bg-gray\">" + v.protocol + "</td>" +
                    "<td class=\"bg-gray\">" + v.status + "</td>" +
                    "<td class=\"bg-gray\">" + v.type + "</td>" + "</tr>");
            });
            cont.parents("table").show();
        }
    });
    _requests.push(aj);
});

$('[data-role="Wol"]').on("click", function () {
    var mac = $(this).attr("data-mac");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/asset/wol/" + mac,
        type: "GET",
        success: function () {
        }
    });
    _requests.push(aj);
});

function SetConnectionLink() {
    var host = $("#NoVncHost").val();
    var port = $("#NoVncPort").val();
    $("#NoVncConnect").attr("href", "?host=" + host + "&port=" + port);
}

$('input[data-role="novnc-setup"]').on("keyup", function () {
    SetConnectionLink();
});