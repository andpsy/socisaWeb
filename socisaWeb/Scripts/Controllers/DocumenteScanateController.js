'use strict';
/*
function toggleDivs(activeDiv) {
    document.getElementById('detaliiDocument').style.display = activeDiv == "detaliiDocument" ? (document.getElementById('detaliiDocument').style.display == 'block' ? 'none' : 'block') : 'none'; 
    document.getElementById('incarcareFisiere').style.display = activeDiv == "incarcareFisiere" ? (document.getElementById('incarcareFisiere').style.display == 'block' ? 'none' : 'block') : 'none';
}
function toggleAllThumbs(e) {
    $('.checkOverImage').prop('checked', e.checked);
}
*/

app.controller('DocumenteScanateController',
function ($scope, $http, $filter, $rootScope, $window, $q, Upload, ngDialog, PromiseUtils, myService) {
    $scope.lastActiveIdDosar = "";
    $scope.model = {};
    $scope.curDocumentIndex = -1;
    $scope.curDocumentSubIndex = 0;
    $scope.model.TipuriDocumente = [];
    $scope.model.CurDocumentScanat = {};
    $scope.fileIndex = $scope.filesLength = -1;
    $scope.curThumb = $scope.defaultThumb = "../Content/empty.jpg";

    $scope.toggleDivs = function (activeDiv, brutForce) {
        console.log('toggleDivs - ' + activeDiv);
        document.getElementById('detaliiDocument').style.display = activeDiv == "detaliiDocument" ? brutForce != null ? brutForce : (document.getElementById(activeDiv).style.display == 'block' ? 'none' : 'block') : 'none';
        document.getElementById('incarcareFisiere').style.display = activeDiv == "incarcareFisiere" ? brutForce != null ? brutForce : (document.getElementById(activeDiv).style.display == 'block' ? 'none' : 'block') : 'none';
    };

    $scope.toggleAllThumbs = function () {
        console.log('toggleAllThumbs - ' + $scope.toggle_all_docs);
        //$('.checkOverImage').prop('checked', $scope.toggle_all_docs);
        for (var i = 0; i < $scope.model.TipuriDocumente[$scope.curDocumentIndex].DocumenteScanate.length; i++) {
            $scope.model.TipuriDocumente[$scope.curDocumentIndex].DocumenteScanate[i].VIZA_CASCO = $scope.toggle_all_docs;
            $scope.AvizareDocument($scope.model.TipuriDocumente[$scope.curDocumentIndex].DocumenteScanate[i]);
        }
    };

    $scope.showDocumentByIndex = function (index) {
        $scope.curDocumentIndex = index;
        try
        {
            if ($scope.curDocumentSubIndex > $scope.model.TipuriDocumente[index].DocumenteScanate.length) $scope.curDocumentSubIndex = 0;

            var doc = $scope.model.TipuriDocumente[index].DocumenteScanate[$scope.curDocumentSubIndex];
            angular.copy(doc, $scope.model.CurDocumentScanat);
            if ($scope.model.TipuriDocumente[index].DocumenteScanate.length > 0) {
                $scope.curThumb = $scope.getThumbnailFile($scope.model.CurDocumentScanat.CALE_FISIER, $scope.model.CurDocumentScanat.EXTENSIE_FISIER);
                $scope.toggleDivs('incarcareFisiere', 'none');
                $scope.checkAllAvizari();
            }
            else {
                $scope.curThumb = $scope.defaultThumb;
                $scope.toggleDivs('incarcareFisiere', 'block');
            }

            if ($scope.model.CurDocumentScanat == null || $scope.model.CurDocumentScanat.ID == null)
            {
                $scope.model.CurDocumentScanat = {};
                $scope.model.CurDocumentScanat.FILE_CONTENT = null;
                $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
                $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[index].TipDocument.ID;
                $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
                $scope.curThumb = $scope.defaultThumb;
            }
        } catch (e) {
            $scope.model.CurDocumentScanat = {};
            $scope.model.CurDocumentScanat.FILE_CONTENT = null;
            $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
            $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[index].TipDocument.ID;
            $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
            $scope.curThumb = $scope.defaultThumb;
        }
    };

    $scope.SetCurDocument = function (doc, index) {
        $scope.curDocumentSubIndex = index;
        angular.copy(doc, $scope.model.CurDocumentScanat);
        //model.CurDocumentScanat = documentScanat;
        $scope.curThumb = $scope.getThumbnailFile($scope.model.CurDocumentScanat.CALE_FISIER, $scope.model.CurDocumentScanat.EXTENSIE_FISIER);
    };

    $scope.areDocumentAvizat = function (id_tip_document) {
        try {
            for (var i = 0; i < $scope.model.TipuriDocumente.length; i++) {
                if ($scope.model.TipuriDocumente[i].TipDocument.ID == id_tip_document) {
                    //alert($scope.model.TipuriDocumente[i].DocumenteScanate);
                    if ($scope.model.TipuriDocumente[i].DocumenteScanate == null || $scope.model.TipuriDocumente[i].DocumenteScanate.length == 0) {
                        //alert('0');
                        return 0;
                    }
                    for (var j = 0; j < $scope.model.TipuriDocumente[i].DocumenteScanate.length; j++) {
                        if ($scope.model.TipuriDocumente[i].DocumenteScanate[j].VIZA_CASCO) {
                            //alert('2');
                            return 2;
                        }
                    }
                }
            }
            //alert('1');
            return 1; // 2 = AVIZAT / 1 = are document, dar nu e avizat.
        } catch (e) { alert(e); return 0; }
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
                    var tDoc = $scope.getTipDocumentByDenumire("POLITA VINOVAT");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "POLITA VINOVAT":
                    var tDoc = $scope.getTipDocumentByDenumire("CEDAM");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "FACTURA DE REPARATII":
                    var tDoc = $scope.getTipDocumentByDenumire("CALCUL VMD");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "CALCUL VMD":
                    var tDoc = $scope.getTipDocumentByDenumire("FACTURA DE REPARATII");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;

                case "PROCES VERBAL":
                    var tDoc = $scope.getTipDocumentByDenumire("CONSTATARE AMIABILA");
                    if (tDoc != null) {
                        if ($scope.areDocumentAvizat(tDoc.ID) == 2) {
                            return false;
                        }
                    }
                    break;
                case "CONSTATARE AMIABILA":
                    var tDoc = $scope.getTipDocumentByDenumire("PROCES VERBAL");
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

    $scope.countDocs = function (tipDoc) {
        var toReturn = [];
        var avizate = 0; var neavizate = 0;
        for (var i = 0; i < tipDoc.DocumenteScanate.length; i++) {
            if (tipDoc.DocumenteScanate[i].VIZA_CASCO)
                avizate++;
            else
                neavizate++;
        }
        toReturn.push(avizate); toReturn.push(neavizate);
        return toReturn;
    };

    //avizare cu bife indirecta
    $scope.AvizareDocumente = function (avizat) {
        //$scope.model.CurDocumentScanat.VIZA_CASCO = avizat;
        //$scope.SaveEdit();
        var tDoc = $scope.model.TipuriDocumente[$scope.curDocumentIndex];
        for (var i = 0; i < tDoc.DocumenteScanate.length; i++) {
            var id = '#chk_' + tDoc.DocumenteScanate[i].ID;
            var chk = $(id).prop('checked');
            if (chk && tDoc.DocumenteScanate[i].VIZA_CASCO != avizat)
            {
                tDoc.DocumenteScanate[i].VIZA_CASCO = avizat;
                spinner.spin(document.getElementById(ACTIVE_DIV_ID));
                var data = tDoc.DocumenteScanate[i];
                $http.post('/DocumenteScanate/Avizare', data)
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $('.alert').show();
                            $scope.showMessage = true;
                            $scope.result = response.data;
                            
                            $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
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

    //avizare cu bifa direct
    $scope.AvizareDocument = function (document_scanat) {
        console.log('AvizareDocument - ' + document_scanat.VIZA_CASCO);

        //var id = '#chk_' + document_scanat.ID;
        //var chk = $(id).prop('checked');
        //if (chk != document_scanat.VIZA_CASCO) {
            //document_scanat.VIZA_CASCO = chk;
            spinner.spin(document.getElementById(ACTIVE_DIV_ID));
            $http.post('/DocumenteScanate/Avizare', document_scanat)
                .then(function (response) {
                    if (response != 'null' && response != null && response.data != null) {
                        $('.alert').show();
                        $scope.showMessage = true;
                        $scope.result = response.data;
                        $scope.model.CurDocumentScanat.VIZA_CASCO = document_scanat.VIZA_CASCO;
                        $scope.checkAllAvizari();


                        $http.post('/Dosare/ValidareAvizare', { id: $rootScope.ID_DOSAR })
                            .then(function (response2) {
                                if (response2 != 'null' && response2 != null && response2.data != null) {
                                    console.log('validare avizare - ' + JSON.parse(response2.data.toLowerCase()));
                                    $rootScope.validForAvizare = JSON.parse(response2.data.toLowerCase());
                                }
                            },function (response2) {
                                    alert('Erroare: ' + response2.status + ' - ' + response2.data);
                            });

                        $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                        /*
                        if ($scope.result.Status) {
                            $scope.model.CurDocumentScanat = {};
                            $scope.ShowDocuments($rootScope.ID_DOSAR);
                        }
                        */
                    }
                    spinner.stop();
                }, function (response) {
                    spinner.stop();
                    alert('Erroare: ' + response.status + ' - ' + response.data);
                });
        //}
    };

    $scope.checkAllAvizari = function () {
        var cnt = 0;
        for (var i = 0; i < $scope.model.TipuriDocumente[$scope.curDocumentIndex].DocumenteScanate.length; i++)
            if ($scope.model.TipuriDocumente[$scope.curDocumentIndex].DocumenteScanate[i].VIZA_CASCO) cnt++;
        console.log('checkAllAvizari - ' + cnt);
        $scope.toggle_all_docs = (cnt == $scope.model.TipuriDocumente[$scope.curDocumentIndex].DocumenteScanate.length);
    };

    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined && $rootScope.activeTab == 'documente') {
            $scope.lastActiveIdDosar = "";
            $scope.model = {};
            $scope.curDocumentIndex = -1;
            $scope.curDocumentSubIndex = 0;
            $scope.model.TipuriDocumente = [];
            $scope.model.CurDocumentScanat = {};
            $scope.fileIndex = $scope.filesLength = -1;
            $scope.curThumb = "";

            $scope.model.ID_DOSAR = newValue;
            $scope.ShowDocuments(newValue);
        }
    });

    $rootScope.$watch('activeTab', function (newValue, oldValue) {
        if (newValue == 'documente' && $rootScope.ID_DOSAR != null && $rootScope.ID_DOSAR != undefined && $scope.lastActiveIdDosar != $rootScope.ID_DOSAR) {
            $scope.lastActiveIdDosar = "";
            $scope.model = {};
            $scope.curDocumentIndex = -1;
            $scope.curDocumentSubIndex = 0;
            $scope.model.TipuriDocumente = [];
            $scope.model.CurDocumentScanat = {};
            $scope.fileIndex = $scope.filesLength = -1;
            $scope.curThumb = "";

            $scope.model.ID_DOSAR = $rootScope.ID_DOSAR;
            $scope.ShowDocuments($rootScope.ID_DOSAR);
            $scope.lastActiveIdDosar = $rootScope.ID_DOSAR;
        }
    });

    $scope.ShowDocuments = function (id_dosar) {
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        console.log('showdocuments');
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
        $scope.toggleDivs('null', null);
        $window.open("../scans/" + $scope.model.CurDocumentScanat.CALE_FISIER);
    };

    //varianta cu bife multipe
    $scope.deleteDocs = function () {
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
                spinner.spin(document.getElementById(ACTIVE_DIV_ID));
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

    //varianta fara bife multipe
    $scope.deleteDoc = function () {
        $scope.toggleDivs('null', null);
        //document.getElementById("modal").style.display = 'table';
        /*
        ngDialog.openConfirm({
            template: '<div><p>De ce? </p><button type="button" class="btn btn-default" ng-click="closeThisDialog(0)">Nu</button><button type="button" class="btn btn-primary" ng-click="confirm(1)">Da</button></div>',
                plain: true
        }).then(
			function (value) {
			    document.getElementById("modal").style.display = "none";
        */
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        var id = $scope.model.CurDocumentScanat.ID;
        $http.get('/DocumenteScanate/Delete/' + id)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    if ($scope.result.Status) {
                        $scope.model.CurDocumentScanat = {};
                        $scope.curThumb = $scope.defaultThumb;
                        $scope.ShowDocuments($rootScope.ID_DOSAR);
                    }
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };


    $scope.SaveEdit = function (doc) {
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        var data = doc == null ? $scope.model.CurDocumentScanat : doc;
        $http.post('/DocumenteScanate/Edit', data)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result = response.data;
                    
                    $(".alert").delay(MESSAGE_DELAY).fadeOut(MESSAGE_FADE_OUT);
                    /*
                    if (doc == null) {
                        if ($scope.result.Status) {
                            $scope.model.CurDocumentScanat = {};
                            $scope.ShowDocuments($rootScope.ID_DOSAR);
                        }
                    }
                    */
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.Refresh = function () {
        $scope.model.CurDocumentScanat = {};
        $scope.ShowDocuments($rootScope.ID_DOSAR);
        console.log('refresh');
    };

    $scope.SaveAndRefresh = function (doc) {
        $scope.SaveEdit(doc);
        $scope.Refresh();
    };

    // upload on file select or drop
    /*
    $scope.upload = function (file) {
        if (file == null || !Upload.isFile(file)) return;
        spinner.spin(document.getElementById(ACTIVE_DIV_ID));
        $scope.model.CurDocumentScanat = {};
        $scope.model.CurDocumentScanat.ID = null;
        $scope.model.CurDocumentScanat.FILE_CONTENT = null;
        $scope.model.CurDocumentScanat.MEDIUM_ICON = null;
        $scope.model.CurDocumentScanat.ID_TIP_DOCUMENT = $scope.model.TipuriDocumente[$scope.curDocumentIndex].TipDocument.ID;
        $scope.model.CurDocumentScanat.ID_DOSAR = $rootScope.ID_DOSAR;
        Upload.upload({
            url: '/DocumenteScanate/PostFile',
            data: { file: file, id_tip_document: $scope.model.TipuriDocumente[$scope.curDocumentIndex].TipDocument.ID, id_dosar: $rootScope.ID_DOSAR }
        }).then(function (resp) {
            var j = JSON.parse(resp.data);
            $scope.model.CurDocumentScanat.DENUMIRE_FISIER = j.DENUMIRE_FISIER;
            $scope.model.CurDocumentScanat.EXTENSIE_FISIER = j.EXTENSIE_FISIER;
            $scope.model.CurDocumentScanat.CALE_FISIER = j.CALE_FISIER;
            $scope.model.CurDocumentScanat.DATA_INCARCARE = j.DATA_INCARCARE;
            $scope.model.CurDocumentScanat.DIMENSIUNE_FISIER = j.DIMENSIUNE_FISIER;
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
    */

    // for multiple files:
    $scope.uploadFiles = function (files) {
        if (files && files.length) {
            spinner.spin(document.getElementById(ACTIVE_DIV_ID));
            $scope.filesLength = files.length;
            var qs = [];
            for (var i = 0; i < files.length; i++) {
                if (files[i] == null || !Upload.isFile(files[i])) break;
                $scope.fileIndex = i;
                //$scope.upload(files[i]); // old version
                ////Upload.upload({..., data: {file: files[i]}, ...})...;
                var x = Upload.upload({
                    url: '/DocumenteScanate/PostFile',
                    data: { file: files[i], id_tip_document: $scope.model.TipuriDocumente[$scope.curDocumentIndex].TipDocument.ID, id_dosar: $rootScope.ID_DOSAR }
                })
                x.then(function (resp) { }, function (resp) { }, function (evt) {
                    var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                    console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
                });
                qs.push(x);
            }
            // or send them all together for HTML5 browsers:
            //Upload.upload({..., data: {file: files}, ...})...;
            $q.all(qs).then(function (response) {
                $scope.Refresh();
                spinner.stop();
            }, function (response) {
                alert('q err');
                spinner.stop();
            });
        }
    };

    $scope.getThumbnailFile = function (file_name, ext) {
        var supported_extensions = ".jpg,.jpeg,.png,.bmp,.pdf";
        var thumb = "";
        if (file_name == null || file_name == "" || ext == null || supported_extensions.indexOf(ext.toLowerCase()) == -1) {
            thumb = "../content/images/UnsupportedType_Custom.jpg";
        } else {
            thumb = "../scans/" + file_name.substring(0, file_name.indexOf(ext)) + "_Custom.jpg";
        }
        return thumb;
    };
});