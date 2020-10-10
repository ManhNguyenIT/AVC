"use strict";

$(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/live").build();

    connection.on("Update", function (date, machine, log, total) {
        document.getElementById("date").innerHTML = date;
        document.getElementById("result").innerHTML = total._totalTime + "|" + machine._totalTime
        var li = document.createElement("li");
        li.style.cssText = machine.value === 1 ? 'background-color:green' : 'background-color:red';
        li.textContent = log.ip + "|" + log._start + "|" + log._finish;
        document.getElementById("messagesList").prepend(li);
    });

    connection.start().then(function () {
        $.get("/service-center/init", function () { });
    }).catch(function (err) {
        return console.error(err.toString());
    });

});