$("#ShowNetworkVirtualIf").on("click", function () {
    $('[data-show="NetworkVirtualIf"]').toggle();
});

$("#ReloadNetworkInterfacesInformation").on("click", function () {
    $(this).addClass();
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