﻿'use strict';

app.controller('DocumenteScanateController',
function ($scope, $http, $filter, $rootScope, Upload) {
    $scope.model = {};
    $scope.curDocumentIndex = -1;
    $scope.model.TipuriDocumente = [{}];
    $scope.model.DocumenteScanate = [{}];
    $scope.model.CurDocumentScanat = {};

    $scope.showDocument = function (index) {
        $scope.curDocumentIndex = index;
        try {
            angular.copy($scope.getDocument($scope.model.TipuriDocumente[index].ID), $scope.model.CurDocumentScanat);
            //$scope.model.CurDocumentScanat = $scope.getDocument($scope.model.TipuriDocumente[index].ID);
            if ($scope.model.CurDocumentScanat == null || $scope.model.CurDocumentScanat.ID == null) {
                $scope.model.CurDocumentScanat = {};
                $scope.model.CurDocumentScanat.FILE_CONTENT = null;
                $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
                $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[index].ID;
                $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
            }
        } catch (e) {
            $scope.model.CurDocumentScanat = {};
            $scope.model.CurDocumentScanat.FILE_CONTENT = null;
            $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
            $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[index].ID;
            $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
        }
    };

    $scope.areDocumentAvizat = function (id_tip_document) {
        try {
            var doc = $scope.getDocument(id_tip_document);
            return doc.VIZA_CASCO ? 2 : 1; // 2 = AVIZAT / 1 = are document, dar nu e avizat.
        } catch (e) { return 0; }
    };

    $scope.getDocument = function(id_tip_document){
        try {
            var i = 0;
            for (i = 0; i < $scope.model.DocumenteScanate.length; i++) {
                var doc = $scope.model.DocumenteScanate[i];
                if (doc.ID_TIP_DOCUMENT == id_tip_document) {
                    return doc;
                }
            }
            return null;
        } catch (e) { return null; }
    };

    $scope.showMandatory = function (tipDoc) {
        try{
            if($scope.model.DocumenteScanate != null && $scope.model.DocumenteScanate != undefined)
                return !$scope.areDocumentAvizat(tipDoc.ID) && tipDoc.MANDATORY;
            return tipDoc.MANDATORY;
        } catch (e) { return false;}
    };
    /*
    $scope.$watch('model.CurDocumentScanat.VIZA_CASCO', function (newValue, oldValue) {
        if (newValue != oldValue) {
            $scope.SaveEdit();
        }
    });
    */
    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined) {
            $scope.model.ID_DOSAR = newValue;
            $scope.ShowDocuments(newValue);
        }
    });

    $scope.ShowDocuments = function (id_dosar) {
        //spinner = new Spinner(opts).spin(document.getElementById('main'));
        //spinner.spin(document.getElementById('main'));
        $http.get('/DocumenteScanate/Details/' + id_dosar).then(function (response) {
            if (response != 'null' && response != null && response.data != null && response.data.Result != null && response.data.Result != "") {
                $scope.model.DocumenteScanate = response.data.Result;
            }
            else {
                $scope.model.DocumenteScanate = null;
            }
            //spinner.stop();
        }, function (response) {
            alert('Erroare: ' + response.status + ' - ' + response.data);
            //spinner.stop();
        });
    };

    $scope.ShowDocument = function (id_document) {
        //spinner = new Spinner(opts).spin(document.getElementById('main'));
        //spinner.spin(document.getElementById('main'));
        $http.get('/DocumenteScanate/Detail/' + id_document).then(function (response) {
            if (response != 'null' && response != null && response.data != null && response.data.Result != null && response.data.Result != "") {
                $scope.model.CurDocumentScanat = response.data.Result;
                angular.copy($scope.model.CurDocumentScanat, $scope.model.DocumenteScanate[$scope.curDocumentIndex]);
            }
            //spinner.stop();
        }, function (response) {
            alert('Erroare: ' + response.status + ' - ' + response.data);
            //spinner.stop();
        });
    };

    $scope.findValue = function (searchValues, enteredValue) {
        if (searchValues != null && searchValues != undefined) {
            var found = $filter('getById')(searchValues, enteredValue);
            return found;
        }
    };

    $scope.deleteDoc = function () {
        spinner.spin(document.getElementById('main'));
        var id = $scope.model.CurDocumentScanat.ID;
        alert(id);
        $http.get('/DocumenteScanate/Delete/' + id)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if ($scope.result.Status) {
                        $scope.model.CurDocumentScanat = {};
                        $scope.model.DocumenteScanate.splice($scope.curDocumentIndex, 1);
                        $scope.curDocumentIndex = -1;
                    }
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.SaveEdit = function () {
        spinner.spin(document.getElementById('main'));
        var data = $scope.model.CurDocumentScanat;
        //alert(data.CurDocumentScanat.ID + ' - ' + data.CurDocumentScanat.ID_DOSAR + ' - ' + data.CurDocumentScanat.ID_TIP_DOCUMENT + ' - ' + data.CurDocumentScanat.CALE_FISIER + ' - ' + data.CurDocumentScanat.DENUMIRE_FISIER + ' - ' + data.CurDocumentScanat.EXTENSIE_FISIER);
        $http.post('/DocumenteScanate/Edit', data)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if ($scope.result.Status) {
                        if ($scope.result.InsertedId != null) {
                            $scope.model.CurDocumentScanat.ID = $scope.result.InsertedId;
                            $scope.model.DocumenteScanate.push($scope.model.CurDocumentScanat);
                        }
                        $scope.ShowDocument($scope.model.CurDocumentScanat.ID);
                    }
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    // upload on file select or drop
    $scope.upload = function (file) {
        spinner.spin(document.getElementById('main'));
        if (!Upload.isFile(file)) return;
        Upload.upload({
            url: '/DocumenteScanate/PostFile',
            data: {file: file}
        }).then(function (resp) {
            var j = JSON.parse(resp.data);
            //alert(j.DENUMIRE_FISIER);
            $scope.model.CurDocumentScanat.DENUMIRE_FISIER = j.DENUMIRE_FISIER;
            $scope.model.CurDocumentScanat.EXTENSIE_FISIER = j.EXTENSIE_FISIER;
            $scope.model.CurDocumentScanat.CALE_FISIER = j.CALE_FISIER;
            //console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
            spinner.stop();
            $scope.SaveEdit();
        }, function (resp) {
            alert(resp.status + ' - ' + resp.data);
            console.log('Error status: ' + resp.status);
            spinner.stop();
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
        });
    };
    // for multiple files:
    $scope.uploadFiles = function (files) {
        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                //Upload.upload({..., data: {file: files[i]}, ...})...;
            }
            // or send them all together for HTML5 browsers:
            //Upload.upload({..., data: {file: files}, ...})...;
        }
    }
});