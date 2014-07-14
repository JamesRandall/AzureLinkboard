'use strict';
app.factory('settingsService', [
    function () {
        return {
            remoteServer: "https://azurelinkboardwebapi.azurewebsites.net/",
            server: "https://azurelinkboardweb.azurewebsites.net/"
            //server: "https://localhost:44300/",
            //remoteServer: "https://localhost:44301/"
        };
    }
]);