var lastkeytime = new Date();
var lastruntime = new Date();
var waiting_interval = 500; // miliseconds
var lastfiltervalue = "";
'use strict';
app.controller('DosareNavigatorController', 
function ($scope, $http) {
    $scope.searchMode = 1;
    $scope.editMode = 0;
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
            lastfiltervalue = document.getElementById('Dosar_NR_SCA').value;
            if (response != 'null' && response != null) {
                //alert(response); alert(response.data.Result);
                $scope.DosarFiltru.DosareResult = response.data.Result;
                $scope.curDosarIndex = 0;
                $scope.ShowDosar($scope.curDosarIndex);
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

    $scope.Afisare21 = function (e) {
        var now = new Date();
        //var filter_value = document.getElementById('Dosar_NR_SCA').value;
        var filter_value = e.target.value;
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
        var idx = 0;
        $scope.ShowDosar(idx);
    };
    $scope.prevDosar = function () {
        var idx = $scope.curDosarIndex == 0 ? 0 : $scope.curDosarIndex - 1;
        $scope.ShowDosar(idx);
    };
    $scope.nextDosar = function () {
        var idx = $scope.curDosarIndex == $scope.DosarFiltru.DosareResult.length - 1 ? $scope.DosarFiltru.DosareResult.length - 1 : $scope.curDosarIndex + 1;
        $scope.ShowDosar(idx);
    };
    $scope.lastDosar = function () {
        var idx = $scope.DosarFiltru.DosareResult.length - 1;
        $scope.ShowDosar(idx);
    };

    $scope.ShowDosar = function (index) {
        $scope.curDosarIndex = index;
        $scope.DosarFiltru.Dosar = $scope.DosarFiltru.DosareResult[index];
        document.getElementById("Dosar_ID_SOCIETATE_CASCO").value = $scope.DosarFiltru.Dosar.ID_SOCIETATE_CASCO;
        document.getElementById("Dosar_ID_SOCIETATE_RCA").value = $scope.DosarFiltru.Dosar.ID_SOCIETATE_RCA;
    }

    $scope.showHideDiv = function (divId) {
        var div = document.getElementById(divId);
        if (div != null) {
            div.style.display = div.style.display == 'none' ? 'block' : 'none';
        }
    };
});