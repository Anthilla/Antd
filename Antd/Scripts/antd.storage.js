var $storage = jQuery.noConflict();

$storage("i.show-units-mgmt").on("click", function () {
    var g = $storage(this).attr("data-guid");
    $storage('tr.Mount-units[data-guid="' + g + '"]').toggle();
});

$storage('input[data-role="show-dashboard"]').on("click", function () {
    var g = $storage(this).attr("data-guid");
    $storage("#AntdMountdDashboard").toggle();
    $storage("#AntdMountdDashboard").find('input[name="Guid"]').val(g);
    $storage("#AntdMountdDashboard").find('input[name="Mount"]').val($storage(this).attr("data-mntpth"));
});

$storage('i[data-role="remove-this-unit"]').on("click", function () {
    var g = $storage(this).attr("data-guid");
    var u = $storage(this).attr("data-unit");
    jQuery.support.cors = true;
    var aj = $storage.ajax({
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

$storage('[data-role="CreatePartitionTable"]').on("click", function () {
    var d = $storage(this).attr("data-name");
    var t = $storage(this).parents("td").find("select > option:selected").val();
    var aj = $storage.ajax({
        url: "/parted/mklabel",
        type: "POST",
        data: {
            Disk: d,
            Type: t,
            Confirm: "Yes"
        },
        success: function () {
            var aj = $storage.ajax({
                url: "/parted/print",
                type: "POST",
                data: {
                    Disk: d
                },
                success: function (data) {
                    $storage('[data-partitiontable="' + g + '"]').text(data);
                }
            });
            _requests.push(aj);
        }
    });
    _requests.push(aj);
});

$storage('[data-role="show-disk-details-0"]').on("click", function () {
    var g = $storage(this).attr("data-guid");
    $storage('[data-row="' + g + '"]').toggle();
    var d = $storage(this).attr("data-name");
    var aj = $storage.ajax({
        url: "/parted/print",
        type: "POST",
        data: {
            Disk: d
        },
        success: function (data) {
            $storage('[data-partitiontable="' + g + '"]').text("Partition Table: " + data);
            if (data !== "unknown") {
                $storage('[data-button="' + g + '"]').addClass("disabled").attr("disabled", "disabled");
            }
        }
    });
    _requests.push(aj);
});

$storage('[data-role="show-details"]').on("click", function () {
    var g = $storage(this).attr("data-guid");
    $storage('[data-row="' + g + '"]').toggle();
});

$storage('[data-role="EnablePoolSnap"]').on("click", function () {
    var pool = $storage(this).attr("data-pool");
    var interval = $storage(this).parents("td").find("select > option:selected").val();
    var aj = $storage.ajax({
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

$storage('[data-role="DisablePoolSnap"]').on("click", function () {
    var id = $storage(this).attr("data-id");
    var aj = $storage.ajax({
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