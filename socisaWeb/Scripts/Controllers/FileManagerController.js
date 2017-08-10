'use strict';

app.controller('FileManagerController',
function ($scope, $http, $filter, $rootScope, $compile, $interval, myService) {
    $scope.deAfisat = "";
    //$scope.all_files = true;
    //$scope.all_docs = true;

    $scope.GetOrphanFiles = function () {
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        $http.get('/FileManager/GetOrphanFiles')
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = $scope.OrphanFiles = response.data;
                    $scope.deAfisat = 0;
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.GetOrphanDocuments = function () {
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        $http.get('/FileManager/GetOrphanDocuments')
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = $scope.OrphanDocuments = response.data;
                    $scope.deAfisat = 1;
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.CheckAllFiles = function () {
        console.log('check all files - ' + $scope.all_files);
        for (var i = 0; i < $scope.OrphanFiles.length; i++) {
            $scope.OrphanFiles[i].SELECTED = $scope.all_files;
        }
    };

    $scope.CheckAllDocs = function () {
        console.log('check all docs - ' + $scope.all_docs);
        for (var i = 0; i < $scope.OrphanDocuments.length; i++) {
            $scope.OrphanDocuments[i].SELECTED = $scope.all_docs;
        }
    };

    $scope.DeleteOrphanFile = function (fileName) {
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        $http.post('/FileManager/DeleteOrphanFile', { fileName: fileName })
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.DeleteOrphanFiles = function () {
        var qs = [];
        for (var i = 0; i < $scope.OrphanFiles.length; i++) {
            if ($scope.OrphanFiles[i].SELECTED) {
                var x = $scope.DeleteOrphanFile($scope.OrphanFiles[i].FILE_NAME);
                qs.push(x);
            }
        }
    };

    $scope.RestoreOrphanDocument = function (id) {
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        $http.post('/FileManager/RestoreOrphanDocument', { id: id })
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.RestoreOrphanDocuments = function () {
        for (var i = 0; i < $scope.OrphanDocuments.length; i++) {
            if ($scope.OrphanDocuments[i].SELECTED) {
                $scope.DeleteOrphanFile($scope.OrphanDocuments[i].DOCUMENT_SCANAT.ID);
            }
        }
    };
});