$('[data-role="AssetScanStart"]').on("click", function () {
    var container = $("#AssetScanResult");
    var subnet = $('[data-role="NetscanSettingSelect"]').find("option:selected").val();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/asset/scan/" + subnet,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        data: {
            Subnet: subnet
        },
        beforeSend: function() {
            $('[data-role="AssetScanIco"]').show();
        },
        success: function (data) {
            container.html("");
            $.each(data, function (k, value) {
                var html =
'<div class="container"> ' +
'<div class="bg-anthilla-gray" style="width: 100%; height: 38px; margin-bottom: 5px; padding: 0 12px; font-weight: normal !important; line-height: 40px;">' +
'<i class="icon-monitor on-left fg-anthilla-violet" style="margin-right: 8px;"></i>' +
'<p style="display: inline-block !important; width: 200px;">' + value + "</p>" +
'<p style="display: inline-block !important; width: 200px;"></p>' +
'<p style="display: inline-block !important; width: 500px;"></p>' +
'<i data-role="ToggleAssetContent" class="icon-arrow-right-5 fg-white" style="margin-left: 8px; float: right; cursor: pointer; border: 2px solid white; padding: 5px; margin-top: 5px;"></i>' +
'<i data-hint="|Open VNC" data-hint-position="bottom" id="NoVncConnect" class="fg-orange icon-monitor-2" style="margin-left: -2px; float: right; cursor: pointer; border: 2px solid #FA6800; padding: 5px; margin-top: 5px;"></i>' +
'<input id="NoVncPort" data-role="novnc-setup" type="text" style="float: right; width: 75px; margin-left: 8px; margin-top: 5px !important; border: 2px solid #FA6800 !important; height: 27px; text-align: center;" value="5900" />' +
'<input id="NoVncHost" type="hidden" data-role="novnc-setup" value="" />' +
'<i data-hint="|Scan ports" data-hint-position="bottom" data-role="ShowNmap" data-ip="" class="icon-search fg-anthilla-blu" style="margin-left: 8px; float: right; cursor: pointer; border: 2px solid #60A1E4; padding: 5px; margin-top: 5px;"></i>' +
'<i data-hint="|Wake On Lan" data-hint-position="bottom" data-role="Wol" data-mac="@Current.MacAddress" class="icon-lightning fg-yellow" style="margin-left: 8px; float: right; cursor: pointer; border: 2px solid #E3C800; padding: 5px; margin-top: 5px;"></i>' +
'<i data-hint="|Share SSH key" data-hint-position="bottom" data-role="ShareSshKeys" data-ip="@Current.Ip" data-port="@Current.Port" class="icon-key-2 fg-anthilla-green" style="margin-left: 8px; float: right; cursor: pointer; border: 2px solid #A7BD39; padding: 5px; margin-top: 5px;"></i>' +
'<a data-hint="|Open Antd" data-hint-position="bottom"href="http://@Current.Ip:@Current.Port/" target="_blank" class="button bg-anthilla-gray border-2-anthilla-violet" style="width: 27px; height: 27px; padding: 4px 6px; float: right; margin-top: 5px; margin-left: 5px;"><i class="icon-exit fg-anthilla-violet"></i></a></div>' +
'<div class="bg-anthilla-gray-m border-2-anthilla-gray content" style="display: none; width: 100%; height: auto; margin-bottom: 5px; margin-top: -5px; padding: 10px 12px; font-weight: normal !important;">' +
'<table class="table" style="margin-top: -18px;"><thead><tr><td><label>Port/Protocol</label></td><td><label>Status</label></td>' +
'<td><label>Type</label></td></tr></thead><tbody data-role="NmapResult"></tbody></table></div></div>';
                container.append(html);
            });
            $('[data-role="AssetScanIco"]').hide();
        }
    });
    _requests.push(aj);
});

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
        if (label.length > 0) {
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
                    console.log("setting saved");
                }
            });
            _requests.push(aj);
        }
    });
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

function SetConnectionLink() {
    var host = $("#NoVncHost").val();
    var port = $("#NoVncPort").val();
    $("#NoVncConnect").attr("href", "?host=" + host + "&port=" + port);
}