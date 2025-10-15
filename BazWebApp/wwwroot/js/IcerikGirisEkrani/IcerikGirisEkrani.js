var IcerikKategoriList = [];
var TekrarliSpot;

var IcerikModelVM = {};
var quill;
$(document).ready(function () {
    SpotEkle();
    MedyaEkle();
    IcerikAnaKategorileriGetir();
    //editor ekleme
    var toolbarOptions = [
        ['bold', 'italic', 'underline'],        // kalın, italik, altı çizili
        [{ 'header': 1 }, { 'header': 2 }],     // başlık 1 ve 2
        [{ 'list': 'ordered' }, { 'list': 'bullet' }]  // sıralı ve sırasız liste
    ];

    quill = new Quill('#editor_1', {
        theme: 'snow',
        modules: {
            toolbar: toolbarOptions
        }
    });
});

function IcerikAnaKategorileriGetir() {
    $.ajax({
        type: "GET",
        url: "/IcerikGiris/IcerikKategorileriGetir/",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                IcerikKategoriList = result.value;
                var anakategori = IcerikKategoriList.filter(x => x.ustId == 0);
                ParamDDLDoldur("AnaKategori", anakategori);
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}



function IcerikKategorileriGetir(value) {
    var altkategoriler = IcerikKategoriList.filter(x => x.ustId == value);
    ParamDDLDoldur("Kategori", altkategoriler);
}
function IcerikAltKategoriGetir(value) {
    var altkategoriler = IcerikKategoriList.filter(x => x.ustId == value);
    ParamDDLDoldur("Alt-Kategori", altkategoriler);
}
//Tekrarlı alan spot ekleme
var tekrarliSpotId = -1;

function TekrarliSpotEkle() {
    let showRemove = tekrarliSpotId !== 0; // ilk spot için false

    let spotHtml = '<div class="row">\
        <div class="col-md-6">\
            <div class="form-group">\
                <label class="Lbl">Spot</label>\
                <input class="form-control" type="text" id="Spot'+ tekrarliSpotId + '" required>\
                <label for="spot" id="spotHata'+ tekrarliSpotId + '" class="validationMessage">Spot Giriniz</label>\
            </div>\
        </div>\
        <div class="d-flex">\
            <button onclick="SpotEkle()" class="btn mb-0">+ Yeni Spot Ekle</button>\
        </div>\
        <div class="d-flex align-items-center SpotRemoveButonWrapper" style="display:' + (showRemove ? 'flex' : 'none') + '">\
            <div class="SpotRemoveButonGrp">\
                <button type="button" class="btn btn-xs SpotRemoveButton" onclick="SpotSil(this)">\
                    <i class="fas fa-minus-circle accent-danger"></i>\
                </button>\
            </div>\
        </div>\
    </div>';

    return spotHtml;
}
function SpotEkle() {
    tekrarliSpotId++;
    let html = TekrarliSpotEkle();
    $("#tekrarliSpot").append($(html));
}
function SpotSil(button) {
    // Eğer sadece 1 tane spot kaldıysa silme
    if ($("#tekrarliSpot .row").length <= 1) return;

    // Butona basıldığı satırı sil
    $(button).closest(".row").remove();
    SpotIdleriGuncelle();
}
function SpotIdleriGuncelle() {
    $("#tekrarliSpot .row").each(function (index) {
        // Her .row içindeki input'u bul
        let input = $(this).find('input.form-control');
        if (input.length) {
            // input id'sini Spot + index şeklinde güncelle
            input.attr('id', 'Spot' + index);
        }
    });
}
var tekrarliMedyaId = -1;
function TekrarliMedyaEkle() {
    let showRemove = tekrarliMedyaId !== 0; // ilk spot için false
    let medyaHtml = '<div class="row">\
        <div class="col-md-4" >\
                <div class="form-group">\
                    <label class="Lbl">Medya Tipi</label>\
                    <select id="medyaTipi'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    </select>\
                    <label for="medyaTipi" id="MedyaTipiHata'+ tekrarliMedyaId + '" class="validationMessage">Medya Tipi Seçiniz</label>\
                </div>\
            </div>\
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
                    <label class="Lbl">Alt Metin</label>\
                    <input id="altMetin'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    <label for="altMetin" id="altMetinHata'+ tekrarliMedyaId + '" class="validationMessage">Alt Metin Seçiniz</label>\
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
    var medyaTipList = ParamMedyaTipleriGetir();
    var str = "<option value='0'>Seçiniz</option>";
    for (var i of medyaTipList) {
        var tanim = i.title == undefined ? i.tanim : i.title;
        str += "<option value='" + i.id + "'>" + i.title + "</option>";
    }
    $("#medyaTipi" + tekrarliMedyaId).html(str);
    $('#medyaTipi' + tekrarliMedyaId).select2();
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
        $(this).find('#medyaTipi' + (index + 1)).attr('id', 'medyaTipi' + index);
        $(this).find('#medyaAdi' + (index + 1)).attr('id', 'medyaAdi' + index);
        $(this).find('#gosterimAdi' + (index + 1)).attr('id', 'gosterimAdi' + index);
        $(this).find('#sira' + (index + 1)).attr('id', 'sira' + index);
        $(this).find('#altMetin' + (index + 1)).attr('id', 'altMetin' + index);
        $(this).find('#dosyaEkle' + (index + 1)).attr('id', 'dosyaEkle' + index);
    });
}
function ParamMedyaTipleriGetir() {
    var data = {
        ModelName: "ParamMedyaTipleri",
        UstId: 0,
        KurumId: 0,
        TabloID: 0,
        Tanim: "test",
        DilID: 1,
        EsDilID: 0
    }
    var list = [];
    $.ajax({
        data: data,
        url: "/panel/param/listparam",
        type: "POST",
        dataType: "json",
        async: false,
        success: function (x) {
            for (var item of x.valueOrDefault) {
                list.push({
                    id: item.tabloID,
                    title: item.tanim,
                });
            }
            //licenseService.ParaBirimleriSayfayaBas(list);
        },
        error: function (e) {
            console.log(e);
        }
    })
    return list;
}
function HataUyarilari() {
    let Errors = false;

    // Ana Kategori
    const anaKategori = document.getElementById("AnaKategori");
    const anaKategoriHata = document.getElementById("AnaKategoriHata");
    if (!anaKategori.value || anaKategori.value === "0" || anaKategori.value === "") {
        anaKategoriHata.style.display = "block";
        Errors = true;
    } else {
        anaKategoriHata.style.display = "none";
    }

    // Kategori
    const kategori = document.getElementById("Kategori");
    const kategoriHata = document.getElementById("KategoriHata");
    if (!kategori.value || kategori.value === "0" || kategori.value === "") {
        kategoriHata.style.display = "block";
        Errors = true;
    } else {
        kategoriHata.style.display = "none";
    }

    // Alt Kategori
    const altKategori = document.getElementById("Alt-Kategori");
    const altKategoriHata = document.getElementById("Alt-KategoriHata");
    if (!altKategori.value || altKategori.value === "0" || altKategori.value === "") {
        altKategoriHata.style.display = "block";
        Errors = true;
    } else {
        altKategoriHata.style.display = "none";
    }
    // Başlık
    const baslik = document.getElementById("Baslik_1");
    const baslikHata = document.getElementById("validationMessageBaslik_1");
    if (!baslik.value.trim()) {
        baslikHata.style.display = "block";
        Errors = true;
    } else {
        baslikHata.style.display = "none";
    }

    // Quill editörü (Metin)
    const editorText = quill.root.innerHTML;
    const metinHata = document.getElementById("validationMessageAnaMetin_1");
    if (!editorText || editorText === "") {
        metinHata.style.display = "block";
        Errors = true;
    } else {
        metinHata.style.display = "none";
    }

    // Yayınlanma Tarihi
    const yayinlanmaTarihi = document.getElementById("yayinlanmaTarihi");
    const yayinlanmaHata = document.getElementById("yayinlanmaTarihiHata");

    if (!yayinlanmaTarihi.value) {
        yayinlanmaHata.style.display = "block";
        Errors = true;
    } else {
        yayinlanmaHata.style.display = "none";

    }

    // Kaldırılma Tarihi
    const kaldirilmaTarihi = document.getElementById("KaldirilmaTarihi");
    const kaldirilmaHata = document.getElementById("KaldirilmaTarihiHata");

    if (!kaldirilmaTarihi.value) {
        kaldirilmaHata.style.display = "block";
        Errors = true;
    } else {
        kaldirilmaHata.style.display = "none";

    }

    // ================================
    // Spot Alanları (Tekrarlı)
    // ================================
    const spotInputs = document.querySelectorAll('[id^="Spot"]');
    spotInputs.forEach(input => {
        // Spot ID'sinden numarayı ayıkla (örnek: Spot0 -> 0)
        const inputId = input.id; // Örn: Spot0
        const index = inputId.replace("Spot", ""); // "0", "1" gibi
        const hataLabel = document.getElementById("spotHata" + index); // spotHata0, spotHata1...

        if (!input.value.trim()) {
            if (hataLabel) {
                hataLabel.style.display = "block";
            }
            Errors = true;
        } else {
            if (hataLabel) {
                hataLabel.style.display = "none";
            }
        }
    });

    // ================================
    // Medya Alanları (Tekrarlı)
    // ================================
    const medyaCount = document.querySelectorAll('[id^="medyaTipi"]').length;

    for (let i = 0; i < medyaCount; i++) {
        const medyaTipi = document.getElementById(`medyaTipi${i}`);
        const medyaAdi = document.getElementById(`medyaAdi${i}`);
        const gosterimAdi = document.getElementById(`gosterimAdi${i}`);
        const sira = document.getElementById(`sira${i}`);
        const altMetin = document.getElementById(`altMetin${i}`);
        const dosyaEkle = document.getElementById(`dosyaEkle${i}`);

        const medyaTipiHata = document.getElementById(`MedyaTipiHata${i}`);
        const medyaAdiHata = document.getElementById(`MedyaAdiHata${i}`);
        const gosterimAdiHata = document.getElementById(`gosterimAdiHata${i}`);
        const siraHata = document.getElementById(`siraHata${i}`);
        const altMetinHata = document.getElementById(`altMetinHata${i}`);
        const dosyaEkleHata = document.getElementById(`dosyaEkleHata${i}`);

        if (!medyaTipi || !medyaAdi || !gosterimAdi || !sira || !altMetin || !dosyaEkle) continue;

        if (medyaTipi.value === "0" || medyaTipi.value === "") {
            if (medyaTipiHata) medyaTipiHata.style.display = "block";
            Errors = true;
        } else {
            if (medyaTipiHata) medyaTipiHata.style.display = "none";
        }

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

        if (!altMetin.value.trim()) {
            if (altMetinHata) altMetinHata.style.display = "block";
            Errors = true;
        } else {
            if (altMetinHata) altMetinHata.style.display = "none";
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
function IcerikKaydet() {
    var hataVar = HataUyarilari();
    if (hataVar) {
        IcerikModeliKaydet();
    }
    Errors = false;
}
function VerileriModeleBas() {
    let IcerikModelVM = {};

    // Temel Alanlar
    IcerikModelVM["ParamAnaKategoriID"] = $("#AnaKategori").val();
    IcerikModelVM["ParamIcerikKategoriID"] = $("#Kategori").val();
    IcerikModelVM["ParamAltKategoriID"] = $("#Alt-Kategori").val();
    IcerikModelVM["IcerikBaslik"] = $("#Baslik_1").val();
    IcerikModelVM["IcerikTamMetin"] = quill && quill ? quill.root.innerHTML : "";

    IcerikModelVM["IcerikYayinlanmaZamani"] = $("#yayinlanmaTarihi").val();
    IcerikModelVM["IcerikKaldirilmaZamani"] = $("#KaldirilmaTarihi").val();
    IcerikModelVM["TaslakMi"] = $("#TaslakMiCheckbox").is(":checked");

    // Spotlar (Tekrarli Spot)
    IcerikModelVM["IcerikSpot"] = [];
    $('#tekrarliSpot input[id^="Spot"]').each(function () {
        IcerikModelVM["IcerikSpot"].push({ SpotMetni: $(this).val() });
    });
    // Medyalar (Tekrarli Medya)
    IcerikModelVM["Medyalar"] = [];
    $('#tekrarliMedya > div').each(function () {
        let container = $(this);
        let idSuffix = container.find('input[type="file"]').attr("id").replace("dosyaEkle", "");

        IcerikModelVM["Medyalar"].push({
            ParamMedyaTipiID: $("#medyaTipi" + idSuffix).val(),
            MedyaAdi: $("#medyaAdi" + idSuffix).val(),
            GosterimAdi: $("#gosterimAdi" + idSuffix).val(),
            SiraNo: $("#sira" + idSuffix).val(),
            AltMetin: $("#altMetin" + idSuffix).val(),
            MedyaID: $("#dosyaPreview" + idSuffix).attr("data-media-id") || null,
            MedyaAciklama: $("#medyaAciklama" + idSuffix).val(), //string?
        });
    });
    console.log(IcerikModelVM);
    return IcerikModelVM;
}
function IcerikModeliKaydet() {
    var model = VerileriModeleBas();
    $.ajax({
        type: "POST",
        async: false,
        url: "/IcerikGiris/IcerikKaydet",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (result) {
            if (result.isSuccess == true) {
                var d = result.value;
                window.location.href = "/IcerikGiris/IcerikListelemeEkrani"
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
