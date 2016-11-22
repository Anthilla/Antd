var $fw = jQuery.noConflict();

$fw('[data-role="SaveNftConfiguration"]').on("click", function () {
    var tables = [];
    $fw('[data-nft="Table"]').each(function () {
        var self = $fw(this);
        var tableType = self.attr("data-nft-type");
        var tableName = self.attr("data-nft-name");

        var tableSets = [];
        self.find('[data-nft="Set"]').each(function () {
            var selfSet = $fw(this);
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
            var selfChain = $fw(this);
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
    var aj = $fw.ajax({
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

$fw(document).on("ready", function () {
    $fw(".autoheight").each(function () {
        this.style.height = "1px";

        var text = $fw(this).val();
        var lines = text.split(/\r|\r\n|\n/);
        var count = lines.length;
        console.log(count);

        this.style.height = (count * 20) + "px";
    });
});

$fw('input[data-role="enable-mac-address"]').on("click", function () {
    var guid = $fw(this).attr("data-object-guid");
    jQuery.support.cors = true;
    var aj = $fw.ajax({
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

$fw('input[data-role="disable-mac-address"]').on("click", function () {
    var guid = $fw(this).attr("data-object-guid");
    jQuery.support.cors = true;
    var aj = $fw.ajax({
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

$fw("#ReloadMacAddressList").on("click", function () {
    jQuery.support.cors = true;
    var aj = $fw.ajax({
        url: "/firewall/discover/macadd",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$fw('div[data-object="ruleset"]').dblclick(function () {
    var table = $fw(this).attr("data-table");
    var type = $fw(this).attr("data-type");
    var hook = $fw(this).attr("data-hook");
    var db = $fw("#NewListDashboard");
    db.find('[name="Table"]').val(table);
    db.find('[name="Type"]').val(type);
    db.find('[name="Hook"]').val(hook);
    $fw('[data-role="display-table-context"]').text(table + " " + type + " " + hook);
    $fw(window).scrollTop($fw("#ValueBundleDashboard").offset().top - 50);
    jQuery.support.cors = true;
    var aj = $fw.ajax({
        url: "/firewall/getrule/" + table + "/" + type + "/" + hook,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            var container = $fw("#FirewallConfigurationLists");
            container.html("");
            $fw.each(data, function (i, list) {
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

            var aj2 = $fw.ajax({
                url: "/firewall/getruleset/" + table + "/" + type + "/" + hook,
                type: "GET",
                dataType: "json",
                contentType: "application/json;charset=utf-8",
                success: function (rules) {
                    $fw("#RuleList").html("");
                    $fw.each(rules, function (i, rule) {
                        var row = '<tr data-index="' + i + '"><td style="width:200px;"></td><td>' + rule + "</td></tr>";
                        $fw("#RuleList").append(row);
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
    $fw('[data-role="firewall-reload-values"]').on("click", function () {
        var guid = $fw(this).attr("data-guid");
        var bkup = $fw(this).attr("data-backup");
        $fw('input[data-guid="' + guid + '"]').val(bkup);
        return false;
    });
}

function SaveListValues() {
    $fw('[data-role="firewall-update-values"]').on("click", function () {
        var guid = $fw(this).attr("data-guid");
        var el = $fw('input[data-guid="' + guid + '"]').val();
        jQuery.support.cors = true;
        var aj = $fw.ajax({
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

$fw('i[data-role="stop-rule"]').on("click", function () {
    var guid = $fw(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $fw.ajax({
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

$fw("#ApplyFirewallConfiguration").on("click", function () {
    jQuery.support.cors = true;
    var aj = $fw.ajax({
        url: "/firewall/conf/export",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$fw("#ExportFirewallConfiguration").on("click", function () {
    jQuery.support.cors = true;
    var aj = $fw.ajax({
        url: "/firewall/conf/export",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});