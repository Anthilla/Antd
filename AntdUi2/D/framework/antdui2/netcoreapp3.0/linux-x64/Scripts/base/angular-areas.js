var ngAreas = angular.module('ngAreas', []);

ngAreas.version = '1.0.2';
ngAreas._areas = {};
ngAreas.areaid = -1;
ngAreas.directive("ngAreas", ['$parse', function ($parse) {
	return {
		restrict : 'A',
		controller : function($scope, $element, $attrs) {
			var imageSelectAreas = function() { };
			var mainImageSelectAreas = new imageSelectAreas();
			var allow = (typeof $attrs.ngAreasAllow === 'undefined') ? {}: eval('('+$attrs.ngAreasAllow+')');
	    	var getParameters = function(){
	    			$scope.ngAreas_areas = $scope.$eval($attrs.ngAreas);
	    			console.log(JSON.stringify($scope.ngAreas_areas));
						return  {
							width : (typeof $attrs.ngAreasWidth === 'undefined') ? 800 : $attrs.ngAreasWidth,
							areas : (typeof $attrs.ngAreas ==='undefined') ? [] : $scope.ngAreas_areas,
							onLoaded : (typeof $attrs.ngAreasOnLoad === 'undefined') ? null: $scope.$eval($attrs.ngAreasOnLoad),
							onAdd : (typeof $attrs.ngAreasOnAdd === 'undefined') ? null : $scope.$eval($attrs.ngAreasOnAdd),
							onDelete : (typeof $attrs.ngAreasOnRemove === 'undefined') ? null : $scope.$eval($attrs.ngAreasOnRemove),
							onChanged : (typeof $attrs.ngAreasOnChange === 'undefined') ? null: $scope.$eval($attrs.ngAreasOnChange),
							allowEdit:   (typeof allow.edit === 'undefined') ? true :allow.edit,
			                allowMove:   (typeof allow.move === 'undefined') ? true :allow.move,
			                allowResize: (typeof allow.resize === 'undefined') ? true :allow.resize,
			                allowSelect: (typeof allow.select === 'undefined') ? true :allow.select,
			                allowDelete: (typeof allow.remove === 'undefined') ? true :allow.remove,
			                allowNudge:  (typeof allow.nudge === 'undefined') ? true :allow.nudge
					};
	    	}
				var destroy = function(){
					console.log('ngAreas:destroy');
					mainImageSelectAreas.destroy();
				}

				var reload = function(){
					console.log('ngAreas:reload');
					mainImageSelectAreas.init($element, getParameters());
					console.log('global Areas: '+JSON.stringify(ngAreas._areas));
				}

				var renameByAreaId = function(event, args) {
					var areaid = args.areaid;
					var name = args.name;
					mainImageSelectAreas.renameArea(areaid, name);
					$(".name-area-id-"+areaid).html(name);
				}

				$scope.$on("ngAreas:reload", reload);

				$scope.$on("ngAreas:destroy", destroy);

				$scope.$on("ngAreas:renameByAreaId", renameByAreaId);

				$element.css('display', 'none');
	    	$element.bind('load', function() {
					mainImageSelectAreas.init($element, getParameters());
					$element.css('display', 'block');
	      });

			 $element.bind('error', function(){
				 $.error( "Method " +  customOptions + " on load error" );
			 });

		   $scope.imageArea = function(parent, areaid) {
		        var options = parent.options,
		            $image = parent.$image,
		            $trigger = parent.$trigger,
		            $outline,
		            $selection,
		            $resizeHandlers = {},
		            $btDelete,
		            resizeHorizontally = true,
		            resizeVertically = true,
		            selectionOffset = [0, 0],
		            selectionOrigin = [0, 0],
		            area =$.extend({
		                areaid: areaid,
		                x: 0,
		                y: 0,
		                z: 0,
		                height: 0,
		                width: 0,
		                name: 'no name',
		                cssClass: ""
		            },(typeof $attrs.ngAreasExtendData ==='undefined') ? {} : $scope.$eval($attrs.ngAreasExtendData)),
		            blur = function () {
		                area.z = 0;
		                refresh("blur");
		            },
		            focus = function () {
		                parent.blurAll();
		                area.z = 100;
		                refresh();
		            },
		            getData = function () {
		                return area;
		            },
		            fireEvent = function (event) {
		                $image.trigger(event, [ parent.removeRatio(area), parent.areas()]);
		            },
		            cancelEvent = function (e) {
		                var event = e || window.event || {};
		                event.cancelBubble = true;
		                event.returnValue = false;
		                event.stopPropagation && event.stopPropagation(); // jshint ignore: line
		                event.preventDefault && event.preventDefault(); // jshint ignore: line
		            },
		            off = function() {
		                $.each(arguments, function (key, val) {
		                    on(val);
		                });
		            },
		            on = function (type, handler) {
		                var browserEvent, mobileEvent;
		                switch (type) {
		                    case "start":
		                        browserEvent = "mousedown";
		                        mobileEvent = "touchstart";
		                        break;
		                    case "move":
		                        browserEvent = "mousemove";
		                        mobileEvent = "touchmove";
		                        break;
		                    case "stop":
		                        browserEvent = "mouseup";
		                        mobileEvent = "touchend";
		                        break;
		                    default:
		                        return;
		                }
		                if (handler && jQuery.isFunction(handler)) {
		                    $(window.document).on(browserEvent, handler).on(mobileEvent, handler);
		                } else {
		                    $(window.document).off(browserEvent).off(mobileEvent);
		                }
		            },
		            updateSelection = function () {
		                // Update the outline layer
		            	$outline.addClass(area.cssClass);
		            	$outline.css({
		                    cursor: "default",
		                    width: area.width,
		                    height: area.height,
		                    left: area.x - 3,
		                    top: area.y - 3
		                });

		                // Update the selection layer
		            	$selection.addClass(area.cssClass);

		                $selection.css({
		                    backgroundPosition : ( - area.x - 1) + "px " + ( - area.y - 1) + "px",
		                    cursor : options.allowMove ? "move" : "default",
		                    width: (area.width - 2 > 0) ? (area.width - 2) : 0,
		                    height: (area.height - 2 > 0) ? (area.height - 2) : 0,
		                    left : area.x - 2,
		                    top : area.y - 2,
		                    "z-index": area.z + 2
		                });
		                $name = $($selection).empty().append($("<div><span class=\"select-area-field-label "+area.cssClass+" border-thin name-area-id-"+area.areaid+"\">"+area.name+"</span></div>"));
		            },
		            updateResizeHandlers = function (show) {
		                if (! options.allowResize) {
		                    return;
		                }
		                if (show) {
		                    $.each($resizeHandlers, function(name, $handler) {
		                        var top,
		                            left,
		                            semiwidth = Math.round($handler.width() / 2),
		                            semiheight = Math.round($handler.height() / 2),
		                            vertical = name[0],
		                            horizontal = name[name.length - 1];

		                        if (vertical === "n") {             // ====== North* ======
		                            top = - semiheight - 10;

		                        } else if (vertical === "s") {      // ====== South* ======
		                            top = area.height - semiheight - 1;

		                        } else {                            // === East & West ===
		                            top = Math.round(area.height / 2) - semiheight - 1;
		                        }

		                        if (horizontal === "e") {           // ====== *East ======
		                            left = area.width - semiwidth - 1;

		                        } else if (horizontal === "w") {    // ====== *West ======
		                            left = - semiwidth - ((semiwidth> 20)? 10: 5);

		                        } else {                            // == North & South ==
		                            left = Math.round(area.width / 2) - semiwidth - 1;
		                        }

		                        $handler.css({
		                            display: "block",
		                            left: area.x + left,
		                            top: area.y + top,
		                            "z-index": area.z + 1
		                        });
		                    });
		                } else {
		                    $(".select-areas-resize-handler").each(function() {
		                        $(this).css({ display: "none" });
		                    });
		                }
		            },
		            updateBtDelete = function (visible) {
		                if ($btDelete) {
		                    $btDelete.css({
		                        display: visible ? "block" : "none",
		                        left: area.x + area.width + 1,
		                        top: area.y - $btDelete.outerHeight() - 1,
		                        "z-index": area.z + 1
		                    });
		                }
		            },
		            updateCursor = function (cursorType) {
		                $outline.css({
		                    cursor: cursorType
		                });

		                $selection.css({
		                    cursor: cursorType
		                });
		            },
		            refresh = function(sender) {
		                switch (sender) {
		                    case "startSelection":
		                        parent._refresh();
		                        updateSelection();
		                        updateResizeHandlers();
		                        updateBtDelete(true);
		                        break;

		                    case "pickSelection":
		                    case "pickResizeHandler":
		                        updateResizeHandlers();
		                        break;

		                    case "resizeSelection":
		                        updateSelection();
		                        updateResizeHandlers();
		                        updateCursor("crosshair");
		                        updateBtDelete(true);
		                        break;

		                    case "moveSelection":
		                        updateSelection();
		                        updateResizeHandlers();
		                        updateCursor("move");
		                        updateBtDelete(true);
		                        break;

		                    case "blur":
		                        updateSelection();
		                        updateResizeHandlers();
		                        updateBtDelete();
		                        break;

		                    //case "releaseSelection":
		                    default:
		                        updateSelection();
		                        updateResizeHandlers(true);
		                        updateBtDelete(true);
		                }
		            },
		            startSelection  = function (event) {
		                cancelEvent(event);

		                // Reset the selection size
		                area.width = options.minSize[0];
		                area.height = options.minSize[1];
		                focus();
		                on("move", resizeSelection);
		                on("stop", releaseSelection);

		                // Get the selection origin
		                selectionOrigin = getMousePosition(event);
		                if (selectionOrigin[0] + area.width > $image.width()) {
		                    selectionOrigin[0] = $image.width() - area.width;
		                }
		                if (selectionOrigin[1] + area.height > $image.height()) {
		                    selectionOrigin[1] = $image.height() - area.height;
		                }
		                // And set its position
		                area.x = selectionOrigin[0];
		                area.y = selectionOrigin[1];
		                area.newBox = true;

		                refresh("startSelection");
		            },
		            pickSelection = function (event) {
		                cancelEvent(event);
		                focus();
		                on("move", moveSelection);
		                on("stop", releaseSelection);

		                var mousePosition = getMousePosition(event);

		                // Get the selection offset relative to the mouse position
		                selectionOffset[0] = mousePosition[0] - area.x;
		                selectionOffset[1] = mousePosition[1] - area.y;

		                refresh("pickSelection");
		            },
		            pickResizeHandler = function (event) {
		                cancelEvent(event);
		                focus();

		                var card = event.target.className.split(" ")[2];
		                if (card[card.length - 1] === "w") {
		                    selectionOrigin[0] += area.width;
		                    area.x = selectionOrigin[0] - area.width;
		                }
		                if (card[0] === "n") {
		                    selectionOrigin[1] += area.height;
		                    area.y = selectionOrigin[1] - area.height;
		                }
		                if (card === "n" || card === "s") {
		                    resizeHorizontally = false;
		                } else if (card === "e" || card === "w") {
		                    resizeVertically = false;
		                }

		                on("move", resizeSelection);
		                on("stop", releaseSelection);

		                refresh("pickResizeHandler");
		            },
		            resizeSelection = function (event) {
		                cancelEvent(event);
		                focus();

		                var mousePosition = getMousePosition(event);

		                // Get the selection size
		                var height = mousePosition[1] - selectionOrigin[1],
		                    width = mousePosition[0] - selectionOrigin[0];

		                // If the selection size is smaller than the minimum size set it to minimum size
		                if (Math.abs(width) < options.minSize[0]) {
		                    width = (width >= 0) ? options.minSize[0] : - options.minSize[0];
		                }
		                if (Math.abs(height) < options.minSize[1]) {
		                    height = (height >= 0) ? options.minSize[1] : - options.minSize[1];
		                }
		                // Test if the selection size exceeds the image bounds
		                if (selectionOrigin[0] + width < 0 || selectionOrigin[0] + width > $image.width()) {
		                    width = - width;
		                }
		                if (selectionOrigin[1] + height < 0 || selectionOrigin[1] + height > $image.height()) {
		                    height = - height;
		                }
		                // Test if the selection size is bigger than the maximum size (ignored if minSize > maxSize)
		                if (options.maxSize[0] > options.minSize[0] && options.maxSize[1] > options.minSize[1]) {
		                    if (Math.abs(width) > options.maxSize[0]) {
		                        width = (width >= 0) ? options.maxSize[0] : - options.maxSize[0];
		                    }

		                    if (Math.abs(height) > options.maxSize[1]) {
		                        height = (height >= 0) ? options.maxSize[1] : - options.maxSize[1];
		                    }
		                }

		                // Set the selection size
		                if (resizeHorizontally) {
		                    area.width = width;
		                }
		                if (resizeVertically) {
		                    area.height = height;
		                }

		                if (area.width < 0) {
		                    area.width = Math.abs(area.width);
		                    area.x = selectionOrigin[0] - area.width;
		                } else {
		                    area.x = selectionOrigin[0];
		                }
		                if (area.height < 0) {
		                    area.height = Math.abs(area.height);
		                    area.y = selectionOrigin[1] - area.height;
		                } else {
		                    area.y = selectionOrigin[1];
		                }

		                fireEvent("changing");
		                refresh("resizeSelection");
		            },
		            moveSelection = function (event) {
		                cancelEvent(event);
		                if (! options.allowMove) {
		                    return;
		                }
		                focus();

		                var mousePosition = getMousePosition(event);
		                moveTo({
		                    x: mousePosition[0] - selectionOffset[0],
		                    y: mousePosition[1] - selectionOffset[1]
		                });

		                fireEvent("changing");
		            },
		            moveTo = function (point) {
		                // Set the selection position on the x-axis relative to the bounds
		                // of the image
		                if (point.x > 0) {
		                    if (point.x + area.width < $image.width()) {
		                        area.x = point.x;
		                    } else {
		                        area.x = $image.width() - area.width;
		                    }
		                } else {
		                    area.x = 0;
		                }
		                // Set the selection position on the y-axis relative to the bounds
		                // of the image
		                if (point.y > 0) {
		                    if (point.y + area.height < $image.height()) {
		                        area.y = point.y;
		                    } else {
		                        area.y = $image.height() - area.height;
		                    }
		                } else {
		                    area.y = 0;
		                }
		                refresh("moveSelection");
		            },
		            releaseSelection = function (event) {
		                cancelEvent(event);
		                off("move", "stop");

		                // Update the selection origin
		                selectionOrigin[0] = area.x;
		                selectionOrigin[1] = area.y;

		                // Reset the resize constraints
		                resizeHorizontally = true;
		                resizeVertically = true;

		                if(typeof area.id === 'undefined' && area.newBox ){
		                	area.newBox = false;
		                	delete area.newBox;
		                	fireEvent("onAdd");
		                }else{
		                	fireEvent("changed");
			              }

		                refresh("releaseSelection");
		            },
		            deleteSelection = function (event) {
		                cancelEvent(event);
		                $selection.remove();
		                $outline.remove();
		                $.each($resizeHandlers, function(card, $handler) {
		                    $handler.remove();
		                });
		                $btDelete.remove();
		                parent._remove(areaid);
		                fireEvent("onDelete");
		                fireEvent("changed");
		            },
		            getElementOffset = function (object) {
		                var offset = $(object).offset();

		                return [offset.left, offset.top];
		            },
		            getMousePosition = function (event) {
		                var imageOffset = getElementOffset($image);

		                if (! event.pageX) {
		                    if (event.originalEvent) {
		                        event = event.originalEvent;
		                    }

		                    if(event.changedTouches) {
		                        event = event.changedTouches[0];
		                    }

		                    if(event.touches) {
		                        event = event.touches[0];
		                    }
		                }
		                var x = event.pageX - imageOffset[0],
		                    y = event.pageY - imageOffset[1];

		                x = (x < 0) ? 0 : (x > $image.width()) ? $image.width() : x;
		                y = (y < 0) ? 0 : (y > $image.height()) ? $image.height() : y;

		                return [x, y];
		            };


		        // Initialize an outline layer and place it above the trigger layer
		        $outline = $("<div class=\"ngAreas-element select-areas-outline\" />")
		            .css({
		                opacity : options.outlineOpacity,
		                position : "absolute"
		            })
		            .insertAfter($trigger);

		        // Initialize a selection layer and place it above the outline layer
		        $selection = $("<div class=\"ngAreas-element  border-medium\" />")
		            .addClass("select-areas-background-area")
		            .css({
		                background : "#fff url(" + $image.attr("src") + ") no-repeat",
		                backgroundSize : $image.width() + "px",
		                position : "absolute"
		            })
		            .insertAfter($outline);

		        // Initialize all handlers
		        if (options.allowResize) {
		            $.each(["nw", "n", "ne", "e", "se", "s", "sw", "w"], function (key, card) {
		                $resizeHandlers[card] =  $("<div class=\"ngAreas-element select-areas-resize-handler " + card + "\"/>")
		                    .css({
		                        opacity : 0.5,
		                        position : "absolute",
		                        cursor : card + "-resize"
		                    })
		                    .insertAfter($selection)
		                    .mousedown(pickResizeHandler)
		                    .bind("touchstart", pickResizeHandler);
		            });
		        }
		        // initialize delete button
		        if (options.allowDelete) {
		            var bindToDelete = function ($obj) {
		                $obj.click(deleteSelection)
		                    .bind("touchstart", deleteSelection)
		                    .bind("tap", deleteSelection);
		                return $obj;
		            };
		            $btDelete = bindToDelete($("<div class=\"ngAreas-element delete-area\" />"))
		                .append(bindToDelete($("<div class=\"ngAreas-element select-areas-delete-area\" />")))
		                .insertAfter($selection);
		        }

		        if (options.allowMove) {
		            $selection.mousedown(pickSelection).bind("touchstart", pickSelection);
		        }

		        focus();

		        return {
		            getData: getData,
		            startSelection: startSelection,
		            deleteSelection: deleteSelection,
		            options: options,
		            blur: blur,
		            focus: focus,
		            nudge: function (point) {
		                point.x = area.x;
		                point.y = area.y;
		                if (point.d) {
		                    point.y = area.y + point.d;
		                }
		                if (point.u) {
		                    point.y = area.y - point.u;
		                }
		                if (point.l) {
		                    point.x = area.x - point.l;
		                }
		                if (point.r) {
		                    point.x = area.x + point.r;
		                }
		                moveTo(point);
		                fireEvent("changing");
		            },
		            set: function (dimensions, silent) {
		                area = $.extend(area, dimensions);
		                selectionOrigin[0] = area.x;
		                selectionOrigin[1] = area.y;
		                if (! silent) {
		                    fireEvent("loaded");
		                }
		            },
		            contains: function (point) {
		                return (point.x >= area.x) && (point.x <= area.x + area.width) &&
		                       (point.y >= area.y) && (point.y <= area.y + area.height);
		            }
		        };
		    };


		    imageSelectAreas.prototype.init = function (object, customOptions) {
		        var that = this,
		            defaultOptions = {
		                allowEdit: true,
		                allowMove: true,
		                allowResize: true,
		                allowSelect: true,
		                allowDelete: true,
		                allowNudge: true,
		                ratio: 1,
		                minSize: [0, 0],
		                maxSize: [0, 0],
		                width: 0,
		                maxAreas: 0,
		                outlineOpacity: 0.5,
		                overlayOpacity: 0.5,
		                areas: [],
		                onLoaded: null,
		                onChanging: null,
		                onChanged: null,
		                onAdd: null,
		                onDelete: null
		            };

		        this.options = $.extend(defaultOptions, customOptions);

		        if (! this.options.allowEdit) {
		            this.options.allowSelect = this.options.allowMove = this.options.allowResize = this.options.allowDelete = false;
		        }

		        this._areas = ngAreas._areas;

		        // Initialize the image layer
		        this.$image = $(object);

		        if (this.options.width && this.$image.width() && this.options.width !== this.$image.width()) {
		        	this.options.ratio = this.options.width / this.$image[0].naturalWidth;
		            this.$image.width(this.options.width);
		        }

		        if (this.options.onChanging) {
		            this.$image.on("changing", this.options.onChanging);
		        }
		        if (this.options.onChanged) {
		            this.$image.on("changed", this.options.onChanged);
		        }
		        if (this.options.onAdd) {
		            this.$image.on("onAdd", this.options.onAdd);
		        }
		        if (this.options.onDelete) {
		        	this.$image.on("onDelete", this.options.onDelete);
		        }

		        if (this.options.onLoaded) {
		            this.$image.on("loaded", this.options.onLoaded);
		        }

		        // Initialize an image holder
		        this.$holder = $("<div class=\"ngAreas-element ngAreas-holder\"/>")
		            .css({
		                position : "relative",
		                width: this.$image.width(),
		                height: this.$image.height()
		            });

		        // Wrap the holder around the image
		        this.$image.wrap(this.$holder)
		            .css({
		                position : "absolute"
		            });

		        // Initialize an overlay layer and place it above the image
		        this.$overlay = $("<div class=\"ngAreas-element select-areas-overlay\" />")
		            .css({
		                opacity : this.options.overlayOpacity,
		                position : "absolute",
		                width: this.$image.width(),
		                height: this.$image.height()
		            })
		            .insertAfter(this.$image);

		        // Initialize a trigger layer and place it above the overlay layer
		        this.$trigger = $("<div class=\"ngAreas-element\" />")
		            .css({
		                backgroundColor : "#000000",
		                opacity : 0,
		                position : "absolute",
		                width: this.$image.width(),
		                height: this.$image.height()
		            })
		            .insertAfter(this.$overlay);
		        var that = this;
		        $.each(this.options.areas, function (key, area) {
		            that._add(that.applyRatio(area), false);
		        });


		        this.blurAll();
		        this._refresh();

		        if (this.options.allowSelect) {
		            // Bind an event handler to the "mousedown" event of the trigger layer
		            this.$trigger.mousedown($.proxy(this.newArea, this)).on("touchstart", $.proxy(this.newArea, this));
		        }
		        if (this.options.allowNudge) {
		            $('html').keydown(function (e) { // move selection with arrow keys
		                var codes = {
		                        37: "l",
		                        38: "u",
		                        39: "r",
		                        40: "d"
		                    },
		                    direction = codes[e.which],
		                    selectedArea;

		                if (direction) {
		                    that._eachArea(function (area) {
		                        if (area.getData().z === 100) {
		                            selectedArea = area;
		                            return false;
		                        }
		                    });
		                    if (selectedArea) {
		                        var move = {};
		                        move[direction] = 1;
		                        selectedArea.nudge(move);
		                    }
		                }
		            });
		        }
		    };

		    imageSelectAreas.prototype.applyRatio = function (area) {
		    	  var that = this;
  		    	var apply = function (val){
  		    		return Math.round(val * that.options.ratio);
  		    	}

  		    	var tmp = $.extend({}, area);
  		    	tmp.x = apply(tmp.x);
  		    	tmp.y = apply(tmp.y);
  		    	tmp.width = apply(tmp.width);
  		    	tmp.height = apply(tmp.height);

  		    	return tmp;
	      };

		    imageSelectAreas.prototype.removeRatio = function (area) {
		    		var that = this;
  		    	var removeIt = function (val){
  		    		return Math.round(val / that.options.ratio);
  		    	}

  		    	var tmp = $.extend({}, area);
  		    	tmp.x = removeIt(tmp.x);
  		    	tmp.y = removeIt(tmp.y);
        	  tmp.width = removeIt(tmp.width);
        	  tmp.height = removeIt(tmp.height);

        	  return tmp;
	      };


		    imageSelectAreas.prototype._refresh = function () {
		        var nbAreas = this.areas().length;
		        this.$overlay.css({
		            display : nbAreas? "block" : "none"
		        });
		        if (nbAreas) {
		            this.$image.addClass("blurred");
		        } else {
		            this.$image.removeClass("blurred");
		        }
		        this.$trigger.css({
		            cursor : this.options.allowSelect ? "crosshair" : "default"
		        });
		    };

		    imageSelectAreas.prototype._eachArea = function (cb) {
		        $.each(this._areas, function (areaid, area) {
		            if (area) {
		                return cb(area, areaid);
		            }
		        });
		    };

		    imageSelectAreas.prototype._remove = function (areaid) {
		        delete this._areas[areaid];
		        this._refresh();
		    };

		    imageSelectAreas.prototype.remove = function (areaid) {
		        if (this._areas[areaid]) {
		            this._areas[areaid].deleteSelection();
		        }
		    };

		    imageSelectAreas.prototype.newArea = function (event) {
		        var areaid = -1;
		        this.blurAll();
		        if (this.options.maxAreas && this.options.maxAreas <=  this.areas().length) {
		            return areaid;
		        }
		        this._eachArea(function (area, index) {
		        	areaid = Math.max(areaid, parseInt(index, 10));
		        });
		        ngAreas.areaid += 1;
		        areaid = ngAreas.areaid;
		        this._areas[areaid] = $scope.imageArea(this, areaid);
		        if (event) {
		            this._areas[areaid].startSelection(event);
		        }
		        return areaid;
		    };

		    imageSelectAreas.prototype.set = function (areaid, options, silent) {
		        if (this._areas[areaid]) {
		        	options.areaid = areaid;
		            this._areas[areaid].set(options, silent);
		            this._areas[areaid].focus();
		        }
		    };

		    imageSelectAreas.prototype._add = function (options, silent) {
		        var areaid = this.newArea();
		        this.set(areaid, options, silent);
		    };

		    imageSelectAreas.prototype.add = function (options) {
		        var that = this;
		        this.blurAll();
		        if ($.isArray(options)) {
		            $.each(options, function (key, val) {
		                that._add(val);
		            });
		        } else {
		            this._add(options);
		        }
		        this._refresh();
		        if (! this.options.allowSelect && ! this.options.allowMove && ! this.options.allowResize && ! this.options.allowDelete) {
		            this.blurAll();
		        }
		    };

		    imageSelectAreas.prototype.reset = function () {
		        var that = this;
		        this._eachArea(function (area, areaid) {
		            delete that._areas[areaid];
		        });
		        this._refresh();
		    };

		    imageSelectAreas.prototype.destroy = function (element) {
		      this.reset();
			    $('.ngAreas-holder').find('img').unwrap();
	        $('.ngAreas-element').remove();
	        this.$overlay.remove();
	        this.$trigger.remove();
	        this.$image.removeData("mainImageSelectAreas");
	      };

		    imageSelectAreas.prototype.areas = function () {
		        var ret = [];
		        var that = this;
		        var areas = this.relativeAreas();
	          for (var i = 0; i < areas.length; i++) {
		            ret[i] = $.extend({}, areas[i]);
		            ret[i] = that.removeRatio(ret[i]);
		        }

		        return ret;
		    };

		    imageSelectAreas.prototype.relativeAreas = function () {
		    	var ret = [];
		        this._eachArea(function (area) {
		            ret.push(area.getData());
		        });
		        return ret;
		    };

		    imageSelectAreas.prototype.renameArea = function (areaid, name) {
		        this._eachArea(function (area) {
		            if(area.getData().areaid===areaid){
		            	area.getData().name = name;
		            }
		        });
		    };
		    imageSelectAreas.prototype.blurAll = function () {
		        this._eachArea(function (area) {
		            area.blur();
		        });
		    };

		    imageSelectAreas.prototype.contains  = function (point) {
		        var res = false;
		        this._eachArea(function (area) {
		            if (area.contains(point)) {
		                res = true;
		                return false;
		            }
		        });
		        return res;
		    };


		    var execCommand = function (currentObject, command){
		    	if ( imageSelectAreas.prototype[command] ) { // Method call
		            var ret = imageSelectAreas.prototype[ command ].apply( $scope.selectAreas(currentObject), Array.prototype.slice.call( arguments, 1 ));
		            return typeof ret === "undefined" ? this : ret;

		        }else{
		        	console.error("error command "+ command +" not found!");
		        }
		    }
		},
		link : function(scope, element, attrs) {
			console.log('linked ngSelectAreas link');
		}
	};
}]);
