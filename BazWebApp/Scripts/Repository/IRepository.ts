class Repository {
    constructor(serviceConfiguration?: ServiceConfiguration) {
        this._serviceConfiguration = serviceConfiguration;
        if (this._serviceConfiguration) {
            if (this._serviceConfiguration.coreUrl)
                this.coreUrl = this._serviceConfiguration.coreUrl;
        }
    }
    _serviceConfiguration: ServiceConfiguration;
    coreUrl: string
    add(entity: kendo.data.Model) {
        this.postData(entity.toJSON(), this._serviceConfiguration.Create)
    }
    update(entity: kendo.data.Model) {
        this.postData(entity.toJSON(), this._serviceConfiguration.Update)
    }
    remove(entity: kendo.data.Model) {
        this.postData(entity.toJSON(), this._serviceConfiguration.Remove)
    }
    single(id: number) {
        //verilen id nin nesnesini sorgular. 
        this._serviceConfiguration.Single.url += "/" + id;
        this.postData(null, this._serviceConfiguration.Single)
    }
    postData(data: any, configuration?: AjaxConfiguration) {
        if (!configuration || !configuration.url)
            return;
        if (!configuration.type)
            configuration.type = AjaxType.POST;
        if (!configuration.dataType)
            configuration.dataType = "json";
        var c = {
            type: configuration.type.toString(),
            dataType: configuration.dataType,
            data: data,
            url: configuration.url,
            async: false,
            success: function (result) {
                if (configuration.success) {
                    configuration.success(result);
                }
            },
            error: function (error) {
                if (configuration.error) {
                    configuration.error(error);
                }
            }
        };
        if (configuration.contentType)
            c["contentType"] = configuration.contentType;
        $.ajax(c);
    }
    getData(data: any, configuration?: AjaxConfiguration) {
        if (!configuration || !configuration.url)
            return;
        if (!configuration.type)
            configuration.type = AjaxType.GET;
        if (!configuration.dataType)
            configuration.dataType = "json";
        $.ajax({
            type: configuration.type.toString(),
            dataType: configuration.dataType,
            contentType: configuration.contentType,
            data: data,
            url: configuration.url,
            async: false,
            success: function (result) {
                if (configuration.success) {
                    configuration.success(result);
                }
            },
            error: function (error) {
                if (configuration.error) {
                    configuration.error(error);
                }
            }
        })
    }
}
class ServiceConfiguration {
    Create: AjaxConfiguration;
    Update: AjaxConfiguration;
    Remove: AjaxConfiguration;
    Single: AjaxConfiguration;
    List: AjaxConfiguration;
    coreUrl: string;
    static CreateDefault(coreUrl: string): ServiceConfiguration {

        var config = new ServiceConfiguration();
        config.coreUrl = coreUrl;
        config.Create = AjaxConfiguration.postDefault();
        config.Update = AjaxConfiguration.postDefault();
        config.Remove = AjaxConfiguration.postDefault();
        config.Single = AjaxConfiguration.getDafault();
        config.List = AjaxConfiguration.getDafault();
        config.Create.url = coreUrl + "/add";
        config.Update.url = coreUrl + "/update";
        config.Remove.url = coreUrl + "/remove";
        config.List.url = coreUrl + "/list";
        config.Single.url = coreUrl + "/single";
        return config;
    }
}
class AjaxConfiguration {
    type?: AjaxType;
    contentType?: string;
    dataType?: string;
    url?: string;
    success?: any;
    error?: any;
    async: boolean;
    constructor() { }
    static getDafault(): AjaxConfiguration {
        var item = new AjaxConfiguration();
        item.type = AjaxType.GET;
        item.dataType = "json";
        item.async = true;
        return item;
    }
    static postDefault(): AjaxConfiguration {
        var item = new AjaxConfiguration();
        item.type = AjaxType.POST;
        item.dataType = "json";
        item.async = true;
        return item;
    }
}
enum AjaxType {
    GET = 'GET',
    POST = 'POST'
}

