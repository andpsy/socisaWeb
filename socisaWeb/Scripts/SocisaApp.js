var MESSAGE_DELAY = 3000;
var MESSAGE_FADE_OUT = 2000;
var MESSAGES_REFRESH_RATE = 300000;
var DATE_FORMAT = 'dd.MM.yyyy';
var DATE_TIME_FORMAT = 'dd.MM.yyyy HH:mm:ss';
var ACTIVE_DIV_ID = 'mainDashboard';


function openNav() {
    document.getElementById("mySidenav").style.width = "200px";
    //document.getElementById("main").style.marginLeft = "250px";
    document.getElementById(ACTIVE_DIV_ID).style.marginLeft = "250px";
}
function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
    //document.getElementById("main").style.marginLeft = "50px";
    document.getElementById(ACTIVE_DIV_ID).style.marginLeft = "50px";
}

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

function ToggleDivCss(divId) {
    var did = '#' + divId;
    var top = document.getElementById(ACTIVE_DIV_ID).offsetTop;
    var left = document.getElementById(ACTIVE_DIV_ID).offsetLeft;
    var height = document.getElementById(ACTIVE_DIV_ID).offsetHeight;
    var width = document.getElementById(ACTIVE_DIV_ID).offsetWidth;
    var right = left + width;
    document.getElementById(divId).style.marginLeft = document.getElementById(ACTIVE_DIV_ID).style.marginLeft;
    var tmpAId = '#' + ACTIVE_DIV_ID;
    $(tmpAId).fadeOut(1000, function () {
        $(did).fadeIn(1000);
        /*
        var mDiv = document.getElementById(divId);
        var oDiv = document.getElementById(ACTIVE_DIV_ID);
        var tmpId = mDiv.id;
        mDiv.id = oDiv.id;
        oDiv.id = tmpId;
        */
        ACTIVE_DIV_ID = divId;
    });
};

var app = angular.module('SocisaApp', ['ngFileUpload', 'ngAnimate', 'ngDialog']);

