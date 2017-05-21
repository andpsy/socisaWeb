﻿var MESSAGE_DELAY = 3000;
var MESSAGE_FADE_OUT = 2000;

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

function ToggleDiv(divId) {
    var top = document.getElementById('main').offsetTop;
    var left = document.getElementById('main').offsetLeft;
    var height = document.getElementById('main').offsetHeight;
    var width = document.getElementById('main').offsetWidth;
    var right = left + width;
    //alert('dims: ' + top + '-' + left + '-' + height + '-' + width + '-' + right);
    /*
    document.getElementById('main').style.left = (left - width) + 'px';
    document.getElementById(divId).style.top = top + 'px';
    document.getElementById(divId).style.left = right + 'px';
    document.getElementById(divId).style.height = height + 'px';
    document.getElementById(divId).style.width = width + 'px';
    document.getElementById(divId).style.left = left + 'px';
    */
    //document.getElementById(divId).style.top = top + 'px';
    document.getElementById(divId).style.marginLeft = document.getElementById('main').style.marginLeft;
    $('#main').fadeOut(1000, function () {
        var did = '#' + divId;
        $(did).fadeIn(1000);
        var mDiv = document.getElementById(divId);
        var oDiv = document.getElementById('main');
        var tmpId = mDiv.id;
        mDiv.id = oDiv.id;
        oDiv.id = tmpId;
    });
};

var app = angular.module('SocisaApp', ['ngFileUpload', 'ngAnimate', 'ngDialog']);

app.run(function ($http) {
    $http.defaults.headers.common['__RequestVerificationToken'] = angular.element('input[name="__RequestVerificationToken"]').attr('value');
    $http.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';


    //disable IE ajax request caching
    //$http.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
    // extra
    $http.defaults.headers.common['Cache-Control'] = 'no-cache';
    $http.defaults.headers.common['Pragma'] = 'no-cache';
});

app.config(['$compileProvider',
  function($compileProvider) {
      $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|file|ftp|blob):|data:image\//);
  }
]);
/*
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);
*/
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

    this.getlist = function (method, url, data) {
        if (method == 'GET') {
            return $http.get(url)
                .then(function (response) {
                    return response;
                }, function (response) {
                    return response;
                })
        }
        if (method == 'POST') {
            return $http.post(url, data)
                .then(function (response) {
                    return response;
                }, function (response) {
                    return response;
                })
        }
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

app.directive('aDisabled', function() {
    return {
        compile: function(tElement, tAttrs, transclude) {
            //Disable ngClick
            tAttrs["ngClick"] = "!("+tAttrs["aDisabled"]+") && ("+tAttrs["ngClick"]+")";

            //Toggle "disabled" to class when aDisabled becomes true
            return function (scope, iElement, iAttrs) {
                scope.$watch(iAttrs["aDisabled"], function(newValue) {
                    if (newValue !== undefined) {
                        iElement.toggleClass("disabled", newValue);
                    }
                });

                //Disable href on click
                iElement.on("click", function(e) {
                    if (scope.$eval(iAttrs["aDisabled"])) {
                        e.preventDefault();
                    }
                });
            };
        }
    };
});