(function() {
  'use strict';

  angular.module('view.file', [
    'view.file.tpls',
    'cb.x2js',
    'com.2fdevs.videogular',
    'com.2fdevs.videogular.plugins.controls',
    'hljs',
    'ngJsonExplorer',
    'ngSanitize',
    'RecursionHelper'
  ]);

}());

(function () {
  'use strict';

  angular.module('view.file')
    .controller('FriendlyJsonCtrl', FriendlyJsonCtrl);

  FriendlyJsonCtrl.$inject = ['$scope', '$sce', '$templateCache', '$http'];

  function FriendlyJsonCtrl($scope, $sce, $templateCache, $http) {
    var ctrl = this;
    ctrl.trustUri = function(uri) {
      $sce.trustAsResourceUrl(uri);
    };
    ctrl.load = function(uri) {
      var json = $templateCache.get(uri);

      if (!json) {
        $http.get(uri, {
          cache: $templateCache,
          transformResponse: function(data, headersGetter) {
            // Return the raw string, so $http doesn't parse it
            // if it's json.
            return data;
          }
        }).then(function (response) {
          $scope.loading = false;
          $scope.json = JSON.parse(response.data);
        });
      } else if (angular.isArray(json)) {
        $scope.loading = false;
        $scope.json = JSON.parse(json[1]);
      } else {
        json.then(function(response) {
          $scope.json = JSON.parse(response.data);
        });
      }
    };
  }
}());

 /**
  * @ngdoc directive
  * @memberOf 'view.file'
  * @name friendly-json
  * @description
  *   Angular directive for rendering nested JSON structures in a user-friendly way.
  *
  * @attr {String}    json          Optional. JSON contents to be rendered. Do not use together with uri.
  * @attr {String}    template      Optional. Url of HTML template to use for this directive.
  * @attr {String}    uri           Optional. Url of JSON file to be rendered. Url must be trusted upfront.
  *
  * @example
  * <friendly-json uri="ctrl.viewUri" template="/my/json-template.html"></friendly-json>
  * 
  * or
  * 
  * <friendly-json json="ctrl.json" template="/my/json-template.html"></friendly-json>
  */

(function () {

  'use strict';

  angular.module('view.file')
  .directive('friendlyJson', friendlyJsonDirective)
  .filter('isObject', function() {
    return function(val) {
      return angular.isObject(val);
    };
  })
  .filter('isArray', function() {
    return function(val) {
      return angular.isArray(val);
    };
  })
  .filter('isFunction', function() {
    return function(val) {
      return angular.isFunction(val);
    };
  });

  friendlyJsonDirective.$inject = ['RecursionHelper'];

  function friendlyJsonDirective(RecursionHelper) {
    return {
      restrict: 'E',
      controller: 'FriendlyJsonCtrl',
      controllerAs: 'ctrl',
      scope: {
        uri: '=?',
        json: '=?'
      },
      templateUrl: template,
      compile: function(element) {
        // Use the compile function from the RecursionHelper,
        // And return the linking function(s) which it returns
        return RecursionHelper.compile(element, function ($scope, $elem, $attrs, ctrl) {
          $scope.loading = true;

          $scope.$watch('uri', function(newUri) {
            if (newUri) {
              ctrl.load(newUri);
            }
          });
        });
      }
    };

    function template(element, attrs) {
      var url;

      if (attrs.template) {
        url = attrs.template;
      } else {
        url = '/view-file-ng/friendly-json.html';
      }

      return url;
    }
  }

}());

