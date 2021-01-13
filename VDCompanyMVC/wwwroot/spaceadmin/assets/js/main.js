var id_case = 0;
function openmodal(id) {
    id_case = id;
    $('#modal').modal('show');
}
function closemodal() {
    id_case = 0;
    $('#modal').modal('hide');
}
function addLawyer() {
    id_adv = $('#id_adv').val();
    if (id_adv == 0) return;
    $.ajax({
        url: '/Admin/AddLawyer',
        method: 'post',
        data: { id_case: id_case, id_lawyer: id_adv },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "success") { // можно добавить алерт
                $('#lawyersincase').css('display', '');
                addrow(a.object.FIO + ' (' + a.object.Login + ')', a.object.Id);
            }
            if (a.status == "isset") {
                // добавить алерт, о уже существующем (добавленом)
            }
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку -выкинет на авторизацию
                window.location.reload();
            }
        }
    });
}
function del(b) {
    var butt = $(b);
    var id = butt.attr('law_id');
    $.ajax({
        url: '/Admin/DelLawyer',
        method: 'post',
        data: { id_case: id_case, id_lawyer: id },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "success") { 
                butt.parent().parent().remove();
            }
            if (a.status == "error") {
                alert(a.data);
            }
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку -выкинет на авторизацию
                window.location.reload();
            }
        }
    });
    
}
function addrow(name, id) {
        var el = $('<tr>' +
        '<td>' + name + '</td>' +
        '<td style="width: 50px;">' +
        '<button class="btn btn-outline-danger" law_id=' + id + ' title="Удалить из дела" onclick="del(this)">✕</button>' +
        '</td>' +
        '</tr>');
    $('#tablemembers').append(el);
}
function addoption(value, text) {
    $('#id_adv').append('<option value="' + value + '">' + text + '</option>');
}
function getmembers(id_case) {
    $.ajax({
        url: '/Admin/GetLawyers',
        method: 'post',
        data: { id_case: id_case },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                if (a.CaseLawyers.length <= 0) { $('#lawyersincase').css('display', 'none'); }
                else { $('#lawyersincase').css('display', ''); }
                $('#tablemembers').empty();
                $('#id_adv').empty();
                a.CaseLawyers.forEach(function (item) {
                    addrow(item.FIO + ' (' + item.Login + ')', item.Id);
                });
                addoption(0, 'Не выбрано');
                a.AllLawyers.forEach(function (item) {
                    addoption(item.Id, item.FIO + ' (' + item.Login + ')');
                });
                openmodal(id_case);
            }
        }
    });
}

