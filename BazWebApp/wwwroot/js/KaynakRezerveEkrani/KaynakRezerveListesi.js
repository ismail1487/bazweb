var KaynakRezerve = [];
$(document).ready(function () {
    KaynakListesiGetir();
});

function EklemeSayfasi() {
    window.location.href = `/KaynakRezerve/KaynakRezerveGiris`;
}

function GuncelemeSayfasinaGit(urlId) {
    window.location.href = `/KaynakRezerve/KaynakRezerveGuncelleme/${urlId}`;
}

function KaynakListesiGetir() {
    $.ajax({
        type: "GET",
        url: "/KaynakRezerve/KaynakRezerveListele",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                console.log(result.value);
                $("#KaynaklisresiTable").DataTable().destroy();
                var satirlar = "";
                console.log(result.value);
                data = result.value;
                for (var i of data) {
                    for (var items in i.kaynakRezerveTanimVM) {
                        var rezerveSaatBaslangic = i.kaynakRezerveTanimVM.rezerveSaatBaslangicDegeri;
                        var rezerveSaatBitis = i.kaynakRezerveTanimVM.rezerveSaatBitisDegeri;
                        var RezerveGunTipi = i.kaynakRezerveTanimVM.uygunGunTipleri || "-";
                        var gun;
                        if (RezerveGunTipi == 1) {
                            gun = "Hafta İçi";
                        }
                        if (RezerveGunTipi == 2) {
                            gun = "Hafta Sonu";
                        }
                        if (RezerveGunTipi == 3) {
                            gun = "Hafta Boyu";
                        }
                    }
                    for (var items in i.kaynakTanimVM) {
                        var kaynakAdi = i.kaynakTanimVM.kaynakAdi || "-";
                        var kaynakAciklama = i.kaynakTanimVM.kaynakAciklama || "-";
                    }
                    satirlar += "<tr >\
                    <td >" + rezerveSaatBaslangic + "-" + rezerveSaatBitis + "</td>\
                    <td >" + kaynakAdi + "</td>\
                    <td >" + gun + "</td>\
                    <td >" + kaynakAciklama +"</td>\
                    <td> <button type='button' class='btn btn-sm mx-1' onclick='GuncelemeSayfasinaGit(" + i.tabloID + ")'> <img src='/img/edit.svg'/> </button>\
                    <button type='button' class='btn btn-sm' onclick='Sil(" + i.tabloID + ")'> <img src='/img/trash.svg'/></td></tr> ";
                }
                $("#tbody").html(satirlar);
                $('#KaynakListesiTable').DataTable({
                    "paging": true,
                    "responsive": true,
                    "language": {
                        "lengthMenu": siteLang.lengthComputable,
                        "info": siteLang.info,
                        "searchPlaceholder": siteLang.searchPlaceholder,
                        "zeroRecords": siteLang.zeroRecords,
                        "paginate": {
                            "infoEmpty": siteLang.infoEmpty,
                            "first": siteLang.First,
                            "last": siteLang.Last,
                            "next": siteLang.Next,
                            "previous": siteLang.Previous,
                        }
                    },
                    "dom": 'Bfrtip',
                    "buttons": [
                        {
                            "extend": 'excel',
                            "text": siteLang.ExceleAktar,
                            "className": "btn btn-success",
                            "exportOptions": {
                                "columns": ":not(:last-child)"
                            }
                        }
                    ]
                })
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }, //success: function (result) {
    });
}
function tarihDüzenleme(tarih) {
    // Gelen tarih dizgesini Date nesnesine dönüştür
    var date = new Date(tarih);

    // Tarihi istenen formata dönüştür
    var formattedDate = date.toLocaleString('tr-TR', { year: 'numeric', month: '2-digit', day: '2-digit' });

    // "T" harfini kaldır
    formattedDate = formattedDate.replace('T', ' ');

    return formattedDate; // formattedDate değişkenini döndür
}

function Sil(TabloId) {
    console.log("Id: " + TabloId);
    Swal.fire({
        title: "Emin misiniz?",
        text: "Geri alma imkanınız olmayacak!!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sil!"
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: '/KaynakRezerve/SilKaynak/' + TabloId,
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ TabloId: TabloId }),
                success: function (data) {
                    Swal.fire({
                        title: "Silindi!",
                        text: "Kaynak Listesinden silindi",
                        icon: "success"
                    });
                    setTimeout(() => window.location.reload(), 2000);
                },
                error: function () {
                    console.log("Hata :" + TabloId);
                }
            });
        }
    });
}