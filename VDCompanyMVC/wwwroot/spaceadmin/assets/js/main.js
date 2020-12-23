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
        }
    });
}
function del(b) {
    var butt = $(b);
    var id = butt.attr('law_id');
    butt.parent().parent().remove();
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
    });
}