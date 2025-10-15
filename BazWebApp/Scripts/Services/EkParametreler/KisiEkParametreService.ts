
class KisiEkParametreler extends kendo.data.ObservableObject {

    _repository: Repository;

    constructor(value?: any) {
        super(value);
        super.init(this);
        this._repository = new Repository(ServiceConfiguration.CreateDefault(""));

    }

    data = new KisiEkParametreler({
        ParametreAdi: "",
        ParametreTipi: "",
        ParametreGosterimTipi:""
    });





    GetURLParameter() {
        var sPageURL = window.location.href;
        var indexOfLastSlash = sPageURL.lastIndexOf("/");

        if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
            return parseInt(sPageURL.substring(indexOfLastSlash + 1));
        else
            return 0;
    }
    static initialize() {
        kisiEkParametreler = new KisiEkParametreler();
        /*kisiEkParametreler.List();*/
        var id;
        if (id) {
          
        }
        kendo.bind($(".model-bind"), kisiEkParametreler);
    }
}

var kisiEkParametreler: KisiEkParametreler;

$(document).ready(function () {
    KisiEkParametreler.initialize();
})