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

app.controller('DosareDashboardAdminAndSuperController',
function ($scope, $http, $filter, $rootScope, $window) {
    $scope.model = {};
    $scope.model.DosareExtended = [];
    $scope.model.UtilizatoriExtended = [];
    $scope.generalQueryText = {};
    $scope.generalQueryText.$ = null;
    $scope.query = '1';
    $scope.propertyName = 'Dosar.NR_DOSAR_CASCO';
    $scope.UtilizatoriDosare = [];

    $scope.toggleDashboardChecks = function (id_prefix) {
        var chk_All_Id = '#all_' + id_prefix;
        var chk = $(chk_All_Id).prop('checked');
        //var srcId = "input[id^='" + id_prefix + "']";
        //$(srcId).prop('checked', chk);
        //$(srcId).trigger('input'); // Use for Chrome/Firefox/Edge
        //$(srcId).trigger('change'); // Use for Chrome/Firefox/Edge + IE11
        //angular.forEach($scope.model.UtilizatoriExtended, function (value, key) {
        switch (id_prefix) {
            case 'chk_Utilizator':
                angular.forEach($scope.utilizatoriFiltrati, function (value, key) {
                    value.selected = chk;
                });
                break;
            case 'chk_Dosar':
                angular.forEach($scope.dosareFiltrate, function (value, key) {
                    value.selected = chk;
                });
                break;
        }
    };

    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined && $rootScope.activeTab == 'utilizatori') {
            $scope.getUtilizatoriAsignati();
        }
    });

    $rootScope.$watch('activeTab', function (newValue, oldValue) {
        if (newValue == 'utilizatori' && $rootScope.ID_DOSAR != null && $rootScope.ID_DOSAR != undefined && $scope.lastActiveIdDosar != $rootScope.ID_DOSAR) {
            $scope.getUtilizatoriAsignati();
            $scope.lastActiveIdDosar = $rootScope.ID_DOSAR;
        }
    });

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
        //alert(new Date(lastLogin) + ' - ' + new Date(item.Dosar.DATA_CREARE) + ' - ' + new Date(item.Dosar.DATA_AVIZARE));
        if (item.Dosar.ID_SOCIETATE_CASCO == $('#idSoc').val()) {
            toReturn = item.Dosar.DATA_CREARE != null ? new Date(item.Dosar.DATA_CREARE) >= new Date(lastLogin) : false;
        }
        else {
            toReturn = item.Dosar.DATA_AVIZARE != null ? new Date(item.Dosar.DATA_AVIZARE) >= new Date(lastLogin) : false;
        }
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
                            str = $filter('date')(str, 'dd.MM.yyyy');
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
            str = $filter('date')(item.Dosar.DATA_SCA, 'dd.MM.yyyy');
            if (str.indexOf($scope.generalQueryText.DATA_SCA) > -1) {
                //return true;
                toReturn2 = true;
            }
        }
        else {
            toReturn2 = true;
        }

        if ($scope.generalQueryText.DATA_EVENIMENT != null && $scope.generalQueryText.DATA_EVENIMENT != "") {
            str = $filter('date')(item.Dosar.DATA_EVENIMENT, 'dd.MM.yyyy');
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

    $scope.getUtilizatoriAsignati = function () {
        $http.post('/UtilizatoriDosare/GetUtilizatoriAsignati', { 'id_dosar': $rootScope.ID_DOSAR })
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    for (var j = 0; j < $scope.utilizatoriFiltrati.length; j++) {
                        $scope.utilizatoriFiltrati[j].selected = false;
                        for (var i = 0; i < response.data.length; i++) {
                            if ($scope.utilizatoriFiltrati[j].Utilizator.ID == response.data[i].ID)
                            {
                                $scope.utilizatoriFiltrati[j].selected = true;
                                break;
                            }
                        }
                    }
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });

    };

    $scope.AssignDosareToUtilizatori = function () {
        if ($rootScope.ID_DOSAR == null) {
            for (var i = 0; i < $scope.dosareFiltrate.length; i++) {
                if ($scope.dosareFiltrate[i].selected) {
                    for (var j = 0; j < $scope.utilizatoriFiltrati.length; j++) {
                        if ($scope.utilizatoriFiltrati[j].selected) {
                            var ud = { 'ID': null, 'ID_UTILIZATOR': $scope.utilizatoriFiltrati[j].Utilizator.ID, 'ID_DOSAR': $scope.dosareFiltrate[i].Dosar.ID };
                            $scope.UtilizatoriDosare.push(ud);
                            /*
                            // -- pentru inserare one-by-one --
                            $http.post('/UtilizatoriDosare/Edit', { id_utilizator: $scope.utilizatoriFiltrati[j].Utilizator.ID, id_dosar: $scope.dosareFiltrate[i].Dosar.ID })
                                .then(function (response) {
                                    if (response != 'null' && response != null && response.data != null) {
                                        $('.alert').show();
                                        $scope.showMessage = true;
                                        $scope.result = response.data;
    
                                        $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                                    }
                                    spinner.stop();
                                }, function (response) {
                                    spinner.stop();
                                    alert('Erroare: ' + response.status + ' - ' + response.data);
                                });
                            */
                        }
                    }
                }
            }
        }
        else{
            for (var j = 0; j < $scope.utilizatoriFiltrati.length; j++) {
                if ($scope.utilizatoriFiltrati[j].selected) {
                    var ud = { 'ID': null, 'ID_UTILIZATOR': $scope.utilizatoriFiltrati[j].Utilizator.ID, 'ID_DOSAR': $rootScope.ID_DOSAR };
                    $scope.UtilizatoriDosare.push(ud);
                }
            }
        }
        $http.post('/UtilizatoriDosare/Edit', $scope.UtilizatoriDosare)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $('.alert').show();
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                    if ($scope.result.Status) {
                        if ($rootScope.ID_DOSAR == null) {
                            /*
                            $scope.dosareFiltrate = $scope.dosareFiltrate.filter(function (item) {
                                return !item.selected
                            });
                            */
                            $scope.model.DosareExtended = $scope.model.DosareExtended.filter(function (item) {
                                return !item.selected
                            });
                            for (var c = 0; c < $scope.utilizatoriFiltrati.length; c++) {
                                $scope.utilizatoriFiltrati[c].selected = false;
                            }
                        }
                        else {
                            $scope.getUtilizatoriAsignati();
                        }
                    }
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };
});