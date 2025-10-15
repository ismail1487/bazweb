var SliderKategoriList = [];
var SliderModelVM = {};

$(document).ready(function () {
    MedyaEkle();
});

var tekrarliMedyaId = -1;
function TekrarliMedyaEkle() {
    let showRemove = tekrarliMedyaId !== 0; // ilk spot için false
    let medyaHtml = '<div class="row">\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Medya Adı</label>\
                    <input id="medyaAdi'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    <label for="medyaAdi" id="MedyaAdiHata'+ tekrarliMedyaId + '" class="validationMessage">Medya Adı Giriniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Gösterim Adı</label>\
                    <input id="gosterimAdi'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    <label for="gosterimAdi" id="gosterimAdiHata'+ tekrarliMedyaId + '" class="validationMessage">Gösterim Adı Giriniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Sıra</label>\
                    <select id="sira'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    </select>\
                    <label for="sira" id="siraHata'+ tekrarliMedyaId + '" class="validationMessage">Sıra Seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Dosya Ekle</label>\
                    <input type="file" id="dosyaEkle' + tekrarliMedyaId + '" class="form-control" style="width: 100%;"/>\
                    <label for="dosyaEkleHata" id="dosyaEkleHata'+ tekrarliMedyaId + '" class="validationMessage">Dosya Seçiniz</label>\
                </div>\
         </div>\
            <div class="col-md-8">\
                <div class="form-group">\
                    <label class="Lbl">Açıklama</label>\
                    <textarea id="medyaAciklama'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required></textarea>\
                    <label for="medyaAciklama" id="medyaAciklamaHata'+ tekrarliMedyaId + '" class="validationMessage">Açıklama</label>\
                </div>\
            </div>\
            <div id="dosyaPreview' + tekrarliMedyaId + '" class="col-md-4 dosya-preview" style="margin-top:5px;"></div>\
            <div class="d-flex justify-content-end">\
        <button id = "addSpot" onclick = "MedyaEkle(this)" class="btn mb-0" data - accordion - id="${datacordion}" > + Yeni Medya Ekle</button >\
        <div class="d-flex align-items-center MedyaRemoveButonWrapper" style="display:' + (showRemove ? 'flex' : 'none') + '">\
            <div class="MedyaRemoveButonGrp">\
                <button type="button" class="btn btn-xs MedyaRemoveButton" onclick="MedyaSil(this)">\
                    <i class="fas fa-minus-circle accent-danger"></i>\
                </button>\
            </div>\
        </div>\
        </div>\
      </div>';
    return medyaHtml;


}
// Dinamik olarak oluşturulan dosya inputları için event delegation kullan:
$(document).on("change", "[id^='dosyaEkle']", function () {
    var input = this;  // değişen input
    var idBul = input.id.replace("dosyaEkle", "");
    if (input.files.length) {
        var fd = new FormData();
        fd.append('file', input.files[0]);

        $.ajax({
            url: "/panel/resimyukle/",
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.isSuccess && response.value) {
                    var data = response.value;
                    var previewDiv = document.getElementById('dosyaPreview' + idBul);
                    if (previewDiv) {
                        previewDiv.setAttribute('data-media-id', data.tabloID);
                    }
                    console.log("Yüklenen dosyanın tabloID'si:", data.tabloID);

                } else {
                    toastr.error("Dosya gönderilemedi");
                }
            }
        });
    }
});


