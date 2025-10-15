var TekrarliMedya;
var kayitEdilenMedyaTabloID = {};
var KaynakRezerveVM = {};
$(document).ready(function () {
    MedyaEkle();
    KaynakTipiGetir();
    IstisnaEkle();
});

function TekrarliMedyaEkle() {
    let showRemove = tekrarliMedyaId !== 0; // ilk Medya için false

    let MedyaHtml = '<div class="row">\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Kaynak Görsel Ekle</label>\
                    <input type="file" id="dosyaEkle' + tekrarliMedyaId + '" class="form-control" style="width: 100%;"/>\
                    <label for="dosyaEkleHata" id="dosyaEkleHata'+ tekrarliMedyaId + '" class="validationMessage">Dosya Seçiniz</label>\
                </div>\
         </div>\
        <div class="col-md-8" id="dosyaPreview'+ tekrarliMedyaId + '" class="dosya-preview" style="margin-top:5px;"></div>\
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

    return MedyaHtml;
}
var tekrarliMedyaId = -1;
function MedyaEkle() {
    tekrarliMedyaId++;
    let html = TekrarliMedyaEkle();
    $("#tekrarliMedya").append($(html));
}
function MedyaSil(button) {
    // Eğer sadece 1 tane Medya kaldıysa silme
    if ($("#tekrarliMedya .row").length <= 1) return;

    // Butona basıldığı satırı sil
    $(button).closest(".row").remove();
    MedyaIdleriGuncelle();
}
function MedyaIdleriGuncelle() {
    $("#tekrarliMedya .row").each(function (index) {
        // Her .row içindeki input'u bul
        let input = $(this).find('input.form-control');
        if (input.length) {
            // input id'sini Medya + index şeklinde güncelle
            input.attr('id', 'Medya' + index);
        }
    });
}
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
                        kayitEdilenMedyaTabloID[idBul] = data.tabloID;
                        console.log(kayitEdilenMedyaTabloID);
                    } else {
                        toastr.error("Dosya gönderilemedi");
                    }
                }
            }
        });
    }
});
var KaynakListe = [];
function KaynakTipiGetir() {
    $.ajax({
        url: "/KaynakRezerve/KaynakTipiGetir",
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (data.isSuccess == true) {
                KaynakListe = data.value;
                var deger = KaynakListe.filter(x => x.ustId == 0);
                ParamDDLDoldur("kaynakTipi", deger);
            } else {
                alert("Kaynak tipleri alınırken hata oluştu.");
            }
        },
    });
}