(function () {
  'use strict';

  angular.module('view.file')
    .controller('FriendlyXmlCtrl', FriendlyXmlCtrl);

  FriendlyXmlCtrl.$inject = ['$scope', '$sce', '$templateCache', '$http', 'x2js'];

  function FriendlyXmlCtrl($scope, $sce, $templateCache, $http, x2js) {
    var ctrl = this;

    ctrl.trustUri = function(uri) {
      $sce.trustAsResourceUrl(uri);
    };

    ctrl.load = function(uri) {
      var xml = $templateCache.get(uri);

      if (!xml) {
        $http.get(uri, {
          cache: $templateCache,
          transformResponse: function(data, headersGetter) {
            // Return the raw string, so $http doesn't parse it
            // if it's json.
            return data;
          }
        }).then(function (response) {
          $scope.loading = false;
          ctrl.parse(response.data);
        });
      } else if (angular.isArray(xml)){
        $scope.loading = false;
        ctrl.parse(xml[1]);
      } else {
        xml.then(function(response) {
          $scope.loading = false;
          ctrl.parse(response.data);
        });
      }
    };

    ctrl.parse = function(xml) {
      /* jshint camelcase: false */
      /* jscs: disable requireCamelCaseOrUpperCaseIdentifiers*/
      $scope.json = x2js.xml_str2json(xml);
      /* jscs: enable requireCamelCaseOrUpperCaseIdentifiers*/
      /* jshint camelcase: true */
    };
  }
}());

 /**
  * @ngdoc directive
  * @memberOf 'view.file'
  * @name friendly-xml
  * @description
  *   Angular directive for rendering nested JSON structures in a user-friendly way.
  *
  * @attr {String}    template      Optional. Url of HTML template to use for this directive.
  * @attr {String}    uri           Optional. Url of XML file to be rendered. Url must be trusted upfront.
  * @attr {String}    xml           Optional. XML contents to be rendered. Do not use together with uri.
  *
  * @example
  * <friendly-xml uri="ctrl.viewUri" template="/my/xml-template.html"></friendly-xml>
  * 
  * or
  * 
  * <friendly-xml xml="ctrl.xml" template="/my/xml-template.html"></friendly-xml>
  */

(function () {

  'use strict';

  angular.module('view.file')
  .directive('friendlyXml', friendlyXmlDirective);

  friendlyXmlDirective.$inject = ['RecursionHelper'];

  function friendlyXmlDirective(RecursionHelper) {
    return {
      restrict: 'E',
      controller: 'FriendlyXmlCtrl',
      controllerAs: 'ctrl',
      scope: {
        uri: '=?',
        xml: '=?'
      },
      templateUrl: template,
      compile: function(element) {
        // Use the compile function from the RecursionHelper,
        // And return the linking function(s) which it returns
        return RecursionHelper.compile(element, function ($scope, $elem, $attrs, ctrl) {
          $scope.loading = true;

          $scope.$watch('uri', function(newUri) {
            if (newUri) {
              ctrl.load(newUri);
            }
          });

          $scope.$watch('xml', function(newXML) {
            if (newXML) {
              ctrl.parse(newXML);
            }
          });
        });
      }
    };

    function template(element, attrs) {
      var url;

      if (attrs.template) {
        url = attrs.template;
      } else {
        url = '/view-file-ng/friendly-json.html';
      }

      return url;
    }
  }

}());

 /**
  * @ngdoc directive
  * @memberOf 'view.file'
  * @name include-safe
  * @description
  *   Angular attribute directive for including sanitized HTML from url.
  *
  * @attr {String}    include-safe  Required. Url of HTML file to be inserted.
  *
  * @example
  * <div include-safe="ctrl.viewUri">Loading..</div>
  */

(function () {

  'use strict';

  angular.module('view.file')
  .directive('includeSafe', includeSafeDirective);

  includeSafeDirective.$inject = ['$http', '$sanitize'];

  function includeSafeDirective($http, $sanitize) {
    // directive factory creates a link function
    return function(scope, element, attrs) {
      scope.$watch(
        function(scope) {
          // watch the 'compile' expression for changes
          return scope.$eval(attrs.includeSafe);
        },
        function(value) {
          $http.get(value).then(function(response) {
            var html = response.data;
            // when the 'compile' expression changes
            // assign it into the current DOM
            element.html($sanitize(html));
          });
        }
      );
    };
  }


}());

 /**
  * @ngdoc service
  * @memberOf 'view.file'
  * @name ModalService
  * @param {service}  $uibModal     angular-bootstrap modal service
  * @description
  *   Angular helper service displaying, and handling modal overlays. Wraps around $uibModal.
  */

