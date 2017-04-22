function setRequiredFields() {
    $('*').each(function () {
        var req = $(this).attr('data-val-required');
        if (undefined != req) {
            var label = $('label[for="' + $(this).attr('id') + '"]');
            var text = label.text();
            if (text.length > 0) {
                label.append('<span style="color:red"> *</span>');
            }
        }
    });
};

var app = angular.module('SocisaApp', ['ngFileUpload', 'ngAnimate', 'ngDialog']);

app.config(['$compileProvider',
  function($compileProvider) {
      $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|file|ftp|blob):|data:image\//);
  }
]);

app.filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});

app.filter('getById', function () {
    return function (input, id) {
        var i = 0, len = input.length;
        for (; i < len; i++) {
            if (+input[i].ID == +id) {
                return input[i];
            }
        }
        return null;
    }
});

app.filter('bytetobase', function () {
    return function (buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    };
});
/*
//only file definition
app.directive("fileread", [function () {
    return {
        scope: {
            fileread: "="
        },
        link: function (scope, element, attributes) {
            element.bind("change", function (changeEvent) {
                scope.$apply(function () {
                    scope.fileread = changeEvent.target.files[0];
                    // or all selected files:
                    // scope.fileread = changeEvent.target.files;
                });
            });
        }
    }
}]);
*/
//full file content
app.directive("fileread", [function () {
    return {
        scope: {
            fileread: "="
        },
        link: function (scope, element, attributes) {
            element.bind("change", function (changeEvent) {
                var reader = new FileReader();
                reader.onload = function (loadEvent) {
                    scope.$apply(function () {
                        scope.fileread = loadEvent.target.result;
                    });
                }
                reader.onerror = function (loadEvent) {
                    alert("Error!");
                }
                //reader.readAsDataURL(changeEvent.target.files[0]);
                reader.readAsArrayBuffer(changeEvent.target.files[0]);
            });
        }
    }
}]);

app.directive('jqdatepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            $(element).datepicker({
                dateFormat: 'dd.mm.yy',
                changeMonth: true,
                changeYear: true,
                onSelect: function (date) {
                    ctrl.$setViewValue(date);
                    ctrl.$render();
                    scope.$apply();
                }
            });
        }
    };
});

app.service("PromiseUtils", function ($q) {
    return {
        getPromiseHttpResult: function (httpPromise) {
            var deferred = $q.defer();
            httpPromise.then(function (data) {
                deferred.resolve(data);
            },function () {
                deferred.reject(arguments);
            });
            return deferred.promise;
        }
    }
});

app.factory('myService', function ($http, $q) {

    this.getlist = function (url) {
        return $http.get(url)
            .then(function (response) {
                return response;
            }, function (response) { return response; })
    }
    return this;
});

app.directive('dynamic', function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic, function (html) {
                ele.html(html);
                $compile(ele.contents())(scope);
            });
        }
    };
});

app.directive('dynamic2', function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic2, function (html) {
                ele.html(html);
                $compile(ele.contents())(scope);
            });
        }
    };
});