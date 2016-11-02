$("i.show-units-mgmt").on("click", function () {
    var g = $(this).attr("data-guid");
    $('tr.Mount-units[data-guid="' + g + '"]').toggle();
});

$('input[data-role="show-dashboard"]').on("click", function () {
    var g = $(this).attr("data-guid");
    $("#AntdMountdDashboard").toggle();
    $("#AntdMountdDashboard").find('input[name="Guid"]').val(g);
    $("#AntdMountdDashboard").find('input[name="Mount"]').val($(this).attr("data-mntpth"));
});

$('i[data-role="remove-this-unit"]').on("click", function () {
    var g = $(this).attr("data-guid");
    var u = $(this).attr("data-unit");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/system/Mount/unit",
        type: "DELETE",
        data: {
            Guid: g,
            Unit: u
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="CreatePartitionTable"]').on("click", function () {
    var d = $(this).attr("data-name");
    var t = $(this).parents("td").find("select > option:selected").val();
    var aj = $.ajax({
        url: "/parted/mklabel",
        type: "POST",
        data: {
            Disk: d,
            Type: t,
            Confirm: "Yes"
        },
        success: function () {
            var aj = $.ajax({
                url: "/parted/print",
                type: "POST",
                data: {
                    Disk: d
                },
                success: function (data) {
                    $('[data-partitiontable="' + g + '"]').text(data);
                }
            });
            _requests.push(aj);
        }
    });
    _requests.push(aj);
});

$('[data-role="show-disk-details-0"]').on("click", function () {
    var g = $(this).attr("data-guid");
    $('[data-row="' + g + '"]').toggle();
    var d = $(this).attr("data-name");
    var aj = $.ajax({
        url: "/parted/print",
        type: "POST",
        data: {
            Disk: d
        },
        success: function (data) {
            $('[data-partitiontable="' + g + '"]').text("Partition Table: " + data);
            if (data !== "unknown") {
                $('[data-button="' + g + '"]').addClass("disabled").attr("disabled", "disabled");
            }
        }
    });
    _requests.push(aj);
});

$('[data-role="show-details"]').on("click", function () {
    var g = $(this).attr("data-guid");
    $('[data-row="' + g + '"]').toggle();
});

$('[data-role="EnablePoolSnap"]').on("click", function () {
    var pool = $(this).attr("data-pool");
    var interval = $(this).parents("td").find("select > option:selected").val();
    var aj = $.ajax({
        url: "/zfs/snap",
        type: "POST",
        data: {
            Pool: pool,
            Interval: interval
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="DisablePoolSnap"]').on("click", function () {
    var id = $(this).attr("data-id");
    var aj = $.ajax({
        url: "/zfs/snap/disable",
        type: "POST",
        data: {
            Guid: id
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});