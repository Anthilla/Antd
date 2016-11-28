$('[data-role="SyslogNgUpdate"]').on("click", function () {
    jQuery.support.cors = true;
    var container = $(this).parents('[data-role="SyslogNgConfiguration"]');
    var aj = $.ajax({
        url: "/log/syslog/set",
        type: "POST",
        data: {
            Root: container.find('[data-type="SyslogNgRootPath"]').val(),
            Path1: container.find('[data-type="SyslogNgPortNet1"]').val(),
            Path2: container.find('[data-type="SyslogNgPortNet2"]').val(),
            Path3: container.find('[data-type="SyslogNgPortNet3"]').val()
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-row="report"]').dblclick(function () {
    var thisRow = $(this);
    var path = thisRow.attr("data-path");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/log/journalctl/report/" + path,
        type: "GET",
        success: function (file) {
            if (thisRow.next('tr[data-row="report-content"]').length === 0) {
                var newRow = '<tr data-row="report-content"><td><textarea class="bg-anthilla-gray-m border-anthilla-gray fg-white" readonly="readonly" style="width: 90%; max-width: 90%; min-width: 90%; min-height: 200px;">' + file + "</textarea></td></tr>";
                thisRow.after(newRow);
            }
        }
    });
    _requests.push(aj);
});

$("#GenerateNewReport").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/log/journalctl/report",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ShowLogsVerb").on("click", function () {
    var filter = $('[name="LogVerb"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/log/journalctl/all/" + filter,
        type: "GET",
        success: function (logs) {
            var container = $("#LogTable").find("tbody");
            container.html("");
            $.each(logs, function (i, log) {
                container.append('<tr><td style="font-weight: normal; font-size: 90%;">' + log + "</td></tr>");
            });
        }
    });
    _requests.push(aj);
});

$("#ShowLogsLastHours").on("click", function () {
    var h = $('[name="LogLastHours"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/log/journalctl/last/" + h,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (logs) {
            var container = $("#SystemLogTable").find("tbody");
            container.html("");
            $.each(logs, function (i, log) {
                container.append('<tr><td style="font-weight: normal; font-size: 90%;">' + log + "</td></tr>");
            });
        }
    });
    _requests.push(aj);
});