function HataUyarilari() {
    let Errors = false;

    // =====================
    // Ana Kategori
    // =====================
    const anaKategori = document.getElementById("kaynakTipi");
    const anaKategoriHata = document.getElementById("kaynakTipiHata");
    if (!anaKategori.value || anaKategori.value === "0") {
        anaKategoriHata.style.display = "block";
        Errors = true;
    } else {
        anaKategoriHata.style.display = "none";
    }

    // =====================
    // Başlık
    // =====================
    const baslik = document.getElementById("kaynakTanim");
    const baslikHata = document.getElementById("kaynakTanimHata");
    if (!baslik.value.trim()) {
        baslikHata.style.display = "block";
        Errors = true;
    } else {
        baslikHata.style.display = "none";
    }

    // =====================
    // Açıklama
    // =====================
    const aciklama = document.getElementById("kaynakAciklama");
    const aciklamaHata = document.getElementById("kaynakAciklamaHata");
    if (!aciklama.value.trim()) {
        aciklamaHata.style.display = "block";
        Errors = true;
    } else {
        aciklamaHata.style.display = "none";
    }

    // =====================
    // Medya Alanları
    // =====================
    const medyaCount = document.querySelectorAll('[id^="medyaTipi"]').length;
    for (let i = 0; i < medyaCount; i++) {
        const dosyaEkle = document.getElementById(`dosyaEkle${i}`);
        const dosyaEkleHata = document.getElementById(`dosyaEkleHata${i}`);

        // dosyaEkle varsa kontrol et
        if (dosyaEkle) {
            if (!dosyaEkle.value) {
                dosyaEkleHata.style.display = "block";
                Errors = true;
            } else {
                dosyaEkleHata.style.display = "none";
            }
        }
    }

    // =====================
    // Istısnalar
    // =====================
    const istisnaCount = document.querySelectorAll('[id^="istisnaTarihBaslangic"]').length;
    for (let i = 0; i < istisnaCount; i++) {
        const istisnaTarihBaslangic = document.getElementById(`istisnaTarihBaslangic${i}`);
        const istisnaTarihBaslangicHata = document.getElementById(`istisnaTarihBaslangicHata${i}`);
        const istisnaTarihBitis = document.getElementById(`istisnaTarihBitis${i}`);
        const istisnaTarihBitisHata = document.getElementById(`istisnaTarihBitisHata${i}`);
        const molaSaatBaslangic = document.getElementById(`molaSaatBaslangic${i}`);
        const molaSaatBaslangicHata = document.getElementById(`molaSaatBaslangicHata${i}`);
        const molaSaatBitis = document.getElementById(`molaSaatBitis${i}`);
        const molaSaatBitisHata = document.getElementById(`molaSaatBitisHata${i}`);

        if (istisnaTarihBaslangic && istisnaTarihBaslangicHata) {
            if (!istisnaTarihBaslangic.value) {
                istisnaTarihBaslangicHata.style.display = "block";
                Errors = true;
            } else {
                istisnaTarihBaslangicHata.style.display = "none";
            }
        }

        if (istisnaTarihBitis && istisnaTarihBitisHata) {
            if (!istisnaTarihBitis.value) {
                istisnaTarihBitisHata.style.display = "block";
                Errors = true;
            } else {
                istisnaTarihBitisHata.style.display = "none";
            }
        }

        if (molaSaatBaslangic && molaSaatBaslangicHata) {
            if (!molaSaatBaslangic.value) {
                molaSaatBaslangicHata.style.display = "block";
                Errors = true;
            } else {
                molaSaatBaslangicHata.style.display = "none";
            }
        }

        if (molaSaatBitis && molaSaatBitisHata) {
            if (!molaSaatBitis.value) {
                molaSaatBitisHata.style.display = "block";
                Errors = true;
            } else {
                molaSaatBitisHata.style.display = "none";
            }
        }
    }

    // =====================
    // Rezerve Alanları
    // =====================
    const secenek1 = document.getElementById("HaftaIci");
    const secenek2 = document.getElementById("HaftaSonu");
    const secenek3 = document.getElementById("TamHafta");
    const secenekHata = document.getElementById("RezerveGunTipiHata");

    const baslangic = document.getElementById("rezerveSaatBaslangic");
    const baslangicHata = document.getElementById("rezerveSaatBaslangicHata");

    const bitis = document.getElementById("rezerveSaatBitis");
    const bitisHata = document.getElementById("rezerveSaatBitisHata");

    if (!baslangic.value || baslangic.value === "0") {
        baslangicHata.style.display = "block";
        Errors = true;
    } else {
        baslangicHata.style.display = "none";
    }

    if (!bitis.value || bitis.value === "0") {
        bitisHata.style.display = "block";
        Errors = true;
    } else {
        bitisHata.style.display = "none";
    }

    if (!secenek1.checked && !secenek2.checked && !secenek3.checked) {
        secenekHata.style.display = "block";
        Errors = true;
    } else {
        secenekHata.style.display = "none";
    }

    //====================
    // Kapasite
    //====================
    const kapasite = document.getElementById("kaynakKapasite");
    const kapasiteHata = document.getElementById("kaynakKapasiteHata");

    if (!kapasite.value || kapasite.value === "0") {
        kapasiteHata.style.display = "block";
        Errors = true;
    }
    else {
        kapasiteHata.style.display = "none";
    }


    return !Errors;
}


function KaynakKaydet() {
    const loader = document.getElementById("loader");

    var hataVar = HataUyarilari();

    if (hataVar) {
        loader.style.display = "block"
        KaynakRezerveKaydet();
    }
    Errors = false;
}

