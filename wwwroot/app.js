var config = {
    weatherUrl: "/api/weather/v1",
    railwayUrl: "/api/railway/v1",
    weatherRefreshRate: 1000 * 60 * 5,
    railwayRefreshRate: 1000 * 60 * 5
};

window.padTimeString = function(input) {
    input = "" + input;
    if(input.length == 1) {
        return "0" + input;
    } else {
        return input;
    }
}

var vm = (function (cfg, ko, $, window, console) {
    console.log("Board starting...");
    var self = this;

    self.railwayVm = new RailwayVm(cfg, $, ko, window, console);
    self.weatherVm = new WeatherVm(cfg, $, ko, window, console);
    self.clockVm = new ClockVm(ko, window);

    console.log("Board started!");
})(config, ko, $, window, console);

function RailwayVm(config, $, ko, window, console) {
    var self = this;
    self.railwayData = ko.observableArray([]);

    self.loadRailwayData = function () {
        console.log("Refreshing railway data...");
        var request = $.get(config.railwayUrl);
        request.then((result) => {
            console.log(result);
            for(var d of result) {
                var departure = new Date(d.departureTime);
                d.departureTimeString = window.padTimeString(departure.getHours()) + ":" + window.padTimeString(departure.getMinutes());
            }
            self.railwayData(result);
        });       
    }

    self.railTimerHandle = window.setTimeout(() => {
        self.loadRailwayData();
    }, config.railwayRefreshRate);

    self.loadRailwayData();
}

function WeatherVm(config, $, ko, window, console) {
    var self = this;

    self.weatherData = ko.observable(null);

    self.loadWeatherData = function () {
        console.log("Refreshing weather data...");
        var request = $.get(config.weatherUrl);
        request.then(self.weatherData);
    }

    self.weatherTimerHandler = window.setTimeout(() => {
        self.loadWeatherData();
    }, config.weatherRefreshRate);

    self.loadWeatherData();
}

function ClockVm(ko, window) {
    var self = this;

    self.time = ko.observable("??:??");
    self.date = ko.observable("??-??-????");
    self.day = ko.observable("?");

    self.clockTimerHandle = window.setTimeout(() => {
        var now = new Date();
        self.time(window.padTimeString(now.getHours()) + ":" + window.padTimeString(now.getMinutes()));
        self.date(now.getDate() + "-" + (now.getMonth() + 1) + "-" + now.getFullYear());
        self.day(self.getDayName(now.getDay()));
    }, 1000);

    self.getDayName = function (dayNum) {
        // 0: sunday...
        switch (dayNum) {
            case 0:
                return "Zondag";
            case 1:
                return "Maandag"
            case 2:
                return "Dinsdag";
            case 3:
                return "Woensdag"
            case 4:
                return "Donderdag";
            case 5:
                return "Vrijdag"
            case 6:
                return "Zaterdag";
            default:
                return "???";
        }
    }
}

ko.applyBindings(vm, document.getElementById("dashboard"));