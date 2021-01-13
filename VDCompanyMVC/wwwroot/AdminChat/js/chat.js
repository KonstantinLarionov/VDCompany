var current_user = "";
var token = "-";

const hubConnection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 30;

hubConnection.on("Send", function (msg) {
    var result = JSON.parse(msg);
    add_message(result.Item1, result.Item2);
});
hubConnection.start();

function Connect() {
        hubConnection.invoke("Connect", "admin")
    }
window.onload = function () {
    var t = getUrlParameter("token");
    if (t === undefined) {
        swal("Ошибка", "Отсутствует маркер доступа", "error");
    } else {
        token = t;
    }
    Connect();
    if (t !== undefined)
        get_users();
}

function send_my_message(){
    var message = $('#my_message').val();
    if(message != null || message != "")
    {
        $('#my_message').val('');
        hubConnection.invoke("Send", message, "admin", "123321", current_user, token);
    }
}

function add_user(user, info = ''){
    var messages = $('#chatDirectMsg');
    var a = $('<a href="#" onclick="select_dialog(this);" class="media"></a>');
    if(user.indexOf('@') > -1)
        var divh = $('<div class="avatar avatar-sm"><span class="avatar-initial bg-success rounded-circle">@</span></div>');
    else 
        var divh = $('<div class="avatar avatar-sm"><span class="avatar-initial bg-dark rounded-circle">#</span></div>');
        var divb = $('<div class="media-body mg-l-10"></div>');
        var h6 = $('<h6 class="mg-b-0">' + user + '</h6>');
        var small = $('<small class="d-block tx-color-04">' + info + '</small>');
        divb.append(h6).append(small);
        a.append(divh).append(divb);
        messages.append(a);
}
function get_users(){
    $.ajax({
        type: "POST",
        url: "../Home/GetUsers",
        data: "token=" + token,
        success: function (msg) {
            var result = JSON.parse(msg);
            result.forEach(function (item, i, arr) {
                if(item.Login === 'admin')
                {
                    //
                } else {
                   
                var dt = new Date(Date.parse(item.Date));
                var d = (dt.getHours() + "").padStart(2, "0") +
                ":" + (dt.getMinutes() + "").padStart(2, "0") +
                ":" + (dt.getSeconds() + "").padStart(2, "0") + 
                "  |  " + (dt.getDay() + "").padStart(2, "0") + 
                "/" + (dt.getMonth() + "").padStart(2, "0") + 
                "/" + (dt.getFullYear() + "");
                add_user(item.Login, d);
                }
            });
        }
    });
}
function showinfo() {
    swal("В разработке", "Тут будут дополнительные сведения о текущем диалоге", "info");
}
function get_history() {
    $.ajax({
        type: "POST",
        data: "login=" + current_user + "&token=" + token,
        url: "../Home/GetHistory",
        success: function (msg) {
            var result = JSON.parse(msg);
            result.forEach(function (item, i, arr) {
                add_message(item.User.Login, item.Text);
            });
        }
    });
}
function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;
    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
}
function add_message(user, message, info = ''){
    var list = $('#chat_list');
    var div1 = $('<div class="media"><div>');
    if(user === 'admin')
        var div2 = $('<div class="avatar avatar-sm"><span class="avatar-initial bg-danger rounded-circle">a</span></div>');
    else if(user.indexOf('@') > -1)
        var div2 = $('<div class="avatar avatar-sm"><span class="avatar-initial bg-success rounded-circle">@</span></div>');
    else
        var div2 = $('<div class="avatar avatar-sm"><span class="avatar-initial bg-dark rounded-circle">#</span></div>');
    var div3 = $('<div class="media-body"><div>');
    var h6 = $('<h6>' + user + ' <small>' + info + '</small></h6>');
    var p = $('<p>' + message + '</p>');
    div3.append(h6).append(p);
    div1.append(div2).append(div3);
    list.append(div1);
    list.scrollTop(list.prop('scrollHeight'));
}

function add_adminmessage3(message) {
    //var dt = new Date();
    //var time = (dt.getHours() + "").padStart(2, "0") + ":" + (dt.getMinutes() + "").padStart(2, "0");
    $('#chat_list').append('<div class="msg-admin">' + message + /*'<br><span style=" color: #858585; text-align: right;">' + time + '</span>*/'</div>');
    $('#chat_list').scrollTop($('#chat_list').prop('scrollHeight'));
}         // Добавление сообщения от администратора

// direct message click
function select_dialog(element){

    $(element).addClass('active');
    $(element).siblings().removeClass('active');

    $('#allChannels .active').removeClass('active');

    var directUser = $(element).find('h6').text();
    current_user = directUser;
    $('#chat_list').empty();
    get_history();
    $('#directTitle h6').text('Диалог с ' + directUser);

    var avatar = $(element).find('.avatar');
    $('#directTitle .avatar').replaceWith(avatar.clone());

    // view direct title
    $('#channelTitle').addClass('d-none');
    $('#directTitle').removeClass('d-none');

    // view direct nav icon
    $('#channelNav').addClass('d-none');
    $('#directNav').removeClass('d-none');

    if(window.matchMedia('(max-width: 991px)').matches) {
      showChatContent();
    }

    $('body').removeClass('show-sidebar-right');
    $('#showMemberList').removeClass('active');

  }

  function showChatContent() {
    $('#mainMenuOpen').addClass('d-none');
    $('#chatContentClose').removeClass('d-none');

    $('body').addClass('chat-content-show');
  }

  function hideChatContent() {
    $('#chatContentClose').addClass('d-none');
    $('#mainMenuOpen').removeClass('d-none');

    $('body').removeClass('chat-content-show');
  }
