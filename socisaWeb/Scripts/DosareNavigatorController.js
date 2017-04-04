var lastkeytime = new Date();
var lastruntime = new Date();
var waiting_interval = 500; // miliseconds
var lastfiltervalue = "";
'use strict';
app.controller('DosareNavigatorController', 
function ($scope, $http) {
    $scope.DosarFiltru = {};
    $scope.gasit = true;
    $scope.curDosarIndex = -1;

    $scope.Afisare2 = function () {
        var data = $scope.DosarFiltru;
        $http.post('/Dosare/Search', data, {
            headers:
            { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            lastruntime = new Date();
            lastfiltervalue = document.getElementById('dosarJson_NR_SCA').value;
            if (response != 'null' && response != null) {
                //alert(response); alert(response.data.Result);
                $scope.DosarFiltru.DosareResult = response.data.Result;
                $scope.curDosarIndex = $scope.DosarFiltru.DosareResult[0].ID;
            }
            else {
                alert('dddd');
                $scope.gasit = false;
            }
        }, function (response) {
            alert('Erroare: ' + response);
            $scope.gasit = false;
        });
    };

    $scope.Afisare21 = function () {
        var now = new Date();
        var filter_value = document.getElementById('dosarJson_NR_SCA').value;
        if ((now > lastruntime && lastfiltervalue != "" && filter_value.length >= 3) ||
            (now > lastruntime && filter_value != lastfiltervalue && filter_value.length >= 3 && now - lastkeytime > waiting_interval)) {
            $scope.Afisare2();
        }
        else {
            setTimeout(Afisare21, waiting_interval);
        }
        lastkeytime = now;
    };

    $scope.firstDosar = function () {
        $scope.Dosar = $scope.Dosare[0];
        $scope.ShowInfoDosar($scope.Dosar.ID);
        $scope.curDosarIndex = 0;
    };
    $scope.prevDosar = function () {
        var idx = $scope.curDosarIndex == 0 ? 0 : $scope.curDosarIndex - 1;
        $scope.Dosar = $scope.Dosare[idx];
        $scope.ShowInfoDosar($scope.Dosar.ID);
        $scope.curDosarIndex = idx;
    };
    $scope.nextDosar = function () {
        var idx = $scope.curDosarIndex == $scope.Dosare.length - 1 ? $scope.Dosare.length - 1 : $scope.curDosarIndex + 1;
        $scope.Dosar = $scope.Dosare[idx];
        $scope.ShowInfoDosar($scope.Dosar.ID);
        $scope.curDosarIndex = idx;
    };
    $scope.lastDosar = function () {
        $scope.Dosar = $scope.Dosare[$scope.Dosare.length - 1];
        $scope.ShowInfoDosar($scope.Dosar.ID);
        $scope.curDosarIndex = $scope.Dosare.length - 1;
    };

    $scope.showHideDiv = function (divId) {
        var div = document.getElementById(divId);
        if (div != null) {
            div.style.display = div.style.display == 'none' ? 'block' : 'none';
        }
    };


    $scope.NouDosar = function () {
        $scope.Dosar.ID = "";
        $scope.Dosar.NR_SCA = "";
        $scope.Dosar.DATA_SCA = "";
        $scope.Dosar.ID_ASIGURAT_CASCO = "";
        $scope.Dosar.NR_POLITA_CASCO = "";
        $scope.Dosar.ID_AUTO_CASCO = "";
        $scope.Dosar.ID_SOCIETATE_CASCO = "";
        $scope.Dosar.NR_POLITA_RCA = "";
        $scope.Dosar.ID_AUTO_RCA = "";
        $scope.Dosar.VALOARE_DAUNA = "";
        $scope.Dosar.VALOARE_REGRES = "";
        $scope.Dosar.ID_INTERVENIENT = "";
        $scope.Dosar.NR_DOSAR_CASCO = "";
        $scope.Dosar.VMD = "";
        $scope.Dosar.OBSERVATII = "";
        $scope.Dosar.ID_SOCIETATE_RCA = "";
        $scope.Dosar.DATA_EVENIMENT = "";
        $scope.Dosar.REZERVA_DAUNA = "";
        $scope.Dosar.DATA_INTRARE_RCA = "";
        $scope.Dosar.DATA_IESIRE_CASCO = "";
        $scope.Dosar.NR_INTRARE_RCA = "";
        $scope.Dosar.NR_IESIRE_CASCO = "";
        $scope.Dosar.ID_ASIGURAT_RCA = "";
        $scope.Dosar.ID_TIP_DOSAR = "";
        $scope.Dosar.SUMA_IBNR = "";
        $scope.Dosar.DATA_AVIZARE = "";
        $scope.Dosar.DATA_NOTIFICARE = "";
        $scope.Dosar.DATA_ULTIMEI_MODIFICARI = "";

        $scope.Dosare = new Array();
        $scope.UtilizatoriDosare = new Array();
        $scope.DocumenteScanate = new Array();
        $scope.DosareImportate = new Array();
        $scope.filtru2 = "";
        $scope.gasit = true;
        $scope.mesajRaspuns = "";
        $scope.DocScanat = {};
        //$scope.Procese = new Array();
        //$scope.Proces = null;
        $scope.AsiguratCasco = {};
        $scope.AsiguratRca = {};
        $scope.SocietateCasco = {};
        $scope.SocietateRca = {};
        $scope.TipDosar = {};
        $scope.AutoCasco = {};
        $scope.AutoRca = {};
        $scope.Intervenient = {};
    };
});