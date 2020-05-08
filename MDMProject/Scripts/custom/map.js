/// <reference path="../typings/_references.ts" />
var MAP_CONTAINER_ID = 'mapID';
var GEOCODE_API = "https:\//nominatim.openstreetmap.org/search?q=#postcode,Poland&accept-language=pl&format=json";
var MAP_LAYER_URL = "https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw";
var INIT_MAP_BOUNDARIES = [51.643078, 19.609658];
var POINT_VIEW_ZOOM = 19;
var POST_CODE_ZOOM = 15;
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
    Map.prototype.setViewToPoint = function (point, zoom) {
        this.map.setView(point, zoom);
    };
    Map.prototype.addMarker = function (marker) {
        if (!marker.isPostCodeMarker)
            this.markerCluster.addLayer(marker.mapMarker);
        else
            this.postCodeMarkerCluster.addLayer(marker.mapMarker);
    };
    Map.prototype.removeMarker = function (marker) {
        marker.mapMarker.remove();
    };
    Map.prototype.clearAllMarkers = function () {
        this.markerCluster.clearLayers();
        this.postCodeMarkerCluster.clearLayers();
    };
    Map.prototype.clearPostCodeMarkers = function () {
        this.postCodeMarkerCluster.clearLayers();
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
    function Marker(id, point, icon, isPostCodeMarker) {
        if (isPostCodeMarker === void 0) { isPostCodeMarker = false; }
        this.id = id;
        this.point = point;
        this.isPostCodeMarker = isPostCodeMarker;
        this.mapMarker = L
            .marker(point, { icon: icon })
            .on('click', this.onClick);
    }
    Marker.prototype.onClick = function () {
        var leafletMarker = this;
        mapMdm.setViewToPoint(leafletMarker.getLatLng(), POINT_VIEW_ZOOM);
        if (leafletMarker._itemViewModel != null) {
            viewModel.activeItem(leafletMarker._itemViewModel);
        }
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
    Marker.iconRed = L.icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
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
        this.activeItem = ko.observable();
        this.activeItem.subscribe(function (newActiveItem) {
            if (newActiveItem != null && newActiveItem.details() == null)
                newActiveItem.loadDetails();
        });
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
            this.activeItem(null);
            ko.utils.arrayForEach(this.items(), function (item) { return item.updateDistanceToPostCode(null); });
            mapMdm.clearPostCodeMarkers();
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
                _this.activeItem(null);
                ko.utils.arrayForEach(_this.items(), function (item) { return item.updateDistanceToPostCode(_this.lastPostCodePoint()); });
                mapMdm.clearPostCodeMarkers();
                mapMdm.addMarker(new Marker(null, _this.lastPostCodePoint(), Marker.iconRed, true));
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
                var isVisible = item.distanceToPostCode() < (POST_CODE_DISTANCE * 1000);
                item.isVisible(isVisible);
                if (isVisible)
                    newVisibleItems.push(item);
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
    MapViewModel.prototype.exitActiveItemDetails = function () {
        viewModel.activeItem(null);
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
        this.marker.mapMarker._itemViewModel = this;
        this.details = ko.observable();
        this.isVisible = ko.observable(false);
        this.distanceToPostCode = ko.observable();
        this.distanceToPostCodeFormatted = ko.computed(this.getDistanceToPostCodeFormatted.bind(this));
    }
    MapEntryItem.prototype.getDistanceToPostCodeFormatted = function () {
        if (this.distanceToPostCode() == null)
            return null;
        var distanceInKm = Math.round(this.distanceToPostCode() / 100) / 10;
        return distanceInKm + 'km';
    };
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
        viewModel.activeItem(this);
    };
    MapEntryItem.prototype.loadDetails = function () {
        var _this = this;
        $.getJSON('/api/suppliers/' + this.id, function (data) {
            var detailsViewModel = new MapEntryItemDetails(data);
            _this.details(detailsViewModel);
        });
    };
    return MapEntryItem;
}());
var MapEntryItemDetails = /** @class */ (function () {
    function MapEntryItemDetails(model) {
        this.id = model.Id;
        this.userType = model.UserType;
        this.companyName = model.CompanyName;
        this.personName = model.PersonName;
        this.email = model.Email;
        this.phoneNumber = model.PhoneNumber;
        this.address = model.Address != null ? new MapEntryAddress(model.Address) : null;
    }
    return MapEntryItemDetails;
}());
var MapEntryAddress = /** @class */ (function () {
    function MapEntryAddress(model) {
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