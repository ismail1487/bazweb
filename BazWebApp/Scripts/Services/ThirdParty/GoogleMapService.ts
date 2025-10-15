class GoogleMapService extends kendo.data.ObservableObject {
    constructor(value?: any) {
        super(value);
        this.init(value);
    }
    latitude: number;
    longitude: number;
    map: google.maps.Map;
    infoWindow: google.maps.InfoWindow;
    center: google.maps.LatLngLiteral;
    initalize() {
        var lat = parseFloat($(".googlemap").data("lat"));
        var long = parseFloat($(".googlemap").data("long"));
        this.set("latitude", lat);
        this.set("longitude", long);
        kendo.bind($(".googlemap"), googleMapService);
        this.initMap();
    }
    initMap() {
        this.center = { lat: this.latitude, lng: this.longitude };
        this.map = new google.maps.Map(document.getElementById("map") as HTMLElement, {
            center: this.center,
            zoom: 15
        });
        this.infoWindow = new google.maps.InfoWindow({ content: "", position: this.center });
        this.infoWindow.open(this.map);
        this.map.addListener("click", (mapsMouseEvent) => {
            googleMapService.latitude = mapsMouseEvent.latLng.toJSON().lat;
            googleMapService.longitude = mapsMouseEvent.latLng.toJSON().lng;
            googleMapService.infoWindow.close();
            googleMapService.infoWindow = new google.maps.InfoWindow({
                position: mapsMouseEvent.latLng,
            });
            googleMapService.infoWindow.setContent(
                JSON.stringify(mapsMouseEvent.latLng.toJSON(), null, 2)
            );
            googleMapService.infoWindow.open(googleMapService.map);
        })
    }
}


var googleMapService: GoogleMapService;
$(document).ready(function () {
    if ($(".googlemap").length > 0) {
        googleMapService = new GoogleMapService({ latitude: 0, longitude: 0 });
        googleMapService.initalize();
    }
})