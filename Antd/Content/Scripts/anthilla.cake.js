(function ($) {
    $.fn.cake = function (options) {
        var container = $(this);
        var slice = $('<li></li>');
        container.append(slice);
        var content = $('<div></div>');
        slice.append(content);
        var slice1 = $('<li></li>');
        container.append(slice1);
        var content1 = $('<div></div>');
        slice1.append(content1);        
        var slice2 = $('<li></li>');
        container.append(slice2);
        var content2 = $('<div></div>');
        slice2.append(content2);        
        var slice3 = $('<li></li>');
        container.append(slice3);
        var content3 = $('<div></div>');
        slice3.append(content3);        
        
        var settings = $.extend({
            data: {
                total: '',
                used: ''
            }
        }, options);

        container.css({
            position: 'relative',
            'background-color': '#EBEBEB',
            width: '100px',
            height: '100px',
            padding: '0',
            'border-radius': '50%',
            'list-style': 'none',
            'z-index' : '0'
        });
        
        var total = settings.data.total;
        var used = settings.data.used;
        var percentage = (used * 100)/total;

        if(percentage > 25){ //25-50%
            var percentageAngle1 = ((percentage - 25) * 360)/100;
            var skewAngle1 = (90 - percentageAngle1).toString().split('.')[0];
			var skewS1 = 'rotate(90deg) skewY(-' + skewAngle1 + 'deg)';
            var skewC1 = 'skewY(' + skewAngle1 + 'deg)';
            slice1.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
				'transform': skewS1
            });
            content1.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
				'transform': skewC1
            });
            slice.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
                'transform': 'skewY(0deg)'
            });     
            content.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
                'transform': 'skewY(0deg)'
            });
        }        
        if(percentage > 50){ //50-75%
            var percentageAngle2 = ((percentage - 50) * 360)/100;
            var skewAngle2 = (90 - percentageAngle2).toString().split('.')[0];
			var skewS2 = 'rotate(180deg) skewY(-' + skewAngle2 + 'deg)';
            var skewC2 = 'skewY(' + skewAngle2 + 'deg)';
            slice2.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
				'transform': skewS2
            });
            content2.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
				'transform': skewC2
            });
            slice1.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
				'transform': 'rotate(90deg) skewY(0deg)'
            });
            content1.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
				'transform': 'skewY(0deg)'
            });
            slice.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
                'transform': 'skewY(0deg)'
            });     
            content.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
                'transform': 'skewY(0deg)'
            });
        }
        if(percentage > 75){ //75-100%
            var percentageAngle3 = ((percentage - 75) * 360)/100;
            var skewAngle3 = (90 - percentageAngle3).toString().split('.')[0];
			var skewS3 = 'rotate(270deg) skewY(-' + skewAngle3 + 'deg)';
            var skewC3 = 'skewY(' + skewAngle3 + 'deg)';
            slice3.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
				'transform': skewS3
            });
            content3.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
				'transform': skewC3
            }); 
            slice2.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
				'transform': 'rotate(180deg) skewY(0deg)'
            });
            content2.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
				'transform': 'skewY(0deg)'
            });
            slice1.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
				'transform': 'rotate(90deg) skewY(0deg)'
            });
            content1.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
				'transform': 'skewY(0deg)'
            });
            slice.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
                'transform': 'skewY(0deg)'
            });     
            content.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
                'transform': 'skewY(0deg)'
            });
        }
        if (percentage < 25) { //0-25%
            //convert % to deg x% : 100% = x : 360
            var percentageAngle = (percentage * 360)/100;
            //skew = 90 - angolo da rappresentare
            var skewAngle = (90 - percentageAngle).toString().split('.')[0];
            var skewS = 'skewY(-' + skewAngle + 'deg)';
            var skewC = 'skewY(' + skewAngle + 'deg)';
            slice.css({
                overflow: 'hidden',
                position: 'absolute',
                top: '0',
                right: '0',
                width: '50%',
                height: '50%',
                'transform-origin': '0% 100%',
                'transform': skewS
            });     
            content.css({
                'background-color': '#548ED4',
                position: 'absolute',
                left: '-100%',
                width: '200%',
                height: '200%',
                'border-radius': '50%',
                'transform': skewC
            });
        }
        WriteValues(container, total, used);
        //$('p').text('used: ' + percentage + '%');
        return container;
    };
}(jQuery));

function WriteValues(container, total, used) {
    var table = container.parents('table');

    table.find('.totDim').text(total.toString() + ' GB');
    table.find('.useDim').text(used.toString() + ' GB');
    table.find('.freDim').text((total - used).toString() + ' GB');

    var usePerc = ((used * 100) / total).toString().split('.')[0];;
    var frePerc = 100 - usePerc;
    table.find('.totPer').text('100%');
    table.find('.usePer').text(usePerc.toString() + '%');
    table.find('.frePer').text(frePerc.toString() + '%');

    return false;
}
//usage
//$( ".circle" ).cake({            
//    data: {
//        total: '100',
//        used: '85'
//    }
//});