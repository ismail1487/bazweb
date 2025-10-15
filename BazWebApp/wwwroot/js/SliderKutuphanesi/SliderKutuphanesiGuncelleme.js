var SliderModelVM = {};
const SilinenMedyalar = [];
// #endregion

$(document).ready(function () {
    MedyaEkle();
    SliderVeriGetir();
    URlIdCek();
});
// #region VeriGerirme - TekrarlıAlan
function URlIdCek() {
    const path = window.location.pathname;
    const parts = path.split('/');
    var urlId = parts[parts.length - 1];

    console.log("URL ID: " + urlId);
    return urlId;
}
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
            <div class="col-md-4" id="dosyaPreview'+ tekrarliMedyaId + '" class="dosya-preview" style="margin-top:5px;"></div>\
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
                        var imageUrl = data.medyaUrl || null;
                        if (imageUrl) {
                            previewDiv.innerHTML = `<img src="http://localhost:51305${imageUrl}" style="width: 100px;" />`;
                        }
                        console.log("Yüklenen dosyanın tabloID'si:", data.tabloID);

                    } else {
                        toastr.error("Dosya gönderilemedi");
                    }
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
function SliderVeriGetir() {
    console.log("Slider veri getir çalıştırıldı");
    $.ajax({
        type: "GET",
        url: "/Slider/SliderVeriGetir/" + URlIdCek(), //Url'den Çekilen Id
        dataType: "json",
        success: function (result) {

            console.log("Gelen veri JSON:", JSON.stringify(result.value, null, 2));
            if (result.isSuccess && result.value && result.value.length > 0) {
                let veri = result.value[0];
                SliderVeriDoldur(veri);
            }
            else {
                console.log("Veri getirilemedi gelen değer Boş" + result.value + " " + result.isSuccess);
            }

        }
    })
}

function SliderVeriDoldur(veri) {

    // Başlık
    $("#Baslik_1").val(veri.sliderGorselTanim);
    //Açıklama
    $("#Aciklama_1").val(veri.sliderGorselAciklama);
    // Medyalar
    $("#tekrarliMedya").empty();
    tekrarliMedyaId = -1;
    if (veri.sliderResimlerMedyalarVM && veri.sliderResimlerMedyalarVM.length > 0) {
        veri.sliderResimlerMedyalarVM.forEach(m => {
            console.log("Medya nesnesi:", m);
            MedyaEkle();
            let id = tekrarliMedyaId;
            $("#medyaAdi" + id).val(m.medyaAdi);
            $("#gosterimAdi" + id).val(m.gosterimAdi);
            $("#sira" + id).val(m.siraNo);
            $("#medyaAciklama" + id).val(m.medyaAciklama);
            if (m.medyaUrl) {
                let html = `<img style="width: 100px; margin-right: 5px;" src="http://localhost:51305${m.medyaUrl}" />`;
                $("#dosyaPreview" + id).html(html);
                $("#dosyaEkle" + id).removeAttr("required");
            } else {
                $("#dosyaEkle" + id).attr("required", "required");
            }
            $("#dosyaPreview" + id).attr("data-media-id", m.medyaID);
        });
    } else {
        MedyaEkle();
    }
}

function MedyaEkle() {
    tekrarliMedyaId++;
    let html = TekrarliMedyaEkle();
    $("#tekrarliMedya").append($(html));
    SiraSelectDoldur('sira' + tekrarliMedyaId, 1, 100);
}
function MedyaSil(button) {
    const $row = $(button).closest(".row");
    const dataId = $row.attr("data-id");

    if (dataId !== undefined) {
        const dosyaAdi = $("#medyaAdi").val();
        SilinenMedyalar.push({
            MedyaAdi: dosyaAdi
        });

        console.log("Silinen medya eklendi:", { id: dataId, dosyaAdi: dosyaAdi });
    }

    // DOM'dan sil
    $row.remove();
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
// #endregion


// #region Guncelleme
function SliderGuncelle() {
    var hataVar = HataUyarilari();
    if (hataVar) {
        SliderModeliGuncelle();
    }
    Errors = false;
}
function VerileriModeleBas() {
    let SliderModelVM = {};

    // Temel Alanlar
    SliderModelVM["TabloID"] = URlIdCek();
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
function SliderModeliGuncelle() {
    var model = VerileriModeleBas();
    $.ajax({
        type: "POST",
        url: "/Slider/SliderGuncelle",
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

// #endregion