/*                     */
var id_lawyer = 0;
function editlawyer(id) {
    id_lawyer = id;
    $('#ModalLabel').text('Редактирование данных юриста id = ' + id)
    $('#newlawyer').hide();
    $('#editlawyer').show();
    $.ajax({
        url: '/Admin/GetInfoLawyer',
        method: 'post',
        data: { id: id_lawyer },
        success: function (data) {
            var a = JSON.parse(data);
            console.log(a);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                $('#fio').val(a.data.FIO);
                $('#log').val(a.data.Login);
                $('#psw').val(a.data.Password);
                $('#modal').modal('show');
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
    
}
function newlawyer() {
    $('#fio').val("");
    $('#log').val("");
    $('#psw').val("");
    $('#ModalLabel').text('Добавление нового юриста')
    $('#newlawyer').show();
    $('#editlawyer').hide();
    $('#modal').modal('show');
}
function New_Lawyer() {
    var fio = $('#fio').val();
    var log = $('#log').val();
    var psw = $('#psw').val();
    $.ajax({
        url: '/Admin/NewLawyer',
        method: 'post',
        data: { fio: fio , login: log, password: psw },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = '<tr>' +
                    '<th scope="row">' + a.id + '</th>' +
                    '<td>' + fio + '</td>' +
                    '<td>' + log + '</td>' +
                    '<td>' + psw + '</td>' +
                    '<td>' +
                    '<div class="btn-group w-100" role="group">' +
                    '<button type="button" class="btn btn-outline-primary" onclick="editlawyer(' + a.id + ')">Редакт.</button>' +
                    '<button type="button" class="btn btn-outline-danger" onclick="Remove_Lawyer(' + a.id + ')">Удалить</button>' +
                    '</div>' +
                    '</td>' +
                    '</tr>';
                $('tbody').append(row);
                $('#modal').modal('hide');
            }
        }
    });
}
function Edit_Lawyer() {
    var fio = $('#fio').val();
    var log = $('#log').val();
    var psw = $('#psw').val();
    $.ajax({
        url: '/Admin/EditLawyer',
        method: 'post',
        data: { id: id_lawyer, fio: fio, login: log, password: psw },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = $($('th').filter(function () { return $(this).text() == id_lawyer; })[0]).parent()[0];
                $(row.children[1]).text(fio);
                $(row.children[2]).text(log);
                $(row.children[3]).text(psw);
                $('#modal').modal('hide');
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function Remove_Lawyer(id) {
    $.ajax({
        url: '/Admin/RemoveLawyer',
        method: 'post',
        data: { id: id },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = $($('th').filter(function () { return $(this).text() == id; })[0]).parent()[0];
                row.remove();
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}



/*                  */

var id_user = 0;
function edituser(id) {
    id_user = id;
    $.ajax({
        url: '/Admin/GetInfoUser',
        method: 'post',
        data: { id: id_user },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                $('#fio').val(a.data.Name);
                $('#log').val(a.data.Login);
                $('#psw').val(a.data.Password);
                $('#modaluseredit').modal('show');
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function Edit_User() {
    var fio = $('#fio').val();
    var log = $('#log').val();
    var psw = $('#psw').val();
    $.ajax({
        url: '/Admin/EditUser',
        method: 'post',
        data: { id: id_user, fio: fio, login: log, password: psw },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = $($('th').filter(function () { return $(this).text() == id_user; })[0]).parent()[0];
                $(row.children[1]).text(fio);
                $(row.children[2]).text(log);
                $(row.children[3]).text(psw);
                $('#modaluseredit').modal('hide');
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function billsuser(id) {
    id_user = id;
    $.ajax({
        url: '/Admin/GetUserBills',
        method: 'post',
        data: { id: id_user },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                $('#bills').empty();
                a.data.forEach(function (item) {                
                    var row = '<tr>' +
                        '<th>' + item.Id + '</th>' +
                        '<td>' + item.NameCase + '</td>' +
                        '<td>' + item.Amount + '</td>' +
                        '<td>' + item.Requizit + '</td>' +
                        '<td class="text-center">';
                    if (item.Status == 0) row += '<button class="btn btn-danger btn-sm p-1 m-1" disabled>Отклонено</button>';
                    if (item.Status == 1) row += '<button class="btn btn-success btn-sm p-1 m-1" disabled>Оплачено</button>';
                    if (item.Status == 2) row +=
                        '<button class="btn btn-success btn-sm p-1 m-1" onclick="billpaid(' + item.Id + ');">Оплачено</button>' +
                    '<button class="btn btn-danger btn-sm p-1 m-1" onclick="billcancel(' + item.Id + ');">Отклонено</button>';
                    row += '</td ></tr >';
                    $('#bills').append(row);
                });
                $('#modaluserbills').modal('show');
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function casesuser(id) {
    id_user = id;
    $.ajax({
        url: '/Admin/GetUserCases',
        method: 'post',
        data: { id: id_user },
        success: function (data) {
            var a = JSON.parse(data);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {

            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function addbill() {
    $('#name').val('');
    $('#amount').val('');
    $('#req').val('');
    $('#modalnewbill').modal('show');
}
function Add_bill() {
    var name = $('#name').val();
    var amount = $('#amount').val();
    var req = $('#req').val();

    $.ajax({
        url: '/Admin/AddUserBill',
        method: 'post',
        data: { id: id_user, name: name, amount: amount, req: req },
        success: function (data) {
            var a = JSON.parse(data);
            console.log(a);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = '<tr>' +
                    '<th>' + a.id + '</th>' +
                    '<td>' + name + '</td>' +
                    '<td>' + amount + '</td>' +
                    '<td>' + req + '</td>' +
                    '<td class="text-center">' +
                    '<button class="btn btn-success btn-sm p-1 m-1" onclick="billpaid(' + a.id + ');">Оплачено</button>' +
                    '<button class="btn btn-danger btn-sm p-1 m-1" onclick="billcancel(' + a.id + ');">Отклонено</button>' +
                    '</td ></tr>';
                $('#bills').append(row);
                $('#modalnewbill').modal('hide');
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function billpaid(id) {
    $.ajax({
        url: '/Admin/UserBillChangeStatus',
        method: 'post',
        data: { user_id: id_user, bill_id: id, status: "payed" },
        success: function (data) {
            var a = JSON.parse(data);
            console.log(a);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = $($('#bills > tr > th').filter(function () { return $(this).text() == id; })[0]).parent()[0];
                var stat = $(row.children[4]);
                stat.children('.btn-danger').remove();
                stat.children('.btn-success').prop('disabled', true);
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}
function billcancel(id) {
    $.ajax({
        url: '/Admin/UserBillChangeStatus',
        method: 'post',
        data: { user_id: id_user, bill_id: id, status: "cancel" },
        success: function (data) {
            var a = JSON.parse(data);
            console.log(a);
            if (a.status == "not_authorized") { // слетела сессия, обновим страничку - выкинет на авторизацию
                window.location.reload();
                return;
            }
            if (a.status == "success") {
                var row = $($('#bills > tr > th').filter(function () { return $(this).text() == id; })[0]).parent()[0];
                var stat = $(row.children[4]);
                stat.children('.btn-danger').prop('disabled', true);
                stat.children('.btn-success').remove();
            }
            if (a.status == "error") {
                alert(a.data);
            }
        }
    });
}