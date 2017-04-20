
app.controller('MesajeController',
function ($scope, $http, $filter, $rootScope, $compile, myService) {
    $scope.searchMode = 1;
    $scope.model = {};
    $scope.model.Mesaj = {};
    $scope.model.Receivers = "";
    $scope.model.InvolvedParties = [{}];
    $scope.model.Mesaje = [{}];
    $scope.result = {};
    $scope.involvedParty = "";

    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined) {
            $scope.GetMessages(newValue);
        }
    });

    $scope.AddReceiver = function () {
        var select = document.getElementById("select_involvedParties");
        alert($scope.model.Receivers + ' - ' + $scope.involvedParty);
        if ($scope.model.Receivers.indexOf($scope.involvedParty) > -1) return;
        $scope.model.Receivers = ($scope.model.Receivers + ($scope.model.Receivers == "" ? "" : ";") + $scope.involvedParty);
        document.getElementById("receivers").innerHTML += ('<a href="#">' + select.options[select.selectedIndex].text + '&nbsp;<span class="badge">x</span></a>&nbsp;&nbsp;');
    };

    $scope.GetMessages = function (id_dosar) {
        spinner.spin(document.getElementById('main'));
        myService.getlist('/Mesaje/GetMessages/' + id_dosar)
          .then(function (response) {
              if (response != 'null' && response != null && response.data != null) // && response.data.Result != null && response.data.Result != "")
              {
                  //$scope.model.Mesaje = response.data.Result;
                  //document.getElementById("mesaje").innerHTML = response.data;
                  $scope.html = response.data;
              }
              else {
                  //$scope.model.DocumenteScanate = null;
                  //document.getElementById("mesaje").innerHTML = "";
                  $scope.html = "";
              }
              spinner.stop();
          }, function (response) {
              spinner.stop();
              alert('Erroare: ' + response.status + ' - ' + response.data);
          });
    };

    $scope.SelectMessage = function (id) {
        var i = 0;
        alert('L = ' + $scope.model.Mesaje.length);
        for (i = 0; i < $scope.model.Mesaje.length; i++) {
            if ($scope.model.Mesaje[i].ID == id) {
                alert(id);
                angular.copy($scope.model.Mesaje[i], $scope.model.Mesaj);
                break;
            }
        }
    };

});