'use strict';
var lastkeytime = new Date();
var lastruntime = new Date();
var waiting_interval = 1000; // miliseconds
var lastfiltervalue = "";

var spinner = new Spinner(opts);

function showDosareSideNav(on_off) {
    var dosareSideNav = document.getElementById("dosareSideNav");
    var relTarget = document.getElementById("DosareSearch");
    var rect = relTarget.getBoundingClientRect();
    if (on_off) {
        dosareSideNav.style.top = (Math.round(rect.top))-5 + 'px'; // relTarget.style.top;
        dosareSideNav.style.left = (Math.round(rect.left))-5 + 'px'; // relTarget.style.left;
        dosareSideNav.style.height = (Math.round(rect.bottom) - Math.round(rect.top))+10 + 'px'; // relTarget.style.height;
        dosareSideNav.style.width = '250px';
    }
    else {
        console.log('showDosareSideNav - ' + on_off);
        dosareSideNav.style.top = 0;
        dosareSideNav.style.left = 0;
        dosareSideNav.style.height = 0;
        dosareSideNav.style.width = 0;
    }
}

$(document).on('click', function (e) {
    var dosareSideNav = document.getElementById("dosareSideNav");
    if (dosareSideNav != null && dosareSideNav != undefined && dosareSideNav.style.width == '250px' && e.target.id != 'buttonShowListaDosare') {
        console.log('click - ');
        //$(elem).hide();
        showDosareSideNav(false);
    }
});

$(document).on('keydown', function (e) {
    var dosareSideNav = document.getElementById("dosareSideNav");
    if (e.keyCode === 27 && dosareSideNav != null && dosareSideNav != undefined && dosareSideNav.style.width == '250px') {
        console.log('keydown - ');
        //$(elem).hide();
        showDosareSideNav(false);
    }
});

