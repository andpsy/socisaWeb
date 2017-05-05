'use strict';
function toggleChecks(e) {
    $('.checkForImport').prop('checked', e.checked);
}
var spinner = new Spinner(opts);

app.controller('DosareImportController',
function ($scope, $http, $filter, $rootScope, $window, Upload) {
    $scope.model = {};
    $scope.model.ImportDates = [];

    $scope.upload = function (file) {
        if (file == null || !Upload.isFile(file)) return;
        spinner.spin(document.getElementById('main'));

        Upload.upload({
            url: '/Dosare/PostExcelFile',
            data: { file: file }
        }).then(function (response) {
            if (!response.data.Status && response.data.Message != null && response.data.Result == null) {
                $scope.result = response.data;
                $scope.showMessage = true;
            }
            else {
                $scope.model.ImportDosarView = response.data.Result;
                document.getElementById("IncarcareFisierExcel").style.display = document.getElementById("IncarcareFisierExcel").style.display == 'none' ? 'block' : 'none';
            }
            spinner.stop();
        }, function (response) {
            alert(response.status + ' - ' + response.data);
            console.log('Error status: ' + response.status);
            spinner.stop();
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
        });
    };

    $scope.toggleDiv = function () {
        document.getElementById("IncarcareFisierExcel").style.display = document.getElementById("IncarcareFisierExcel").style.display == 'none' ? 'block' : 'none';
        $scope.model.ImportDosarView = null;
    };

    $scope.GetDosareFromLog = function (date) {
        document.getElementById("IncarcareFisierExcel").style.display = 'none';
        spinner.spin(document.getElementById('main'));
        $http.post('/Dosare/GetDosareFromLog', { ImportDate: date })
            .then(function (response) {
                $scope.model.ImportDosarView = response.data.Result;
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };
});