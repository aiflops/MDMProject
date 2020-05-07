/// <reference path="../typings/_references.ts" />

const MAP_CONTAINER_ID = 'mapID';
const GEOCODE_API = "https:\//nominatim.openstreetmap.org/search?q=#postcode,Poland&accept-language=pl&format=json";
const MAP_LAYER_URL = "https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw";
const INIT_MAP_BOUNDARIES = [51.643078, 19.609658];
const POINT_VIEW_ZOOM = 19;
const POST_CODE_ZOOM = 13;
const POST_CODE_DISTANCE = 20; // Distance for post code search in kilometers

let mapMdm: Map;
let viewModel: MapViewModel;

class Map {
    map: any;
    markerCluster: any;

    constructor() {
        this.map = L
            .map(MAP_CONTAINER_ID)
            .on('moveend', this.moveMap)
            .setView(INIT_MAP_BOUNDARIES, 6);

        L.tileLayer(MAP_LAYER_URL, {
            maxZoom: 25,
            id: 'mapbox/streets-v11',
            tileSize: 512,
            zoomOffset: -1
        }).addTo(this.map);

        this.markerCluster = L.markerClusterGroup({
            chunkedLoading: true,
            spiderfyOnMaxZoom: false,
            disableClusteringAtZoom: 22,
            showCoverageOnHover: false,
            removeOutsideVisibleBounds: true,
            polygonOptions: null,
        });
        this.map.addLayer(this.markerCluster);
    }

    setViewToPoint(point: any, zoom: number): void {
        this.map.setView(point, zoom);
    }

    addMarker(marker: Marker): void {
        this.markerCluster.addLayer(marker.mapMarker);
    }

    clearAllMarkers(): void {
        this.markerCluster.clearLayers();
    }

    moveMap(): void {
        let bounds = (<any>this).getBounds();
        viewModel.updateVisibleItems(bounds);
    }

    getBounds(): any {
        return this.map.getBounds();
    }
}

class Marker {
    id: number;
    point: any;
    mapMarker: any;

    constructor(id: number, point: any, icon: any) {
        this.id = id;
        this.point = point;

        this.mapMarker = L
            .marker(point, { icon: icon })
            .on('click', this.onClick);
    }

    onClick(): void {
        mapMdm.setViewToPoint((<any>this).getLatLng(), POINT_VIEW_ZOOM);
    }

    static iconMask = L.icon({
        iconUrl: '/Content/images/mask.png',
        iconSize: [40, 40],
        iconAnchor: [20, 20],
    });

    static iconGreen = L.icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
    });
}

class MapViewModel {
    postCode: KnockoutObservable<string>;
    isPostCodeIncorrect: KnockoutObservable<boolean>;

    lastPostCodeSearch: KnockoutObservable<string>;
    lastPostCodePoint: KnockoutObservable<any>;

    items: KnockoutObservableArray<MapEntryItem>;
    isDataLoaded: KnockoutObservable<boolean>;
    visibleItems: KnockoutObservable<MapEntryItem[]>;

    constructor() {
        this.postCode = ko.observable();
        this.isPostCodeIncorrect = ko.observable(false);

        this.lastPostCodeSearch = ko.observable();
        this.lastPostCodePoint = ko.observable();

        this.items = ko.observableArray();
        this.isDataLoaded = ko.observable(false);

        this.visibleItems = ko.observable([]);
    }

    loadData(): void {
        $.getJSON('/api/suppliers', (data) => {
            let dataItems = ko.utils.arrayMap(data || [], item => new MapEntryItem(item));
            ko.utils.arrayPushAll(this.items, dataItems);

            this.addAllMarkers();
            let mapBounds = mapMdm.getBounds();
            this.updateVisibleItems(mapBounds);
        });
    }

    searchByPostCode(): void {
        let api_url = GEOCODE_API.replace('#postcode', this.postCode());

        // Reset warning message
        this.isPostCodeIncorrect(false);

        // if post code has been reset
        if (utils.isEmptyOrSpaces(this.postCode())) {
            this.lastPostCodeSearch(null);
            this.lastPostCodePoint(null);

            let mapBounds = mapMdm.getBounds();
            this.updateVisibleItems(mapBounds);
        }
        // if post code has been provided and is valid
        else if (this.isPostCodeValid()) {
            $.getJSON(api_url, (value) => {
                let firstResult = value[0];
                let point = L.latLng(firstResult.lat, firstResult.lon);

                this.lastPostCodeSearch(this.postCode());
                this.lastPostCodePoint(point);

                let mapBounds = mapMdm.getBounds();
                this.updateVisibleItems(mapBounds);
                mapMdm.setViewToPoint(point, POST_CODE_ZOOM);
            }).fail(() => alert('Błąd! Wystąpił błąd podczas pobierania danych...'));
        }
        else {
            this.isPostCodeIncorrect(true);
        }
    }

    isPostCodeValid(): boolean {
        return this.postCode().match(/\d{2}-\d{3}/) != null;
    }

