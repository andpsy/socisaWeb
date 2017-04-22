'use strict';
app.controller('MesajeController',
function ($scope, $http, $filter, $rootScope, $compile, myService) {
    $scope.searchMode = 1;
    $scope.model = {};
    $scope.model.Mesaj = null;
    $scope.model.Receivers = [];
    $scope.model.InvolvedParties = [{}];
    $scope.model.Mesaje = [{}];
    $scope.model.TipuriMesaj = [{}];
    $scope.result = {};
    $scope.involvedParty = "";
    $scope.tipMesaj = "";
    $scope.html2 = "";

    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined) {
            $scope.GetMessages(newValue);
            $scope.model.Mesaj.ID_DOSAR = newValue;
        }
    });

    $scope.$watch('model.Mesaj.ID_TIP_MESAJ', function (newValue, oldValue) {
        document.getElementById("select_TipuriMesaj").value = newValue;
    });

    $scope.$watch('tipMesaj', function (newValue, oldValue) {
        $scope.model.Mesaj.ID_TIP_MESAJ = newValue;
    });

    $scope.AddReceiver = function () {
        var este = false;
        angular.forEach($scope.model.Receivers, function (value, key) {
            if (value.ID == $scope.involvedParty) {
                este = true;
                return;
            }
        });
        if (este) return;

        angular.forEach($scope.model.InvolvedParties, function (value, key) {
            if (value.ID == $scope.involvedParty) {
                $scope.model.Receivers.push(value);
            }
        });

        var select = document.getElementById("select_involvedParties");
        $scope.html2 = "";
        //$scope.html2 += ('<a id="receiver_' + $scope.involvedParty + '" href="#" ng-click="RemoveReceiver(' + $scope.involvedParty + ')">' + select.options[select.selectedIndex].text + '&nbsp;<span class="badge">x</span></a>&nbsp;&nbsp;');
        angular.forEach($scope.model.Receivers, function (value, key) {
            $scope.html2 += ('<a id="receiver_' + value.ID + '" href="#" ng-click="RemoveReceiver(' + value.ID + ')">' + value.NUME_COMPLET +' (' + value.EMAIL + ')'+ '&nbsp;<span class="badge">x</span></a>&nbsp;&nbsp;');
        });
    };

    $scope.RemoveReceiver = function (ID) {
        //$scope.model.Receivers = $scope.model.Receivers.replace((value + ";"), "");
        var idx = -1;
        var i = 0;
        for (i = 0; i < $scope.model.Receivers.length; i++) {
            if ($scope.model.Receivers[i].ID == ID) {
                idx = i;
                break;
            }
        }
        $scope.model.Receivers.splice(idx, 1);
        var rec = document.getElementById("receiver_" + ID);
        rec.parentNode.removeChild(rec);
    };

    $scope.GetMessages = function (id_dosar) {
        spinner.spin(document.getElementById('main'));
        myService.getlist('/Mesaje/GetMessages/' + id_dosar)
          .then(function (response) {
              if (response != 'null' && response != null && response.data != null)
              {
                  $scope.html = response.data;
              }
              else {
                  $scope.html = "";
              }
              spinner.stop();
          }, function (response) {
              spinner.stop();
              alert('Erroare: ' + response.status + ' - ' + response.data);
          });
    };

    $scope.LoadMessages = function (messages) {
        $scope.model.Mesaje = messages;
    };

    $scope.SelectMessage = function (mesaj) {
        /*
        $scope.model.Mesaj = {};
        angular.copy(mesaj, $scope.model.Mesaj);
        $scope.tipMesaj = mesaj.ID_TIP_MESAJ;
        */
        $scope.model.Mesaj = mesaj;
        //$scope.tipMesaj = mesaj.ID_TIP_MESAJ;
    };

    $scope.SendMessage = function () {
        spinner.spin(document.getElementById('main'))
        var data = $scope.model;

        $http.post('/Mesaje/Send', data)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if ($scope.result.Status) {
                        if ($scope.result.InsertedId != null) {
                            $scope.model.Mesaj.ID = $scope.result.InsertedId;
                            $scope.model.Mesaje.push($scope.model.Mesaj);
                        }
                    }
                }
                spinner.stop();
            }, function (response) {
                alert('Erroare: ' + response.status + ' - ' + response.data);
                spinner.stop();
            });
    };

    $scope.NewMessage = function () {
        $scope.model.Receivers = [];
        $scope.html2 = "";
        $scope.model.Mesaj = {};
    };

    $scope.CancelMessage = function () {
        $scope.model.Mesaj = null;
        $scope.model.Receivers = [];
        $scope.html2 = "";
    };

});