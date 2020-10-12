"use strict";

$(function () {
    let gridStore = new DevExpress.data.ArrayStore({
        key: "id",
        data: [],
    });
    gridStore.load = function () {
        var deferred = $.Deferred();
        $.get('machines').done(function (response) {
            console.log(response)
            deferred.resolve(response);
        });
        return deferred.promise();
    };

    var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/live").build();
    connection.on("Update", function (date, machine, log, total) {
        console.log(total);
        gridStore.push([{ type: "update", key: total.id, data: total }]);
        var li = document.createElement("li");
        li.style.cssText = machine.gpio.filter(i => i.value === 1).length > 0 ? 'background-color:green' : 'background-color:red';
        li.textContent = log.machine.ip + "\t" + log.total;
        document.getElementById("messagesList").prepend(li);
    });

    connection.start().then(function () {
        DevExpress.ui.notify('Connected', 'success', 600);
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $("#gridContainer").dxDataGrid({
        dataSource: {
            store: gridStore,
            reshapeOnPush: true
        },
        repaintChangesOnly: true,
        highlightChanges: true,
        columnAutoWidth: true,
        showBorders: true,
        paging: {
            pageSize: 10
        },
        columns: [{
            caption: '#',
            allowEditing: false,
            cellTemplate: function (container, options) {
                container.text(options.row.rowIndex + 1)
            }
        }, {
            caption: "Tên Máy",
            dataField: "machine.name",
        }, {
            caption: "IP",
            dataField: "machine.ip",
        }, {
            caption: "Trạng thái",
            dataField: "machine.status",
        }, {
            caption: "Current date",
            dataField: "date",
        }, {
            caption: "Total ON in date",
            dataField: "totalON",
        }, {
            caption: "Total OFF in date",
            dataField: "totalOFF",
        }, {
            caption: "Total ON",
            dataField: "machine.totalON",
        }, {
            caption: "Total OFF",
            dataField: "machine.totalOFF",
        }],
    });

});