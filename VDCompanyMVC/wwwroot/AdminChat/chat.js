
const hubConnection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 30;

hubConnection.on("Send", function (msg) {
    var result = JSON.parse(msg);
    console.log(result);
    if (result.Item1 !== "admin") {
        Interface.add_clientmessage(result.Item2);
    } else {
        Interface.add_adminmessage(result.Item2);
    }
});
hubConnection.start();

function Connect() {
        hubConnection.invoke("Connect", "admin")
    }
window.onload = function () {
    Connect();
}

function send_my_message(){
    var message = $('#my_message').val();
    if(message !== null || message !== "")
    {
        $('#my_message').val('');
        hubConnection.invoke("Send", message, "admin", "123321", 'mr.mishana-319@yandex.ru');
    }
}
