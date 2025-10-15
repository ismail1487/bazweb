var UrunListesi = [];
$(document).ready(function () {
    UrunListesiGetir();
});

function GuncelemeSayfasinaGit(urlId) {
    window.location.href = `/UrunGiris/UrunGuncellemeEkrani/${urlId}`;
}

function EklemeSayfasi() {
    window.location.href = `/UrunGiris/UrunGirisEkrani/`;
}
function UrunListesiGetir() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/UrunListesi",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                console.log(result.value);
                $("#UrunListesiTable").DataTable().destroy();
                var satirlar = "";
                console.log(result.value);
                data = result.value;
                for (var i of data) {
                    var tarih = i.urunYayinlanmaZamani;
                    var tarihh = tarihDüzenleme(tarih);
                    var taslakmi = "-";
                    if (i.taslakMi == 1) {
                        taslakmi == "evet"
                    } else {
                        taslakmi = "hayir"
                    }
                    var fiyatUrun = "-"; // default

                    var fiyatUrun = "-";
                    var fiyatDeger = "-";

                    if (Array.isArray(i.urunFiyatVM) && i.urunFiyatVM.length > 0) {

                        fiyatUrun = i.urunFiyatVM[0].urunFiyat;

                        fiyatDeger = i.urunFiyatVM[0].paraBirimTanim || "-";
                    }

                    satirlar += "<tr >\
                    <td >" + i.paramAnaKategoriAdi + "</td>\
                    <td >" + i.paramUrunKategoriAdi + "</td>\
                    <td >" + i.paramAltKategoriAdi + "</td>\
                    <td >" + i.urunAdi + "</td>\
                    <td >" + i.taslakMi + "</td>\
                    <td >" + fiyatUrun + " " + fiyatDeger +"</td>\
                    <td> <button type='button' class='btn btn-sm mx-1' onclick='GuncelemeSayfasinaGit(" + i.tabloID + ")'> <img src='/img/edit.svg'/> </button>\
                    <button type='button' class='btn btn-sm' onclick='Sil(" + i.tabloID + ")'> <img src='/img/trash.svg'/></td></tr> ";
                }
                $("#tbody").html(satirlar);
                $('#UrunListesiTable').DataTable({
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
    var formattedDate = date.toLocaleString('tr-TR', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' });

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
                url: '/UrunGiris/UrunSil/' + TabloId,
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ TabloId: TabloId }),
                success: function (result) {
                    Swal.fire({
                        title: "Silindi!",
                        text: "İçerik Listesinden silindi",
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