app.run(function ($http) {
    $http.defaults.headers.common['__RequestVerificationToken'] = angular.element('input[name="__RequestVerificationToken"]').attr('value');
    $http.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

    //disable IE ajax request caching
    $http.defaults.headers.common['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
    // extra
    $http.defaults.headers.common['Cache-Control'] = 'no-cache';
    $http.defaults.headers.common['Pragma'] = 'no-cache';
});

app.run(function ($rootScope, $http) {
    $rootScope.DATE_FORMAT = DATE_FORMAT;
    $rootScope.DATE_TIME_FORMAT = DATE_TIME_FORMAT;
    $rootScope.ID_DOSAR = null;

    $rootScope.divId = ACTIVE_DIV_ID;
    $rootScope.HasHtml = []; // aici stocam id-urile div-urilor generate deja, ca sa nu le incarcam de fiecare data.
    $rootScope.HasHtml.push(ACTIVE_DIV_ID);
    $rootScope.Url = "";

    $rootScope.ToggleDiv = function (divId, generateContent, id) {
        $rootScope.ID_DOSAR = id;
        /*
        if (divId != "mainDosareDashboard" && generateContent && $rootScope.ID_DOSAR != null) // pastram id-ul dosarului curent din div-ul de dosare, pentru revenire.
        {
            console.log('1 - ' + $rootScope.ID_DOSAR + ' - ' + $rootScope.TEMP_ID_DOSAR);
            $rootScope.TEMP_ID_DOSAR = $rootScope.ID_DOSAR;
            $rootScope.ID_DOSAR = null;
        }
        if (divId == "mainDosareDashboard" && generateContent && $rootScope.TEMP_ID_DOSAR != null) // pastram id-ul dosarului curent din div-ul de dosare, pentru revenire.
        {
            console.log('2 - ' + $rootScope.ID_DOSAR + ' - ' + $rootScope.TEMP_ID_DOSAR);
            $rootScope.ID_DOSAR = $rootScope.TEMP_ID_DOSAR;
            $rootScope.TEMP_ID_DOSAR = null;
        }
        */
        $rootScope.divId = divId;
        if ((!generateContent || $rootScope.HasHtml.indexOf(divId) > -1) && (id == null || id == undefined)) {  // mai trebuie sa punem conditia pt. id generat deja (link dosare)
            console.log('aici3 - ' + divId);
            ToggleDivCss(divId);
            return;
        }
        else {
            //var url = '';
            switch (divId) {
                case "mainDashboard":
                    //url = '@Url.Action("Index", "Dosare")';
                    $rootScope.Url = '/Dashboard/IndexMain';
                    break;
                case "mainDosareDashboard":
                    //url = '@Url.Action("Index", "Dosare")';
                    $rootScope.Url = '/Dosare/Index';
                    break;
                case "mainDosareDashboardAdminAndSuper":
                    //url = '@Url.Action("GetDosareDashboardAdminAndSuper", "Dashboard")';
                    $rootScope.Url = '/Dashboard/GetDosareDashboardAdminAndSuper';
                    break;
                case "mainDosareDashboardRegular":
                    //url = '@Html.Raw(Url.Action("GetDosareDashboardRegular", "Dashboard"))';
                    $rootScope.Url = '/Dashboard/GetDosareDashboardRegular';
                    break;
                case "mainMesajeDashboard":
                    //url = '@Html.Raw(Url.Action("IndexMain", "Mesaje"))';
                    $rootScope.Url = '/Mesaje/IndexMain';
                    break;
                case "mainUtilizatoriDashboard":
                    //url = '@Html.Raw(Url.Action("Index", "Utilizatori"))';
                    $rootScope.Url = '/Utilizatori/Index';
                    break;
                case "mainDosareImportDashboard":
                    //url = '@Html.Raw(Url.Action("Import", "Dosare"))';
                    $rootScope.Url = '/Dosare/Import';
                    break;
            }
            if (id != null && id != undefined) {
                $rootScope.Url += "/" + id;  // aici vom modifica pt. parametru complex ...
            }
            var did = '#' + divId;

            spinner.spin(document.getElementById(ACTIVE_DIV_ID));
            $http.get($rootScope.Url)
                .then(function (response) {
                    spinner.stop();
                    if (response != 'null' && response != null && response.data != null) {
                        //console.log('aici!!! - ' + divId);
                        $rootScope.html = response.data;
                        /*
                        if (id != null && id != undefined) {
                            $rootScope.ID_DOSAR = id;
                            $rootScope.TEMP_ID_DOSAR = null;
                        }
                        */
                        ToggleDivCss(divId);
                    }
                }, function (response) {
                    alert('Erroare: ' + response.status + ' - ' + response.data);
                    spinner.stop();
                });
        }
    };
});

app.directive('dynamic', function ($compile, $rootScope) {
    return {
        restrict: 'A',
        replace: true,
        scope: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic, function (newValue, oldValue) {
                //console.log(ele.attr('id') + ' - ' + $rootScope.divId + ' - ' + ACTIVE_DIV_ID + ' - ' + $rootScope.HasHtml.indexOf(ele.attr('id')));
                if (ele.attr('id') == $rootScope.divId && ($rootScope.HasHtml.indexOf(ele.attr('id')) == -1 || newValue != oldValue)) {
                    ele.html(newValue);
                    $compile(ele.contents())(scope);
                    $rootScope.HasHtml.push(ele.attr('id'));
                    //console.log('aici2 - ' + ele.attr('id'));
                }
            });
        }
    };
});
/*
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
*/
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
            if (Object.prototype.toString.call(item) === '[object Date]' || jQuery.type(item) === 'date' || (Date.parse(item) !== 'Invalid Date' && !isNaN(Date.parse(item)) && !isNaN(new Date(item).getMonth()) ))
                return item;
            else
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