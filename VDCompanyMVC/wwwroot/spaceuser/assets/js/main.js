//listmembers listdocs chatcontent
"use strict";
var caseid = 0;
var isEnter = false;
var isCtrl = false;

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/casehub")
    //.configureLogging(signalR.LogLevel.None)
    .build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 10;

hubConnection.on("ReceiveMessage", function (msg) {
    var a = JSON.parse(msg);
    console.log(a);
    var u = "undefined";
    if (a.Admin != null) u = a.Admin;
    if (a.Lawyer != null) u = a.Lawyer;
    if (a.User != null) u = a.User;
    addmessage(u, a.Message, (a.Date).split('.')[0].replace('T', ' '));
    chatscrolltobottom();
});

hubConnection.on("SendHistory", function (msg) {
    chatclear();
    var a = JSON.parse(msg);
    console.log(a);
    a.forEach(function (item) {
        var u = "unnnamed";
        if (item.Admin != null) u = item.Admin;
        if (item.Lawyer != null) u = item.Lawyer;
        if (item.User != null) u = item.User;
        addmessage(u, item.Message, (item.Date).split('.')[0].replace('T', ' '));
    });
    chatscrolltobottom();
});

hubConnection.start().then(function () {
    // Если соединение успешное установлено, 
    GetHistory();
}).catch(function (err) {
    // Если соединение не удалось
    return console.error(err.toString());
});

function chatscrolltobottom() {
    $('.chat-content-body').scrollTop($('.chat-group').prop("scrollHeight"));
}
function SendMsg(msg) {
    hubConnection.invoke("SendMessage", $.cookie("login"), $.cookie("password"), caseid, msg);
}
function GetHistory() {
    hubConnection.invoke("GetHistory", $.cookie("login"), $.cookie("password"), caseid);
}
window.onload = function(){
    if(window.matchMedia('(max-width: 991px)').matches) {
      $('#chatcontent').addClass('mobile contentright contentleft');
      $('#listdocs').addClass('toggled-right').css('width', '100%');
      $('#listmembers').addClass('toggled-left').css('width', '100%');
    }
  };
$('media').on({
   'hover': $('delete').css('dispay', 'block') 
});
$('#buttondocs').on({
    'click': toggleright });
$('#buttonmembers').on({
    'click': toggleleft });
$('#sendbutton').on({
    'click': function () {
        event.preventDefault();
        send();
    }
});
function toggleright(){
    event.preventDefault();
    if(!$('#chatcontent').hasClass('mobile')){
    $('#chatcontent').toggleClass('contentright');
    }
    $('#listdocs').toggleClass('toggled-right');
    
}
function toggleleft(){
    event.preventDefault();
    if(!$('#chatcontent').hasClass('mobile')){
    $('#chatcontent').toggleClass('contentleft');
    }
    $('#listmembers').toggleClass('toggled-left');
}
$('#message').keyup(function(e){
    if(e.which == 13) isEnter=false;
    if(e.which == 17) isCtrl=false;
});
$('#message').keydown(function(e){
    if(e.which == 13) isEnter=true;
    if(e.which == 17) isCtrl=true;
    if(isCtrl && isEnter) send();
});
function send() {
    if ($('#message').val().trim() == "") return;
    SendMsg($('#message').val());
    $('#message').val('');
}
function addmessage(user, message, date) {
    var block = $(
        '<div class="media">' +
            '<div class="avatar avatar-sm"><span class="avatar-initial rounded-circle">' + user[0] + '</span></div>' +
            '<div class="media-body">' +
        '<h6>' + user + '<small>&nbsp ' + date + '</small></h6>' +
                '<p>' + message + '</p>' +
            '</div>' +
        '</div>');
    $('.chat-group').append(block);
}
function chatclear() {
    $('.chat-group').empty();
}