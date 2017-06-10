'use strict';
/*
function toggleDashboardChecks(id_prefix) {
    //var chk = $('#chk_All').prop('checked');
    //$('.checkForImport').prop('checked', chk);
    var chk_All_Id = '#all_' + id_prefix;
    var chk = $(chk_All_Id).prop('checked');
    var srcId = "input[id^='" + id_prefix + "']";
    $(srcId).prop('checked', chk);
};
*/
var spinner = new Spinner(opts);

app.controller('DosareDashboardRegularController',
function ($scope, $http, $filter, $rootScope, $window) {
    $scope.model = {};
    $scope.model.DosareExtended = [];
    $scope.model.UtilizatoriExtended = [];
    $scope.generalQueryText = {};
    $scope.generalQueryText.$ = null;
    $scope.query = '1';
    $scope.propertyName = 'Dosar.NR_DOSAR_CASCO';
    $scope.UtilizatoriDosare = [];

    $scope.sortBy = function (propertyName) {
        $scope.reverse = ($scope.propertyName === propertyName) ? !$scope.reverse : false;
        $scope.propertyName = propertyName;
    };

    $scope.applyFilter = function (element) {
        var idSoc = $('#idSoc').val();
        switch ($scope.query) {
            case '1':
                return true;
                break;
            case '2': // dosare Casco
                return element.Dosar.ID_SOCIETATE_CASCO == idSoc;
                break;
            case '3': // dosare RCA
                return element.Dosar.ID_SOCIETATE_RCA == idSoc;
                break;
            case '4': // dosare noi
                return $scope.getItemsByDate(element);
                break;
            case '5': // dosare noi CASCO
                return $scope.getItemsByDate(element) && element.Dosar.ID_SOCIETATE_CASCO == idSoc;
                break;
            case '6': // dosare noi RCA
                return $scope.getItemsByDate(element) && element.Dosar.ID_SOCIETATE_RCA == idSoc;
                break;
        }
        //return element.name.match(/^Ma/) ? true : false;
    };

    $scope.myFilter = function (item) {
        return item.Utilizator.ID_SOCIETATE == $('#idSoc').val();
    };

    $scope.getItemsByDate = function (item) {
        var toReturn = false;
        var lastLogin = $('#idLastLogin').val();
        toReturn = item.Dosar.DATA_ULTIMEI_MODIFICARI != null ? new Date(item.Dosar.DATA_ULTIMEI_MODIFICARI) >= new Date(lastLogin) : item.Dosar.DATA_CREARE != null ? new Date(item.Dosar.DATA_CREARE) >= new Date(lastLogin) : true;
        return toReturn;
    };

    $scope.filterByColumns = function (item) {
        if (($scope.generalQueryText.$ == null || $scope.generalQueryText.$ == "") && ($scope.generalQueryText.DATA_SCA == null || $scope.generalQueryText.DATA_SCA == "") && ($scope.generalQueryText.DATA_EVENIMENT == null || $scope.generalQueryText.DATA_EVENIMENT == "")) return true;

        var toReturn1 = false;
        var toReturn2 = false;
        var toReturn3 = false;

        if ($scope.generalQueryText.$ != null && $scope.generalQueryText.$ != "") {
            for (var key_1 in item) { // sub objects (Dosar, AsiguratCasco, AutoCasco etc...)
                var subItem = item[key_1];
                for (var key_2 in subItem) {
                    try {
                        var str = subItem[key_2];
                        if (key_2.indexOf("DATA_") > -1) {
                            str = $filter('date')(str, $rootScope.DATE_FORMAT);
                        }
                        if (str.toString().toLowerCase().indexOf($scope.generalQueryText.$.toLowerCase()) > -1) {
                            //return true;
                            toReturn1 = true;
                            break;
                        }
                    } catch (e) {; }
                }
                if (toReturn1) break;
            }
        }
        else {
            toReturn1 = true;
        }

        if ($scope.generalQueryText.DATA_SCA != null && $scope.generalQueryText.DATA_SCA != "") {
            str = $filter('date')(item.Dosar.DATA_SCA, $rootScope.DATE_FORMAT);
            if (str.indexOf($scope.generalQueryText.DATA_SCA) > -1) {
                //return true;
                toReturn2 = true;
            }
        }
        else {
            toReturn2 = true;
        }

        if ($scope.generalQueryText.DATA_EVENIMENT != null && $scope.generalQueryText.DATA_EVENIMENT != "") {
            str = $filter('date')(item.Dosar.DATA_EVENIMENT, $rootScope.DATE_FORMAT);
            if (str.indexOf($scope.generalQueryText.DATA_EVENIMENT) > -1) {
                //return true;
                toReturn3 = true;
            }
        }
        else {
            toReturn3 = true;
        }
        return toReturn1 && toReturn2 && toReturn3;
    };
});