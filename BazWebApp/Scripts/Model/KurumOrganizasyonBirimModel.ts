
class KurumOrganizasyonBirimModel extends kendo.data.Model{
    constructor(value?: any) {
        super(value)
    }

    fields = {
        Tanim: {
            type: "string",
            defaultValue: "",
            validation: {
                required: true
            }
        },
        TipId: {
            type: "number",
            defaultValue: 8
        },
        TabloId: {
            type: "number",
            defaultValue: 0
        },
        KurumId: {
            type: "number",
            defaultValue: 1
        }
    }
}