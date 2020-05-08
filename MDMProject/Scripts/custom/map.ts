/// <reference path="../typings/_references.ts" />

const MAP_CONTAINER_ID = 'mapID';
const GEOCODE_API = "https:\//nominatim.openstreetmap.org/search?q=#postcode,Poland&accept-language=pl&format=json";
const MAP_LAYER_URL = "https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw";
const INIT_MAP_BOUNDARIES = [51.643078, 19.609658];
const POINT_VIEW_ZOOM = 19;
const POST_CODE_ZOOM = 15;
const POST_CODE_DISTANCE = 20; // Distance for post code search in kilometers

let mapMdm: Map;
let viewModel: MapViewModel;

class Map {
    map: any;
    markerCluster: any;
    postCodeMarkerCluster: any;

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

        this.postCodeMarkerCluster = L.markerClusterGroup({
            chunkedLoading: true,
            spiderfyOnMaxZoom: false,
            disableClusteringAtZoom: 22,
            showCoverageOnHover: false,
            removeOutsideVisibleBounds: true,
            polygonOptions: null,
        });
        this.map.addLayer(this.postCodeMarkerCluster);
    }

    setViewToPoint(point: any, zoom: number): void {
        this.map.setView(point, zoom);
    }

    addMarker(marker: Marker): void {
        if (!marker.isPostCodeMarker)
            this.markerCluster.addLayer(marker.mapMarker);
        else
            this.postCodeMarkerCluster.addLayer(marker.mapMarker);
    }

    removeMarker(marker: Marker): void {
        marker.mapMarker.remove()
    }

    clearAllMarkers(): void {
        this.markerCluster.clearLayers();
        this.postCodeMarkerCluster.clearLayers();
    }

    clearPostCodeMarkers(): void {
        this.postCodeMarkerCluster.clearLayers();
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
    isPostCodeMarker: boolean;

    constructor(id: number, point: any, icon: any, isPostCodeMarker: boolean = false) {
        this.id = id;
        this.point = point;
        this.isPostCodeMarker = isPostCodeMarker;

        this.mapMarker = L
            .marker(point, { icon: icon })
            .on('click', this.onClick);
    }

    onClick(): void {
        let leafletMarker = (<any>this);
        mapMdm.setViewToPoint(leafletMarker.getLatLng(), POINT_VIEW_ZOOM);

        if (leafletMarker._itemViewModel != null) {
            viewModel.activeItem(leafletMarker._itemViewModel);
        }
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

    static iconRed = L.icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
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

    activeItem: KnockoutObservable<MapEntryItem>;

    constructor() {
        this.postCode = ko.observable();
        this.isPostCodeIncorrect = ko.observable(false);

        this.lastPostCodeSearch = ko.observable();
        this.lastPostCodePoint = ko.observable();

        this.items = ko.observableArray();
        this.isDataLoaded = ko.observable(false);

        this.visibleItems = ko.observable([]);

        this.activeItem = ko.observable();

        this.activeItem.subscribe((newActiveItem) => {
            if (newActiveItem != null && newActiveItem.details() == null)
                newActiveItem.loadDetails();
        });
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
            this.activeItem(null);

            ko.utils.arrayForEach(this.items(), item => item.updateDistanceToPostCode(null));

            mapMdm.clearPostCodeMarkers();

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
                this.activeItem(null);

                ko.utils.arrayForEach(this.items(), item => item.updateDistanceToPostCode(this.lastPostCodePoint()));

                mapMdm.clearPostCodeMarkers();
                mapMdm.addMarker(new Marker(null, this.lastPostCodePoint(), Marker.iconRed, true));

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
                let isVisible = item.distanceToPostCode() < (POST_CODE_DISTANCE * 1000);
                item.isVisible(isVisible);

                if (isVisible)
                    newVisibleItems.push(item);
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

        // Sort items by:
        // 1. distance to post code (if provided)
        // 2. city name
        // 3. person name
        // 4. id
        newVisibleItems.sort(function (a, b) {
            if (a.distanceToPostCode() != b.distanceToPostCode()) {
                return a.distanceToPostCode() - b.distanceToPostCode();
            }
            else if (a.city != b.city) {
                return a.city.localeCompare(b.city);
            }
            else if (a.name != b.name) {
                return a.name.localeCompare(b.name);
            }
            else {
                return a.id - b.id;
            }
        });

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

    exitActiveItemDetails(): void {
        viewModel.activeItem(null);
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

    details: KnockoutObservable<MapEntryItemDetails>;

    isVisible: KnockoutObservable<boolean>;
    distanceToPostCode: KnockoutObservable<number>;

    distanceToPostCodeFormatted: KnockoutComputed<string>;

    constructor(model: any) {
        this.id = model.Id;
        this.name = model.Name;
        this.city = model.City;
        this.latitude = parseFloat(model.Latitude);
        this.longitude = parseFloat(model.Longitude);
        this.point = L.latLng(this.latitude, this.longitude);

        this.marker = new Marker(this.id, this.point, Marker.iconGreen);
        this.marker.mapMarker._itemViewModel = this;

        this.details = ko.observable();

        this.isVisible = ko.observable(false);
        this.distanceToPostCode = ko.observable();

        this.distanceToPostCodeFormatted = ko.computed(this.getDistanceToPostCodeFormatted.bind(this));
    }

    getDistanceToPostCodeFormatted(): string {
        if (this.distanceToPostCode() == null)
            return null;

        let distanceInKm = Math.round(this.distanceToPostCode() / 100) / 10;
        return distanceInKm + 'km';
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
        viewModel.activeItem(this);
    }

    loadDetails(): void {
        $.getJSON('/api/suppliers/' + this.id, (data) => {
            let detailsViewModel = new MapEntryItemDetails(data);
            this.details(detailsViewModel);
        });
    }
}

class MapEntryItemDetails {
    id: number;
    userType: UserTypeEnum;
    companyName: string;
    personName: string;
    email: string;
    phoneNumber: string;
    address: MapEntryAddress;

    constructor(model: any) {
        this.id = model.Id;
        this.userType = model.UserType;
        this.companyName = model.CompanyName;
        this.personName = model.PersonName;
        this.email = model.Email;
        this.phoneNumber = model.PhoneNumber;
        this.address = model.Address != null ? new MapEntryAddress(model.Address) : null;
    }
}

class MapEntryAddress {
    city: string;
    streetName: string;
    houseNumber: string;
    flatNumber: string;
    postalCode: string;
    latitude: number;
    longitude: number;

    streetFormatted: string;
    cityFormatted: string;

    constructor(model: any) {
        this.city = model.City;
        this.streetName = model.StreetName;
        this.houseNumber = model.HouseNumber;
        this.flatNumber = model.FlatNumber;
        this.postalCode = model.PostalCode;
        this.latitude = parseFloat(model.Latitude);
        this.longitude = parseFloat(model.Longitude);

        this.streetFormatted = (this.streetName != null ? this.streetName + " " : "") + this.houseNumber + (this.flatNumber != null ? "/" + this.flatNumber : "");
        this.cityFormatted = this.postalCode + " " + this.city;
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