(function () {
  'use strict';

  angular.module('view.file')
  .service('ModalService', ['$uibModal', ModalService]);

  function ModalService($modal) {

    var service = {
      show: showModal
    };

    return service;

    /**
     * Show a modal for given template, title, and data.
     * @memberof ModalService
     * @param {String}     template      Required. Url of modal template.
     * @param {String}     title         Optional. Title for modal overlay.
     * @param {Object}     ctrl          Optional. Object with data and callbacks for use on modal. For instance link to parent Ctrl.
     * @param {function}   validate      Optional. Callback function to validate input before closing modal. Expected to return an Array of Strings.
     * @param {Object}     modalOptions  Optional. Additional modal options.
     * @returns {Promise}  result        Promise for updated ctrl if ok was selected, or null if cancel.
     */
    function showModal(template, title, ctrl, validate, modalOptions) {
      return $modal.open(
        angular.extend({
          templateUrl: template+'',
          controller: ['$scope', '$uibModalInstance', 'title', 'ctrl', 'validate', function ($scope, $modalInstance, title, ctrl, validate) {
            $scope.title = title;
            $scope.ctrl = ctrl;
            $scope.alerts = [];
            $scope.ok = function () {
              if (validate) {
                $scope.alerts = validate($scope);
              }
              if ($scope.alerts.length === 0) {
                $modalInstance.close($scope.ctrl);
              }
            };
            $scope.cancel = function () {
              $modalInstance.dismiss('cancel');
            };
          }],
          size: 'lg',
          resolve: {
            title: function () {
              return title;
            },
            ctrl: function () {
              return ctrl;
            },
            validate: function () {
              return validate || function($s) {
                var errors = [];
                if ($s.form) {
                  angular.forEach($s.form.$error, function(err, key) {
                    angular.forEach(err, function(e, index) {
                      errors.push(key + ': ' + (e.$name || index));
                    });
                  });
                }
                return errors;
              };
            }
          }
        }, modalOptions)
      ).result;
    }
  }
}());

(function () {
  'use strict';

  angular.module('view.file')
    .controller('ViewFileCtrl', ViewFileCtrl);

  ViewFileCtrl.$inject = ['$scope', '$sce', '$templateCache', '$http', 'ModalService'];

  function ViewFileCtrl($scope, $sce, $templateCache, $http, modal) {
    var ctrl = this;
    ctrl.trustUri = function(uri) {
      $sce.trustAsResourceUrl(uri);
    };
    ctrl.loadHljs = function(uri) {
      $scope.loading = !$templateCache.get(uri);

      if ($scope.loading) {
        $http.get(uri, {
          cache: $templateCache,
          transformResponse: function(data, headersGetter) {
            // Return the raw string, so $http doesn't parse it
            // if it's json.
            return data;
          }
        }).then(function (result) {
          $scope.loading = false;
          $scope.hljsUri = uri;
        });
      } else {
        $scope.hljsUri = uri;
      }
    };
    ctrl.showModal = function() {
      if ($scope.allowModal) {
        modal.show('/view-file-ng/show-file.modal.html', null, $scope);
      }
    };
    ctrl.toggleCode = function() {
      $scope.showCode = !$scope.showCode;
    };
  }
}());

 /**
  * @ngdoc directive
  * @memberOf 'view.file'
  * @name view-file
  * @description
  *   Angular directive for viewing files. Leverages a.o. highlightjs, json-explorer, sanitize, videogular, x2js.
  *
  * @attr {String}    data          Optional. Data of file to be viewed. Do not use together with uri.
  * @attr {String}    uri           Optional. Url of file to be viewed.
  * @attr {String}    content-type  Required. Mime-type of file to be viewed.
  * @attr {Boolean}   allow-modal   Optional. Allow opening of file in modal overlay. Default: true.
  * @attr {Boolean}   controls      Optional. Show controls on left. Default: true if download-uri or allow-modal.
  * @attr {String}    download-uri  Optional. Url of file for download purpose. Default: null.
  * @attr {String}    file-name     Optional. Filename for display. Default: uri portion after last /.
  * @attr {Boolean}   show-code     Optional. Show raw code initially for JSON, HTML, Text, and XML. Default: false.
  * @attr {String}    template      Optional. Url of HTML template to use for this directive.
  * @attr {Boolean}   trust-uri     Optional. Apply trustAsResourceUrl on uri (not recommended). Default: false.
  *
  * @example
  * <view-file uri="ctrl.viewUri" content-type="ctrl.contentType"
  *   allow-modal="true" controls="true" download-uri="ctrl.downloadUri" file-name="ctrl.fileName"
  *   show-code="false" template="/my/view-template.html" trust-uri="false">
  * </view-file>
  *
  * or
  *
  * <view-file data="ctrl.viewData" content-type="ctrl.contentType"
  *   allow-modal="true" controls="true" download-uri="ctrl.downloadUri" file-name="ctrl.fileName"
  *   show-code="false" template="/my/view-template.html" trust-uri="false">
  * </view-file>
  */

