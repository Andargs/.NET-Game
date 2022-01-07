const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationhub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
           
    try {
        await connection.start();

        console.log("SignalR Connected.");
        

    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
    try {
        var theGame = document.getElementsByClassName("Game")[0].getAttribute("id");
        var GroupName = "Game"+theGame;
        await connection.invoke("AddToGroup",GroupName);
        console.log("joined Group")
        console.log(GroupName);
    }catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};
connection.on("ReceiveNotification", () => {
   console.log("time to reload")
    location.reload(true);
});

connection.onclose(async () => {
    await start();
});

// Start the connection.
start();