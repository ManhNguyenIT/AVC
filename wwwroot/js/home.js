"use strict";

$(async () => {
    let data = []
    let machine = null
    let dataStore = new DevExpress.data.ArrayStore({
        key: "id",
        data: data
    });
    let machineStore = new DevExpress.data.ArrayStore({
        key: "name",
        data: data
    });
    dataStore.load = function () {
        var deferred = $.Deferred();
        $.get('summaries').done(function (response) {
            if (machine === null) {
                data = response
                deferred.resolve(data);
            } else {
                data = response.filter(i => i.name === machine)
                deferred.resolve(data);
            }
        });
        return deferred.promise();
    }

    machineStore.load = function () {
        var deferred = $.Deferred();
        $.get('machines').done(function (response) {
            deferred.resolve(response);
        });
        return deferred.promise();
    }
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/live")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.onreconnecting(error => {
        var li = document.createElement("li");
        li.textContent = `Connection lost due to error "${error}". Reconnecting.`;
        document.getElementById("messagesList").prepend(li);
    });
    connection.onreconnected(connectionId => {
        var li = document.createElement("li");
        li.textContent = `Connection reestablished. Connected with connectionId "${connectionId}".`;
        document.getElementById("messagesList").prepend(li);
    });

    connection.onclose(error => {
        var li = document.createElement("li");
        li.textContent = `Connection closed due to error "${error}". Try refreshing this page to restart the connection.`;
        document.getElementById("messagesList").prepend(li);
        // start();
    });

    async function start() {
        await connection.start().then(function () {
            DevExpress.ui.notify('Connected', 'success', 600);
        }).catch(function (err) {
            return console.error(err.toString());
        });
    };

    connection.on("Log", function (log) {
        var li = document.createElement("li");
        // li.style.cssText = gpio.value === 1 ? 'background-color:green' : 'background-color:red';
        li.textContent = log.ip + "\t" + log.gpio.name + "\t" + log.gpio.value;
        document.getElementById("messagesList").prepend(li);
    });

    connection.on("Summaries", processing);

    function processing(response) {
        for (let index = 0; index < response.length; index++) {
            if (machine !== null && response[index].name !== machine) {
                if (response.find(i => i.id === data[index].id) === null) {
                    data.pop(data[index])
                    dataStore.push([{ type: "remove", key: response[index].id, data: response[index] }]);
                }
                continue;
            }
            if (data.find(i => i.id === response[index].id) === null) {
                data.push(response[index])
                dataStore.push([{ type: "insert", data: response[index] }]);
            } else {
                dataStore.push([{ type: "update", key: response[index].id, data: response[index] }]);
            }

            if (response.find(i => i.id === data[index].id) === null) {
                data.pop(data[index])
                dataStore.push([{ type: "remove", key: response[index].id, data: response[index] }]);
            }
        }
        $("#gridContainer").dxDataGrid("instance").refresh();
    }

    $("#gridContainer").dxDataGrid({
        dataSource: {
            store: dataStore,
            reshapeOnPush: true
        },
        repaintChangesOnly: true,
        highlightChanges: true,
        columnAutoWidth: true,
        showBorders: true,
        searchPanel: {
            visible: true
        },
        loadPanel: {
            enabled: false
        },
        paging: {
            pageSize: 10
        },
        headerFilter: {
            visible: true,
            allowSearch: true
        },
        wordWrapEnabled: true,
        export: {
            enabled: true,
            allowExportSelectedData: true
        },
        onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet('summeries');
            DevExpress.excelExporter.exportDataGrid({
                component: e.component,
                worksheet: worksheet,
                autoFilterEnabled: true
            }).then(function () {
                workbook.xlsx.writeBuffer().then(function (buffer) {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'summeries-' + (new Date().toLocaleDateString().replace(/\/|\-/g, '.')) + '.xlsx');
                });
            });
            e.cancel = true;
        },
        columns: [{
            //     caption: '#',
            //     width: 50,
            //     dataType: "number",
            //     allowEditing: false,
            //     cellTemplate: function (container, options) {
            //         container.text(options.row.rowIndex + 1)
            //     }
            // }, {
            caption: "IP",
            dataField: "ip",
        }, {
            caption: "Name",
            dataField: "name",
        }, {
            caption: "Date",
            dataField: "date",
            dataType: "date",
            // groupIndex: 0
        }, {
            caption: "Total time",
            dataField: "time",
            dataType: "time",
        }, {
            caption: "Count",
            dataField: "count",
            dataType: "number",
        }],
        onToolbarPreparing: function (e) {
            e.toolbarOptions.items.unshift({
                location: "before",
                widget: "dxSelectBox",
                options: {
                    width: 200,
                    dataSource: {
                        store: machineStore,
                        reshapeOnPush: true
                    },
                    valueExpr: "name",
                    displayExpr: "name",
                    showClearButton: true,
                    deferRendering: false,
                    onContentReady: function (e) {
                        // let firstItem = e.component.option("items[0]");
                        // if (firstItem) {
                        //     e.component.option("value", firstItem.name);
                        // }
                    },
                    onValueChanged: function (e) {
                        machine = e.value;
                        processing(data);
                    }
                }
            }, {
                location: "after",
                widget: "dxButton",
                options: {
                    icon: "refresh",
                    onClick: function () {
                        e.component.refresh();
                    }
                }
            });
        }
    }).dxDataGrid("instance");

    $("#chartContainer")
        .append($('<div class="col">').dxChart({
            palette: "Green Mist",
            dataSource: {
                store: dataStore,
                reshapeOnPush: true
            },
            loadingIndicator: {
                enabled: false
            },
            size: {
                height: 300,
            },
            commonSeriesSettings: {
                argumentField: "date",
                valueField: "time",
                type: "bar",
                hoverMode: "allArgumentPoints",
                selectionMode: "allArgumentPoints",
                label: {
                    visible: true,
                    format: {
                        type: "fixedPoint",
                        precision: 0
                    }
                }
            },
            seriesTemplate: {
                nameField: "name"
            },
            valueAxis: [{
                title: {
                    text: "Hour",
                    font: {
                        color: "#e91e63"
                    }
                },
                label: {
                    font: {
                        color: "#e91e63"
                    }
                },
                name: "time",
                position: "left",
                constantLines: [{
                    value: 8,
                    color: "#fc3535",
                    dashStyle: "dash",
                    width: 2,
                    label: {
                        text: "Low Hour"
                    },
                }],
                // }, {
                //     title: {
                //         text: "Count",
                //         font: {
                //             color: "#03a9f4"
                //         }
                //     },
                //     label: {
                //         font: {
                //             color: "#03a9f4"
                //         }
                //     },
                //     name: "count",
                //     position: "right",
                //     showZero: true,
                //     valueMarginsEnabled: false
            }],
            // series: [{
            //     valueField: "time",
            //     axis: "time",
            //     name: "Hour",
            // }, {
            //     type: "spline",
            //     valueField: "count",
            //     axis: "count",
            //     name: "Count",
            // }],
            tooltip: {
                enabled: true,
                customizeTooltip: function (arg) {
                    return {
                        html: arg.seriesName === "Count" ? "" : "<div><div class='tooltip-header'>" +
                            arg.argumentText + "</div>" +
                            "<div class='tooltip-body'><div class='series-name'>" +
                            arg.seriesName +
                            ": </div><div class='value-text'>" +
                            arg.valueText + "(" + arg.point.data.percent + "%)" +
                            "</div></div></div>"
                    };
                }
            },
            legend: {
                verticalAlignment: "bottom",
                horizontalAlignment: "center"
            },
            customizePoint: function (arg) {
                if (this.value < 8) {
                    return { color: "#ff7c7c", hoverStyle: { color: "#ff7c7c" } };
                }
            },
            customizeLabel: function (arg) {
                // if (arg.seriesName === "Hour") {
                return {
                    visible: true,
                    customizeText: function () {
                        return arg.data.display;
                    }
                };
                // }
            },
        }))
        .append($('<div class="col">').dxChart({
            palette: "Green Mist",
            dataSource: {
                store: dataStore,
                reshapeOnPush: true
            },
            loadingIndicator: {
                enabled: false
            },
            size: {
                height: 300,
            },
            commonSeriesSettings: {
                argumentField: "date",
                valueField: "count",
                type: "bar",
                hoverMode: "allArgumentPoints",
                selectionMode: "allArgumentPoints",
                label: {
                    visible: true,
                    format: {
                        type: "fixedPoint",
                        precision: 0
                    }
                }
            },
            seriesTemplate: {
                nameField: "name"
            },
            valueAxis: [{
            }, {
                title: {
                    text: "Count",
                    font: {
                        color: "#03a9f4"
                    }
                },
                label: {
                    font: {
                        color: "#03a9f4"
                    }
                },
                name: "count",
                position: "right",
                showZero: true,
                valueMarginsEnabled: false
            }],
            tooltip: {
                enabled: true,
                customizeTooltip: function (arg) {
                    return {
                        html: arg.seriesName === "Count" ? "" : "<div><div class='tooltip-header'>" +
                            arg.argumentText + "</div>" +
                            "<div class='tooltip-body'><div class='series-name'>" +
                            arg.seriesName +
                            ": </div></div></div>"
                    };
                }
            },
            legend: {
                verticalAlignment: "bottom",
                horizontalAlignment: "center"
            },
            customizePoint: function (arg) {
                if (arg.seriesName === "Hour" && this.value < 8) {
                    return { color: "#ff7c7c", hoverStyle: { color: "#ff7c7c" } };
                }
            },
            customizeLabel: function (arg) {
                if (arg.seriesName === "Hour") {
                    return {
                        visible: true,
                        customizeText: function () {
                            return arg.data.display;
                        }
                    };
                }
            },
        }));
    await start();
});