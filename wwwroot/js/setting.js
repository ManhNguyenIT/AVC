"use strict";

$(function () {
    let gridStore = new DevExpress.data.ArrayStore({
        key: "id",
        data: [],
    });

    gridStore.load = function () {
        var deferred = $.Deferred();
        $.get('machines').done(function (response) {
            deferred.resolve(response);
        });
        return deferred.promise();
    }

    $("#tabPanel").dxTabPanel({
        dataSource: [{
            title: 'Quản lý Machine',
            template: function () {
                return $("<div>").dxDataGrid({
                    dataSource: {
                        store: gridStore,
                        reshapeOnPush: true
                    },
                    repaintChangesOnly: true,
                    showBorders: true,
                    editing: {
                        mode: "batch",
                        refreshMode: "reshape",
                        allowAdding: true,
                        allowUpdating: true,
                        allowDeleting: true,
                        useIcons: true
                    },
                    scrolling: {
                        mode: "virtual"
                    },
                    columns: [{
                        caption: "Tên Máy",
                        dataField: "name",
                        validationRules: [{ type: "required" }]
                    }, {
                        caption: "IP",
                        dataField: "ip",
                        validationRules: [{ type: "required" }, {
                            type: "pattern",
                            message: 'Your IP must have "xxx-xxx-xxx" format!',
                            pattern: /^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\.(?!$)|$)){4}$/i
                        }]
                    }, {
                        caption: "Trạng thái",
                        dataField: "status",
                        dataType: "boolean"
                    }, {
                        caption: "IO",
                        dataField: "gpio",
                        validationRules: [{ type: "required" }]
                    }],
                    onRowInserted: function (e) {
                        $.ajax({
                            type: "POST",
                            url: "create-machine",
                            data: {
                                '__RequestVerificationToken': token,
                                machine: e.data
                            },
                            success: function () {
                                gridStore.load();
                                DevExpress.ui.notify('Done', 'success', 600);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
                            }
                        });
                    },
                    onRowUpdated: function (e) {
                        $.ajax({
                            type: "PUT",
                            url: "update-machine",
                            data: {
                                '__RequestVerificationToken': token,
                                machine: e.data
                            },
                            success: function () {
                                gridStore.load();
                                DevExpress.ui.notify('Done', 'success', 600);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
                            }
                        });
                    },
                    onRowRemoved: function (e) {
                        $.ajax({
                            type: "DELETE",
                            url: "delete-machine",
                            data: {
                                '__RequestVerificationToken': token,
                                machine: e.data
                            },
                            success: function () {
                                gridStore.load();
                                DevExpress.ui.notify('Done', 'success', 600);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
                            }
                        });
                    }
                });
            }
        }],
        deferRendering: false
    });
});