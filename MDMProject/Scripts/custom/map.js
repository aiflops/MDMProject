/// <reference path="../typings/_references.ts" />
var MAP_CONTAINER_ID = 'mapID';
var GEOCODE_API = "https:\//nominatim.openstreetmap.org/search?q=#postcode,Poland&accept-language=pl&format=json";
var MAP_LAYER_URL = "https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw";
var INIT_MAP_BOUNDARIES = [51.643078, 19.609658];
var POINT_VIEW_ZOOM = 19;
var POST_CODE_ZOOM = 13;
var POST_CODE_DISTANCE = 20; // Distance for post code search in kilometers
var mapMdm;
var viewModel;
var Map = /** @class */ (function () {
    function Map() {
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
    Map.prototype.setViewToPoint = function (point, zoom) {
        this.map.setView(point, zoom);
    };
    Map.prototype.addMarker = function (marker) {
        this.markerCluster.addLayer(marker.mapMarker);
    };
    Map.prototype.clearAllMarkers = function () {
        this.markerCluster.clearLayers();
    };
    Map.prototype.moveMap = function () {
        var bounds = this.getBounds();
        viewModel.updateVisibleItems(bounds);
    };
    Map.prototype.getBounds = function () {
        return this.map.getBounds();
    };
    return Map;
}());
var Marker = /** @class */ (function () {
    function Marker(id, point, icon) {
        this.id = id;
        this.point = point;
        this.mapMarker = L
            .marker(point, { icon: icon })
            .on('click', this.onClick);
    }
    Marker.prototype.onClick = function () {
        mapMdm.setViewToPoint(this.getLatLng(), POINT_VIEW_ZOOM);
    };
    Marker.iconMask = L.icon({
        iconUrl: '/Content/images/mask.png',
        iconSize: [40, 40],
        iconAnchor: [20, 20],
    });
    Marker.iconGreen = L.icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
    });
    return Marker;
}());
var MapViewModel = /** @class */ (function () {
    function MapViewModel() {
        this.postCode = ko.observable();
        this.isPostCodeIncorrect = ko.observable(false);
        this.lastPostCodeSearch = ko.observable();
        this.lastPostCodePoint = ko.observable();
        this.items = ko.observableArray();
        this.isDataLoaded = ko.observable(false);
        this.visibleItems = ko.observable([]);
    }
    MapViewModel.prototype.loadData = function () {
        var _this = this;
        $.getJSON('/api/suppliers', function (data) {
            var dataItems = ko.utils.arrayMap(data || [], function (item) { return new MapEntryItem(item); });
            ko.utils.arrayPushAll(_this.items, dataItems);
            _this.addAllMarkers();
            var mapBounds = mapMdm.getBounds();
            _this.updateVisibleItems(mapBounds);
        });
    };
    MapViewModel.prototype.searchByPostCode = function () {
        var _this = this;
        var api_url = GEOCODE_API.replace('#postcode', this.postCode());
        // Reset warning message
        this.isPostCodeIncorrect(false);
        // if post code has been reset
        if (utils.isEmptyOrSpaces(this.postCode())) {
            this.lastPostCodeSearch(null);
            this.lastPostCodePoint(null);
            var mapBounds = mapMdm.getBounds();
            this.updateVisibleItems(mapBounds);
        }
        // if post code has been provided and is valid
        else if (this.isPostCodeValid()) {
            $.getJSON(api_url, function (value) {
                var firstResult = value[0];
                var point = L.latLng(firstResult.lat, firstResult.lon);
                _this.lastPostCodeSearch(_this.postCode());
                _this.lastPostCodePoint(point);
                var mapBounds = mapMdm.getBounds();
                _this.updateVisibleItems(mapBounds);
                mapMdm.setViewToPoint(point, POST_CODE_ZOOM);
            }).fail(function () { return alert('Błąd! Wystąpił błąd podczas pobierania danych...'); });
        }
        else {
            this.isPostCodeIncorrect(true);
        }
    };
    MapViewModel.prototype.isPostCodeValid = function () {
        return this.postCode().match(/\d{2}-\d{3}/) != null;
    };
    MapViewModel.prototype.updateVisibleItems = function (mapBounds) {
        var newVisibleItems = [];
        var searchByPostCode = false;
        var showAll = false;
        var isMapBoundsSinglePoint = mapBounds._northEast.equals(mapBounds._southWest);
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
            ko.utils.arrayForEach(this.items(), function (item) {
                item.updateDistanceToPostCode(viewModel.lastPostCodePoint());
                var isVisible = item.distanceToPostCode() < (POST_CODE_DISTANCE * 1000);
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
            ko.utils.arrayForEach(this.items(), function (item) {
                var isVisible = showAll || mapBounds.contains(item.point);
                item.isVisible(isVisible);
                if (isVisible)
                    newVisibleItems.push(item);
            });
        }
        this.visibleItems(newVisibleItems);
    };
    MapViewModel.prototype.addAllMarkers = function () {
        mapMdm.clearAllMarkers();
        ko.utils.arrayForEach(this.items(), function (item) {
            mapMdm.addMarker(item.marker);
        });
    };
    MapViewModel.prototype.onKeyPress = function (data, event) {
        var keyCode = (event.which ? event.which : event.keyCode);
        if (keyCode === 13) {
            viewModel.searchByPostCode();
        }
        return true;
    };
    return MapViewModel;
}());
//#region ViewModel types
var MapEntryItem = /** @class */ (function () {
    function MapEntryItem(model) {
        this.id = model.Id;
        this.name = model.Name;
        this.city = model.City;
        this.latitude = parseFloat(model.Latitude);
        this.longitude = parseFloat(model.Longitude);
        this.point = L.latLng(this.latitude, this.longitude);
        this.marker = new Marker(this.id, this.point, Marker.iconGreen);
        this.isVisible = ko.observable(false);
        this.distanceToPostCode = ko.observable();
    }
    MapEntryItem.prototype.updateDistanceToPostCode = function (postCodeCenterPoint) {
        if (postCodeCenterPoint != null) {
            var distance = this.point.distanceTo(postCodeCenterPoint);
            this.distanceToPostCode(distance);
        }
        else {
            this.distanceToPostCode(null);
        }
    };
    MapEntryItem.prototype.listItemClick = function () {
        mapMdm.setViewToPoint(this.point, POINT_VIEW_ZOOM);
    };
    return MapEntryItem;
}());
var MapEntryItemDetails = /** @class */ (function () {
    function MapEntryItemDetails(model) {
        this.id = ko.observable(model.Id);
        this.userType = ko.observable(model.UserType);
        this.companyName = ko.observable(model.CompanyName);
        this.personName = ko.observable(model.PersonName);
        this.email = ko.observable(model.Email);
        this.phoneNumber = ko.observable(model.PhoneNumber);
        this.address = ko.observable(model.Address != null ? new MapEntryAddress(model.Address) : null);
    }
    return MapEntryItemDetails;
}());
var MapEntryAddress = /** @class */ (function () {
    function MapEntryAddress(model) {
        this.city = ko.observable(model.City);
        this.streetName = ko.observable(model.StreetName);
        this.houseNumber = ko.observable(model.HouseNumber);
        this.flatNumber = ko.observable(model.FlatNumber);
        this.postalCode = ko.observable(model.PostalCode);
        this.latitude = ko.observable(parseFloat(model.Latitude));
        this.longitude = ko.observable(parseFloat(model.Longitude));
    }
    return MapEntryAddress;
}());
//#endregion
$(document).ready(function () {
    viewModel = new MapViewModel();
    ko.applyBindings(viewModel);
    mapMdm = new Map();
    viewModel.loadData();
});
//# sourceMappingURL=map.js.map