(function () {

  'use strict';

  angular.module('view.file')
  .directive('viewFile', viewFileDirective);

  function viewFileDirective() {

    function isTrue(b, def) {
      if (b !== undefined) {
        return b === true || b === 'true';
      } else {
        return def;
      }
    }

    function getFileType(contentType) {
      var type = 'other';
      if (/\/[x]?html/.test(contentType)) {
        type = 'html';
      } else if (/[\+\/]json$/.test(contentType)) {
        type = 'json';
      } else if (/[\+\/]xml$/.test(contentType)) {
        type = 'xml';
      } else if (/^(audio|image|text|video|xml)\//.test(contentType)) {
        type = contentType.split('/')[0];
      } else if (/^application\//.test(contentType)) {
        // TODO
      }
      return type;
    }

    return {
      restrict: 'E',
      controller: 'ViewFileCtrl',
      controllerAs: 'ctrl',
      scope: {
        data: '=?',
        uri: '=?',
        contentType: '=',
        _allowModal: '@allowModal',
        _controls: '@controls',
        downloadUri: '=?',
        fileName: '=?',
        _showCode: '@showCode',
        _trustUri: '@trustUri'
      },
      templateUrl: template,
      link: function ($scope, $elem, $attrs, ctrl) {

        $scope.allowModal = isTrue($scope._allowModal, true);
        $scope.controls = isTrue($scope._controls, $scope.allowModal || !!$scope.downloadUri);
        $scope.showCode = isTrue($scope._showCode, false);
        $scope.trustUri = isTrue($scope._trustUri, false);

        if ($attrs.uri) {
          $scope.loading = true;
          $scope.$watch('uri', function(newUri) {
            if (newUri) {
              $scope.fileName = $scope.fileName || newUri.split('/').pop();
              $scope.fileType = getFileType($scope.contentType);

              if ($scope.trustUri) {
                ctrl.trustUri(newUri);
              }
              if ($scope.fileType === 'xml') {
                ctrl.loadHljs(newUri);
              } else {
                $scope.loading = false;
              }
            }
          });
        } else {
          $scope.loading = false;
          $scope.fileType = getFileType($scope.contentType);
        }
      }
    };

    function template(element, attrs) {
      var url;

      if (attrs.template) {
        url = attrs.template;
      } else {
        url = '/view-file-ng/view-file.html';
      }

      return url;
    }
  }

}());

 /**
  * @ngdoc directive
  * @memberOf 'view.file'
  * @name view-object
  * @description
  *   Angular directive for including HTML object tag dynamically.
  *
  * @attr {String}    data          Required. Url of file to be viewed. Url must be trusted upfront.
  * @attr {String}    type          Optional. Mime-type of file to be viewed.
  * @attr {String}    height        Optional. Height value to be applied to object tag.
  * @attr {String}    width         Optional. Width value to be applied to object tag.
  *
  * @example
  * <view-object data="ctrl.viewUri" type="ctrl.contentType" height="'600px'" width="'100%'"></view-object>
  */

