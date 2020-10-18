"use strict";

$(function () {
    let from, to, data = [];
    let dataStore = new DevExpress.data.ArrayStore({
        key: "id",
        data: data
    });

    $("#formContainer").dxForm({
        colCount: 5,
        formData: { from: from, to: to },
        items: [{
            dataField: "from",
            editorType: "dxDateBox",
            editorOptions: {
                width: 200,
                type: "date",
                max: new Date(),
                value: new Date(),
                onContentReady: function (e) {
                    e.component.option("value", new Date());
                },
                onValueChanged: function (e) {
                    from = new Date(e.value).toISOString().split('T')[0];
                }
            }
        }, {
            dataField: "to",
            editorType: "dxDateBox",
            editorOptions: {
                width: 200,
                type: "date",
                max: new Date(),
                value: new Date(),
                onContentReady: function (e) {
                    e.component.option("value", new Date());
                },
                onValueChanged: function (e) {
                    to = new Date(e.value).toISOString().split('T')[0];
                }
            }
        }, {
            itemType: "button",
            buttonOptions: {
                text: "Search",
                stylingMode: "outlined",
                type: "success",
                icon: "search",
                onClick: function () {
                    $.post('report', {
                        '__RequestVerificationToken': token,
                        from: from,
                        to: to
                    }).done(function (response) {
                        console.log(response)
                        for (let index = 0; index < response.length; index++) {
                            response[index].machineName = response[index].machine.name;
                            if (data.length === 0 || data.find(i => i.id === response[index].id) === null) {
                                data.push(response[index])
                                dataStore.push([{ type: "insert", data: response[index] }]);
                            } else {
                                dataStore.push([{ type: "update", key: response[index].id, data: response[index] }]);
                            }

                            if (data.length !== 0 && response.find(i => i.id === data[index].id) === null) {
                                data.pop(data[index])
                                dataStore.push([{ type: "remove", key: response[index].id, data: response[index] }]);
                            }
                        }
                        if ($("#gridContainer").dxDataGrid("instance") !== undefined) {
                            $("#gridContainer").dxDataGrid("instance").refresh();
                        }
                    });
                }
            }
        }]
    });

    $('#tabPanelContainer').dxTabPanel({
        height: 460,
        swipeEnabled: true,
        animationEnabled: true,
        items: [{
            title: 'Hour',
        }, {
            title: 'Count',
        }, {
            title: 'Detail',
        }],
        itemTemplate: function (itemData, itemIndex, itemElement) {
            switch (itemData.title) {
                case 'Hour':
                    $("<div>").dxChart({
                        palette: "Green Mist",
                        dataSource: {
                            store: dataStore,
                            reshapeOnPush: true
                        },
                        loadingIndicator: {
                            enabled: true
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
                            nameField: "machineName"
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
                            verticalAlignment: "top",
                            horizontalAlignment: "right"
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
                    }).appendTo(itemElement)
                    break;
                case 'Count':
                    $("<div>").dxChart({
                        palette: "Green Mist",
                        dataSource: {
                            store: dataStore,
                            reshapeOnPush: true
                        },
                        loadingIndicator: {
                            enabled: true
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
                            nameField: "machineName"
                        },
                        valueAxis: [{
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
                            position: "left",
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
                            verticalAlignment: "top",
                            horizontalAlignment: "right"
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
                    }).appendTo(itemElement)
                    break;
                case 'Detail':
                    $("<div id='gridContainer'>").dxDataGrid({
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
                        scrolling: {
                            mode: "virtual"
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
                            var worksheet = workbook.addWorksheet('report');
                            DevExpress.excelExporter.exportDataGrid({
                                component: e.component,
                                worksheet: worksheet,
                                autoFilterEnabled: true
                            }).then(function () {
                                workbook.xlsx.writeBuffer().then(function (buffer) {
                                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'report-' + (new Date().toLocaleDateString().replace(/\/|\-/g, '.')) + '.xlsx');
                                });
                            });
                            e.cancel = true;
                        },
                        columns: [{
                            caption: '#',
                            width: 50,
                            dataType: "number",
                            allowEditing: false,
                            cellTemplate: function (container, options) {
                                container.text(options.row.rowIndex + 1)
                            }
                        }, {
                            caption: "Name",
                            dataField: "machineName",
                        }, {
                            caption: "Date",
                            dataField: "date",
                            dataType: "date",
                        }, {
                            caption: "Total time",
                            dataField: "display",
                            dataType: "time",
                        }, {
                            caption: "Count",
                            dataField: "count",
                            dataType: "number",
                        }],
                    }).appendTo(itemElement);
                    break;
                default:
                    break;
            }
        }
    });
});