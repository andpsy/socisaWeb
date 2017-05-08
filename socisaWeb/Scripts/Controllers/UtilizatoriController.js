'use strict';
function toggleChecks(id_prefix) {
    var e_id = "#" + id_prefix + "all";
    var chk = $(e_id).prop('checked');
    $('input[id^="' + id_prefix + '"]').prop('checked', chk);
}

app.controller('UtilizatoriController',
function ($scope, $http, $filter, $rootScope, $q) {
    $scope.ID_UTILIZATOR = null;
    $scope.model = {};
    $scope.model.UtilizatorJson = {};
    $scope.model.Drepturi = [];
    $scope.model.Actions = [];
    $scope.model.TipuriUtilizator = [];
    $scope.model.SocietatiAsigurare = [];
    $scope.model.SocietatiAsigurareAdministrate = [];
    $scope.TipUtilizator = {};
    $scope.UtilizatorJson = {};
    $scope.SocietateAsigurare = {};
    $scope.NewRights = [];
    $scope.NewActions = [];
    $scope.NewSocietatiAdministrate = [];
    $scope.result = [];

    $scope.toggleDrepturiChecks = function (id_prefix) {
        var e_id = "#" + id_prefix + "all";
        var chk = $(e_id).prop('checked');
        for (var i = 0; i < $scope.model.Drepturi.length; i++) {
            $scope.model.Drepturi[i].selected = chk;
            $scope.AddRemoveDrept($scope.model.Drepturi[i]);
        }
    };

    $scope.toggleActionsChecks = function (id_prefix) {
        var e_id = "#" + id_prefix + "all";
        var chk = $(e_id).prop('checked');
        for (var i = 0; i < $scope.model.Actions.length; i++) {
            $scope.model.Actions[i].selected = chk;
            $scope.AddRemoveAction($scope.model.Actions[i]);
        }
    };

    $scope.toggleSocietatiAdministrateChecks = function (id_prefix) {
        var e_id = "#" + id_prefix + "all";
        var chk = $(e_id).prop('checked');
        for (var i = 0; i < $scope.model.SocietatiAsigurareAdministrate.length; i++) {
            $scope.model.SocietatiAsigurareAdministrate[i].selected = chk;
            $scope.AddRemoveSocietateAdministrata($scope.model.SocietatiAsigurareAdministrate[i]);
        }
    };

    $scope.setUtilizator = function (utilizator) {
        angular.copy(utilizator, $scope.UtilizatorJson);

        angular.copy(utilizator.Drepturi, $scope.NewRights);
        angular.copy(utilizator.Actions, $scope.NewActions);
        angular.copy(utilizator.SocietatiAsigurareAdministrate, $scope.NewSocietatiAdministrate);

        $scope.setTipUtilizator(utilizator.TipUtilizator);
        $scope.setSocietate(utilizator.SocietateAsigurare);
        $scope.setDrepturiActive();
        $scope.setActiuniActive();
        $scope.setSocietatiAdministrateActive();
        $scope.ID_UTILIZATOR = utilizator.Utilizator.ID;
    };

    $scope.setTipUtilizator = function (tipUtilizator) {
        angular.copy(tipUtilizator, $scope.TipUtilizator);
        $scope.UtilizatorJson.Utilizator.ID_TIP_UTILIZATOR = tipUtilizator.ID;
    };

    $scope.setSocietate = function (societate) {
        angular.copy(societate, $scope.SocietateAsigurare);
        $scope.UtilizatorJson.Utilizator.ID_SOCIETATE = societate.ID;
    };

    $scope.setDrepturiActive = function () {
        //angular.copy($scope.model.UtilizatorJson.Drepturi, $scope.NewRights);
        //$('input[id^="chk_drept_"]').prop('checked', false);
        angular.forEach($scope.model.Drepturi, function (value1, key1)
        {
            var gasit = false;
            angular.forEach($scope.UtilizatorJson.Drepturi, function (value2, key2)
            {
                if (value1.ID == value2.ID)
                {
                    //$("#chk_drept_" + value1.ID).prop("checked", true);
                    gasit = true;
                    return;
                }
            });
            value1.selected = gasit;
        });
    };

    $scope.setActiuniActive = function () {
        //angular.copy($scope.model.UtilizatorJson.Actions, $scope.NewActions);
        //$('input[id^="chk_action_"]').prop('checked', false);
        angular.forEach($scope.model.Actions, function (value1, key1) {
            var gasit = false;
            angular.forEach($scope.UtilizatorJson.Actions, function (value2, key2) {
                if (value1.ID == value2.ID) {
                    //$("#chk_action_" + value1.ID).prop("checked", true);
                    gasit = true;
                    return;
                }
            });
            value1.selected = gasit;
        });
    };

    $scope.getStatus = function (id) {
        for (var i = 0; i < $scope.result.length; i++) {
            if ($scope.result[i].ID == id) {
                return $scope.result[i];
            }
        }
    };

    $scope.setSocietatiAdministrateActive = function () {
        //angular.copy($scope.model.UtilizatorJson.SocietatiAsigurareAdministrate, $scope.NewSocietatiAdministrate);
        //$('input[id^="chk_societate_administrata_"]').prop('checked', false);
        angular.forEach($scope.model.SocietatiAsigurareAdministrate, function (value1, key1) {
            var gasit = false;
            angular.forEach($scope.UtilizatorJson.SocietatiAsigurareAdministrate, function (value2, key2) {
                if (value1.ID == value2.ID) {
                    //$("#chk_societate_administrata_" + value1.ID).prop("checked", true);
                    gasit = true;
                    return;
                }
            });
            value1.selected = gasit;
        });
    };

    $scope.SaveUtilizator = function () {
        spinner.spin(document.getElementById('main'));
        $http.post('/Utilizatori/Save', $scope.UtilizatorJson.Utilizator)
            .then(function (response) {
                if (response != 'null' && response != null && response.data != null) {
                    $scope.showMessage = true;
                    $scope.result.push(response.data);
                    if (response.data.Status) {
                    }
                }
                spinner.stop();
            }, function (response) {
                spinner.stop();
                alert('Erroare: ' + response.status + ' - ' + response.data);
            });
    };

    $scope.AddRemoveDrept = function (drept) {
        if (drept.selected) {
            $scope.NewRights.push(drept);
        }
        else {
            var idx = -1;
            for (var i = 0; i < $scope.NewRights.length; i++) {
                if ($scope.NewRights[i].ID === drept.ID) {
                    idx = i;
                    break;
                }
            }
            $scope.NewRights.splice(idx, 1);
        }
    };

    $scope.AddRemoveAction = function (action) {
        if (action.selected) {
            $scope.NewActions.push(action);
        }
        else {
            var idx = -1;
            for (var i = 0; i < $scope.NewActions.length; i++) {
                if ($scope.NewActions[i].ID === action.ID) {
                    idx = i;
                    break;
                }
            }
            $scope.NewActions.splice(idx, 1);
        }
    };

    $scope.AddRemoveSocietateAdministrata = function (societate_administrata) {
        if (societate_administrata.selected) {
            $scope.NewSocietatiAdministrate.push(societate_administrata);
        }
        else {
            var idx = -1;
            for (var i = 0; i < $scope.NewSocietatiAdministrate.length; i++) {
                if ($scope.NewSocietatiAdministrate[i].ID === societate_administrata.ID) {
                    idx = i;
                    break;
                }
            }
            $scope.NewSocietatiAdministrate.splice(idx, 1);
        }
    };

    $scope.qSaveDrepturi = function () {
        $q.when($scope.SaveDrepturi()).then(function (response) {
            if ($scope.existaModificari) alert('vsdfdsfsd');
        });
    };

    $scope.SaveDrepturi = function () {
        $scope.showMessage = true;
        $scope.existaModificari = false;
        var qs = [];
        for (var i = 0; i < $scope.NewRights.length; i++) {
            var gasit = false;
            for (var j = 0; j < $scope.UtilizatorJson.Drepturi.length; j++) {
                if ($scope.NewRights[i].ID == $scope.UtilizatorJson.Drepturi[j].ID) {
                    gasit = true;
                    break;
                }
            }
            if (!gasit) {
                spinner.spin(document.getElementById('main'));
                
                var x = $http.post('/UtilizatoriDrepturi/Save', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_DREPT: $scope.NewRights[i].ID })
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.existaModificari = true;
                            $scope.result.push(response.data);
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
                qs.push(x);
                //qs.push($http.post('/UtilizatoriDrepturi/Save', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_DREPT: $scope.NewRights[i].ID }));
            }
        }

        for (var i = 0; i < $scope.UtilizatorJson.Drepturi.length; i++) {
            var gasit = false;
            for (var j = 0; j < $scope.NewRights.length; j++) {
                if ($scope.UtilizatorJson.Drepturi[i].ID == $scope.NewRights[j].ID) {
                    gasit = true;
                    break;
                }
            }
            if (!gasit) {
                spinner.spin(document.getElementById('main'));
                
                var y = $http.post('/UtilizatoriDrepturi/Delete', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_DREPT: $scope.UtilizatorJson.Drepturi[i].ID })
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.existaModificari = true;
                            $scope.result.push(response.data);
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
                qs.push(y);
                //qs.push($http.post('/UtilizatoriDrepturi/Delete', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_DREPT: $scope.UtilizatorJson.Drepturi[i].ID }));
            }
        }

        $q.all(qs).then(function (response) {
            if ($scope.existaModificari) {
                $http.get('/Utilizatori/IndexJson').then(function (response) {
                    $scope.model = response.data;
                    for (var i = 0; i < $scope.model.UtilizatorJson.UtilizatoriSubordonati.length; i++) {
                        if ($scope.model.UtilizatorJson.UtilizatoriSubordonati[i].Utilizator.ID === $scope.UtilizatorJson.Utilizator.ID) {
                            $scope.setUtilizator($scope.model.UtilizatorJson.UtilizatoriSubordonati[i]);
                            break;
                        }
                    }                    
                });
            }
        });
    };


    $scope.SaveActiuni = function () {
        $scope.showMessage = true;
        $scope.existaModificari = false;
        var qs = [];
        for (var i = 0; i < $scope.NewActions.length; i++) {
            var gasit = false;
            for (var j = 0; j < $scope.UtilizatorJson.Actions.length; j++) {
                if ($scope.NewActions[i].ID == $scope.UtilizatorJson.Actions[j].ID) {
                    gasit = true;
                    break;
                }
            }
            if (!gasit) {
                spinner.spin(document.getElementById('main'));
                var x = $http.post('/UtilizatoriActions/Save', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_ACTION: $scope.NewActions[i].ID })
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.existaModificari = true;
                            $scope.result.push(response.data);
                            /*
                            var _id = "#msg_action_" + $scope.NewActions[i].ID;
                            var _class = !response.data.Status ? 'label label-danger' : 'label label-success';
                            $(_id).text(response.data.Status ? "Succes!" : "Error!");
                            $(_id).attr("class", _class);
                            */
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
                qs.push(x);
            }
        }

        for (var i = 0; i < $scope.UtilizatorJson.Actions.length; i++) {
            var gasit = false;
            for (var j = 0; j < $scope.NewActions.length; j++) {
                if ($scope.UtilizatorJson.Actions[i].ID == $scope.NewActions[j].ID) {
                    gasit = true;
                    break;
                }
            }
            if (!gasit) {
                spinner.spin(document.getElementById('main'));
                var y = $http.post('/UtilizatoriActions/Delete', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_ACTION: $scope.UtilizatorJson.Actions[i].ID })
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.existaModificari = true;
                            $scope.result.push(response.data);
                            /*
                            var _id = "#msg_action_" + $scope.UtilizatorJson.Actions[i].ID;
                            var _class = !response.data.Status ? 'label label-danger' : 'label label-success';
                            $(_id).text(response.data.Status ? "Succes!" : "Error!");
                            $(_id).attr("class", _class);
                            */
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
                qs.push(y);
            }
        }

        $q.all(qs).then(function (response) {
            if ($scope.existaModificari) {
                $http.get('/Utilizatori/IndexJson').then(function (response) {
                    $scope.model = response.data;
                    for (var i = 0; i < $scope.model.UtilizatorJson.UtilizatoriSubordonati.length; i++) {
                        if ($scope.model.UtilizatorJson.UtilizatoriSubordonati[i].Utilizator.ID === $scope.UtilizatorJson.Utilizator.ID) {
                            $scope.setUtilizator($scope.model.UtilizatorJson.UtilizatoriSubordonati[i]);
                            break;
                        }
                    }
                });
            }
        });
    };

    $scope.SaveSocietatiAdministrate = function () {
        $scope.showMessage = true;
        $scope.existaModificari = false;
        var qs = [];
        for (var i = 0; i < $scope.NewSocietatiAdministrate.length; i++) {
            var gasit = false;
            for (var j = 0; j < $scope.UtilizatorJson.SocietatiAsigurareAdministrate.length; j++) {
                if ($scope.NewSocietatiAdministrate[i].ID == $scope.UtilizatorJson.SocietatiAsigurareAdministrate[j].ID) {
                    gasit = true;
                    break;
                }
            }
            if (!gasit) {
                spinner.spin(document.getElementById('main'));
                var x = $http.post('/UtilizatoriSocietatiAdministrate/Save', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_SOCIETATE_ADMINISTRATA: $scope.NewSocietatiAdministrate[i].ID })
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.existaModificari = true;
                            $scope.result.push(response.data);
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
                qs.push(x);
            }
        }

        for (var i = 0; i < $scope.UtilizatorJson.SocietatiAsigurareAdministrate.length; i++) {
            var gasit = false;
            for (var j = 0; j < $scope.NewSocietatiAdministrate.length; j++) {
                if ($scope.UtilizatorJson.SocietatiAsigurareAdministrate[i].ID == $scope.NewSocietatiAdministrate[j].ID) {
                    gasit = true;
                    break;
                }
            }
            if (!gasit) {
                spinner.spin(document.getElementById('main'));
                var y = $http.post('/UtilizatoriSocietatiAdministrate/Delete', { ID_UTILIZATOR: $scope.ID_UTILIZATOR, ID_SOCIETATE_ADMINISTRATA: $scope.UtilizatorJson.SocietatiAsigurareAdministrate[i].ID })
                    .then(function (response) {
                        if (response != 'null' && response != null && response.data != null) {
                            $scope.existaModificari = true;
                            $scope.result.push(response.data);
                        }
                        spinner.stop();
                    }, function (response) {
                        spinner.stop();
                        alert('Erroare: ' + response.status + ' - ' + response.data);
                    });
                qs.push(y);
            }
        }

        $q.all(qs).then(function (response) {
            if ($scope.existaModificari) {
                $http.get('/Utilizatori/IndexJson').then(function (response) {
                    $scope.model = response.data;
                    for (var i = 0; i < $scope.model.UtilizatorJson.UtilizatoriSubordonati.length; i++) {
                        if ($scope.model.UtilizatorJson.UtilizatoriSubordonati[i].Utilizator.ID === $scope.UtilizatorJson.Utilizator.ID) {
                            $scope.setUtilizator($scope.model.UtilizatorJson.UtilizatoriSubordonati[i]);
                            break;
                        }
                    }
                });
            }
        });
    };

});