app.controller('DosareNavigatorController',
function ($scope, $http, $filter, $rootScope, $window) {
    $rootScope.activeTab = "detalii";
    $rootScope.dynaStyle = $rootScope.dynaStyleDefault = { 'background-color': 'white' };
    $rootScope.AVIZAT = false; // variabila generala pt. statusul dosarului (vizibila intre controllere)
    
    $scope.tmpCalitateSocietate = "CASCO"
    $rootScope.calitateSocietateCurenta = $scope.ccalitatedr = "CASCO";
    $scope.searchMode = 1;
    $scope.TempDosarFilter = {};
    $scope.editMode = 0;
    
    $scope.DosarFiltru = {};
    $scope.DosarFiltru.dosarJson = {};
    $scope.DosarFiltru.Dosar = {};
    
    //$scope.DosarFiltru.DosareResult = [];
    $scope.curDosarIndex = -1;
    $scope.TempDosarEdit = {};
    //$scope.TempDosarFilter.DosareResult = [];
    $scope.result = {};
    $scope.IDSocietateRep = "";
    $scope.Interactive = false;
    $rootScope.validForAvizare = false; // variabila generala pt. validitatea dosarului de a fi avizat (vizibil intre controllere)
    

    $scope.SetId = function (id) {
        console.log('id: ' + id);
        /*
        if ($rootScope.calitateSocietateCurenta == "CASCO") {
            try {
                $scope.ID_SOCIETATE_CASCO = $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO;
            } catch (e) {
                $scope.ID_SOCIETATE_RCA = "";
            }
        }
        if ($rootScope.calitateSocietateCurenta == "RCA") {
            try {
                $scope.ID_SOCIETATE_RCA = $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA;
            } catch (e) {
                $scope.ID_SOCIETATE_CASCO = "";
            }
        }
        */
        $scope.DosarFiltru.Dosar.ID = $rootScope.ID_DOSAR = id;
        angular.copy($scope.DosarFiltru, $scope.TempDosarFilter);
        $scope.DosarFiltru.DosareResult = [];
        $scope.DosarFiltru.DosareResult.push($scope.DosarFiltru.Dosar);
        $scope.ShowDosar(0);
    };

    $rootScope.setActiveTab = function (atab) {
        $rootScope.activeTab = atab;

        var lnkId = "#lnkDosareDetalii";
        switch ($rootScope.activeTab) {
            case "detalii":
                lnkId = "#lnkDosareDetalii";
                break;
            case "documente":
                lnkId = "#lnkDocumenteScanateDetalii";
                break;
            case "mesaje":
                lnkId = "#lnkMesajeDetalii";
                break;
            case "utilizatori":
                lnkId = "#lnkUtilizatoriDetalii";
                break;
        }
        $scope.switchTabsClass(lnkId);
    };
    /*
    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != oldValue) {
            console.log('watch ID_DOSAR - newValue= ' + newValue + ' - oldValue= ' + oldValue);
            $scope.SetId(newValue);
            //$scope.Afisare(newValue, null);
        }
    });
    */
    $scope.$watch('DosarFiltru.Dosar', function (newValue, oldValue) {
        //alert(newValue.DATA_EVENIMENT);
        console.log('watch DosarFiltru.Dosar - ' + $rootScope.ID_DOSAR);
        if ($rootScope.calitateSocietateCurenta == "CASCO") {
            console.log('aici2');
            try {
                //document.getElementById("Dosar_ID_SOCIETATE_CASCO").value = newValue.ID_SOCIETATE_CASCO;
                $scope.ID_SOCIETATE_CASCO = newValue.ID_SOCIETATE_CASCO;
            } catch (e) {
                //document.getElementById("Dosar_ID_SOCIETATE_RCA").selectedIndex = -1;
                $scope.ID_SOCIETATE_RCA = "";
            }
        }
        if ($rootScope.calitateSocietateCurenta == "RCA") {
            console.log('aici3');
            try {
                //document.getElementById("Dosar_ID_SOCIETATE_RCA").value = newValue.ID_SOCIETATE_RCA;
                $scope.ID_SOCIETATE_RCA = newValue.ID_SOCIETATE_RCA;
            } catch (e) {
                //document.getElementById("Dosar_ID_SOCIETATE_CASCO").selectedIndex = -1;
                $scope.ID_SOCIETATE_CASCO = "";
            }
        }
        //document.getElementById("lnkDocumenteScanateDetalii").className  = document.getElementById("lnkMesajeDetalii").className  = newValue.ID == null ? 'disabled' : '';
    });

    $scope.$watch('DosarFiltru.Dosar.ID', function (newValue, oldValue) {
        if (newValue == null) {
            console.log('watch DosarFiltru.Dosar.ID - ' + $rootScope.ID_DOSAR);
            $scope.switchTabsClass("#lnkDosareDetalii");
            $rootScope.dynaStyle = $rootScope.dynaStyleDefault;
        }
        if (newValue != null && newValue != oldValue) {
            $rootScope.AVIZAT = $scope.DosarFiltru.Dosar.AVIZAT;
        }
        if (newValue != undefined && newValue != "" && newValue != null && newValue == oldValue) {
            console.log('aici1');
            $rootScope.AVIZAT = $scope.DosarFiltru.Dosar.AVIZAT;
            $scope.DosarFiltru.DosareResult = [];
            $scope.DosarFiltru.DosareResult.push($scope.DosarFiltru.Dosar);
            $scope.TempDosarFilter.DosareResult = [];
            $scope.TempDosarFilter.DosareResult.push($scope.DosarFiltru.Dosar);
            $scope.curDosarIndex = 0;
            $scope.ShowDosar($scope.curDosarIndex);
            $scope.ID_SOCIETATE_RCA = $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA;
            $scope.TIP_CAZ = $scope.DosarFiltru.Dosar.CAZ;
            $scope.ID_TIP_DOSAR = $scope.DosarFiltru.Dosar.ID_TIP_DOSAR;
        }
    });

    $scope.$watch('tmpCalitateSocietate', function (newValue, oldValue) {
        //alert(newValue);
        console.log('watch tmpCalitateSocietate');
        $rootScope.calitateSocietateCurenta = newValue;
        if ($scope.editMode == 0 && newValue != oldValue) {
            //alert($scope.searchMode + ' - ' + $scope.editMode);
            //$scope.searchMode = 2;
            $scope.SwitchBackToSearchMode();
        }
    });

    $rootScope.$watch('calitateSocietateCurenta', function (newValue, oldValue) {
        if (newValue != oldValue) {
            console.log('watch calitate - ' + $rootScope.ID_DOSAR);
            $scope.ccalitatedr = newValue;
            $scope.ClearFilters();
            document.getElementById('Dosar_ID_SOCIETATE_CASCO').disabled = !($scope.searchMode == 2 && newValue == 'RCA');
            document.getElementById('Dosar_ID_SOCIETATE_RCA').disabled = !(($scope.searchMode == 2 || $scope.editMode == 1) && newValue == 'CASCO');
            if (newValue == "CASCO") {
                $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.ID_SOCIETATE_CASCO = $scope.IDSocietateRep;
                $scope.ID_SOCIETATE_RCA = $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = "";
            }
            if (newValue == "RCA")
            {
                $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = $scope.ID_SOCIETATE_RCA = $scope.IDSocietateRep;
                $scope.ID_SOCIETATE_CASCO = $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = "";
                $scope.DosarFiltru.Dosar.AVIZAT = $scope.TempDosarFilter.Dosar.AVIZAT = true;
                document.getElementById("Dosar_AVIZAT").disabled = true;
            }
        }
    });
    
    $scope.$watch('editMode', function (newValue, oldValue) {
        console.log('watch editMode');

        document.getElementById('Dosar_DATA_EVENIMENT').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_SCA').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_AVIZARE').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_NOTIFICARE').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_CREARE').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_ULTIMEI_MODIFICARI').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_IESIRE_CASCO').disabled = !(newValue == 1);
        document.getElementById('Dosar_DATA_INTRARE_RCA').disabled = !(newValue == 1);
        document.getElementById('Dosar_ID_SOCIETATE_RCA').disabled = !(newValue == 1);
        document.getElementById('Dosar_CAZ').disabled = !(newValue == 1);
        document.getElementById('Dosar_ID_TIP_DOSAR').disabled = !(newValue == 1);
        //document.getElementById("lnkDocumenteScanateDetalii").className = document.getElementById("lnkMesajeDetalii").className = newValue == 1 ? 'disabled' : '';
    });

    $scope.$watch('searchMode', function (newValue, oldValue) {
        console.log('watch searchMode - ' + newValue + ' = ' + oldValue);
        document.getElementById('Dosar_ID_SOCIETATE_CASCO').disabled = !(newValue == 2 && $rootScope.calitateSocietateCurenta == 'RCA');
        document.getElementById('Dosar_ID_SOCIETATE_RCA').disabled = !((newValue == 2 || $scope.editMode == 1) && $rootScope.calitateSocietateCurenta == 'CASCO');
        document.getElementById('Dosar_CAZ').disabled = newValue != 2 && $scope.editMode != 1;
        document.getElementById('Dosar_ID_TIP_DOSAR').disabled = newValue != 2 && $scope.editMode != 1;
    });

    $scope.$watch('DosarFiltru.Dosar.ID_SOCIETATE_RCA', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined) {
            console.log('watch DosarFiltru.Dosar.ID_SOCIETATE_RCA - ' + newValue + ' = ' + oldValue);
            document.getElementById("Dosar_ID_SOCIETATE_RCA").value = newValue;
        }
    });

    $scope.$watch('ID_SOCIETATE_RCA', function (newValue, oldValue) {
        console.log('watch ID_SOC_RCA - ' + newValue + ' = ' + oldValue + ' - interactive: ');
        if (newValue != null && newValue != undefined && $scope.Interactive) {
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = newValue;
            //if ($scope.searchMode == 2)
            {
                //console.log('afisare - ID_SOCIETATE_RCA');
                $scope.Afisare(newValue, null);
            }
        }
    });

    $scope.$watch('DosarFiltru.Dosar.ID_SOCIETATE_CASCO', function (newValue, oldValue) {
        //console.log('watch DosarFiltru.Dosar.ID_SOC_CASCO - ' + newValue + ' = ' + oldValue);
        if (newValue != null && newValue != undefined) {
            document.getElementById("Dosar_ID_SOCIETATE_CASCO").value = newValue;
        }
    });

    $scope.$watch('ID_SOCIETATE_CASCO', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined && $scope.Interactive) {
            console.log('watch ID_SOC_CASCO - ' + newValue + ' = ' + oldValue);
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = newValue;
            //if ($scope.searchMode == 2)
            {
                //console.log('afisare - ID_SOCIETATE_CASCO');
                $scope.Afisare(newValue, null);
            }
        }
    });
    /*
    $scope.$watch('DosarFiltru.Dosar.NR_DOSAR_CASCO', function (newValue, oldValue) {
        if ($scope.searchMode == 2) {
            //console.log('afisare - DosarFiltru.Dosar.NR_DOSAR_CASCO');
            $scope.Afisare(newValue, null);
        }
    });
    */

    $scope.$watch('DosarFiltru.Dosar.CAZ', function (newValue, oldValue) {
        document.getElementById("Dosar_CAZ").value = newValue;
    });

    $scope.$watch('DosarFiltru.Dosar.ID_TIP_DOSAR', function (newValue, oldValue) {
        document.getElementById("Dosar_ID_TIP_DOSAR").value = newValue;
    });

    $scope.$watch('TIP_CAZ', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined && $scope.Interactive) {
            console.log('watch TIP_CAZ - ' + newValue + ' = ' + oldValue);
            $scope.DosarFiltru.Dosar.CAZ = newValue;
            //if ($scope.searchMode == 2)
            {
                //console.log('afisare - TIP_CAZ');
                $scope.Afisare(newValue, null);
            }
        }
    });

    $scope.$watch('ID_TIP_DOSAR', function (newValue, oldValue) {
        //console.log('watch ID_DOSAR - ' + newValue);
        if (newValue != null && newValue != undefined && $scope.Interactive) {
            console.log('watch ID_TIP_DOSAR - ' + newValue + ' = ' + oldValue);
            $scope.DosarFiltru.Dosar.ID_TIP_DOSAR = newValue;
            //if ($scope.searchMode == 2)
            {
                //console.log('afisare - ID_TIP_DOSAR');
                $scope.Afisare(newValue, null);
            }
        }
    });


    $scope.$watch('DosarFiltru.Dosar.AVIZAT', function (newValue, oldValue) {
        if ($scope.searchMode == 2) {
            console.log('afisare - DosarFiltru.Dosar.AVIZAT');
            $scope.Afisare(newValue, null);
        }
    });

    $scope.$watch('DosarFiltru.dosarJson.DataEvenimentEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DosarFiltru.Dosar.AVIZAT');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataScaEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataEvenimentEnd');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataAvizareEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataAvizareEnd');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataNotificareEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataNotificareEnd');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataCreareEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataCreareEnd');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataUltimeiModificariEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataUltimeiModificariEnd');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataIesireCascoEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataIesireCascoEnd');
            $scope.Afisare(newValue, null);
        }
    });
    $scope.$watch('DosarFiltru.dosarJson.DataIntrareRcaEnd', function (newValue, oldValue) {
        if (newValue != oldValue) {
            //console.log('afisare - DataIntrareRcaEnd');
            $scope.Afisare(newValue, null);
        }
    });

    $scope.$watch('DosarFiltru.Dosar.DATA_EVENIMENT', function (newDate, oldDate) {
        //alert(newDate+' - '+oldDate);
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_EVENIMENT = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_SCA', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_SCA = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_AVIZARE', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_AVIZARE = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_NOTIFICARE', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_NOTIFICARE = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_CREARE', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_CREARE = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_ULTIMEI_MODIFICARI', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_ULTIMEI_MODIFICARI = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_IESIRE_CASCO', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_IESIRE_CASCO = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });
    $scope.$watch('DosarFiltru.Dosar.DATA_INTRARE_RCA', function (newDate) {
        if (newDate == null || newDate == undefined || newDate.length <= 10 || angular.isDate(newDate)) { return; }
        $scope.DosarFiltru.Dosar.DATA_INTRARE_RCA = $filter('date')(new Date(parseInt(newDate.substr(6))), $rootScope.DATE_FORMAT);
    });

    $scope.formatDate = function(dateValue){
        return $filter('date')(new Date(parseInt(dateValue.substr(6))), $rootScope.DATE_FORMAT);
    }

    $scope.compareObjects = function (o1, o2) {
        return angular.equals(o1, o2);
    }

    $scope.ClearFilters = function () {
        console.log("ClearFilters - " + $rootScope.ID_DOSAR);
        $scope.searchMode = 1;
        $rootScope.ID_DOSAR = null;
        $scope.Interactive = false;
        $scope.DosarFiltru.dosarJson = {};
        $scope.DosarFiltru.Dosar = {};
        //$scope.DosarFiltru.DosareResult = [];
        $scope.DosarFiltru.SocietatiCASCO = [{}];
        $scope.DosarFiltru.SocietatiRCA = [{}];
        $scope.DosarFiltru.TipuriCaz = [{}];
        $scope.DosarFiltru.TipuriDosar = [{}];
        $scope.DosarFiltru.DosareResult = undefined;
        if ($rootScope.calitateSocietateCurenta == "CASCO") {
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.ID_SOCIETATE_CASCO = $scope.IDSocietateRep;
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = $scope.ID_SOCIETATE_RCA = "";
        }
        if ($rootScope.calitateSocietateCurenta == "RCA")
        {
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = $scope.ID_SOCIETATE_RCA = $scope.IDSocietateRep;
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.ID_SOCIETATE_CASCO = "";
        }
        angular.copy($scope.DosarFiltru, $scope.TempDosarFilter);
    };
    /*
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
    */
    $scope.Afisare = function (e, sender) {
        if ($scope.editMode == 1) return;
        var direct = sender == null;
        angular.copy($scope.DosarFiltru, $scope.TempDosarFilter);
        var now = new Date();
        var filter_value = e;
        if ((now - lastkeytime <= waiting_interval && lastfiltervalue != filter_value) || (filter_value.length < 3 && !direct)) {
            setTimeout(Afisare, now - lastkeytime);
            lastkeytime = now;
            console.log("Afisare - " + $rootScope.ID_DOSAR + ' - ' + e);
            return;
        }
        else {
            console.log("afisare2 = filter_value (e) - " + filter_value + ' - ' + e);
            if (filter_value !== "") {
                console.log("Afisare2 - " + $rootScope.ID_DOSAR + ' - ' + e);
                $scope.Afisare2(e);
                lastkeytime = now;
                //return;
            }
        }
        $scope.Interactive = false;
    };

    $scope.Afisare2 = function (input_value) {
        //spinner = new Spinner(opts).spin(document.getElementById(ACTIVE_DIV_ID));
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
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
        //$scope.tmpIdSocietateCasco = $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO;
        //$scope.tmpIdSocietateRca = $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA;
        $scope.searchMode = 2;
        $scope.switchTabsClass("#lnkDosareDetalii");
        $rootScope.dynaStyle = $rootScope.dynaStyleDefault;
        console.log("SwitchBackToSearchMode - " + $rootScope.ID_DOSAR);
        angular.copy($scope.TempDosarFilter, $scope.DosarFiltru);
        if ($rootScope.calitateSocietateCurenta == "CASCO")
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.IDSocietateRep;
        else
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = $scope.IDSocietateRep;
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
        console.log('ShowDosar ID_DOSAR = ' + $rootScope.ID_DOSAR);
        $scope.searchMode = 1;
        $scope.curDosarIndex = index;
        
        if ($scope.DosarFiltru.Dosar == null || $scope.DosarFiltru.Dosar == undefined) {
            alert($scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO);
            $scope.DosarFiltru.Dosar = {};
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.TempDosarEdit.Dosar.ID_SOCIETATE_CASCO;
        }
        
        if (!angular.equals($scope.DosarFiltru.DosareResult[index], $scope.DosarFiltru.Dosar)) {
            angular.copy($scope.DosarFiltru.DosareResult[index], $scope.DosarFiltru.Dosar); // !!!!!!!
            //$scope.DosarFiltru.Dosar = angular.copy($scope.DosarFiltru.DosareResult[index]); // !!!!!!!
        }
        $rootScope.ID_DOSAR = $scope.DosarFiltru.Dosar.ID;
        //document.getElementById("lnkDocumenteScanateDetalii").className = document.getElementById("lnkMesajeDetalii").className = '';

        //spinner = new Spinner(opts).spin(document.getElementById(ACTIVE_DIV_ID));
        //$scope.DosarFiltru.Dosar.DATA_EVENIMENT = $scope.formatDate($scope.DosarFiltru.Dosar.DATA_EVENIMENT);
        var data = $scope.DosarFiltru.Dosar.ID;
        $http.get('/Dosare/Details/' + data, {
            headers:
            { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            if (response != 'null' && response != null && response.data != null) {
                var j = JSON.parse(response.data);
                try {
                    if ($scope.DosarFiltru.dosarJson == null || $scope.DosarFiltru.dosarJson == undefined || $scope.DosarFiltru.dosarJson == "undefined")
                        $scope.DosarFiltru.dosarJson = {};
                    $scope.DosarFiltru.dosarJson.NumeAsiguratCasco = j.aCasco.DENUMIRE;
                    $scope.DosarFiltru.dosarJson.NumeAsiguratRca = j.aRca.DENUMIRE;
                    $scope.DosarFiltru.dosarJson.NumarAutoCasco = j.autoCasco.NR_AUTO;
                    $scope.DosarFiltru.dosarJson.NumarAutoRca = j.autoRca.NR_AUTO;
                    $scope.DosarFiltru.dosarJson.NumeIntervenient = j.intervenient.DENUMIRE;
                    //$scope.DosarFiltru.dosarJson.TipDosar = j.tipDosar.DENUMIRE;
                    $rootScope.validForAvizare = j.validForAvizare;
                    $rootScope.dynaStyle = $scope.DosarFiltru.Dosar.AVIZAT ? { 'background-color': '#e3eded' } : ($rootScope.validForAvizare ? { 'background-color': '#fffff0' } : { 'background-color': '#f8eeee' });
                    $scope.switchTabsClass("#lnkDosareDetalii");
                } catch (e) { }
            }
            //spinner.stop();
        }, function (response) {
            alert('Erroare: ' + response.status + ' - ' + response.data);
            //spinner.stop();
        });        
    };

    $scope.switchTabsClass = function (lnkId) {
        $("#lnkDosareDetalii").removeClass("grad_tab").removeClass("grad_tab_avizat").removeClass("grad_tab_neavizat").removeClass("grad_tab_incomplet");
        $("#lnkDocumenteScanateDetalii").removeClass("grad_tab").removeClass("grad_tab_avizat").removeClass("grad_tab_neavizat").removeClass("grad_tab_incomplet");
        $("#lnkMesajeDetalii").removeClass("grad_tab").removeClass("grad_tab_avizat").removeClass("grad_tab_neavizat").removeClass("grad_tab_incomplet");
        $("#lnkUtilizatoriDetalii").removeClass("grad_tab").removeClass("grad_tab_avizat").removeClass("grad_tab_neavizat").removeClass("grad_tab_incomplet");

        var classToAdd = "";
        if ($scope.searchMode == 2 || $scope.DosarFiltru.Dosar.ID == null)
            classToAdd = "grad_tab";
        else
            classToAdd = $scope.DosarFiltru.Dosar.AVIZAT ? "grad_tab_avizat" : ($rootScope.validForAvizare ? "grad_tab_neavizat" : "grad_tab_incomplet");
        $(lnkId).addClass(classToAdd);
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
        spinner.spin(document.getElementById(ACTIVE_DIV_ID))
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
                    $('.alert').show();
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
                    
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);

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
        if ($rootScope.calitateSocietateCurenta == "CASCO")
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO = $scope.IDSocietateRep;
        else
            $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA = $scope.IDSocietateRep;
        console.log("CancelEdit - " + $scope.TempDosarEdit.Dosar.ID_SOCIETATE_CASCO);
    };

    $scope.TiparireDosar = function () {
        var data = $scope.DosarFiltru.Dosar.ID;
        $http.get('/Dosare/Print/' + data, {
            headers:
            { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            if (response != 'null' && response != null && response.data != null) {
                try {
                    if (response.data.Status)
                        $window.open("../pdfs/" + response.data.Message);
                    else {
                        $('.alert').show();
                        $scope.showMessage = true;
                        $scope.result = response.data;
                        $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                    }
                } catch (e) {;}
            }
            //spinner.stop();
        }, function (response) {
            alert('Erroare: ' + response.status + ' - ' + response.data);
            //spinner.stop();
        });
    };

    $scope.AvizareDosar = function () {
        /*
        $scope.DosarFiltru.Dosar.AVIZAT = _avizat;
        var d = new Date();
        var ds = (((d.getDate() < 10 ? "0" : "") + d.getDate()) + "." + ((d.getMonth() + 1 < 10 ? "0" : "") + (d.getMonth() + 1)) + "." + d.getFullYear());
        $scope.DosarFiltru.Dosar.DATA_AVIZARE = ds;
        $scope.SaveEdit();
        */
        var _avizat = !$scope.DosarFiltru.Dosar.AVIZAT;
        spinner.spin(document.getElementById(ACTIVE_DIV_ID))
        $http.post('/Dosare/Avizare', { id : $scope.DosarFiltru.Dosar.ID, avizat : _avizat})
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $('.alert').show();
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if ($scope.result.Status) {
                        $scope.DosarFiltru.Dosar.AVIZAT = _avizat;
                        var d = new Date();
                        var ds = (((d.getDate() < 10 ? "0" : "") + d.getDate()) + "." + ((d.getMonth() + 1 < 10 ? "0" : "") + (d.getMonth() + 1)) + "." + d.getFullYear());
                        $scope.DosarFiltru.Dosar.DATA_AVIZARE = ds;
                        $rootScope.AVIZAT = $scope.DosarFiltru.Dosar.AVIZAT;
                        $rootScope.dynaStyle = $scope.DosarFiltru.Dosar.AVIZAT ? { 'background-color': '#e3eded' } : ($rootScope.validForAvizare ? { 'background-color': '#fffff0' } : { 'background-color': '#f8eeee' });
                        $scope.switchTabsClass("#lnkDosareDetalii");
                    }
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                }
                spinner.stop();
            }, function (response) {
                alert('Erroare: ' + response.status + ' - ' + response.data);
                spinner.stop();
            });
    };
});