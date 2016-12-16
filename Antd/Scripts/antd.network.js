$("#ApplyConfigNetwork").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/network/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="NetworkSaveInterfaceConfiguration"]').on("click", function () {
    var netif = $(this).attr("data-if");
    var mode = $('[data-role="NetworkInterfaceMode"]').find("option:selected").val();
    var status = $('[data-role="NetworkInterfaceStaticStatus"]').find("option:selected").val();
    var staticAddress = $('[data-role="NetworkInterfaceStaticAddress"]').val();
    var staticRange = $('[data-role="NetworkInterfaceStaticRange"]').find("option:selected").val();
    var txqueuelen = $('[data-role="NetworkInterfaceTxqueuelen"]').val();
    var mtu = $('[data-role="NetworkInterfaceMtu"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/network/interface",
        type: "POST",
        data: {
            Interface: netif,
            Mode: mode,
            Status: status,
            StaticAddres: staticAddress,
            StaticRange: staticRange,
            Txqueuelen: txqueuelen,
            Mtu: mtu
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ShowNetworkVirtualIf").on("click", function () {
    $('[data-show="NetworkVirtualIf"]').toggle();
});

$("#ReloadNetworkInterfacesInformation").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/network/import",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});