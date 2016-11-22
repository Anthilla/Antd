var $net = jQuery.noConflict();

$net("#ShowNetworkVirtualIf").on("click", function () {
    $net('[data-show="NetworkVirtualIf"]').toggle();
});

$net("#ReloadNetworkInterfacesInformation").on("click", function () {
    $net(this).addClass();
    jQuery.support.cors = true;
    var aj = $net.ajax({
        url: "/network/import",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});