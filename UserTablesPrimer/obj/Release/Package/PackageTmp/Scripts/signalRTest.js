$(function () {
    //var signalRHub = $.connection.signalRHub;

    /*var calculateRemainingTime = function (closingDate) {
        var seconds = Math.floor(Math.abs(new Date(closingDate.match(/\d+/)[0] * 1) - new Date()) / 1000);
        var remainingTimeStr = "";

        var days = Math.floor(seconds / (3600 * 24));
        if (days < 10)
            remainingTimeStr += "0" + days + ":";
        else
            remainingTimeStr += days + ":";

        seconds -= days * 3600 * 24;
        var hrs = Math.floor(seconds / 3600);
        if (hrs < 10)
            remainingTimeStr += "0" + hrs + ":";
        else
            remainingTimeStr += hrs + ":";

        seconds -= hrs * 3600;
        var mnts = Math.floor(seconds / 60);
        if (mnts < 10)
            remainingTimeStr += "0" + mnts + ":";
        else
            remainingTimeStr += mnts + ":";

        seconds -= mnts * 60;
        if (seconds < 10)
            remainingTimeStr += "0" + seconds;
        else
            remainingTimeStr += seconds;

        return remainingTimeStr;
    }*/

    signalRHub.client.updateAuctionTime = function (json) {
        var auctions = JSON.parse(json);
        
        for (var i = 0; i < auctions.length; i++) {
            var auction = document.getElementById(auctions[i].Id);
            if (auction != null) {
                var auctionChildren = auction.childNodes;
                for (var j = 0; j < auctionChildren.length; j++) {
                    if (auctionChildren[j].className == "time") {
                        auctionChildren[j].innerHTML = auctions[i].timeLeft;
                    }
                }
                if (auctions[i].Status == 2) {
                    var readyButtonId = "readyButton" + auctions[i].Id;
                    var openButtonId = "openButton" + auctions[i].Id;
                    document.getElementById(readyButtonId).style.display = "none";
                    document.getElementById(openButtonId).style.display = "block";
                }
                if (auctions[i].Status == 3) {
                    var openButtonId = "openButton" + auctions[i].Id;
                    var closeButtonId = "closeButton" + auctions[i].Id;
                    document.getElementById(openButtonId).style.display = "none";
                    document.getElementById(closeButtonId).style.display = "block";
                }
            }
        }

    }

    signalRHub.client.updateAuctionPrice = function (json, currency, tokenValue) {
        var auction = JSON.parse(json);

        var auctionElement = document.getElementById(auction.Id);

        if (auctionElement != null) {
            var auctionElementChildren = auctionElement.childNodes;

            for (var i = 0; i < auctionElementChildren.length; i++) {
                if (auctionElementChildren[i].className == "priceSpan") {
                    auctionElementChildren[i].innerHTML = auction.CurrentPrice + " " + currency;
                    $(auctionElementChildren[i]).animate({ backgroundColor: '#fcf0ab' }).animate({ backgroundColor: 'white' });
                }
                if (auctionElementChildren[i].className == "lastBidder") {
                    auctionElementChildren[i].innerHTML = auction.LastBidder;
                }
            }

            var minBids = $("[auctionId='" + auction.Id+"']").attr("minBids");
            minBids++;
            $("[auctionId='" + auction.Id + "']").attr("minBids", minBids);

            $("#tokenValue").val(tokenValue);
            $("#currencySpan").text(currency);

        }
    }

    /*$.connection.hub.start()
        .done(function () {
            console.log("Now connected! id:" + $.connection.hub.id);
        })
        .fail(function () {
            console.log("Connection failed!");
        });*/
});