    updateVisibleItems(mapBounds: any): void {
        let newVisibleItems = [];
        let searchByPostCode = false;
        let showAll = false;

        let isMapBoundsSinglePoint = mapBounds._northEast.equals(mapBounds._southWest);

        // If map is only one point - we are on mobile and map is not visible
        if (isMapBoundsSinglePoint) {
            if (viewModel.lastPostCodePoint() != null) {
                searchByPostCode = true;
            }
            // If no post code provided return all
            else {
                showAll = true;
            }
        }

        // If search by post code
        if (searchByPostCode) {
            // select only those items, which have distance in correct bounds
            ko.utils.arrayForEach(this.items(), item => {
                item.updateDistanceToPostCode(viewModel.lastPostCodePoint());

                let isVisible = item.distanceToPostCode() < (POST_CODE_DISTANCE * 1000);
                item.isVisible(isVisible);

                if (isVisible)
                    newVisibleItems.push(item);
            });

            // sort items by distance ascending
            newVisibleItems.sort(function (a, b) {
                return a.distanceToPostCode() - b.distanceToPostCode();
            });
        }
        // show all if is mobile version or show visible on map if map is available
        else {
            ko.utils.arrayForEach(this.items(), item => {
                let isVisible = showAll || mapBounds.contains(item.point);
                item.isVisible(isVisible);

                if (isVisible)
                    newVisibleItems.push(item);
            });
        }

        this.visibleItems(newVisibleItems);
    }

    addAllMarkers(): void {
        mapMdm.clearAllMarkers();

        ko.utils.arrayForEach(this.items(), item => {
            mapMdm.addMarker(item.marker);
        });
    }

    onKeyPress(data: any, event: KeyboardEvent): boolean {
        var keyCode = (event.which ? event.which : event.keyCode);
        if (keyCode === 13) {
            viewModel.searchByPostCode();
        }

        return true;
    }
}

//#region ViewModel types
class MapEntryItem {
    id: number;
    name: string;
    city: string;
    latitude: number;
    longitude: number;
    point: any;
    marker: Marker;

    isVisible: KnockoutObservable<boolean>;
    distanceToPostCode: KnockoutObservable<number>;

    constructor(model: any) {
        this.id = model.Id;
        this.name = model.Name;
        this.city = model.City;
        this.latitude = parseFloat(model.Latitude);
        this.longitude = parseFloat(model.Longitude);
        this.point = L.latLng(this.latitude, this.longitude);

        this.marker = new Marker(this.id, this.point, Marker.iconGreen);
        this.isVisible = ko.observable(false);
        this.distanceToPostCode = ko.observable<number>();
    }

    updateDistanceToPostCode(postCodeCenterPoint: any): void {
        if (postCodeCenterPoint != null) {
            let distance = this.point.distanceTo(postCodeCenterPoint);
            this.distanceToPostCode(distance);
        }
        else {
            this.distanceToPostCode(null);
        }
    }

    listItemClick(): void {
        mapMdm.setViewToPoint(this.point, POINT_VIEW_ZOOM);
    }
}

class MapEntryItemDetails {
    id: KnockoutObservable<number>;
    userType: KnockoutObservable<UserTypeEnum>;
    companyName: KnockoutObservable<string>;
    personName: KnockoutObservable<string>;
    email: KnockoutObservable<string>;
    phoneNumber: KnockoutObservable<string>;
    address: KnockoutObservable<MapEntryAddress>;

    constructor(model: any) {
        this.id = ko.observable(model.Id);
        this.userType = ko.observable(model.UserType);
        this.companyName = ko.observable(model.CompanyName);
        this.personName = ko.observable(model.PersonName);
        this.email = ko.observable(model.Email);
        this.phoneNumber = ko.observable(model.PhoneNumber);
        this.address = ko.observable(model.Address != null ? new MapEntryAddress(model.Address) : null);
    }
}

class MapEntryAddress {
    city: KnockoutObservable<string>;
    streetName: KnockoutObservable<string>;
    houseNumber: KnockoutObservable<string>;
    flatNumber: KnockoutObservable<string>;
    postalCode: KnockoutObservable<string>;
    latitude: KnockoutObservable<number>;
    longitude: KnockoutObservable<number>;

    constructor(model: any) {
        this.city = ko.observable(model.City);
        this.streetName = ko.observable(model.StreetName);
        this.houseNumber = ko.observable(model.HouseNumber);
        this.flatNumber = ko.observable(model.FlatNumber);
        this.postalCode = ko.observable(model.PostalCode);
        this.latitude = ko.observable(parseFloat(model.Latitude));
        this.longitude = ko.observable(parseFloat(model.Longitude));
    }
}

const enum UserTypeEnum {
    Company,
    Individual
}

//#endregion

$(document).ready(() => {
    viewModel = new MapViewModel();
    ko.applyBindings(viewModel);

    mapMdm = new Map();

    viewModel.loadData();
});