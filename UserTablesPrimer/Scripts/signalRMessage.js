var signalRHub = $.connection.signalRHub;

$(function () {

    signalRHub.client.showMessage = function () {
        $("#notification").text("New!");

        var divElement = document.getElementById("notifications");

        /*if (divElement != null) {
            var elementToAdd = "<div class='notificationDiv'><span>" + message + "</span> &nbsp;&nbsp;&nbsp;&nbsp;<span class='newNotSpan'>New!</span>";
            $(divElement).prepend(elementToAdd);
        }*/

    }

    signalRHub.client.updateTokens = function (numOfTokens) {
        var tokensElement = document.getElementById("numOfTokens");

        if (tokensElement != null) {
            $(tokensElement).text(numOfTokens);
        }
    }

    $.connection.hub.start()
        .done(function () {
            console.log("Now connected! id:" + $.connection.hub.id);
        })
        .fail(function () {
            console.log("Connection failed!");
        });

});