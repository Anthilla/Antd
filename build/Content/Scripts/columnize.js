(function ($) {
    $.fn.columnize = function (options) {
        //Define widths
        var small = '10%';
        var medium = '30%';
        var large = '70%';
        //Default options
        var settings = $.extend({
            //mettere z-index tipo...
            marginRight: '5px',
            padding: '5px',
            display: 'inline-block',
            float: 'left',
            width: [medium, medium, medium]
        }, options);

        $(this).css({
            width: '1300px',
            overflow: 'hidden',
            display: settings.display,
            padding: settings.padding
        });
        //Define columns inside the container
        $('.column').each(function () {
            $(this).attr('column', 'expandable');
            $(this).attr('index', $(this).index());
        });
        var column0 = $('.column[column*="expandable"]').eq(0);
        var column1 = $('.column[column*="expandable"]').eq(1);
        var column2 = $('.column[column*="expandable"]').eq(2);
        //Set columns style and default width
        column0.css({
            width: settings.width[0],
            display: settings.display,
            padding: settings.padding,
            float: settings.float,
            'margin-right': settings.marginRight
        }).attr('width', settings.width[0]);
        column1.css({
            width: settings.width[1],
            display: settings.display,
            padding: settings.padding,
            float: settings.float,
            'margin-right': settings.marginRight
        }).attr('width', settings.width[1]);
        column2.css({
            width: settings.width[2],
            display: settings.display,
            padding: settings.padding,
            float: settings.float
        }).attr('width', settings.width[2]);
        //Define internal functions
        function ResetWidths() {
            $('span[index*="0"]')
            .attr('width', medium)
            .css({ width: medium });
            $('span[index*="1"]')
            .attr('width', medium)
            .css({ width: medium });
            $('span[index*="2"]')
            .attr('width', medium)
            .css({ width: medium });
        }
        function Enlarge0() {
            $('span[index*="0"]')
            .attr('width', large)
            .css({ width: large });
            $('span[index*="1"]')
            .attr('width', small)
            .css({ width: small });
            $('span[index*="2"]')
            .attr('width', small)
            .css({ width: small });
        }
        function Enlarge1() {
            $('span[index*="0"]')
            .attr('width', small)
            .css({ width: small });
            $('span[index*="1"]')
            .attr('width', large)
            .css({ width: large });
            $('span[index*="2"]')
            .attr('width', small)
            .css({ width: small });
        }
        function Enlarge2() {
            $('span[index*="0"]')
            .attr('width', small)
            .css({ width: small });
            $('span[index*="1"]')
            .attr('width', small)
            .css({ width: small });
            $('span[index*="2"]')
            .attr('width', large)
            .css({ width: large });
        }
        //Define actions & events
        $(document).click(function (e) {
            var el = $(e.target);
            var thisColumn = el.closest('[column*="expandable"]');
            var index = thisColumn.attr('index');
            //Set variable width
            if (index == '0') {
                if (thisColumn.attr('width') == small) {
                    //ResetWidths();
                }
                else if (thisColumn.attr('width') == medium) {
                    //Enlarge0();
                }
                else if (thisColumn.attr('width') == large) {
                    //$(thisColumn).unbind('click');
                }
            }
            if (index == '1') {
                if (thisColumn.attr('width') == small) {
                    ResetWidths();
                }
                else if (thisColumn.attr('width') == medium) {
                    Enlarge1();
                }
                else if (thisColumn.attr('width') == large) {
                    $(thisColumn).unbind('click');
                }
            }
            if (index == '2') {
                if (thisColumn.attr('width') == small) {
                    ResetWidths();
                }
                else if (thisColumn.attr('width') == medium) {
                    Enlarge2();
                }
                else if (thisColumn.attr('width') == large) {
                    $(thisColumn).unbind('click');
                }
            }
        });
        //Return the object/function
        return this;
    };
}(jQuery));

//$('#click').click(function (event) {
//    event.preventDefault();
//    $(this).append('!!');
//});

//$("#Container").columnize();