'use strict';
function resizeTable() {
    $(document).ready(function () {
        //$('#messagesTable').fixedHeaderTable({});
        /*
    $(".treeview li>ul").css('display', 'none'); // Hide all 2-level ul
    $(".collapsible").click(function (e) {
        e.preventDefault();
        $(this).toggleClass("treeview_collapse expand");
        $(this).closest('li').children('ul').slideToggle();
    });
    */

        // Change the selector if needed
        var $table = $('#messagesTable'),
            $bodyCells = $table.find('tbody tr:first').children(),
            colWidth;

        // Adjust the width of thead cells when window resizes
        $(window).resize(function () {
            // Get the tbody columns width array
            colWidth = $bodyCells.map(function () {
                return $(this).width();
            }).get();

            // Set the width of thead columns
            $table.find('thead tr').children().each(function (i, v) {
                $(v).width(colWidth[i]);
            });
        }).resize(); // Trigger resize handler
    });
}

app.controller('MesajeController',
function ($scope, $http, $filter, $rootScope, $compile, $interval, myService) {
    $scope.lastActiveIdDosar = "";
    $scope.model = {};
    $scope.model.MesajJson = {};
    $scope.model.MesajJson.Mesaj = null;
    $scope.model.MesajJson.Receivers = [];
    $scope.model.InvolvedParties = [];
    $scope.model.MesajeJson = [];
    $scope.model.TipuriMesaj = [];
    $scope.result = {};
    $scope.involvedParty = "";
    $scope.tipMesaj = "";
    $scope.html2 = "";
    $scope.inbox = "Inbox";
    $scope.newMessages = "";

    $scope.lastRefresh = new Date();
    $interval(function () {
        if($rootScope.activeTab == 'mesaje' && $rootScope.ID_DOSAR != null && $rootScope.ID_DOSAR != undefined)
            $scope.GetNewMessages($rootScope.ID_DOSAR);
    }, 500000);


    $rootScope.$watch('ID_DOSAR', function (newValue, oldValue) {
        if (newValue != null && newValue != undefined && $rootScope.activeTab == 'mesaje') {
            $scope.GetMessages(newValue);
            $scope.model.MesajJson.Mesaj.ID_DOSAR = newValue;
        }
    });

    $rootScope.$watch('activeTab', function (newValue, oldValue) {
        if (newValue == 'mesaje' && $rootScope.ID_DOSAR != null && $rootScope.ID_DOSAR != undefined && $scope.lastActiveIdDosar != $rootScope.ID_DOSAR) {
            $scope.GetMessages($rootScope.ID_DOSAR);
            $scope.lastActiveIdDosar = $rootScope.ID_DOSAR;
        }
    });

    $scope.$watch('model.MesajJson.Mesaj.ID_TIP_MESAJ', function (newValue, oldValue) {
        document.getElementById("select_TipuriMesaj").value = newValue;
    });

    $scope.$watch('tipMesaj', function (newValue, oldValue) {
        $scope.model.MesajJson.Mesaj.ID_TIP_MESAJ = newValue;
    });

    $scope.AddReceiver = function () {
        var este = false;
        angular.forEach($scope.model.MesajJson.Receivers, function (value, key) {
            if (value.ID == $scope.involvedParty) {
                este = true;
                return;
            }
        });
        if (este) return;

        angular.forEach($scope.model.InvolvedParties, function (value, key) {
            if (value.ID == $scope.involvedParty) {
                $scope.model.MesajJson.Receivers.push(value);
            }
        });

        var select = document.getElementById("select_involvedParties");
        $scope.html2 = "";
        $scope.GenerateReceivers();
    };

    $scope.RemoveReceiver = function (ID) {
        var idx = -1;
        var i = 0;
        for (i = 0; i < $scope.model.MesajJson.Receivers.length; i++) {
            if ($scope.model.MesajJson.Receivers[i].ID == ID) {
                idx = i;
                break;
            }
        }
        $scope.model.MesajJson.Receivers.splice(idx, 1);
        var rec = document.getElementById("receiver_" + ID);
        rec.parentNode.removeChild(rec);
    };

    $scope.GetNewMessages = function (id_dosar) {
        if (id_dosar == null) return;
        spinner.spin(document.getElementById('main'));
        var j = {'id_dosar': id_dosar, 'last_refresh': $scope.lastRefresh};
        myService.getlist('POST', '/Mesaje/GetNewMessages', { j: JSON.stringify(j) })
          .then(function (response) {
              if (response.data == null || !response.data.Status || response.data.Result <= 0) {
                  $scope.newMessages = "";
              }
              else {
                  $scope.newMessages =  "(" + response.data.Result + " mesaje noi!)";
              }
              $scope.lastRefresh = new Date();
              spinner.stop();
          }, function (response) {
              spinner.stop();
              alert('Erroare: ' + response.status + ' - ' + response.data);
          });
    };

    $scope.GetSentMessages = function (id_dosar) {
        $scope.inbox = "Sent";
        spinner.spin(document.getElementById('main'));
        $scope.CancelMessage();
        myService.getlist('GET', '/Mesaje/GetSentMessages/' + id_dosar, null)
          .then(function (response) {
              if (response != 'null' && response != null && response.data != null) {
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

    $scope.GetMessages = function (id_dosar) {
        $scope.newMessages = "";
        $scope.inbox = "Inbox";
        spinner.spin(document.getElementById('main'));
        //$scope.CancelMessage();
        myService.getlist('GET', '/Mesaje/GetMessages/' + id_dosar, null)
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
        $scope.model.MesajeJson = messages;
    };

    $scope.SelectMessage = function (mesaj) {
        /*
        $scope.model.MesajJson = {};
        angular.copy(mesaj, $scope.model.MesajJson);
        $scope.tipMesaj = mesaj.Mesaj.ID_TIP_MESAJ;
        */
        $scope.model.MesajJson = mesaj;
        //$scope.tipMesaj = mesaj.Mesaj.ID_TIP_MESAJ;
        $scope.GenerateReceivers();
    };

    $scope.GenerateReceivers = function () {
        $scope.html2 = "";
        angular.forEach($scope.model.MesajJson.Receivers, function (value, key) {
            $scope.html2 += ('<a style="margin:5px;" id="receiver_' + value.ID + '" href="#" ng-click="RemoveReceiver(' + value.ID + ')">' + value.NUME_COMPLET + ' (' + value.EMAIL + ')' + '&nbsp;<span class="badge">x</span></a>');
        });
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
                        /*
                        if ($scope.result.InsertedId != null) {
                            $scope.model.MesajJson.Mesaj.ID = $scope.result.InsertedId;
                            $scope.model.MesajeJson.push($scope.model.MesajJson);
                        }
                        */
                        $scope.GetMessages($rootScope.ID_DOSAR);
                    }
                }
                spinner.stop();
            }, function (response) {
                alert('Erroare: ' + response.status + ' - ' + response.data);
                spinner.stop();
            });
    };

    $scope.NewMessage = function () {
        $scope.model.MesajJson.Mesaj = {};
        $scope.model.MesajJson.Mesaj.ID_DOSAR = $rootScope.ID_DOSAR;
        $scope.model.MesajJson.Receivers = [];
        $scope.html2 = "";
    };

    $scope.CancelMessage = function () {
        $scope.model.MesajJson.Mesaj = null;
        $scope.model.MesajJson.Receivers = [];
        $scope.html2 = "";
    };

    $scope.Reply = function () {
        $scope.tempMesaj = angular.copy($scope.model.MesajJson);
        $scope.model.MesajJson.Mesaj = {};
        $scope.model.MesajJson.Mesaj.SUBIECT = "Re: " + $scope.tempMesaj.Mesaj.SUBIECT;
        $scope.model.MesajJson.Mesaj.BODY = $scope.tempMesaj.Mesaj.BODY;
        $scope.model.MesajJson.Receivers = [];
        $scope.model.MesajJson.Mesaj.REPLY_TO = $scope.tempMesaj.Mesaj.ID;
        $scope.model.MesajJson.Mesaj.ID_TIP_MESAJ = $scope.tempMesaj.Mesaj.ID_TIP_MESAJ;
        $scope.model.MesajJson.Mesaj.ID_DOSAR = $scope.tempMesaj.Mesaj.ID_DOSAR;
        for (var i = 0; i < $scope.model.InvolvedParties.length; i++) {
            //alert($scope.model.InvolvedParties[i].ID + ' - ' + $scope.tempMesaj.Mesaj.ID_SENDER);
            if ($scope.model.InvolvedParties[i].ID == $scope.tempMesaj.Mesaj.ID_SENDER) {
                $scope.model.MesajJson.Receivers.push($scope.model.InvolvedParties[i])
                break;
            }
        }
        $scope.GenerateReceivers();
    };
});