(function () {
  'use strict';

  angular.module('view.file')
    .directive('viewObject', ['$compile', function($compile) {
      return {
        restrict: 'E',
        link: function(scope, element, attrs) {
          // prepare object tag attributes
          var data = ' data="' + scope.$eval(attrs.data) + '"';
          var type = attrs.type ? (' type="' + scope.$eval(attrs.type) + '"') : '';
          var height = attrs.height ? (' height="' + scope.$eval(attrs.height) + '"') : '';
          var width = attrs.width ? (' width="' + scope.$eval(attrs.width) + '"') : '';

          // tried transclude before, but that didn't seem to work..
          var innerHtml = element.html();
          element.html('<object " ' + height + width + type + data + '>' + innerHtml + '</object>');
          $compile(element.contents())(scope);
        }
      };
    }]);

}());
(function(module) {
try {
  module = angular.module('view.file.tpls');
} catch (e) {
  module = angular.module('view.file.tpls', []);
}
module.run(['$templateCache', function($templateCache) {
  $templateCache.put('/view-file-ng/friendly-json.html',
    '<dl class="dl-horizontal">\n' +
    '  <span ng-repeat="(key,val) in json track by $index" ng-hide="(val | isFunction) || (key.startsWith(\'_\') && key !== \'__text\')">\n' +
    '    <dt>{{ key.startsWith(\'__\') ? key.replace(\'__\', \'\') : key }}</dt>\n' +
    '    <!-- simple value -->\n' +
    '    <dd ng-if="!(val | isObject)">{{ val !== \'\' ? val : \'&#160;\' }}</dd>\n' +
    '    <!-- array or object -->\n' +
    '    <span ng-if="(val | isObject)">\n' +
    '      <!-- array with simple values -->\n' +
    '      <dd ng-if="(val | isArray) && !(val[0] | isObject)">{{ val.join(\', \') }}</dd>\n' +
    '      <!-- add nbsp for better alignment of values -->\n' +
    '      <dd ng-if="!(val | isArray) || (val[0] | isObject)">&#160;</dd>\n' +
    '      <!-- object, recurse -->\n' +
    '      <dd ng-if="!(val | isArray)">\n' +
    '        <!--span ng-init="json = val" ng-include="\'/view-file-ng/friendly-json.html\'"></span-->\n' +
    '        <friendly-json json="val"></friendly-json>\n' +
    '      </dd>\n' +
    '      <!-- array of object, repeat recurse -->\n' +
    '      <dd ng-if="(val | isArray) && (val[0] | isObject)" ng-repeat="v in val track by $index">\n' +
    '        <!--span ng-repeat="json in val track by $index" ng-include="\'/view-file-ng/friendly-json.html\'"></span-->\n' +
    '        <friendly-json json="v"></friendly-json>\n' +
    '      </dd>\n' +
    '    </span>\n' +
    '  </span>\n' +
    '</dl>\n' +
    '');
}]);
})();

