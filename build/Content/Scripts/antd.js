//global?////////////////////////////////////////////////////////////////////////////////////////
$('p[id^=Update]').click(function () {
    var self = $(this);
    var id = self.attr('id');
    var inputID = id.replace('Update', 'New');
    var newValue = $('#' + inputID).val();
    if (newValue.length > 0) {
        jQuery.support.cors = true;
        $.ajax({
            url: '/command/mgmt/ex/' + inputID + '/' + newValue,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (data) {
                console.log(data);
                return false;
            }
        });
    }
});

//page-system-general
$('#NewHostname').keyup(function () {
    var value = $('#NewHostname').val();
    if (value != "" || val != " ") {
        $('#UpdateHostname').show();
    }
});

$('#UpdateHostname').click(function () {
    jQuery.support.cors = true;
    var newHostName = $('#NewHostname').val();
    $.ajax({
        url: '/system/update/hostname/' + newHostName,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewDomainname').keyup(function () {
    var value = $('#NewDomainname').val();
    if (value != "" || val != " ") {
        $('#UpdateDomainname').show();
    }
});

$('#UpdateDomainname').click(function () {
    jQuery.support.cors = true;
    var newDomainName = $('#NewDomainname').val();
    $.ajax({
        url: '/system/update/domainname/' + newDomainName,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTimezone').change(function () {
    $('#UpdateTimezone').show();
});

$('#UpdateTimezone').click(function () {
    jQuery.support.cors = true;
    var newTimezone = $('#NewTimezone>option:selected').val();
    $.ajax({
        url: '/system/update/timezone/' + newTimezone,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTimeserver').keyup(function () {
    var value = $('#NewTimeserver').val();
    if (value != "" || val != " ") {
        $('#UpdateTimeserver').show();
    }
});

$('#UpdateTimeserver').click(function () {
    jQuery.support.cors = true;
    var newTimeserver = $('#NewTimeserver').val();
    $.ajax({
        url: '/system/update/timeserver/' + newTimeserver,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewLanguage').change(function () {
    $('#UpdateLanguage').show();
});

$('#UpdateLanguage').click(function () {
    jQuery.support.cors = true;
    var newLanguage = $('#NewLanguage>option:selected').val();
    $.ajax({
        url: '/system/update/language/' + newLanguage,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#SaveSystemGeneral').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/systemgeneral/',
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

//page-system-advanced

$('#UpdateProtocol').click(function () {
    var newProtocol = $('input[name="WebConfiguratorProtocol"]:checked').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/protocol/' + newProtocol,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTCPport').keyup(function () {
    var value = $('#NewTCPport').val();
    if (value != "" || val != " ") {
        $('#UpdateTCPport').show();
    }
});

$('#UpdateTCPport').click(function () {
    var newTCPport = $('#NewTCPport').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/tcpport/' + newTCPport,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewMaxProcesses').keyup(function () {
    var value = $('#NewMaxProcesses').val();
    if (value != "" || val != " ") {
        $('#UpdateMaxProcesses').show();
    }
});

$('#UpdateMaxProcesses').click(function () {
    var newMaxProcesses = $('#NewMaxProcesses').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/maxprocs/' + newMaxProcesses,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateWebGUIRedirects').click(function () {
    var newWebGUIRedirects = $('#NewWebGUIRedirects').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/webguiredirects/' + newWebGUIRedirects,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateWebGUILoginAutocomplete').click(function () {
    var newWebGUILoginAutocomplete = $('#NewWebGUILoginAutocomplete').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/webguiloginautocomplete/' + newWebGUILoginAutocomplete,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateWebGUIloginmessages').click(function () {
    var newWebGUIloginmessages = $('#NewWebGUIloginmessages').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/webguiloginmessages/' + newWebGUIloginmessages,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateAntiLockout').click(function () {
    var newAntiLockout = $('#NewAntiLockout').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/antilockout/' + newAntiLockout,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDNSRebindCheck').click(function () {
    var newDNSRebindCheck = $('#NewDNSRebindCheck').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/dnsrebindcheck/' + newDNSRebindCheck,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewAlternateHostnames').keyup(function () {
    var value = $('#NewAlternateHostnames').val();
    if (value != "" || val != " ") {
        $('#UpdateAlternateHostnames').show();
    }
});

$('#UpdateAlternateHostnames').click(function () {
    var newAlternateHostnames = $('#NewAlternateHostnames').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/alternatehostnames/' + newAlternateHostnames,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateHTTP_REFERERenforcement').click(function () {
    var newHTTP_REFERERenforcement = $('#NewHTTP_REFERERenforcement').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/refererenforcement/' + newHTTP_REFERERenforcement,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateTabText').click(function () {
    var newTabText = $('#NewTabText').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/tabtext/' + newTabText,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateEnableSecureShell').click(function () {
    var newEnableSecureShell = $('#NewEnableSecureShell').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/enablesecureshell/' + newEnableSecureShell,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateAuthenticationMethod').click(function () {
    var newAuthenticationMethod = $('#NewAuthenticationMethod').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/authenticationmethod/' + newAuthenticationMethod,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewSSHport').keyup(function () {
    var value = $('#NewSSHport').val();
    if (value != "" || val != " ") {
        $('#UpdateSSHport').show();
    }
});

$('#UpdateSSHport').click(function () {
    var newSSHport = $('#NewSSHport').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/sshport/' + newSSHport,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateSerialTerminal').click(function () {
    var newSerialTerminal = $('#NewSerialTerminal').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/serialterminal/' + newSerialTerminal,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewSerialSpeed').change(function () {
    $('#UpdateSerialSpeed').show();
});

$('#UpdateSerialSpeed').click(function () {
    jQuery.support.cors = true;
    var newSerialSpeed = $('#NewSerialSpeed>option:selected').val();
    $.ajax({
        url: '/system/update/serialspeed/' + newSerialSpeed,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewPrimaryConsole').change(function () {
    $('#UpdatePrimaryConsole').show();
});

$('#UpdatePrimaryConsole').click(function () {
    jQuery.support.cors = true;
    var newPrimaryConsole = $('#NewPrimaryConsole>option:selected').val();
    $.ajax({
        url: '/system/update/primaryconsole/' + newPrimaryConsole,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateConsoleMenu').click(function () {
    var newConsoleMenu = $('#NewConsoleMenu').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/consolemenu/' + newConsoleMenu,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateIPDo-Not-Fragment').click(function () {
    var newIPDoNotFragment = $('#NewIPDo-Not-Fragment').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/ipdontfragment/' + newIPDoNotFragment,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateIPRandomID').click(function () {
    var newIPRandomID = $('#NewIPRandomID').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/iprandomid/' + newIPRandomID,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#FirewallOptimizationOptionsSelect').change(function () {
    $('#UpdateFirewallOptimizationOptions').show();
});

$('#UpdateFirewallOptimizationOptions').click(function () {
    jQuery.support.cors = true;
    var newFirewallOptimizationOptions = $('#FirewallOptimizationOptionsSelect>option:selected').val();
    $.ajax({
        url: '/system/update/firewalloptimizationoptions/' + newFirewallOptimizationOptions,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDisableFirewallFilter').click(function () {
    var newDisableFirewallFilter = $('#NewDisableFirewallFilter').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/disablefirewallfilter/' + newDisableFirewallFilter,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDisableFirewallScrub').click(function () {
    var newDisableFirewallScrub = $('#NewDisableFirewallScrub').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/disablefirewallscrub/' + newDisableFirewallScrub,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateFirewallAdaptiveTimeouts').click(function () {
    var newFirewallAdaptiveTimeoutsEnd = $('#NewFirewallAdaptiveTimeoutsEnd').val();
    var newFirewallAdaptiveTimeoutsStart = $('#NewFirewallAdaptiveTimeoutsStart').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/firewalladaptivetimeouts/' + newFirewallAdaptiveTimeoutsEnd + '/' + newFirewallAdaptiveTimeoutsStart,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewFirewallMaximumStates').keyup(function () {
    var value = $('#NewFirewallMaximumStates').val();
    if (value != "" || val != " ") {
        $('#UpdateFirewallMaximumStates').show();
    }
});

$('#UpdateFirewallMaximumStates').click(function () {
    var newFirewallMaximumStates = $('#NewFirewallMaximumStates').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/firewallmaximumstates/' + newFirewallMaximumStates,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewFirewallMaximumTableEntries').keyup(function () {
    var value = $('#NewFirewallMaximumTableEntries').val();
    if (value != "" || val != " ") {
        $('#UpdateFirewallMaximumTableEntries').show();
    }
});

$('#UpdateFirewallMaximumTableEntries').click(function () {
    var newFirewallMaximumTableEntries = $('#NewFirewallMaximumTableEntries').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/firewallmaximumtableentries/' + newFirewallMaximumTableEntries,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateStaticRouteFiltering').click(function () {
    var newStaticRouteFiltering = $('#NewStaticRouteFiltering').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/staticroutefiltering/' + newStaticRouteFiltering,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDisableAutoVPN').click(function () {
    var newDisableAutoVPN = $('#NewDisableAutoVPN').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/disableautovpn/' + newDisableAutoVPN,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDisableReplyto').click(function () {
    var newDisableReplyto = $('#NewDisableReplyto').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/disablereplyto/' + newDisableReplyto,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDisableNegateRules').click(function () {
    var newDisableNegateRules = $('#NewDisableNegateRules').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/disablenegaterules/' + newDisableNegateRules,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewAliasesHostnamesResolveInterval').keyup(function () {
    var value = $('#NewAliasesHostnamesResolveInterval').val();
    if (value != "" || val != " ") {
        $('#UpdateAliasesHostnamesResolveInterval').show();
    }
});

$('#UpdateAliasesHostnamesResolveInterval').click(function () {
    var newAliasesHostnamesResolveInterval = $('#NewAliasesHostnamesResolveInterval').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/aliaseshostnamesresolveinterval/' + newAliasesHostnamesResolveInterval,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateCheckCertificateAliasUrl').click(function () {
    var newCheckCertificateAliasUrl = $('#NewCheckCertificateAliasUrl').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/checkcertificatealiasurl/' + newCheckCertificateAliasUrl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewUpdateFrequency').change(function () {
    $('#UpdateUpdateFrequency').show();
});

$('#UpdateUpdateFrequency').click(function () {
    jQuery.support.cors = true;
    var newUpdateFrequency = $('#NewUpdateFrequency>option:selected').val();
    $.ajax({
        url: '/system/update/updatefrequency/' + newUpdateFrequency,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewNATReflection').change(function () {
    $('#UpdateNATReflection').show();
});

$('#UpdateNATReflection').click(function () {
    jQuery.support.cors = true;
    var newNATReflection = $('#NewNATReflection>option:selected').val();
    $.ajax({
        url: '/system/update/natreflection/' + newNATReflection,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewNATReflectionTout').keyup(function () {
    var value = $('#NewNATReflectionTout').val();
    if (value != "" || val != " ") {
        $('#UpdateNATReflectionTout').show();
    }
});

$('#UpdateNATReflectionTout').click(function () {
    var newNATReflectionTout = $('#NewNATReflectionTout').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/natreflectiontout/' + newNATReflectionTout,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateEnableNATReflection1v1').click(function () {
    var newEnableNATReflection1v1 = $('#NewEnableNATReflection1v1').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/enablenatreflection1v1/' + newEnableNATReflection1v1,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateEnableNATRautomaticOutbound').click(function () {
    var newEnableNATRautomaticOutbound = $('#NewEnableNATRautomaticOutbound').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/enablenatrautomaticoutbound/' + newEnableNATRautomaticOutbound,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTFTPProxy').change(function () {
    $('#UpdateTFTPProxy').show();
});

$('#UpdateTFTPProxy').click(function () {
    jQuery.support.cors = true;
    var newTFTPProxy = $('#NewTFTPProxy>option:selected').val();
    $.ajax({
        url: '/system/update/tftpproxy/' + newTFTPProxy,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateAllowIPv6').click(function () {
    var newAllowIPv6 = $('#NewAllowIPv6').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/allowipv6/' + newAllowIPv6,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#NewTextIPv6onIPv4').keyup(function () {
    var value = $('#NewTextIPv6onIPv4').val();
    if (value != "" || val != " ") {
        $('#UpdateIPv6onIPv4').show();
    }
});

$('#UpdateIPv6onIPv4').click(function () {
    var NewCheckboxIPv6onIPv4 = $('#NewCheckboxIPv6onIPv4').val();
    var NewTextIPv6onIPv4 = $('#NewTextIPv6onIPv4').val();
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/ipv6onipv4/' + NewCheckboxIPv6onIPv4 + '/' + NewTextIPv6onIPv4,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdatePreferIPv4').click(function () {
    var newPreferIPv4 = $('#NewPreferIPv4').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/preferipv4/' + newPreferIPv4,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateDevicePolling').click(function () {
    var newDevicePolling = $('#NewDevicePolling').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/devicepolling/' + newDevicePolling,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateHardwareChecksumOffloading').click(function () {
    var newHardwareChecksumOffloading = $('#NewHardwareChecksumOffloading').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/hardwarechecksumoffloading/' + newHardwareChecksumOffloading,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateHardwareTCPSegmentationOffloading').click(function () {
    var newHardwareTCPSegmentationOffloading = $('#NewHardwareTCPSegmentationOffloading').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/hardwaretcpsegmentationoffloading/' + newHardwareTCPSegmentationOffloading,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateHardwareLargeReceiveOffloading').click(function () {
    var newHardwareLargeReceiveOffloading = $('#NewHardwareLargeReceiveOffloading').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/hardwarelargereceiveoffloading/' + newHardwareLargeReceiveOffloading,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});

$('#UpdateSuppressARP').click(function () {
    var newSuppressARP = $('#NewSuppressARP').prop('checked');
    jQuery.support.cors = true;
    $.ajax({
        url: '/system/update/suppressarp/' + newSuppressARP,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            console.log(data);
            location.reload(true);
        }
    });
});