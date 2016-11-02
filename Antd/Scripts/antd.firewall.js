$('[data-role="SaveNftConfiguration"]').on("click", function () {
    var tables = [];
    $('[data-nft="Table"]').each(function () {
        var self = $(this);
        var tableType = self.attr("data-nft-type");
        var tableName = self.attr("data-nft-name");

        var tableSets = [];
        self.find('[data-nft="Set"]').each(function () {
            var selfSet = $(this);
            var setName = selfSet.attr("data-nft-name");
            var elements = self.find('[data-nft="SetElements"]').text();
            var set = {
                Name: setName,
                Elements: elements
            };
            tableSets.push(set);
        });

        var tableChains = [];
        self.find('[data-nft="Set"]').each(function () {
            var selfChain = $(this);
            var chainName = selfChain.attr("data-nft-name");
            var rules = self.find('[data-nft="ChainRules"]').text();
            var chain = {
                Name: chainName,
                RulesString: rules,
                Rules: null
            };
            tableChains.push(chain);
        });

        var table = {
            Type: tableType,
            Name: tableName,
            Sets: tableSets,
            Chains: tableChains
        };
        tables.push(table);
    });
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/nft/save",
        type: "POST",
        dataType: "json",
        data: {
            Data: JSON.stringify(tables)
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$(document).on("ready", function () {
    $(".autoheight").each(function () {
        this.style.height = "1px";

        var text = $(this).val();
        var lines = text.split(/\r|\r\n|\n/);
        var count = lines.length;
        console.log(count);

        this.style.height = (count * 20) + "px";
    });
});

$('input[data-role="enable-mac-address"]').on("click", function () {
    var guid = $(this).attr("data-object-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/enable/macadd",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('input[data-role="disable-mac-address"]').on("click", function () {
    var guid = $(this).attr("data-object-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/disable/macadd",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadMacAddressList").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/discover/macadd",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('div[data-object="ruleset"]').dblclick(function () {
    var table = $(this).attr("data-table");
    var type = $(this).attr("data-type");
    var hook = $(this).attr("data-hook");
    var db = $("#NewListDashboard");
    db.find('[name="Table"]').val(table);
    db.find('[name="Type"]').val(type);
    db.find('[name="Hook"]').val(hook);
    $('[data-role="display-table-context"]').text(table + " " + type + " " + hook);
    $(window).scrollTop($("#ValueBundleDashboard").offset().top - 50);
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/getrule/" + table + "/" + type + "/" + hook,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            var container = $("#FirewallConfigurationLists");
            container.html("");
            $.each(data, function (i, list) {
                var row = '<tr data-index="' + i + '"><td style="width:200px;"><p style="display:inline-block;" class="fg-white">'
                    + list.Label +
                    '</p></td><td><input type="text" data-guid="' + list.Guid + '" style="height: 25px; width: 90%;" class="bg-anthilla-gray fg-white" value="'
                    + list.ReplaceValues + '"/></td><td><i class="icon-cycle fg-anthilla-violet on-left" data-guid="' + list.Guid +
                    '" data-role="firewall-reload-values" data-backup="'
                    + list.ReplaceValues +
                    '" style="cursor: pointer !important;"></i><i data-role="firewall-update-values" data-guid="' + list.Guid +
                    '" class="icon-checkmark fg-anthilla-green on-left style="cursor: pointer !important;""></i>' +
                    "</td></tr>";
                container.append(row);
                ReloadListValues();
                SaveListValues();
            });

            var aj2 = $.ajax({
                url: "/firewall/getruleset/" + table + "/" + type + "/" + hook,
                type: "GET",
                dataType: "json",
                contentType: "application/json;charset=utf-8",
                success: function (rules) {
                    $("#RuleList").html("");
                    $.each(rules, function (i, rule) {
                        var row = '<tr data-index="' + i + '"><td style="width:200px;"></td><td>' + rule + "</td></tr>";
                        $("#RuleList").append(row);
                    });
                    return false;
                }
            });
            _requests.push(aj2);

            return false;
        }
    });
    _requests.push(aj);
});

function ReloadListValues() {
    $('[data-role="firewall-reload-values"]').on("click", function () {
        var guid = $(this).attr("data-guid");
        var bkup = $(this).attr("data-backup");
        $('input[data-guid="' + guid + '"]').val(bkup);
        return false;
    });
}

function SaveListValues() {
    $('[data-role="firewall-update-values"]').on("click", function () {
        var guid = $(this).attr("data-guid");
        var el = $('input[data-guid="' + guid + '"]').val();
        jQuery.support.cors = true;
        var aj = $.ajax({
            url: networkUrl + "/ipv4/disable/if",
            type: "POST",
            data: {
                Guid: guid,
                Elements: el
            },
            success: function () {
                location.reload(true);
            }
        });
        _requests.push(aj);
    });
}

$('i[data-role="stop-rule"]').on("click", function () {
    var guid = $(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/stoprule",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyFirewallConfiguration").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/conf/export",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ExportFirewallConfiguration").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/firewall/conf/export",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});