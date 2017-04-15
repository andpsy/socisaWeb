﻿var lastkeytime = new Date();
var lastruntime = new Date();
var waiting_interval = 1000; // miliseconds
var lastfiltervalue = "";

var spinner = new Spinner(opts);
'use strict';

function showDosareSideNav(on_off) {
    var x = document.getElementById("dosareSideNav");
    var relTarget = document.getElementById("DosareSearch");
    var rect = relTarget.getBoundingClientRect();
    if (on_off) {
        x.style.top = (Math.round(rect.top))-5 + 'px'; // relTarget.style.top;
        x.style.left = (Math.round(rect.left))-5 + 'px'; // relTarget.style.left;
        x.style.height = (Math.round(rect.bottom) - Math.round(rect.top))+10 + 'px'; // relTarget.style.height;
        x.style.width = '250px';
    }
    else {
        x.style.top = 0;
        x.style.left = 0;
        x.style.height = 0;
        x.style.width = 0;
    }
}

app.controller('DosareNavigatorController',
function ($scope, $http, $filter, $rootScope) {
    $scope.searchMode = 1;
    $scope.TempDosarFilter = {};
    $scope.editMode = 0;
    $scope.DosarFiltru = {};
    $scope.curDosarIndex = -1;
    $scope.TempDosarEdit = {};
    $scope.result = {};

    $scope.$watch('DosarFiltru.Dosar', function (newValue, oldValue) {
        try {
            document.getElementById("Dosar_ID_SOCIETATE_CASCO").value = newValue.ID_SOCIETATE_CASCO;
        } catch (e) {
            document.getElementById("Dosar_ID_SOCIETATE_RCA").selectedIndex = -1;
        }
        //document.getElementById("lnkDocumenteScanateDetalii").className  = document.getElementById("lnkMesajeDetalii").className  = newValue.ID == null ? 'disabled' : '';
    });
    
    $scope.$watch('editMode', function (newValue, oldValue) {
        document.getElementById('Dosar_DATA_EVENIMENT').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_SCA').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_AVIZARE').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_NOTIFICARE').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_ULTIMEI_MODIFICARI').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_IESIRE_CASCO').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_INTRARE_RCA').disabled = !(newValue == 1);
        document.getElementById('Dosar_ID_SOCIETATE_RCA').disabled = !(newValue == 1);
        document.getElementById("lnkDocumenteScanateDetalii").className  = document.getElementById("lnkMesajeDetalii").className  = newValue == 1 ? 'disabled' : '';
    });

    $scope.$watch('searchMode', function (newValue, oldValue) {
        document.getElementById('Dosar_ID_SOCIETATE_RCA').disabled = newValue != 2 && $scope.editMode != 1;
    });

    $scope.$watch('DosarFiltru.Dosar.ID_SOCIETATE_RCA', function (newValue, oldValue) {
        document.getElementById("Dosar_ID_SOCIETATE_RCA").value = newValue;
    });

    $scope.$watch('ID_SOCIETATE_RCA', function (newValue, oldValue) {
        $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = newValue;
        $scope.Afisare(newValue);
    });

    $scope.$watch('DosarFiltru.Dosar.AVIZAT', function (newValue, oldValue) {
        if ($scope.searchMode == 2) {
            $scope.Afisare(newValue);
        }
    });

    $scope.$watch('DosarFiltru.dosarJson.DataEvenimentEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataScaEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataAvizareEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataNotificareEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataUltimeiModificariEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataIesireCascoEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataIntrareRcaEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.Afisare(newValue);
        }
    });

    $scope.$watch('DosarFiltru.Dosar.DATA_EVENIMENT', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_EVENIMENT = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_SCA', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_SCA = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_AVIZARE', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_AVIZARE = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_NOTIFICARE', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_NOTIFICARE = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_ULTIMEI_MODIFICARI', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_ULTIMEI_MODIFICARI = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_IESIRE_CASCO', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_IESIRE_CASCO = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_INTRARE_RCA', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_INTRARE_RCA = $filter('date')(new Date(parseInt(newDate.substr(6))), 'dd.MM.yyyy');
    });

    $scope.formatDate = function(dateValue){
        return $filter('date')(new Date(parseInt(dateValue.substr(6))), 'dd.MM.yyyy');
    }

    $scope.compareObjects = function (o1, o2) {
        return angular.equals(o1, o2);
    }

    $scope.ClearFilters = function () {
        $scope.searchMode = 1;
        $scope.tmpIdSocietateCasco = $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO;
        $scope.DosarFiltru.dosarJson = {};
        $scope.DosarFiltru.Dosar = {};
        $scope.DosarFiltru.SocietatiCASCO = [{}];
        $scope.DosarFiltru.SocietatiRCA = [{}];
        $scope.DosarFiltru.DosareResult = undefined;
        $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.tmpIdSocietateCasco;
        angular.copy($scope.DosarFiltru, $scope.TempDosarFilter);
    };

    $scope.Afisare_Old = function (e) {
        if ($scope.editMode == 1) return;
        
        if ($scope.DosarFiltru.DosareResult == undefined) // !!!
            angular.copy($scope.DosarFiltru, $scope.TempDosarFilter);
        
        var now = new Date();
        var filter_value = e;

        if ((now > lastruntime && (filter_value.length == undefined || filter_value.length >= 3)) ||
            (now > lastruntime && filter_value != lastfiltervalue && now - lastkeytime > waiting_interval)) {
            $scope.Afisare2(e);
        }
        else {
            setTimeout(Afisare, waiting_interval);
        }
        lastkeytime = now;
    };

    $scope.Afisare = function (e) {
        if ($scope.editMode == 1) return;

        angular.copy($scope.DosarFiltru, $scope.TempDosarFilter);
        var now = new Date();
        var filter_value = e;
        if ((now - lastkeytime <= waiting_interval && lastfiltervalue != filter_value) || filter_value.length < 3) {
            setTimeout(Afisare, now - lastkeytime);
            lastkeytime = now;
            return;
        }
        else {
            $scope.Afisare2(e);
            lastkeytime = now;
        }
    };

    $scope.Afisare2 = function (input_value) {
        //spinner = new Spinner(opts).spin(document.getElementById('main'));
        spinner.spin(document.getElementById('main'));
        var data = $scope.DosarFiltru;
        $http.post('/Dosare/Search', data, {
            headers:
            { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            if (response != 'null' && response != null && response.data != null && response.data.Result != null && response.data.Result != "") {
                //$scope.result = response.data;
                $scope.DosarFiltru.DosareResult = response.data.Result;
                $scope.TempDosarFilter.DosareResult = response.data.Result;
                $scope.curDosarIndex = 0;
                $scope.ShowDosar($scope.curDosarIndex);
            }
            else {
                $scope.searchMode = 2;
                $scope.TempDosarFilter.DosareResult = undefined;
                angular.copy($scope.TempDosarFilter, $scope.DosarFiltru);
            }
            spinner.stop();
            lastruntime = new Date();
            lastfiltervalue = input_value;
        }, function (response) {
            alert('Erroare: ' + response.status + ' - ' + response.data);
            spinner.stop();
        });
    };

    $scope.SwitchBackToSearchMode = function () {
        if ($scope.editMode == 1) return;

        $scope.tmpIdSocietateCasco = $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO;
        $scope.searchMode = 2;
        angular.copy($scope.TempDosarFilter, $scope.DosarFiltru);
        $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.tmpIdSocietateCasco;
    };

    $scope.firstDosar = function () {
        if ($scope.DosarFiltru.DosareResult != undefined) {
            var idx = 0;
            $scope.ShowDosar(idx);
        }
    };
    $scope.prevDosar = function () {
        if ($scope.DosarFiltru.DosareResult != undefined) {
            var idx = $scope.curDosarIndex == 0 ? 0 : $scope.curDosarIndex - 1;
            $scope.ShowDosar(idx);
        }
    };
    $scope.nextDosar = function () {
        if ($scope.DosarFiltru.DosareResult != undefined) {
            var idx = $scope.curDosarIndex == $scope.DosarFiltru.DosareResult.length - 1 ? $scope.DosarFiltru.DosareResult.length - 1 : $scope.curDosarIndex + 1;
            $scope.ShowDosar(idx);
        }
    };
    $scope.lastDosar = function () {
        if ($scope.DosarFiltru.DosareResult != undefined) {
            var idx = $scope.DosarFiltru.DosareResult.length - 1;
            $scope.ShowDosar(idx);
        }
    };

    $scope.ShowDosar = function (index) {               
        $scope.searchMode = 1;
        $scope.curDosarIndex = index;
        angular.copy($scope.DosarFiltru.DosareResult[index], $scope.DosarFiltru.Dosar); // !!!!!!!
        $rootScope.ID_DOSAR = $scope.DosarFiltru.Dosar.ID;
        document.getElementById("lnkDocumenteScanateDetalii").className = document.getElementById("lnkMesajeDetalii").className = '';

        //spinner = new Spinner(opts).spin(document.getElementById('main'));
        //$scope.DosarFiltru.Dosar.DATA_EVENIMENT = $scope.formatDate($scope.DosarFiltru.Dosar.DATA_EVENIMENT);
        var data = $scope.DosarFiltru.Dosar.ID;
        $http.get('/Dosare/Details/' + data, {
            headers:
            { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            if (response != 'null' && response != null && response.data != null) {
                var j = JSON.parse(response.data);
                try {
                    $scope.DosarFiltru.dosarJson.NumeAsiguratCasco = j.aCasco.DENUMIRE;
                    $scope.DosarFiltru.dosarJson.NumeAsiguratRca = j.aRca.DENUMIRE;
                    $scope.DosarFiltru.dosarJson.NumarAutoCasco = j.autoCasco.NR_AUTO;
                    $scope.DosarFiltru.dosarJson.NumarAutoRca = j.autoRca.NR_AUTO;
                    $scope.DosarFiltru.dosarJson.NumeIntervenient = j.intervenient.DENUMIRE;
                    $scope.DosarFiltru.dosarJson.TipDosar = j.tipDosar.DENUMIRE;
                } catch (e) { }
            }
            //spinner.stop();
        }, function (response) {
            alert('Erroare: ' + response.status + ' - ' + response.data);
            //spinner.stop();
        });        
    };

    $scope.CheckChanged = function (e) {
        if ($scope.searchMode == 2) {
            $scope.Afisare2(e.target.checked);
        }
    };

    $scope.EnterEditMode = function () {
        $scope.searchMode = 0;
        $scope.editMode = 1;
        angular.copy($scope.DosarFiltru, $scope.TempDosarEdit);
    };

    $scope.EnterAddMode = function () {
        $scope.searchMode = 0;
        $scope.editMode = 1;
        angular.copy($scope.DosarFiltru, $scope.TempDosarEdit);
        $scope.DosarFiltru.Dosar = {};
        $scope.DosarFiltru.dosarJson = {};
        $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.TempDosarEdit.Dosar.ID_SOCIETATE_CASCO;
    };

    $scope.SaveEdit = function () {
        $scope.editMode = 2;
        spinner.spin(document.getElementById('main'))
        var data = $scope.DosarFiltru;
        /*
        try {
            data.Dosar.DATA_EVENIMENT = new Date(parseInt(data.Dosar.DATA_EVENIMENT.substr(6)));
        } catch (e) { data.Dosar.DATA_EVENIMENT = null; }
        try {
            data.Dosar.DATA_SCA = new Date(parseInt(data.Dosar.DATA_SCA.substr(6)));
        } catch (e) { data.Dosar.DATA_SCA = null; }
        try {
            data.Dosar.DATA_AVIZARE = new Date(parseInt(data.Dosar.DATA_AVIZARE.substr(6)));
        } catch (e) { data.Dosar.DATA_AVIZARE = null; }
        try {
            data.Dosar.DATA_NOTIFICARE = new Date(parseInt(data.Dosar.DATA_NOTIFICARE.substr(6)));
        } catch (e) { data.Dosar.DATA_NOTIFICARE = null; }
        try {
            data.Dosar.DATA_ULTIMEI_MODIFICARI = new Date(parseInt(data.Dosar.DATA_ULTIMEI_MODIFICARI.substr(6)));
        } catch (e) { data.Dosar.DATA_ULTIMEI_MODIFICARI = null; }
        try {
            data.Dosar.DATA_IESIRE_CASCO = new Date(parseInt(data.Dosar.DATA_IESIRE_CASCO.substr(6)));
        } catch (e) { data.Dosar.DATA_IESIRE_CASCO = null; }
        try {
            data.Dosar.DATA_INTRARE_RCA = new Date(parseInt(data.Dosar.DATA_INTRARE_RCA.substr(6)));
        } catch (e) { data.Dosar.DATA_INTRARE_RCA = null; }
        */
        $http.post('/Dosare/Edit', data)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if ($scope.result.Status) {
                        $scope.editMode = 0;
                        $scope.searchMode = 1;
                        if ($scope.result.InsertedId != null) {
                            $scope.DosarFiltru.Dosar.ID = $scope.result.InsertedId;
                            if ($scope.DosarFiltru.DosareResult == undefined || $scope.DosarFiltru.DosareResult == null) {
                                $scope.DosarFiltru.DosareResult = [{}];
                                $scope.curDosarIndex = $scope.DosarFiltru.DosareResult.length - 1;
                                angular.copy($scope.DosarFiltru.Dosar, $scope.DosarFiltru.DosareResult[$scope.curDosarIndex]);
                            }
                            else {
                                $scope.DosarFiltru.DosareResult.push($scope.DosarFiltru.Dosar);
                                $scope.curDosarIndex = $scope.DosarFiltru.DosareResult.length - 1;
                            }
                        }
                        else {
                            angular.copy($scope.DosarFiltru.Dosar, $scope.DosarFiltru.DosareResult[$scope.curDosarIndex]);
                        }
                        $scope.ShowDosar($scope.curDosarIndex);
                    }
                    else {
                        $scope.searchMode = 0;
                        $scope.editMode = 1;
                    }
                } else {
                    $scope.searchMode = 0;
                    $scope.editMode = 1;
                }
                spinner.stop();
            }, function (response) {
                alert('Erroare: ' + response.status + ' - ' + response.data);
                spinner.stop();
            });
    };

    $scope.CancelEdit = function () {
        $scope.editMode = 0;
        $scope.searchMode = 1;
        angular.copy($scope.TempDosarEdit, $scope.DosarFiltru);
    };
});