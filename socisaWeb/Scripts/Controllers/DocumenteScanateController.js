'use strict';
function toggleDivs(activeDiv) {
    document.getElementById('detaliiDocument').style.display = activeDiv == "detaliiDocument" ? 'block' : 'none'; 
    document.getElementById('incarcareFisiere').style.display = activeDiv == "incarcareFisiere" ? 'block' : 'none';
}

function toggleAllThumbs(e) {
    $('.checkOverImage').prop('checked', e.checked);
}

app.controller('DocumenteScanateController',
function ($scope, $http, $filter, $rootScope, $window, Upload, ngDialog, PromiseUtils, myService) {
    $scope.lastActiveIdDosar = "";
    $scope.model = {};
    $scope.curDocumentIndex = -1;
    $scope.curDocumentSubIndex = 0;
    $scope.model.TipuriDocumente = [];
    $scope.model.CurDocumentScanat = {};
    $scope.fileIndex = $scope.filesLength = -1;

    $scope.showDocumentByIndex = function (index) {
        $scope.curDocumentIndex = index;
        try
        {
            if ($scope.curDocumentSubIndex > $scope.model.TipuriDocumente[index].DocumenteScanate.length) $scope.curDocumentSubIndex = 0;

            var doc = $scope.model.TipuriDocumente[index].DocumenteScanate[$scope.curDocumentSubIndex];
            angular.copy(doc, $scope.model.CurDocumentScanat);

            if ($scope.model.CurDocumentScanat == null || $scope.model.CurDocumentScanat.ID == null)
            {
                $scope.model.CurDocumentScanat = {};
                $scope.model.CurDocumentScanat.FILE_CONTENT = null;
                $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
                $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[index].TipDocument.ID;
                $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
            }
        } catch (e) {
            $scope.model.CurDocumentScanat = {};
            $scope.model.CurDocumentScanat.FILE_CONTENT = null;
            $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
            $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[index].TipDocument.ID;
            $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
        }
    };

    $scope.SetCurDocument = function (doc, index) {
        $scope.curDocumentSubIndex = index;
        angular.copy(doc, $scope.model.CurDocumentScanat);
        //model.CurDocumentScanat = documentScanat;
    };

    $scope.areDocumentAvizat = function (id_tip_document) {
        try {
            for (var i = 0; i < $scope.model.TipuriDocumente.length; i++) {
                if ($scope.model.TipuriDocumente[i].TipDocument.ID == id_tip_document) {
                    if ($scope.model.TipuriDocumente[i].DocumenteScanate == null || $scope.model.TipuriDocumente[i].DocumenteScanate.length == 0) return 0;
                    for (var j = 0; j < $scope.model.TipuriDocumente[i].DocumenteScanate.length; j++) {
                        if ($scope.model.TipuriDocumente[i].DocumenteScanate[j].VIZA_CASCO) {
                            return 2;
                        }
                    }
                }
            }
            return 1; // 2 = AVIZAT / 1 = are document, dar nu e avizat.
        } catch (e) { return 0; }
    };


    $scope.getTipDocumentByDenumire = function (denumireTipDoc) {
        try{
            for (var i = 0; i < $scope.model.TipuriDocumente.length; i++) {
                var tDoc = $scope.model.TipuriDocumente[i].TipDocument;
                if ($scope.model.TipuriDocumente[i].TipDocument.DENUMIRE == denumireTipDoc) {
                    return tDoc;
                }
            }
            return null;
        } catch (e) { return null;}
    };

    $scope.showMandatory = function (tipDoc) {
        try {
            switch (tipDoc.TipDocument.DENUMIRE) {
                case "CEDAM":
                    var tDoc = $scope.getTipDocumentByDenumire("POLIȚĂ VINOVAT");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "POLIȚĂ VINOVAT":
                    var tDoc = $scope.getTipDocumentByDenumire("CEDAM");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "FACTURĂ DE REPARAȚII":
                    var tDoc = $scope.getTipDocumentByDenumire("CALCUL VMD");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "CALCUL VMD":
                    var tDoc = $scope.getTipDocumentByDenumire("FACTURĂ DE REPARAȚII");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
            }

            return !($scope.areDocumentAvizat(tipDoc.TipDocument.ID) == 2) && tipDoc.TipDocument.MANDATORY;
            return tipDoc.TipDocument.MANDATORY;
        } catch (e) { return false; }
    };

    $scope.AvizareDocument = function (avizat) {
        //$scope.model.CurDocumentScanat.VIZA_CASCO = avizat;
        //$scope.SaveEdit();
        var tDoc = $scope.model.TipuriDocumente[$scope.curDocumentIndex];
        for (var i = 0; i < tDoc.DocumenteScanate.length; i++) {
            var id = '#chk_' + tDoc.DocumenteScanate[i].ID;
            var chk = $(id).prop('checked');
            if (chk && tDoc.DocumenteScanate[i].VIZA_CASCO != avizat)
            {
                tDoc.DocumenteScanate[i].VIZA_CASCO = avizat;
                spinner.spin(document.getElementById('main'));
                var data = tDoc.DocumenteScanate[i];
                $http.post('/DocumenteScanate/Avizare', data)
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.showMessage = true;
                            $scope.result = response.data;
                            if ($scope.result.Status) {
                                $scope.model.CurDocumentScanat = {};
                                $scope.ShowDocuments($rootScope.ID_DOSAR);
                            }
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
            }
        }
    };

    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined && $rootScope.activeTab == 'documente') {
            $scope.model.ID_DOSAR = newValue;
            $scope.ShowDocuments(newValue);
        }
    });

    $rootScope.$watch('activeTab', function (newValue, oldValue) {
        if (newValue == 'documente' && $rootScope.ID_DOSAR != null && $rootScope.ID_DOSAR != undefined && $scope.lastActiveIdDosar != $rootScope.ID_DOSAR) {
            $scope.model.ID_DOSAR = $rootScope.ID_DOSAR;
            $scope.ShowDocuments($rootScope.ID_DOSAR);
            $scope.lastActiveIdDosar = $rootScope.ID_DOSAR;
        }
    });

    $scope.ShowDocuments = function (id_dosar) {
        spinner.spin(document.getElementById('main'));
        myService.getlist('GET', '/DocumenteScanate/Details/' + id_dosar, null)
          .then(function (response) {
              if ($scope.fileIndex == $scope.filesLength - 1 || ($scope.fileIndex == -1 && $scope.filesLength == -1)) {
                  if (response != 'null' && response != null && response.data != null) {
                      $scope.model.TipuriDocumente = response.data.TipuriDocumente;
                  }
                  else {
                      $scope.model.TipuriDocumente = null;
                  }
                  if ($scope.curDocumentIndex > -1) {
                      $scope.showDocumentByIndex($scope.curDocumentIndex);
                  }
              }
              spinner.stop();
          }, function (response) {
              spinner.stop();
              alert('Erroare: ' + response.status + ' - ' + response.data);
          });
    };

    $scope.vizualizareDoc = function () {
        $window.open("scans/" + $scope.model.CurDocumentScanat.CALE_FISIER);
    };

    $scope.deleteDoc = function () {
        //document.getElementById("modal").style.display = 'table';
        /*
        ngDialog.openConfirm({
            template: '<div><p>De ce? </p><button type="button" class="btn btn-default" ng-click="closeThisDialog(0)">Nu</button><button type="button" class="btn btn-primary" ng-click="confirm(1)">Da</button></div>',
                plain: true
        }).then(
			function (value) {
			    document.getElementById("modal").style.display = "none";
        */
        var tDoc = $scope.model.TipuriDocumente[$scope.curDocumentIndex];
        for (var i = 0; i < tDoc.DocumenteScanate.length; i++) {
            var id = '#chk_' + tDoc.DocumenteScanate[i].ID;
            var chk = $(id).prop('checked');
            if (chk) {
                spinner.spin(document.getElementById('main'));
                var id = tDoc.DocumenteScanate[i].ID;
                $http.get('/DocumenteScanate/Delete/' + id)
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.showMessage = true;
                            $scope.result = response.data;
                            if ($scope.result.Status) {
                                $scope.model.CurDocumentScanat = {};
                                $scope.ShowDocuments($rootScope.ID_DOSAR);
                            }
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
            }
        }
        /*
			},
			function (value) {
			    document.getElementById("modal").style.display = "none";
			}
        );
        */
    };

    $scope.SaveEdit = function (doc) {
        spinner.spin(document.getElementById('main'));
        var data = doc == null ? $scope.model.CurDocumentScanat : doc;
        $http.post('/DocumenteScanate/Edit', data)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if (doc == null) {
                        if ($scope.result.Status) {
                            $scope.model.CurDocumentScanat = {};
                            $scope.ShowDocuments($rootScope.ID_DOSAR);
                        }
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
        if (file == null || !Upload.isFile(file)) return;
        spinner.spin(document.getElementById('main'));

        $scope.model.CurDocumentScanat = {};
        $scope.model.CurDocumentScanat.FILE_CONTENT = null;
        $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
        $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[$scope.curDocumentIndex].TipDocument.ID;
        $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;

        Upload.upload({
            url: '/DocumenteScanate/PostFile',
            data: {file: file}
        }).then(function (resp) {
            var j = JSON.parse(resp.data);
            //alert(j.DENUMIRE_FISIER);
            $scope.model.CurDocumentScanat.DENUMIRE_FISIER = j.DENUMIRE_FISIER;
            $scope.model.CurDocumentScanat.EXTENSIE_FISIER = j.EXTENSIE_FISIER;
            $scope.model.CurDocumentScanat.CALE_FISIER = j.CALE_FISIER;
            $scope.model.CurDocumentScanat.DATA_INCARCARE = j.DATA_INCARCARE;
            $scope.model.CurDocumentScanat.DIMENSIUNE_FISIER = j.DIMENSIUNE_FISIER;
            //console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
            spinner.stop();
            $scope.SaveEdit(null);
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
            $scope.filesLength = files.length;
            for (var i = 0; i < files.length; i++) {
                $scope.fileIndex = i;
                $scope.upload(files[i]);
                //Upload.upload({..., data: {file: files[i]}, ...})...;
            }
            // or send them all together for HTML5 browsers:
            //Upload.upload({..., data: {file: files}, ...})...;
        }
    };
});