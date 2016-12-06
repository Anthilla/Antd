$('[data-role="ToggleUnitContent"]').on("click", function () {
    var self = $(this);
    self.toggleClass("icon-arrow-right-5");
    self.toggleClass("icon-arrow-down-5");
    var cont = self.parents("div.container").find("div.content");
    cont.toggle();
});

$('[data-role="ShowLogs"]').on("click", function () {
    var srv = $(this).attr("data-name");
    var ttt = $(this).parents("div.container");
    var cont = ttt.find('[data-role="ServiceLog"]');
    ttt.find("div.content").show();
    var arr = ttt.find('[data-role="ToggleUnitContent"]');
    arr.removeClass("icon-arrow-right-5");
    arr.addClass("icon-arrow-down-5");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/log/journalctl/service/" + srv,
        type: "GET",
        success: function (data) {
            cont.html(data);
        }
    });
    _requests.push(aj);
});