function KaynakRezerveKaydet() {
    var model = VerileriModeleBas();
    console.log(JSON.stringify(model, null, 2));
    $.ajax({
        type: "POST",
        async: false,
        url: "/KaynakRezerve/KaynakRezerveKayit",
        dataType: "JSON",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (result) {
            if (result.isSuccess == true) {
                var d = result.value;
                window.location.href = "/KaynakRezerve/KaynakRezerveListesi";
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (xhr, status, error) {
            console.log("Status: " + status);
            console.log("Error: " + error);
            console.log("Response: ", xhr.responseText);
        }
    });
}

function VerileriModeleBas() {
    var KaynakRezerveVM = {};

    let saatBaslangic = $("#rezerveSaatBaslangic").val();  // "12:00"
    let saatBitis = $("#rezerveSaatBitis").val();          // "18:30"

    // İstersen saniye yoksa ekle (opsiyonel)
    if (saatBaslangic.length === 5) saatBaslangic += ":00";
    if (saatBitis.length === 5) saatBitis += ":00";

    let GunCheck = 0;
    if ($("#HaftaIci").is(':checked')) GunCheck = 1;
    if ($("#HaftaSonu").is(':checked')) GunCheck = 2;
    if ($("#TamHafta").is(':checked')) GunCheck = 3;

    // KAYNAK TANIM
    KaynakRezerveVM["KaynakTanimVM"] = {
        KaynakAdi: $("#kaynakTanim").val(),
        KaynakAciklama: $("#kaynakAciklama").val(),
        ParamKaynakTipiID: parseInt($("#kaynakTipi").val()) || null
    };

    // REZERVE TANIM
    KaynakRezerveVM["KaynakRezerveTanimVM"] = {
        RezerveSaatBaslangicDegeri: saatBaslangic,
        RezerveSaatBitisDegeri: saatBitis,
        UygunGunTipleri: GunCheck
    };

    //KAPASITE
    KaynakRezerveVM["KapsiteVM"] = [];
    KaynakRezerveVM["KapsiteVM"].push({
        Kapasaite: parseInt($("#kaynakKapasite").val()) || null,
        KapasiteBirimID: parseInt($("#kaynakTipi").val()) || null
    });

    // MEDYA
    KaynakRezerveVM["MedyaVM"] = [];
    $('#tekrarliMedya > div').each(function () {
        let container = $(this);
        let idSuffix = container.find('input[type="file"]').attr("id").replace("dosyaEkle", "");

        // doğru isimlerle propery set
        KaynakRezerveVM["MedyaVM"].push({
            ResimID: parseInt($("#dosyaPreview" + idSuffix).attr("data-media-id")) || 0,
            TanimID: 0 // eğer boşsa, 0 gönder
        });
    });

    KaynakRezerveVM["KaynakIstisnalariVM"] = [];
    $('#tekrarliIstisna > div').each(function () {
        let container = $(this);
        let idSuffix = container.find('input[type="date"]').attr("id").replace("istisnaTarihBaslangic", "");

        KaynakRezerveVM["KaynakIstisnalariVM"].push({
            IstisnaSaatBaslangicDegeri: $(`#molaSaatBaslangic${idSuffix}`).val() || null,
            IstisnaSaatBitisDegeri: $(`#molaSaatBitis${idSuffix}`).val() || null,
            IstisnaTarihBaslangicDegeri: $(`#istisnaTarihBaslangic${idSuffix}`).val() || null,
            IstisnaTarihtBitisDegeri: $(`#istisnaTarihBitis${idSuffix}`).val() || null
        });
    });

    console.log(KaynakRezerveVM);
    return KaynakRezerveVM;
}
var tekrarliIstisnaId = -1;
function TekrarliIstisna() {
    let showRemove = tekrarliIstisnaId !== 0;

    let IstisnaHtml = `
    <label class="Lbl">Mola Saatleri / Tatil Günleri</label>
    <div class="row">
        <div class="col-md-3">
            <div class="form-group">
                <label class="Lbl">Başlangıç Tarihi</label>
                <input class="form-control" type="date" id="istisnaTarihBaslangic${tekrarliIstisnaId}" required>
                <label for="istisnaTarihBaslangic${tekrarliIstisnaId}" id="istisnaTarihBaslangicHata${tekrarliIstisnaId}" class="validationMessage">Başlangıç Tarihi Seçiniz</label>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-group">
                <label class="Lbl">Bitiş Tarihi</label>
                <input class="form-control" type="date" id="istisnaTarihBitis${tekrarliIstisnaId}" required>
                <label for="istisnaTarihBitis${tekrarliIstisnaId}" id="istisnaTarihBitisHata${tekrarliIstisnaId}" class="validationMessage">Bitiş Tarihi Seçiniz</label>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-group">
                <label class="Lbl">Mola Başlangıç Saati</label>
                <input class="form-control" type="time" id="molaSaatBaslangic${tekrarliIstisnaId}" required>
                <label for="molaSaatBaslangic${tekrarliIstisnaId}" id="molaSaatBaslangicHata${tekrarliIstisnaId}" class="validationMessage">Mola Saati Seçiniz</label>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-group">
                <label class="Lbl">Mola Bitiş Saati</label>
                <input class="form-control" type="time" id="molaSaatBitis${tekrarliIstisnaId}" required>
                <label for="molaSaatBitis${tekrarliIstisnaId}" id="molaSaatBitisHata${tekrarliIstisnaId}" class="validationMessage">Mola Saati Seçiniz</label>
            </div>
        </div>
        <div class="d-flex justify-content-end">
            <button id="addSpot" onclick="IstisnaEkle(this)" class="btn mb-0" data-accordion-id="{datacordion}">+ Yeni Mola Ekle</button>
            <div class="d-flex align-items-center MedyaRemoveButonWrapper" style="display: ${showRemove ? 'flex' : 'none'}">
                <div class="MedyaRemoveButonGrp">
                    <button type="button" class="btn btn-xs MedyaRemoveButton" onclick="IstisnaSil(this)">
                        <i class="fas fa-minus-circle accent-danger"></i>
                    </button>
                </div>
            </div>
        </div>
    </div>
    `;

    return IstisnaHtml;
}


function IstisnaEkle() {
    tekrarliIstisnaId++;
    let html = TekrarliIstisna();
    $("#tekrarliIstisna").append($(html));
}
function IstisnaSil(button) {
    // Eğer sadece 1 tane Medya kaldıysa silme
    if ($("#tekrarliIstisna .row").length <= 1) return;

    // Butona basıldığı satırı sil
    $(button).closest(".row").remove();
    IstisnaIdleriGuncelle();
}
function IstisnaIdleriGuncelle() {
    $("#tekrarliIstisna .row").each(function (index) {
        // Her .row içindeki input'u bul
        let input = $(this).find('input.form-control');
        if (input.length) {
            // input id'sini Medya + index şeklinde güncelle
            input.attr('id', 'Istisna' + index);
        }
    });
}