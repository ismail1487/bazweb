class IcerikKurumsalSablonTanimlariModel extends kendo.data.Model {
    constructor(value?: any) {
        super(value)
    }
    fields: {
        TabloID: {
            type: "number",
            defaultValue: 0
        },
        IcerikTanim: {
            type: "string",
            defaultValue: ""
        },
        SablonIcerikTipiId: {
            type: "number",
            defaultValue: 0
        },
        IcerikBaslik: {
            type: "string",
            defaultValue: ""
        },
        IcerikTamMetin: {
            type: "string",
            defaultValue: ""
        },
        IcerikGorselMedyaId: {
            type: "number",
            defaultValue: 0
        },
        IcerikRenkKodu: {
            type: "string",
            defaultValue: ""
        },
        SistemMi: {
            type: "boolean",
            defaultValue: false
        },
        GonderimTipi: {
            type: "string",
            defaultValue: ""
        }
    }
}