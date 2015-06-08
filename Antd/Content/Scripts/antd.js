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