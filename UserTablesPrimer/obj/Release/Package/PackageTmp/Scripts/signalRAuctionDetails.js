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

    signalRHub.client.updateAuctionDetails = function (json, currency) {
        var auctions = JSON.parse(json);

        for (var i = 0; i < auctions.length; i++) {
            var auction = document.getElementById(auctions[i].Id);
            if (auction != null) {
                var auctionChildren = auction.childNodes;
                for (var j = 0; j < auctionChildren.length; j++) {
                    if (auctionChildren[j].className == "timeLeftContainer") {
                        auctionChildren[j].innerHTML = auctions[i].timeLeft;
                    }
                }
                if (auctions[i].Status == 2) {
                    document.getElementById("readyButton").style.display = "none";
                    document.getElementById("openButton").style.display = "block";
                }
                if (auctions[i].Status == 3) {
                    document.getElementById("openButton").style.display = "none";
                    document.getElementById("closeButton").style.display = "block";
                }

                break;
            }
        }

    }

    signalRHub.client.updateBidListAndPrice = function (josn, currency, tokenValue) {
        var auction = JSON.parse(josn);
        
        var auctionElement = document.getElementById(auction.Id);

        if (auctionElement != null) {
            var auctionElmenetChildren = auctionElement.childNodes;

            for (var i = 0; i < auctionElmenetChildren.length; i++) {
                if (auctionElmenetChildren[i].className == "currentPriceContainer") {
                    auctionElmenetChildren[i].innerHTML = auction.CurrentPrice + " " + currency;
                    $(".currentPriceContainer").animate({ opacity: '0.7' }).animate({ opacity : '1' });
                    break;
                }
            }

            var bidders = "";

            for (var i = 0; i < auction.Bids.length; i++) {
                bidders += "<b>" + auction.Bids[i].UserName + "</b>" + " --> " + "<span style='color: green'><b>" + auction.Bids[i].Bids + "</b></span> tokens." + "<hr style='margin-top: 5px; margin-bottom: 5px'/>";
            }

            document.getElementById("bidders").innerHTML = bidders;

            $('#bidders').animate({ backgroundColor: '#e0e0e0' }).animate({ backgroundColor: 'white' });

            $("#tokenValue").val(tokenValue);
            $("#currencySpan").text(currency);
            
            var minValue = $("#numberOfTokens").attr('min');

            if (minValue == $("#numberOfTokens").val()) {
                $("#numberOfTokens").val(parseInt(minValue) + 1);
            }

            minValue++;
            $("#numberOfTokens").attr('min', minValue);

                var numberOfTokens = $("#numberOfTokens").val();
                if (!$.isNumeric(numberOfTokens)) {
                    //$("#numberOfTokensValMsg").text("No. of tokens must be a positive integer number greater than 0.");
                } else {
                    var number = parseInt(numberOfTokens);
                    if (number <= 0) {
                        //$("#numberOfTokensValMsg").text("No. of tokens must be a positive integer number greater than 0.");
                    } else {
                        $("#numberOfTokens").trigger('change');
                    }
                }

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