var IcerikListesi = [];
$(document).ready(function () {
    IcerikListesiGetir();
});

function GuncelemeSayfasinaGit(urlId) {
    window.location.href = `/IcerikGiris/IcerikGuncelleme/${urlId}`;
}
function IcerikListesiGetir() {
    $.ajax({
        type: "GET",
        url: "/IcerikGiris/IcerikListesi",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                console.log(result.value);
                $("#IcerikListesiTable").DataTable().destroy();
                var satirlar = "";
                console.log(result.value);
                data = result.value;
                for (var i of data) {
                    var tarih = i.icerikYayinlanmaZamani;
                    var tarihh = tarihDüzenleme(tarih);

                    var spotHtml = "";
                    if (i.icerikSpot && i.icerikSpot.length > 0) {
                        spotHtml += "<div style='max-height: 150px; overflow-y: auto;'>\
    <ul style='padding-left: 20px; margin: 0;'>";

                        for (var spot of i.icerikSpot) {
                            spotHtml += "<li style='margin-bottom: 4px;'>" + spot.spotMetni + "</li>";
                        }

                        spotHtml += "</ul></div>";
                    }
                    var taslakmi = "-";
                    if (i.taslakMi == true) {
                        taslakmi == "evet"
                    } else {
                        taslakmi = "hayir"
                    }
                    var medyaHtml = "";
                    if (i.medyalar.length > 0) {
                        medyaHtml += "<div style='max-height: 200px; overflow-y: auto;'>\
                        <table class='table table-sm'>\
                        <thead><tr>\
                        <th>Medya Bilgiler</th>\
                        <th>Tip</th>\
                        <th>Adı</th>\
                        <th>Gösterim</th>\
                        <th>Sıra</th>\
                        <th>Alt Metin</th>\
                        </tr></thead><tbody>";
                        for (var medya of i.medyalar) {
                            medyaHtml += "<tr>\
                              <td><img src='"+ 'http://localhost:51305' + medya.medyaUrl + "' alt='Medya' style='max-width: 100px; max-height: 100px;'/></td>\
                              <td>" + (medya.medyaTipiAdi || "") + "</td>\
                              <td>" + (medya.medyaAdi || "") + "</td>\
                              <td>" + (medya.gosterimAdi || "") + "</td>\
                              <td>" + (medya.siraNo != null ? medya.siraNo : "") + "</td>\
                              <td>" + (medya.altMetin || "") + "</td>\
                              </tr>";
                        }
                        medyaHtml += "</tbody></table></div>";
                    }
                    satirlar += "<tr >\
                    <td >" + tarihDüzenleme(i.icerikYayinlanmaZamani) + "</td>\
                    <td >" + tarihDüzenleme(i.icerikKaldirilmaZamani) + "</td>\
                    <td >" + i.paramAnaKategoriAdi + "</td>\
                    <td >" + i.paramIcerikKategoriAdi + "</td>\
                    <td >" + i.paramAltKategoriAdi + "</td>\
                    <td >" + i.icerikBaslik + "</td>\
                    <td >" + i.taslakMi + "</td>\
                    <td >" + medyaHtml + "</td>\
                    <td> <button type='button' class='btn btn-sm mx-1' onclick='GuncelemeSayfasinaGit(" + i.tabloID + ")'> <img src='/img/edit.svg'/> </button>\
                    <button type='button' class='btn btn-sm' onclick='Sil(" + i.tabloID + ")'> <img src='/img/trash.svg'/></td></tr> ";
                }
                $("#tbody").html(satirlar);
                $('#IcerikListesiTable').DataTable({
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
                url: '/IcerikGiris/IcerikSil/' + TabloId,
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