function SiraSelectDoldur(selectId, min = 1, max = 100) {
    let options = '<option value="0">Seçiniz</option>';
    for (let i = min; i <= max; i++) {
        options += `<option value="${i}">${i}</option>`;
    }
    $('#' + selectId).html(options);
}
function MedyaEkle() {
    tekrarliMedyaId++;
    let html = TekrarliMedyaEkle();
    $("#tekrarliMedya").append($(html));
    SiraSelectDoldur('sira' + tekrarliMedyaId, 1, 100);
}
function MedyaSil(button) {
    // Eğer sadece 1 tane spot kaldıysa silme
    if ($("#tekrarliMedya .row").length <= 1) return;

    // Butona basıldığı satırı sil
    $(button).closest(".row").remove();
    MedyaIdleriGuncelle();
}
function MedyaIdleriGuncelle() {
    $("#tekrarliMedya .row").each(function (index) {
        $(this).find('#medyaAdi' + (index + 1)).attr('id', 'medyaAdi' + index);
        $(this).find('#gosterimAdi' + (index + 1)).attr('id', 'gosterimAdi' + index);
        $(this).find('#sira' + (index + 1)).attr('id', 'sira' + index);
        $(this).find('#dosyaEkle' + (index + 1)).attr('id', 'dosyaEkle' + index);
    });
}
function HataUyarilari() {
    let Errors = false;

    // Başlık
    const baslik = document.getElementById("Baslik_1");
    const baslikHata = document.getElementById("validationMessageBaslik_1");
    if (!baslik.value.trim()) {
        baslikHata.style.display = "block";
        Errors = true;
    } else {
        baslikHata.style.display = "none";
    }

    // Açıklama
    const aciklama = document.getElementById("Aciklama_1");
    const aciklamabaslikHata = document.getElementById("validationMessageAciklama");
    if (!aciklama.value.trim()) {
        aciklamabaslikHata.style.display = "block";
        Errors = true;
    } else {
        aciklamabaslikHata.style.display = "none";
    }

    // ================================
    // Medya Alanları (Tekrarlı)
    // ================================
    const medyaCount = document.querySelectorAll('[id^="medyaTipi"]').length;

    for (let i = 0; i < medyaCount; i++) {
        const medyaAdi = document.getElementById(`medyaAdi${i}`);
        const gosterimAdi = document.getElementById(`gosterimAdi${i}`);
        const sira = document.getElementById(`sira${i}`);
        const dosyaEkle = document.getElementById(`dosyaEkle${i}`);

        const medyaAdiHata = document.getElementById(`MedyaAdiHata${i}`);
        const gosterimAdiHata = document.getElementById(`gosterimAdiHata${i}`);
        const siraHata = document.getElementById(`siraHata${i}`);
        const dosyaEkleHata = document.getElementById(`dosyaEkleHata${i}`);

        if (!medyaAdi || !gosterimAdi || !sira || !dosyaEkle) continue;

        if (!medyaAdi.value.trim()) {
            if (medyaAdiHata) medyaAdiHata.style.display = "block";
            Errors = true;
        } else {
            if (medyaAdiHata) medyaAdiHata.style.display = "none";
        }

        if (!gosterimAdi.value.trim()) {
            if (gosterimAdiHata) gosterimAdiHata.style.display = "block";
            Errors = true;
        } else {
            if (gosterimAdiHata) gosterimAdiHata.style.display = "none";
        }

        if (sira.value === "0" || sira.value === "") {
            if (siraHata) siraHata.style.display = "block";
            Errors = true;
        } else {
            if (siraHata) siraHata.style.display = "none";
        }

        if (!dosyaEkle.value) {
            if (dosyaEkleHata) dosyaEkleHata.style.display = "block";
            Errors = true;
        } else {
            if (dosyaEkleHata) dosyaEkleHata.style.display = "none";
        }
    }

    return !Errors;
}
function SliderKaydet() {
    var hataVar = HataUyarilari();
    if (hataVar) {
        SliderModeliKaydet();
    }
    Errors = false;
}
function VerileriModeleBas() {
    let SliderModelVM = {};

    // Temel Alanlar
    SliderModelVM["SliderGorselAciklama"] = $("#Aciklama_1").val();
    SliderModelVM["SliderGorselTanim"] = $("#Baslik_1").val();
    // Medyalar (Tekrarli Medya)
    SliderModelVM["SliderResimlerMedyalarVM"] = [];
    $('#tekrarliMedya > div').each(function () {
        let container = $(this);
        let idSuffix = container.find('input[type="file"]').attr("id").replace("dosyaEkle", "");

        SliderModelVM["SliderResimlerMedyalarVM"].push({
            MedyaAdi: $("#medyaAdi" + idSuffix).val(),
            GosterimAdi: $("#gosterimAdi" + idSuffix).val(),
            SiraNo: $("#sira" + idSuffix).val(),
            MedyaID: $("#dosyaPreview" + idSuffix).attr("data-media-id") || null,
            MedyaAciklama: $("#medyaAciklama" + idSuffix).val(), //string?
        });
    });
    console.log(SliderModelVM);
    return SliderModelVM;
}
function SliderModeliKaydet() {
    var model = VerileriModeleBas();
    $.ajax({
        type: "POST",
        async: false,
        url: "/Slider/SliderKaydet",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (result) {
            if (result.isSuccess == true) {
                var d = result.value;
                window.location.href = "/Slider/SliderListeleme"
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