(function(module) {
try {
  module = angular.module('view.file.tpls');
} catch (e) {
  module = angular.module('view.file.tpls', []);
}
module.run(['$templateCache', function($templateCache) {
  $templateCache.put('/view-file-ng/show-file.modal.html',
    '<form class="form" role="form">\n' +
    '  <div class="modal-header">\n' +
    '    <h3 class="modal-title">{{ ctrl.fileName }}</h3>\n' +
    '  </div>\n' +
    '  <div ng-cloak class="modal-body clearfix">\n' +
    '    <view-file uri="ctrl.uri" content-type="ctrl.contentType" controls="false" show-code="{{ctrl.showCode}}"></view-file>\n' +
    '  </div>\n' +
    '  <div class="modal-footer">\n' +
    '    <button class="btn btn-primary pull-right" ng-click="cancel()">Close</button>\n' +
    '  </div>\n' +
    '</form>');
}]);
})();

(function(module) {
try {
  module = angular.module('view.file.tpls');
} catch (e) {
  module = angular.module('view.file.tpls', []);
}
module.run(['$templateCache', function($templateCache) {
  $templateCache.put('/view-file-ng/view-file.html',
    '<div class="view-file container-fluid" ng-class="fileType">\n' +
    '  <div class="row">\n' +
    '    <div class="controls col-sm-1 text-right" ng-if="controls && (((fileType === \'xml\') || (fileType === \'json\') || (fileType === \'html\') || (fileType === \'text\')) || allowModal || downloadUri)">\n' +
    '      <div class="code-control">\n' +
    '        <a ng-if="(fileType === \'xml\') || (fileType === \'json\') || (fileType === \'html\') || (fileType === \'text\')" class="btn btn-default" ng-click="ctrl.toggleCode()">\n' +
    '          <span ng-show="!showCode && ((fileType === \'json\') || (fileType === \'text\'))">{ }</span>\n' +
    '          <i ng-show="!showCode && ((fileType === \'html\') || (fileType === \'xml\'))" class="fa fa-code"></i>\n' +
    '          <i ng-show="showCode" class="fa fa-align-left"></i>\n' +
    '        </a>\n' +
    '      </div>\n' +
    '\n' +
    '      <div class="modal-control">\n' +
    '        <a ng-if="allowModal && fileType !== \'audio\'" class="btn btn-default" ng-click="ctrl.showModal()"><i class="glyphicon glyphicon-resize-full"></i></a>\n' +
    '      </div>\n' +
    '\n' +
    '      <div class="download-control">\n' +
    '        <a ng-if="downloadUri" class="btn btn-default" ng-href="{{downloadUri}}" target="_blank" download><i class="glyphicon glyphicon-download-alt"></i></a>\n' +
    '      </div>\n' +
    '    </div>\n' +
    '\n' +
    '    <div class="viewer-wrapper" ng-class="{\'col-sm-11\': controls, \'col-sm-12\': !controls}">\n' +
    '      <div class="loading" ng-show="loading">\n' +
    '        Loading... <i class="fa fa-spinner fa-spin"></i>\n' +
    '      </div>\n' +
    '\n' +
    '      <div class="viewer" ng-hide="loading">\n' +
    '        <!-- audio / video -->\n' +
    '        <div class="source" ng-if="(fileType === \'audio\' || fileType === \'video\') && uri">\n' +
    '          <videogular ng-class="fileType" ng-if="uri">\n' +
    '            <vg-media vg-src="uri"></vg-media>\n' +
    '            <vg-controls>\n' +
    '              <vg-play-pause-button></vg-play-pause-button>\n' +
    '              <vg-time-display>{{ currentTime | date:\'mm:ss\' }}</vg-time-display>\n' +
    '              <vg-scrub-bar>\n' +
    '                <vg-scrub-bar-current-time></vg-scrub-bar-current-time>\n' +
    '              </vg-scrub-bar>\n' +
    '              <vg-time-display>{{ timeLeft | date:\'mm:ss\' }}</vg-time-display>\n' +
    '              <vg-volume>\n' +
    '                <vg-mute-button></vg-mute-button>\n' +
    '                <vg-volume-bar></vg-volume-bar>\n' +
    '              </vg-volume>\n' +
    '              <vg-fullscreen-button ng-show="fileType === \'video\'"></vg-fullscreen-button>\n' +
    '            </vg-controls>\n' +
    '          </videogular>\n' +
    '        </div>\n' +
    '        <div ng-if="(fileType === \'audio\' || fileType === \'video\') && !uri" class="alert alert-warning">\n' +
    '          Data view not supported for audio and video\n' +
    '        </div>\n' +
    '\n' +
    '        <!-- html / text -->\n' +
    '        <div ng-if="(fileType === \'html\') || (fileType === \'text\')">\n' +
    '          <div ng-if="!showCode && uri">\n' +
    '            <div class="source" include-safe="uri"></div>\n' +
    '          </div>\n' +
    '          <div ng-if="!showCode && !uri">\n' +
    '            <div class="source" ng-bind-html="data"></div>\n' +
    '          </div>\n' +
    '          <div ng-if="showCode && uri">\n' +
    '            <hljs hljs-include="uri"></hljs>\n' +
    '          </div>\n' +
    '          <div ng-if="showCode && !uri">\n' +
    '            <hljs hljs-source="data"></hljs>\n' +
    '          </div>\n' +
    '        </div>\n' +
    '\n' +
    '        <!-- image -->\n' +
    '        <div class="source text-center" ng-if="fileType === \'image\' && uri">\n' +
    '          <img ng-src="{{uri}}">\n' +
    '        </div>\n' +
    '        <div class="alert alert-warning" ng-if="fileType === \'image\' && !uri">\n' +
    '          Data view not supported for images\n' +
    '        </div>\n' +
    '\n' +
    '        <!-- json -->\n' +
    '        <div ng-if="fileType === \'json\'">\n' +
    '          <div ng-if="!showCode && uri">\n' +
    '            <friendly-json class="source" uri="uri"></friendly-json>\n' +
    '          </div>\n' +
    '          <div ng-if="!showCode && !uri">\n' +
    '            <friendly-json class="source" json="data"></friendly-json>\n' +
    '          </div>\n' +
    '          <div ng-if="showCode && uri">\n' +
    '            <json-explorer class="source" url="uri"></json-explorer>\n' +
    '          </div>\n' +
    '          <div ng-if="showCode && !uri">\n' +
    '            <json-explorer class="source" json-data="data"></json-explorer>\n' +
    '          </div>\n' +
    '        </div>\n' +
    '\n' +
    '        <!-- xml -->\n' +
    '        <div ng-if="fileType === \'xml\'">\n' +
    '          <div ng-if="!showCode && uri">\n' +
    '            <friendly-xml class="source" uri="uri"></friendly-xml>\n' +
    '          </div>\n' +
    '          <div ng-if="!showCode && !uri">\n' +
    '            <friendly-xml class="source" xml="data"></friendly-xml>\n' +
    '          </div>\n' +
    '          <div ng-if="showCode && uri">\n' +
    '            <hljs hljs-include="hljsUri"></hljs>\n' +
    '          </div>\n' +
    '          <div ng-if="showCode && !uri">\n' +
    '            <hljs hljs-source="data"></hljs>\n' +
    '          </div>\n' +
    '        </div>\n' +
    '\n' +
    '        <!-- other -->\n' +
    '        <view-object class="source" ng-if="fileType === \'other\' && uri" data="uri" type="contentType">\n' +
    '          <a ng-show="downloadUri" class="btn btn-default" ng-href="{{downloadUri}}">Download</a>\n' +
    '          <div ng-show="!downloadUri" class="alert alert-warning">Alert: cannnot display this file!</div>\n' +
    '        </view-object>\n' +
    '        <div class="alert alert-warning" ng-if="fileType === \'other\' && !uri">\n' +
    '          Data view not supported for binaries\n' +
    '        </div>\n' +
    '      </div>\n' +
    '    </div>\n' +
    '\n' +
    '  </div>\n' +
    '</div>');
}]);
})();
