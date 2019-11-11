'use strict';

/* globals d3 */

angular.module('g1b.calendar-heatmap', []).
    directive('calendarHeatmap', ['$window', function ($window) {

        return {
            restrict: 'E',
            scope: {
                data: '=',
                color: '=?',
                overview: '=?',
                handler: '=?',
                locale: '=?'
            },
            replace: true,
            template: '<div class="calendar-heatmap"></div>',
            link: function (scope, element) {

                // Defaults
                var gutter = 1;
                var item_gutter = 1;
                var width = 1000;
                var height = 200;
                var item_size = 10;
                var label_padding = 40;
                var max_block_height = 20;
                var transition_duration = 500;
                var in_transition = false;

                // Tooltip defaults
                var tooltip_width = 250;
                var tooltip_padding = 15;

                // Initialize current locale
                scope.locale = scope.locale || 'en';
                //console.log("[heatmap] " + scope.locale);
                moment.locale(scope.locale);

                // Initialize current overview type and history
                scope.overview = scope.overview || 'global';
                scope.history = ['global'];
                scope.selected = {};

                // Initialize svg element
                var svg = d3.select(element[0])
                    .append('svg')
                    .attr('class', 'svg');

                // Initialize main svg elements
                var items = svg.append('g');
                var labels = svg.append('g');
                var buttons = svg.append('g');

                // Add tooltip to the same element as main svg
                var tooltip = d3.select(element[0]).append('div')
                    .attr('class', 'heatmap-tooltip')
                    .style('opacity', 0);

                var getNumberOfWeeks = function () {
                    var dayIndex = Math.round((moment() - moment().subtract(1, 'year').startOf('week')) / 86400000);
                    var colIndex = Math.trunc(dayIndex / 7);
                    var numWeeks = colIndex + 1;
                    return numWeeks;
                }

                scope.$watch(function () {
                    return element[0].clientWidth;
                }, function (w) {
                    if (!w) { return; }
                    width = w < 1000 ? 1000 : w;
                    item_size = ((width - label_padding) / getNumberOfWeeks() - gutter);
                    height = label_padding + 7 * (item_size + gutter);
                    svg.attr({ 'width': width, 'height': height });
                    if (!!scope.data && !!scope.data[0].summary) {
                        scope.drawChart();
                    }
                });

                angular.element($window).bind('resize', function () {
                    scope.$apply();
                });

                // Watch for data availability
                scope.$watch('data', function (data) {
                    if (!data) { return; }

                    // Get daily summary if that was not provided
                    if (!data[0].summary) {
                        data.map(function (d) {
                            var summary = d.details.reduce(function (uniques, project) {
                                if (!uniques[project.name]) {
                                    uniques[project.name] = {
                                        'value': project.value
                                    };
                                } else {
                                    uniques[project.name].value += project.value;
                                }
                                return uniques;
                            }, {});
                            var unsorted_summary = Object.keys(summary).map(function (key) {
                                return {
                                    'name': key,
                                    'value': summary[key].value
                                };
                            });
                            d.summary = unsorted_summary.sort(function (a, b) {
                                return b.value - a.value;
                            });
                            return d;
                        });
                    }

                    // Draw the chart
                    scope.drawChart();
                });


                /**
                 * Draw the chart based on the current overview type
                 */
                scope.drawChart = function () {
                    if (!scope.data) { return; }

                    if (scope.overview === 'global') {
                        scope.drawGlobalOverview();
                    } else if (scope.overview === 'year') {
                        scope.drawYearOverview();
                    } else if (scope.overview === 'month') {
                        scope.drawMonthOverview();
                    } else if (scope.overview === 'week') {
                        scope.drawWeekOverview();
                    } else if (scope.overview === 'day') {
                        scope.drawDayOverview();
                    }
                };


                /**
                 * Draw global overview (multiple years)
                 */
                scope.drawGlobalOverview = function () {

                    // Add current overview to the history
                    if (scope.history[scope.history.length - 1] !== scope.overview) {
                        scope.history.push(scope.overview);
                    }

                    // Define start and end of the dataset
                    var start = moment(scope.data[0].date).startOf('year');
                    var end = moment(scope.data[scope.data.length - 1].date).endOf('year');

                    // Define array of years and total values
                    var year_data = d3.time.years(start, end).map(function (d) {
                        var date = moment(d);
                        return {
                            'date': date,
                            'total': scope.data.reduce(function (prev, current) {
                                if (moment(current.date).year() === date.year()) {
                                    prev += current.total;
                                }
                                return prev;
                            }, 0),
                            'summary': function () {
                                var summary = scope.data.reduce(function (summary, d) {
                                    if (moment(d.date).year() === date.year()) {
                                        for (var i = 0; i < d.summary.length; i++) {
                                            if (!summary[d.summary[i].name]) {
                                                summary[d.summary[i].name] = {
                                                    'value': d.summary[i].value,
                                                };
                                            } else {
                                                summary[d.summary[i].name].value += d.summary[i].value;
                                            }
                                        }
                                    }
                                    return summary;
                                }, {});
                                var unsorted_summary = Object.keys(summary).map(function (key) {
                                    return {
                                        'name': key,
                                        'value': summary[key].value
                                    };
                                });
                                return unsorted_summary.sort(function (a, b) {
                                    return b.value - a.value;
                                });
                            }(),
                        };
                    });

                    // Calculate max value of all the years in the dataset
                    var max_value = d3.max(year_data, function (d) {
                        return d.total;
                    });

                    // Define year labels and axis
                    var year_labels = d3.time.years(start, end).map(function (d) {
                        return moment(d);
                    });
                    var yearScale = d3.scale.ordinal()
                        .rangeRoundBands([0, width], 0.05)
                        .domain(year_labels.map(function (d) {
                            return d.year();
                        }));

                    // Add global data items to the overview
                    items.selectAll('.item-block-year').remove();
                    var item_block = items.selectAll('.item-block-year')
                        .data(year_data)
                        .enter()
                        .append('rect')
                        .attr('class', 'item item-block-year')
                        .attr('width', function () {
                            return (width - label_padding) / year_labels.length - gutter * 5;
                        })
                        .attr('height', function () {
                            return height - label_padding;
                        })
                        .attr('transform', function (d) {
                            return 'translate(' + yearScale(d.date.year()) + ',' + tooltip_padding * 2 + ')';
                        })
                        .attr('fill', function (d) {
                            var color = d3.scale.linear()
                                .range(['#ffffff', scope.color || '#ff4500'])
                                .domain([-0.15 * max_value, max_value]);
                            return color(d.total) || '#ff4500';
                        })
                        .on('click', function (d) {
                            if (scope.in_transition) { return; }

                            // Set in_transition flag
                            scope.in_transition = true;

                            // Set selected date to the one clicked on
                            scope.selected = d;

                            // Hide tooltip
                            scope.hideTooltip();

                            // Remove all global overview related items and labels
                            scope.removeGlobalOverview();

                            // Redraw the chart
                            scope.overview = 'year';
                            scope.drawChart();
                        })
                        .style('opacity', 0)
                        .on('mouseover', function (d) {
                            if (scope.in_transition) { return; }

                            // Construct tooltip
                            var tooltip_html = '';
                            tooltip_html += '<div><span><strong>Total time tracked:</strong></span>';

                            var sec = parseInt(d.total, 10);
                            var days = Math.floor(sec / 86400);
                            if (days > 0) {
                                tooltip_html += '<span>' + (days === 1 ? '1 day' : days + ' days') + '</span></div>';
                            }
                            var hours = Math.floor((sec - (days * 86400)) / 3600);
                            if (hours > 0) {
                                if (days > 0) {
                                    tooltip_html += '<div><span></span><span>' + (hours === 1 ? '1 hour' : hours + ' hours') + '</span></div>';
                                } else {
                                    tooltip_html += '<span>' + (hours === 1 ? '1 hour' : hours + ' hours') + '</span></div>';
                                }
                            }
                            var minutes = Math.floor((sec - (days * 86400) - (hours * 3600)) / 60);
                            if (minutes > 0) {
                                if (days > 0 || hours > 0) {
                                    tooltip_html += '<div><span></span><span>' + (minutes === 1 ? '1 minute' : minutes + ' minutes') + '</span></div>';
                                } else {
                                    tooltip_html += '<span>' + (minutes === 1 ? '1 minute' : minutes + ' minutes') + '</span></div>';
                                }
                            }
                            tooltip_html += '<br />';

                            // Add summary to the tooltip
                            if (d.summary.length <= 5) {
                                for (var i = 0; i < d.summary.length; i++) {
                                    tooltip_html += '<div><span><strong>' + d.summary[i].name + '</strong></span>';
                                    tooltip_html += '<span>' + scope.formatTime(d.summary[i].value) + '</span></div>';
                                };
                            } else {
                                for (var i = 0; i < 5; i++) {
                                    tooltip_html += '<div><span><strong>' + d.summary[i].name + '</strong></span>';
                                    tooltip_html += '<span>' + scope.formatTime(d.summary[i].value) + '</span></div>';
                                };
                                tooltip_html += '<br />';

                                var other_projects_sum = 0;
                                for (var i = 5; i < d.summary.length; i++) {
                                    other_projects_sum = + d.summary[i].value;
                                };
                                tooltip_html += '<div><span><strong>Other:</strong></span>';
                                tooltip_html += '<span>' + scope.formatTime(other_projects_sum) + '</span></div>';
                            }

                            // Calculate tooltip position
                            var x = yearScale(d.date.year()) + tooltip_padding * 2;
                            while (width - x < (tooltip_width + tooltip_padding * 5)) {
                                x -= 10;
                            }
                            var y = tooltip_padding * 3;

                            // Show tooltip
                            tooltip.html(tooltip_html)
                                .style('left', x + 'px')
                                .style('top', y + 'px')
                                .transition()
                                .duration(transition_duration / 2)
                                .ease('ease-in')
                                .style('opacity', 1);
                        })
                        .on('mouseout', function () {
                            if (scope.in_transition) { return; }
                            scope.hideTooltip();
                        })
                        .transition()
                        .delay(function (d, i) {
                            return transition_duration * (i + 1) / 10;
                        })
                        .duration(function () {
                            return transition_duration;
                        })
                        .ease('ease-in')
                        .style('opacity', 1)
                        .call(function (transition, callback) {
                            if (transition.empty()) {
                                callback();
                            }
                            var n = 0;
                            transition
                                .each(function () { ++n; })
                                .each('end', function () {
                                    if (!--n) {
                                        callback.apply(this, arguments);
                                    }
                                });
                        }, function () {
                            scope.in_transition = false;
                        });

                    // Add year labels
                    labels.selectAll('.label-year').remove();
                    labels.selectAll('.label-year')
                        .data(year_labels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-year')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return d.year();
                        })
                        .attr('x', function (d) {
                            return yearScale(d.year());
                        })
                        .attr('y', label_padding / 2)
                        .on('mouseenter', function (year_label) {
                            if (scope.in_transition) { return; }

                            items.selectAll('.item-block-year')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    return (moment(d.date).year() === year_label.year()) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (scope.in_transition) { return; }

                            items.selectAll('.item-block-year')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 1);
                        })
                        .on('click', function (d) {
                            if (scope.in_transition) { return; }

                            // Set in_transition flag
                            scope.in_transition = true;

                            // Set selected year to the one clicked on
                            scope.selected = { date: d };

                            // Hide tooltip
                            scope.hideTooltip();

                            // Remove all global overview related items and labels
                            scope.removeGlobalOverview();

                            // Redraw the chart
                            scope.overview = 'year';
                            scope.drawChart();
                        });
                },


                    /**
                     * Draw year overview
                     */
                    scope.drawYearOverview = function () {
                        // Add current overview to the history
                        if (scope.history[scope.history.length - 1] !== scope.overview) {
                            scope.history.push(scope.overview);
                        }

                        var year_ago = moment().startOf('day').subtract(1, 'year');
                        var max_value = d3.max(scope.data, function (d) {
                            return d.total;
                        });

                        // Define start and end date of the selected year
                        var start_of_year = moment(scope.selected.date).startOf('year');
                        var end_of_year = moment(scope.selected.date).endOf('year');

                        // Filter data down to the selected year
                        var year_data = scope.data.filter(function (d) {
                            return start_of_year <= moment(d.date) && moment(d.date) < end_of_year;
                        });

                        // Calculate max value of the year data
                        var max_value = d3.max(year_data, function (d) {
                            return d.total;
                        });

                        var color = d3.scale.linear()
                            .range(['#ffffff', scope.color || '#ff4500'])
                            .domain([-0.15 * max_value, max_value]);

                        var calcItemX = function (d) {
                            var date = moment(d.date);
                            var dayIndex = Math.round((date - moment(start_of_year).startOf('week')) / 86400000);
                            var colIndex = Math.trunc(dayIndex / 7);
                            return colIndex * (item_size + gutter) + label_padding;
                        };
                        var calcItemY = function (d) {
                            return label_padding + moment(d.date).weekday() * (item_size + gutter);
                        };
                        var calcItemSize = function (d) {
                            if (max_value <= 0) { return item_size; }
                            return item_size * 0.75 + (item_size * d.total / max_value) * 0.25;
                        };

                        items.selectAll('.item-circle').remove();
                        items.selectAll('.item-circle')
                            .data(year_data)
                            .enter()
                            .append('rect')
                            .attr('class', 'item item-circle')
                            .style('opacity', 0)
                            .attr('x', function (d) {
                                return calcItemX(d) + (item_size - calcItemSize(d)) / 2;
                            })
                            .attr('y', function (d) {
                                return calcItemY(d) + (item_size - calcItemSize(d)) / 2;
                            })
                            .attr('rx', function (d) {
                                return calcItemSize(d);
                            })
                            .attr('ry', function (d) {
                                return calcItemSize(d);
                            })
                            .attr('width', function (d) {
                                return calcItemSize(d);
                            })
                            .attr('height', function (d) {
                                return calcItemSize(d);
                            })
                            .attr('fill', function (d) {
                                return (d.total > 0) ? color(d.total) : 'transparent';
                            })
                            .on('click', function (d) {
                                if (scope.handler) {
                                    scope.handler(d);
                                    return;
                                }
                                if (in_transition) { return; }

                                // Don't transition if there is no data to show
                                if (d.total === 0) { return; }

                                in_transition = true;

                                // Set selected date to the one clicked on
                                scope.selected = d;

                                // Hide tooltip
                                scope.hideTooltip();

                                // Remove all year overview related items and labels
                                scope.removeYearOverview();

                                // Redraw the chart
                                scope.overview = 'day';
                                scope.drawChart();
                            })
                            .on('mouseover', function (d) {
                                if (in_transition) { return; }

                                // Pulsating animation
                                var circle = d3.select(this);
                                (function repeat() {
                                    circle = circle.transition()
                                        .duration(transition_duration)
                                        .ease('ease-in')
                                        .attr('x', function (d) {
                                            return calcItemX(d) - (item_size * 1.1 - item_size) / 2;
                                        })
                                        .attr('y', function (d) {
                                            return calcItemY(d) - (item_size * 1.1 - item_size) / 2;
                                        })
                                        .attr('width', item_size * 1.1)
                                        .attr('height', item_size * 1.1)
                                        .transition()
                                        .duration(transition_duration)
                                        .ease('ease-in')
                                        .attr('x', function (d) {
                                            return calcItemX(d) + (item_size - calcItemSize(d)) / 2;
                                        })
                                        .attr('y', function (d) {
                                            return calcItemY(d) + (item_size - calcItemSize(d)) / 2;
                                        })
                                        .attr('width', function (d) {
                                            return calcItemSize(d);
                                        })
                                        .attr('height', function (d) {
                                            return calcItemSize(d);
                                        })
                                        .each('end', repeat);
                                })();

                                // Construct tooltip
                                var tooltip_html = '';
                                //tooltip_html += '<div class="header"><strong>' + (d.total ? scope.formatTime(d.total) : 'No time') + ' tracked</strong></div>';
                                var eventKeyword = scope.locale == 'en' ? ' events' : ' eventi';
                                tooltip_html += '<div class="header"><strong>' + (d.total) + eventKeyword + '</strong></div>';
                                tooltip_html += '<div>' + moment(d.date).format('dddd, MMM Do YYYY') + '</div><br>';

                                // Add summary to the tooltip
                                angular.forEach(d.summary, function (d) {
                                    tooltip_html += '<div><span><strong>' + d.name + '</strong></span>';
                                    tooltip_html += '<span>' + scope.formatTime(d.value) + '</span></div>';
                                });

                                // Calculate tooltip position
                                var x = calcItemX(d) + item_size;
                                if (width - x < (tooltip_width + tooltip_padding * 3)) {
                                    x -= tooltip_width + tooltip_padding * 2;
                                }
                                var y = calcItemY(d) + item_size;

                                // Show tooltip
                                tooltip.html(tooltip_html)
                                    .style('left', x + 'px')
                                    .style('top', y + 'px')
                                    .transition()
                                    .duration(transition_duration / 2)
                                    .ease('ease-in')
                                    .style('opacity', 1);
                            })
                            .on('mouseout', function () {
                                if (in_transition) { return; }

                                // Set circle radius back to what it's supposed to be
                                d3.select(this).transition()
                                    .duration(transition_duration / 2)
                                    .ease('ease-in')
                                    .attr('x', function (d) {
                                        return calcItemX(d) + (item_size - calcItemSize(d)) / 2;
                                    })
                                    .attr('y', function (d) {
                                        return calcItemY(d) + (item_size - calcItemSize(d)) / 2;
                                    })
                                    .attr('width', function (d) {
                                        return calcItemSize(d);
                                    })
                                    .attr('height', function (d) {
                                        return calcItemSize(d);
                                    });

                                // Hide tooltip
                                scope.hideTooltip();
                            })
                            .transition()
                            .delay(function () {
                                return (Math.cos(Math.PI * Math.random()) + 1) * transition_duration;
                            })
                            .duration(function () {
                                return transition_duration;
                            })
                            .ease('ease-in')
                            .style('opacity', 1)
                            .call(function (transition, callback) {
                                if (transition.empty()) {
                                    callback();
                                }
                                var n = 0;
                                transition
                                    .each(function () { ++n; })
                                    .each('end', function () {
                                        if (!--n) {
                                            callback.apply(this, arguments);
                                        }
                                    });
                            }, function () {
                                in_transition = false;
                            });
                        //items.selectAll('.item-circle').remove();
                        $(".item-circle").each(function (index, htmlText) {
                            var xAttr = $(htmlText).attr("x");
                            var yAttr = $(htmlText).attr("y");
                            var rxAttr = $(htmlText).attr("rx");
                            var ryAttr = $(htmlText).attr("ry");
                            var textAttr = 1;//(htmlText).text();
                            //$(this).after("<text x='" + xAttr + "' y='" + yAttr + "' rx='" + rxAttr + "' ry='" + ryAttr + "' font-family='Verdana' font-size='10' fill='blue'>" + textAttr + "</text>");
                            $(this).after('<text x="' + xAttr + '" y="' + yAttr + '" font-family="Verdana" font-size="7" fill="blue"></text>').text("1");
                        });

                        // Add month labels
                        var month_labels = d3.time.months(start_of_year, end_of_year);
                        var monthScale = d3.scale.linear()
                            .range([0, width])
                            .domain([0, month_labels.length]);
                        labels.selectAll('.label-month').remove();
                        labels.selectAll('.label-month')
                            .data(month_labels)
                            .enter()
                            .append('text')
                            .attr('class', 'label label-month')
                            .attr('font-size', function () {
                                return Math.floor(label_padding / 3) + 'px';
                            })
                            .text(function (d) {
                                return d.toLocaleDateString(scope.locale, { month: 'short' });
                            })
                            .attr('x', function (d, i) {
                                return monthScale(i) + (monthScale(i) - monthScale(i - 1)) / 2;
                            })
                            .attr('y', label_padding / 2)
                            .on('mouseenter', function (d) {
                                if (in_transition) { return; }

                                var selected_month = moment(d);
                                items.selectAll('.item-circle')
                                    .transition()
                                    .duration(transition_duration)
                                    .ease('ease-in')
                                    .style('opacity', function (d) {
                                        return moment(d.date).isSame(selected_month, 'month') ? 1 : 0.1;
                                    });
                            })
                            .on('mouseout', function () {
                                if (in_transition) { return; }

                                items.selectAll('.item-circle')
                                    .transition()
                                    .duration(transition_duration)
                                    .ease('ease-in')
                                    .style('opacity', 1);
                            })
                            .on('click', function (d) {
                                if (in_transition) { return; }

                                // Check month data
                                var month_data = scope.data.filter(function (e) {
                                    return moment(d).startOf('month') <= moment(e.date) && moment(e.date) < moment(d).endOf('month');
                                });

                                // Don't transition if there is no data to show
                                if (!month_data.length) { return; }

                                // Set selected month to the one clicked on
                                scope.selected = { date: d };

                                in_transition = true;

                                // Hide tooltip
                                scope.hideTooltip();

                                // Remove all year overview related items and labels
                                scope.removeYearOverview();

                                // Redraw the chart
                                scope.overview = 'month';
                                scope.drawChart();
                            });

                        // Add day labels
                        var day_labels = d3.time.days(moment().startOf('week'), moment().endOf('week'));
                        var dayScale = d3.scale.ordinal()
                            .rangeRoundBands([label_padding, height])
                            .domain(day_labels.map(function (d) {
                                return moment(d).weekday();
                            }));
                        labels.selectAll('.label-day').remove();
                        labels.selectAll('.label-day')
                            .data(day_labels)
                            .enter()
                            .append('text')
                            .attr('class', 'label label-day')
                            .attr('x', label_padding / 3)
                            .attr('y', function (d, i) {
                                return dayScale(i) + dayScale.rangeBand() / 1.75;
                            })
                            .style('text-anchor', 'left')
                            .attr('font-size', function () {
                                return Math.floor(label_padding / 3) + 'px';
                            })
                            .text(function (d) {
                                return moment(d).format('dddd')[0];
                            })
                            .on('mouseenter', function (d) {
                                if (in_transition) { return; }

                                var selected_day = moment(d);
                                items.selectAll('.item-circle')
                                    .transition()
                                    .duration(transition_duration)
                                    .ease('ease-in')
                                    .style('opacity', function (d) {
                                        return (moment(d.date).day() === selected_day.day()) ? 1 : 0.1;
                                    });
                            })
                            .on('mouseout', function () {
                                if (in_transition) { return; }

                                items.selectAll('.item-circle')
                                    .transition()
                                    .duration(transition_duration)
                                    .ease('ease-in')
                                    .style('opacity', 1);
                            });

                        // Add button to switch back to previous overview
                        scope.drawButton();
                    };


                /**
                 * Draw month overview
                 */
                scope.drawMonthOverview = function () {
                    // Add current overview to the history
                    if (scope.history[scope.history.length - 1] !== scope.overview) {
                        scope.history.push(scope.overview);
                    }
                    debugger;
                    // Define beginning and end of the month
                    var start_of_month = moment(scope.selected.date).startOf('month');
                    var end_of_month = moment(scope.selected.date).endOf('month');

                    // Filter data down to the selected month
                    var month_data = scope.data.filter(function (d) {
                        return start_of_month <= moment(d.date) && moment(d.date) < end_of_month;
                    });
                    var max_value = d3.max(month_data, function (d) {
                        return d3.max(d.summary, function (d) {
                            return d.value;
                        });
                    });

                    // Define day labels and axis
                    var day_labels = d3.time.days(moment().startOf('week'), moment().endOf('week'));
                    var dayScale = d3.scale.ordinal()
                        .rangeRoundBands([label_padding, height])
                        .domain(day_labels.map(function (d) {
                            return moment(d).weekday();
                        }));

                    // Define week labels and axis
                    var week_labels = [start_of_month.clone()];
                    while (start_of_month.week() !== end_of_month.week()) {
                        week_labels.push(start_of_month.add(1, 'week').clone());
                    }
                    var weekScale = d3.scale.ordinal()
                        .rangeRoundBands([label_padding, width], 0.05)
                        .domain(week_labels.map(function (weekday) {
                            return weekday.week();
                        }));

                    // Add month data items to the overview
                    items.selectAll('.item-block-month').remove();
                    var item_block = items.selectAll('.item-block-month')
                        .data(month_data)
                        .enter()
                        .append('g')
                        .attr('class', 'item item-block-month')
                        .attr('width', function () {
                            return (width - label_padding) / week_labels.length - gutter * 5;
                        })
                        .attr('height', function () {
                            return Math.min(dayScale.rangeBand(), max_block_height);
                        })
                        .attr('transform', function (d) {
                            return 'translate(' + weekScale(moment(d.date).week()) + ',' + ((dayScale(moment(d.date).weekday()) + dayScale.rangeBand() / 1.75) - 15) + ')';
                        })
                        .attr('total', function (d) {
                            return d.total;
                        })
                        .attr('date', function (d) {
                            return d.date;
                        })
                        .attr('offset', 0)
                        .on('click', function (d) {
                            if (in_transition) { return; }

                            // Don't transition if there is no data to show
                            if (d.total === 0) { return; }

                            in_transition = true;

                            // Set selected date to the one clicked on
                            scope.selected = d;

                            // Hide tooltip
                            scope.hideTooltip();

                            // Remove all month overview related items and labels
                            scope.removeMonthOverview();

                            // Redraw the chart
                            scope.overview = 'day';
                            scope.drawChart();
                        });

                    var item_width = (width - label_padding) / week_labels.length - gutter * 5;
                    var itemScale = d3.scale.linear()
                        .rangeRound([0, item_width]);

                    item_block.selectAll('.item-block-rect')
                        .data(function (d) {
                            return d.summary;
                        })
                        .enter()
                        .append('rect')
                        .attr('class', 'item item-block-rect')
                        .attr('x', function (d) {
                            var total = parseInt(d3.select(this.parentNode).attr('total'));
                            var offset = parseInt(d3.select(this.parentNode).attr('offset'));
                            itemScale.domain([0, total]);
                            d3.select(this.parentNode).attr('offset', offset + itemScale(d.value));
                            return offset;
                        })
                        .attr('width', function (d) {
                            var total = parseInt(d3.select(this.parentNode).attr('total'));
                            itemScale.domain([0, total]);
                            return Math.max((itemScale(d.value) - item_gutter), 1)
                        })
                        .attr('height', function () {
                            return Math.min(dayScale.rangeBand(), max_block_height);
                        })
                        .attr('fill', function (d) {
                            var color = d3.scale.linear()
                                .range(['#ffffff', scope.color || '#ff4500'])
                                .domain([-0.15 * max_value, max_value]);
                            return color(d.value) || '#ff4500';
                        })
                        .style('opacity', 0)
                        .on('mouseover', function (d) {
                            if (in_transition) { return; }

                            // Get date from the parent node
                            var date = new Date(d3.select(this.parentNode).attr('date'));

                            // Construct tooltip
                            var tooltip_html = '';
                            tooltip_html += '<div class="header"><strong>' + d.name + '</strong></div><br>';
                            tooltip_html += '<div><strong>' + (d.value ? scope.formatTime(d.value) : 'No time') + ' tracked</strong></div>';
                            tooltip_html += '<div>on ' + moment(date).format('dddd, MMM Do YYYY') + '</div>';

                            // Calculate tooltip position
                            var x = weekScale(moment(date).week()) + tooltip_padding;
                            while (width - x < (tooltip_width + tooltip_padding * 3)) {
                                x -= 10;
                            }
                            var y = dayScale(moment(date).weekday()) + tooltip_padding * 2;

                            // Show tooltip
                            tooltip.html(tooltip_html)
                                .style('left', x + 'px')
                                .style('top', y + 'px')
                                .transition()
                                .duration(transition_duration / 2)
                                .ease('ease-in')
                                .style('opacity', 1);
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }
                            scope.hideTooltip();
                        })
                        .transition()
                        .delay(function () {
                            return (Math.cos(Math.PI * Math.random()) + 1) * transition_duration;
                        })
                        .duration(function () {
                            return transition_duration;
                        })
                        .ease('ease-in')
                        .style('opacity', 1)
                        .call(function (transition, callback) {
                            if (transition.empty()) {
                                callback();
                            }
                            var n = 0;
                            transition
                                .each(function () { ++n; })
                                .each('end', function () {
                                    if (!--n) {
                                        callback.apply(this, arguments);
                                    }
                                });
                        }, function () {
                            in_transition = false;
                        });

                    // Add week labels
                    labels.selectAll('.label-week').remove();
                    labels.selectAll('.label-week')
                        .data(week_labels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-week')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return 'Week ' + d.week();
                        })
                        .attr('x', function (d) {
                            return weekScale(d.week());
                        })
                        .attr('y', label_padding / 2)
                        .on('mouseenter', function (weekday) {
                            if (in_transition) { return; }

                            items.selectAll('.item-block-month')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    return (moment(d.date).week() === weekday.week()) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }

                            items.selectAll('.item-block-month')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 1);
                        })
                        .on('click', function (d) {
                            if (in_transition) { return; }

                            // Check week data
                            var week_data = scope.data.filter(function (e) {
                                return d.startOf('week') <= moment(e.date) && moment(e.date) < d.endOf('week');
                            });

                            // Don't transition if there is no data to show
                            if (!week_data.length) { return; }

                            in_transition = true;

                            // Set selected month to the one clicked on
                            scope.selected = { date: d };

                            // Hide tooltip
                            scope.hideTooltip();

                            // Remove all year overview related items and labels
                            scope.removeMonthOverview();

                            // Redraw the chart
                            scope.overview = 'week';
                            scope.drawChart();
                        });


                    // Add day labels
                    labels.selectAll('.label-day').remove();
                    labels.selectAll('.label-day')
                        .data(day_labels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-day')
                        .attr('x', label_padding / 3)
                        .attr('y', function (d, i) {
                            return dayScale(i) + dayScale.rangeBand() / 1.75;
                        })
                        .style('text-anchor', 'left')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return moment(d).format('dddd')[0];
                        })
                        .on('mouseenter', function (d) {
                            if (in_transition) { return; }

                            var selected_day = moment(d);
                            items.selectAll('.item-block-month')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    return (moment(d.date).day() === selected_day.day()) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }

                            items.selectAll('.item-block-month')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 1);
                        });

                    // Add button to switch back to previous overview
                    scope.drawButton();
                };


                /**
                 * Draw week overview
                 */
                scope.drawWeekOverview = function () {
                    // Add current overview to the history
                    if (scope.history[scope.history.length - 1] !== scope.overview) {
                        scope.history.push(scope.overview);
                    }

                    // Define beginning and end of the week
                    var start_of_week = moment(scope.selected.date).startOf('week');
                    var end_of_week = moment(scope.selected.date).endOf('week');

                    // Filter data down to the selected week
                    var week_data = scope.data.filter(function (d) {
                        return start_of_week <= moment(d.date) && moment(d.date) < end_of_week;
                    });
                    var max_value = d3.max(week_data, function (d) {
                        return d3.max(d.summary, function (d) {
                            return d.value;
                        });
                    });

                    // Define day labels and axis
                    var day_labels = d3.time.days(moment().startOf('week'), moment().endOf('week'));
                    var dayScale = d3.scale.ordinal()
                        .rangeRoundBands([label_padding, height])
                        .domain(day_labels.map(function (d) {
                            return moment(d).weekday();
                        }));

                    // Define week labels and axis
                    var week_labels = [start_of_week];
                    var weekScale = d3.scale.ordinal()
                        .rangeRoundBands([label_padding, width], 0.01)
                        .domain(week_labels.map(function (weekday) {
                            return weekday.week();
                        }));

                    // Add week data items to the overview
                    items.selectAll('.item-block-week').remove();
                    var item_block = items.selectAll('.item-block-week')
                        .data(week_data)
                        .enter()
                        .append('g')
                        .attr('class', 'item item-block-week')
                        .attr('width', function () {
                            return (width - label_padding) / week_labels.length - gutter * 5;
                        })
                        .attr('height', function () {
                            return Math.min(dayScale.rangeBand(), max_block_height);
                        })
                        .attr('transform', function (d) {
                            return 'translate(' + weekScale(moment(d.date).week()) + ',' + ((dayScale(moment(d.date).weekday()) + dayScale.rangeBand() / 1.75) - 15) + ')';
                        })
                        .attr('total', function (d) {
                            return d.total;
                        })
                        .attr('date', function (d) {
                            return d.date;
                        })
                        .attr('offset', 0)
                        .on('click', function (d) {
                            if (in_transition) { return; }

                            // Don't transition if there is no data to show
                            if (d.total === 0) { return; }

                            in_transition = true;

                            // Set selected date to the one clicked on
                            scope.selected = d;

                            // Hide tooltip
                            scope.hideTooltip();

                            // Remove all week overview related items and labels
                            scope.removeWeekOverview();

                            // Redraw the chart
                            scope.overview = 'day';
                            scope.drawChart();
                        });

                    var item_width = (width - label_padding) / week_labels.length - gutter * 5;
                    var itemScale = d3.scale.linear()
                        .rangeRound([0, item_width]);

                    item_block.selectAll('.item-block-rect')
                        .data(function (d) {
                            return d.summary;
                        })
                        .enter()
                        .append('rect')
                        .attr('class', 'item item-block-rect')
                        .attr('x', function (d) {
                            var total = parseInt(d3.select(this.parentNode).attr('total'));
                            var offset = parseInt(d3.select(this.parentNode).attr('offset'));
                            itemScale.domain([0, total]);
                            d3.select(this.parentNode).attr('offset', offset + itemScale(d.value));
                            return offset;
                        })
                        .attr('width', function (d) {
                            var total = parseInt(d3.select(this.parentNode).attr('total'));
                            itemScale.domain([0, total]);
                            return Math.max((itemScale(d.value) - item_gutter), 1)
                        })
                        .attr('height', function () {
                            return Math.min(dayScale.rangeBand(), max_block_height);
                        })
                        .attr('fill', function (d) {
                            var color = d3.scale.linear()
                                .range(['#ffffff', scope.color || '#ff4500'])
                                .domain([-0.15 * max_value, max_value]);
                            return color(d.value) || '#ff4500';
                        })
                        .style('opacity', 0)
                        .on('mouseover', function (d) {
                            if (in_transition) { return; }

                            // Get date from the parent node
                            var date = new Date(d3.select(this.parentNode).attr('date'));

                            // Construct tooltip
                            var tooltip_html = '';
                            tooltip_html += '<div class="header"><strong>' + d.name + '</strong></div><br>';
                            tooltip_html += '<div><strong>' + (d.value ? scope.formatTime(d.value) : 'No time') + ' tracked</strong></div>';
                            tooltip_html += '<div>on ' + moment(date).format('dddd, MMM Do YYYY') + '</div>';

                            // Calculate tooltip position
                            var total = parseInt(d3.select(this.parentNode).attr('total'));
                            itemScale.domain([0, total]);
                            var x = parseInt(d3.select(this).attr('x')) + itemScale(d.value) / 4 + tooltip_width / 4;
                            while (width - x < (tooltip_width + tooltip_padding * 3)) {
                                x -= 10;
                            }
                            var y = dayScale(moment(date).weekday()) + tooltip_padding * 1.5;

                            // Show tooltip
                            tooltip.html(tooltip_html)
                                .style('left', x + 'px')
                                .style('top', y + 'px')
                                .transition()
                                .duration(transition_duration / 2)
                                .ease('ease-in')
                                .style('opacity', 1);
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }
                            scope.hideTooltip();
                        })
                        .transition()
                        .delay(function () {
                            return (Math.cos(Math.PI * Math.random()) + 1) * transition_duration;
                        })
                        .duration(function () {
                            return transition_duration;
                        })
                        .ease('ease-in')
                        .style('opacity', 1)
                        .call(function (transition, callback) {
                            if (transition.empty()) {
                                callback();
                            }
                            var n = 0;
                            transition
                                .each(function () { ++n; })
                                .each('end', function () {
                                    if (!--n) {
                                        callback.apply(this, arguments);
                                    }
                                });
                        }, function () {
                            in_transition = false;
                        });

                    // Add week labels
                    labels.selectAll('.label-week').remove();
                    labels.selectAll('.label-week')
                        .data(week_labels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-week')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return 'Week ' + d.week();
                        })
                        .attr('x', function (d) {
                            return weekScale(d.week());
                        })
                        .attr('y', label_padding / 2)
                        .on('mouseenter', function (weekday) {
                            if (in_transition) { return; }

                            items.selectAll('.item-block-week')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    return (moment(d.date).week() === weekday.week()) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }

                            items.selectAll('.item-block-week')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 1);
                        });

                    // Add day labels
                    labels.selectAll('.label-day').remove();
                    labels.selectAll('.label-day')
                        .data(day_labels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-day')
                        .attr('x', label_padding / 3)
                        .attr('y', function (d, i) {
                            return dayScale(i) + dayScale.rangeBand() / 1.75;
                        })
                        .style('text-anchor', 'left')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return moment(d).format('dddd')[0];
                        })
                        .on('mouseenter', function (d) {
                            if (in_transition) { return; }

                            var selected_day = moment(d);
                            items.selectAll('.item-block-week')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    return (moment(d.date).day() === selected_day.day()) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }

                            items.selectAll('.item-block-week')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 1);
                        });

                    // Add button to switch back to previous overview
                    scope.drawButton();
                };


                /**
                 * Draw day overview
                 */
                scope.drawDayOverview = function () {
                    // Add current overview to the history
                    if (scope.history[scope.history.length - 1] !== scope.overview) {
                        scope.history.push(scope.overview);
                    }

                    // Initialize selected date to today if it was not set
                    if (!Object.keys(scope.selected).length) {
                        scope.selected = scope.data[scope.data.length - 1];
                    }

                    var project_labels = scope.selected.summary.map(function (project) {
                        return project.name;
                    });
                    var projectScale = d3.scale.ordinal()
                        .rangeRoundBands([label_padding, height])
                        .domain(project_labels);

                    var itemScale = d3.time.scale()
                        .range([label_padding * 2, width])
                        .domain([moment(scope.selected.date).startOf('day'), moment(scope.selected.date).endOf('day')]);
                    items.selectAll('.item-block').remove();
                    items.selectAll('.item-block')
                        .data(scope.selected.details)
                        .enter()
                        .append('rect')
                        .attr('class', 'item item-block')
                        .attr('x', function (d) {
                            return itemScale(moment(d.date));
                        })
                        .attr('y', function (d) {
                            return (projectScale(d.name) + projectScale.rangeBand() / 2) - 15;
                        })
                        .attr('width', function (d) {
                            var end = itemScale(d3.time.second.offset(moment(d.date), d.value));
                            return Math.max((end - itemScale(moment(d.date))), 1);
                        })
                        .attr('height', function () {
                            return Math.min(projectScale.rangeBand(), max_block_height);
                        })
                        .attr('fill', function () {
                            return scope.color || '#ff4500';
                        })
                        .style('opacity', 0)
                        .on('mouseover', function (d) {
                            if (in_transition) { return; }

                            // Construct tooltip
                            var tooltip_html = '';
                            tooltip_html += '<div class="header"><strong>' + d.name + '</strong><div><br>';
                            tooltip_html += '<div><strong>' + (d.value ? scope.formatTime(d.value) : 'No time') + ' tracked</strong></div>';
                            tooltip_html += '<div>on ' + moment(d.date).format('dddd, MMM Do YYYY HH:mm') + '</div>';

                            // Calculate tooltip position
                            var x = d.value * 100 / (60 * 60 * 24) + itemScale(moment(d.date));
                            while (width - x < (tooltip_width + tooltip_padding * 3)) {
                                x -= 10;
                            }
                            var y = projectScale(d.name) + projectScale.rangeBand() / 2 + tooltip_padding / 2;

                            // Show tooltip
                            tooltip.html(tooltip_html)
                                .style('left', x + 'px')
                                .style('top', y + 'px')
                                .transition()
                                .duration(transition_duration / 2)
                                .ease('ease-in')
                                .style('opacity', 1);
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }
                            scope.hideTooltip();
                        })
                        .on('click', function (d) {
                            if (scope.handler) {
                                console.log("aaa")
                                scope.handler(d);
                            }
                        })
                        .transition()
                        .delay(function () {
                            return (Math.cos(Math.PI * Math.random()) + 1) * transition_duration;
                        })
                        .duration(function () {
                            return transition_duration;
                        })
                        .ease('ease-in')
                        .style('opacity', 0.5)
                        .call(function (transition, callback) {
                            if (transition.empty()) {
                                callback();
                            }
                            var n = 0;
                            transition
                                .each(function () { ++n; })
                                .each('end', function () {
                                    if (!--n) {
                                        callback.apply(this, arguments);
                                    }
                                });
                        }, function () {
                            in_transition = false;
                        });

                    // Add time labels
                    var timeLabels = d3.time.hours(moment(scope.selected.date).startOf('day'), moment(scope.selected.date).endOf('day'));
                    var timeScale = d3.time.scale()
                        .range([label_padding * 2, width])
                        .domain([0, timeLabels.length]);
                    labels.selectAll('.label-time').remove();
                    labels.selectAll('.label-time')
                        .data(timeLabels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-time')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return moment(d).format('HH:mm');
                        })
                        .attr('x', function (d, i) {
                            return timeScale(i);
                        })
                        .attr('y', label_padding / 2)
                        .on('mouseenter', function (d) {
                            if (in_transition) { return; }

                            var selected = itemScale(moment(d));
                            items.selectAll('.item-block')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    var start = itemScale(moment(d.date));
                                    var end = itemScale(moment(d.date).add(d.value, 'seconds'));
                                    return (selected >= start && selected <= end) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }

                            items.selectAll('.item-block')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 0.5);
                        });

                    // Add project labels
                    labels.selectAll('.label-project').remove();
                    labels.selectAll('.label-project')
                        .data(project_labels)
                        .enter()
                        .append('text')
                        .attr('class', 'label label-project')
                        .attr('x', gutter)
                        .attr('y', function (d) {
                            return projectScale(d) + projectScale.rangeBand() / 2;
                        })
                        .attr('min-height', function () {
                            return projectScale.rangeBand();
                        })
                        .style('text-anchor', 'left')
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .text(function (d) {
                            return d;
                        })
                        .each(function () {
                            var obj = d3.select(this),
                                text_length = obj.node().getComputedTextLength(),
                                text = obj.text();
                            while (text_length > (label_padding * 1.5) && text.length > 0) {
                                text = text.slice(0, -1);
                                obj.text(text + '...');
                                text_length = obj.node().getComputedTextLength();
                            }
                        })
                        .on('mouseenter', function (project) {
                            if (in_transition) { return; }

                            items.selectAll('.item-block')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', function (d) {
                                    return (d.name === project) ? 1 : 0.1;
                                });
                        })
                        .on('mouseout', function () {
                            if (in_transition) { return; }

                            items.selectAll('.item-block')
                                .transition()
                                .duration(transition_duration)
                                .ease('ease-in')
                                .style('opacity', 0.5);
                        });

                    // Add button to switch back to previous overview
                    scope.drawButton();
                };


                /**
                 * Draw the button for navigation purposes
                 */
                scope.drawButton = function () {
                    buttons.selectAll('.button').remove();
                    var button = buttons.append('g')
                        .attr('class', 'button button-back')
                        .style('opacity', 0)
                        .on('click', function () {
                            if (in_transition) { return; }

                            // Set transition boolean
                            in_transition = true;

                            // Clean the canvas from whichever overview type was on
                            if (scope.overview === 'year') {
                                scope.removeYearOverview();
                            } else if (scope.overview === 'month') {
                                scope.removeMonthOverview();
                            } else if (scope.overview === 'week') {
                                scope.removeWeekOverview();
                            } else if (scope.overview === 'day') {
                                scope.removeDayOverview();
                            }

                            // Redraw the chart
                            scope.history.pop();
                            scope.overview = scope.history.pop();
                            scope.drawChart();
                        });
                    button.append('circle')
                        .attr('cx', label_padding / 2.25)
                        .attr('cy', label_padding / 2.5)
                        .attr('r', item_size / 2);
                    button.append('text')
                        .attr('x', label_padding / 2.25)
                        .attr('y', label_padding / 2.5)
                        .attr('dy', function () {
                            return Math.floor(width / 100) / 3;
                        })
                        .attr('font-size', function () {
                            return Math.floor(label_padding / 3) + 'px';
                        })
                        .html('&#x2190;');
                    button.transition()
                        .duration(transition_duration)
                        .ease('ease-in')
                        .style('opacity', 1);
                };


                /**
                 * Transition and remove items and labels related to global overview
                 */
                scope.removeGlobalOverview = function () {
                    items.selectAll('.item-block-year')
                        .transition()
                        .duration(transition_duration)
                        .ease('ease-out')
                        .style('opacity', 0)
                        .remove();
                    labels.selectAll('.label-year').remove();
                },


                    /**
                     * Transition and remove items and labels related to year overview
                     */
                    scope.removeYearOverview = function () {
                        items.selectAll('.item-circle')
                            .transition()
                            .duration(transition_duration)
                            .ease('ease')
                            .style('opacity', 0)
                            .remove();
                        labels.selectAll('.label-day').remove();
                        labels.selectAll('.label-month').remove();
                        scope.hideBackButton();
                    };


                /**
                 * Transition and remove items and labels related to month overview
                 */
                scope.removeMonthOverview = function () {
                    items.selectAll('.item-block-month').selectAll('.item-block-rect')
                        .transition()
                        .duration(transition_duration)
                        .ease('ease-in')
                        .style('opacity', 0)
                        .attr('x', function (d, i) {
                            return (i % 2 === 0) ? -width / 3 : width / 3;
                        })
                        .remove();
                    labels.selectAll('.label-day').remove();
                    labels.selectAll('.label-week').remove();
                    scope.hideBackButton();
                };


                /**
                 * Transition and remove items and labels related to week overview
                 */
                scope.removeWeekOverview = function () {
                    items.selectAll('.item-block-week').selectAll('.item-block-rect')
                        .transition()
                        .duration(transition_duration)
                        .ease('ease-in')
                        .style('opacity', 0)
                        .attr('x', function (d, i) {
                            return (i % 2 === 0) ? -width / 3 : width / 3;
                        })
                        .remove();
                    labels.selectAll('.label-day').remove();
                    labels.selectAll('.label-week').remove();
                    scope.hideBackButton();
                };


                /**
                 * Transition and remove items and labels related to daily overview
                 */
                scope.removeDayOverview = function () {
                    items.selectAll('.item-block')
                        .transition()
                        .duration(transition_duration)
                        .ease('ease-in')
                        .style('opacity', 0)
                        .attr('x', function (d, i) {
                            return (i % 2 === 0) ? -width / 3 : width / 3;
                        })
                        .remove();
                    labels.selectAll('.label-time').remove();
                    labels.selectAll('.label-project').remove();
                    scope.hideBackButton();
                };


                /**
                 * Helper function to hide the tooltip
                 */
                scope.hideTooltip = function () {
                    tooltip.transition()
                        .duration(transition_duration / 2)
                        .ease('ease-in')
                        .style('opacity', 0);
                };


                /**
                 * Helper function to hide the back button
                 */
                scope.hideBackButton = function () {
                    buttons.selectAll('.button')
                        .transition()
                        .duration(transition_duration)
                        .ease('ease')
                        .style('opacity', 0)
                        .remove();
                };


                /**
                 * Helper function to convert seconds to a human readable format
                 * @param seconds Integer
                 */
                scope.formatTime = function (seconds) {
                    var hours = Math.floor(seconds / 3600);
                    var minutes = Math.floor((seconds - (hours * 3600)) / 60);
                    var time = '';
                    if (hours > 0) {
                        time += hours === 1 ? '1 hour ' : hours + ' hours ';
                    }
                    if (minutes > 0) {
                        time += minutes === 1 ? '1 minute' : minutes + ' minutes';
                    }
                    if (hours === 0 && minutes === 0) {
                        time = Math.round(seconds) + ' seconds';
                    }
                    return time;
                };
            }
        };
    }]);
