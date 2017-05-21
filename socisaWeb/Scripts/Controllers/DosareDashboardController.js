'use strict';
var spinner = new Spinner(opts);

app.controller('DosareDashboardController',
function ($scope, $http, $filter, $rootScope, $window, Upload) {
    $scope.model = [];
    $scope.query = '1';

    $scope.applyFilter = function (element) {
        //alert($scope.query);
        //alert(element.Dosar.ID_SOCIETATE_CASCO + ' - ' + element.Dosar.ID_SOCIETATE_RCA);
        switch ($scope.query) {
            case '1':
                return true;
                break;
            case '2': // dosare Casco
                return element.Dosar.ID_SOCIETATE_CASCO == 2;
                break;
            case '3': // dosare RCA
                return element.Dosar.ID_SOCIETATE_RCA == 4;
                break;
        }
        //return element.name.match(/^Ma/) ? true : false;
    };
});