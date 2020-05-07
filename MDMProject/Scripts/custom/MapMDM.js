
window.onload = loadMapSuppliers;
var suppliersList = [];
var suppliersListElHtml = document.getElementById('showSuppliers');
var mymap = L.map('mapID').setView([51.643078, 19.609658], 7);
var GEOCODE_API = "https:\//nominatim.openstreetmap.org/search?q=#postcode,Poland&accept-language=pl&format=json";

L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    maxZoom: 25,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1
}).addTo(mymap);
var popup = L.popup();

function customParseFloat(lat, lon) {
    return [parseFloat(lat), parseFloat(lon)];
}

var Mobile = {
    check: function () {
        return (window.innerWidth < 1000) ? true : false;
    },
    windowResize: function () {
        ListElement.clearAllLi();
        MapMDM.onInitFlag = true;
        ListElement.createMoreLi(suppliersList);
        MapMDM.onInitFlag = false;
    }
};
window.onresize = Mobile.windowResize;

var MapMDM = {
    onInitFlag: true,
    setView: function (lat, lon, zoom) {
        mymap.setView(customParseFloat(lat,lon), zoom);
    },
    
    moveMap: function () {
        ListElement.clearAllLi();
        //MarkersGroupMDM.clearAllMarkers();
        var bounds = this.getBounds();
        MapMDM.checkIfSupplierOnMap(bounds);
        //MapMDM.addCluster(MarkersGroupMDM.cluster);
    },
    checkIfSupplierOnMap: function (bounds) {
        for (var i = 0; i < suppliersList.length; i++) {
            if (suppliersList[i].Latitude !== null || suppliersList[i].Longitude !== null) {
                var isContain = bounds.contains(customParseFloat(suppliersList[i].Latitude, suppliersList[i].Longitude));
                if (isContain)
                    ListElement.createOneLi(suppliersList[i]);
            }
                
        }
    },
    addCluster: function (cluster) {
        mymap.addLayer(cluster);
    }

};
mymap.on('moveend', MapMDM.moveMap);


var ListElement = {
    liListID: [],
    activeElement: '',
    htmlEl: "<div role = \"button\" class= \"entry #Type\" >" +
             "<div class=\"entry__wrapper p-1\"><div class=\"entry__icon\">"+
            "<img src=#Src alt=#Alt></div><h3 class=\"entry__text pl-1\">#Name</h3></div>" +
                        "<div class=\"entry__media p-1\">"+
                            "#City</div></div>",

    createLi: function (item) {
        var tile = this.htmlEl;
        tile = tile.replace('#Id', 'supplier-' + item.Id);

        /* ADDED AFTER REFACTOR */
        tile = tile.replace("#Name", item.Name).replace("#City", item.City);
        tile = tile.replace(/#Src/gi, "/Content/images/mask.png").replace(/#Alt/gi, "Oferuję Maskę").replace(/#Type/gi, "mask");
            if (MapMDM.onInitFlag)
                MarkerMDM.setMarker(item.Latitude, item.Longitude, MarkerMDM.iconsMask, item.Id);
        /* END: ADDED AFTER REFACTOR */


        var liElement = document.createElement('li');
        liElement.classList.add("list__item");
        liElement.id = 'supplier-' + item.Id;
        if (ListElement.activeElement === liElement.id) {
            liElement.classList.add("active__item");
        }
        liElement.dataset.latlng = item.Latitude + '-' + item.Longitude;
        liElement.addEventListener("click", ListElement.clickLi);
        tile = tile.replace(/null/gi, "");
        liElement.innerHTML = tile;
        return liElement;
    },
    clearAllLi: function () {
        suppliersListElHtml.innerHTML = "";
        this.liListID = [];
    },
    createMoreLi: function (itemsList) {
        MarkersGroupMDM.clearAllMarkers();
        for (var i = 0; i < itemsList.length; i++) {
            /* Removed after refactor */
            //if (itemsList[i].OfferedHelp !== null || itemsList[i].OfferedEquipment !== null)
            //    suppliersListElHtml.appendChild(this.createLi(itemsList[i]));

            /* ADDED AFTER REFACTOR */
            suppliersListElHtml.appendChild(this.createLi(itemsList[i]));
            /* END: ADDED AFTER REFACTOR */
        }
        MapMDM.addCluster(MarkersGroupMDM.cluster);
    },
    createOneLi: function (item) {
        suppliersListElHtml.appendChild(this.createLi(item));
    },
    hideLiElement: function (itemId) {
        document.getElementById(itemId).style.display = "none";
    },
    showLiElement: function (itemId) {
        document.getElementById(itemId).style.display = "list-item";
    },
    clickLi: function (e) {
        if (this.dataset.latlng !== "null-null" && !Mobile.check())
            MapMDM.setView(this.dataset.latlng.split('-')[0], this.dataset.latlng.split('-')[1], 19);
    }
}
var MarkersGroupMDM = {
    cluster : L.markerClusterGroup({
        chunkedLoading: true,
        spiderfyOnMaxZoom: false,
        disableClusteringAtZoom: 22,
        showCoverageOnHover: false,
        removeOutsideVisibleBounds: true,
        polygonOptions: null,
    }),
    addMarker: function (marker) {
        MarkersGroupMDM.cluster.addLayer(marker);
    },
    clearAllMarkers: function () {
        MarkersGroupMDM.cluster.clearLayers();
    }
};

var MarkerMDM = {
    markerList:[],
    iconsMask: L.icon({
        iconUrl: '/Content/images/mask.png',
        iconSize: [40, 40],
        iconAnchor: [20, 20],
    }),
    iconsPrinter : L.icon({
        iconUrl: '/Content/images/3d.png',
        iconSize: [40, 40],
        iconAnchor: [20, 20],
    }),
    onClick: function (e) {
        var key = customParseFloat(this.getLatLng().lat, this.getLatLng().lng).join("-")
        //ListElement.activeElement = MarkerMDM.markerList[key];
    },
    setMarker: function (lat, lon, icon, htmlId) {
    if (lat !== null && lon !== null) {
        var latD = parseFloat(lat);
        var lonD = parseFloat(lon);
        //L.marker([latD, lonD], { icon: icon }).addTo(mymap).on('click', this.onClick);
        this.markerList[latD + "-" + lonD] = 'supplier-' + htmlId;
        var marker = L.marker([latD, lonD], { icon: icon }).on('click', MarkerMDM.onClick);
        MarkersGroupMDM.addMarker(marker);
    }
    },
    calculateDistance: function (_firstPoint, _secondPoint) {
        var firstPoint = L.latLng(_firstPoint);
        var secondPoint = L.latLng(_secondPoint);
        return parseInt(L.GeometryUtil.length([firstPoint, secondPoint])*100)/100000;
    },
};

var MobileListElement = {
    allowDistance: 100,
    distanceItemList : [],
    create: function (pointA) {
        ListElement.clearAllLi();
        for (var i = 0; i < suppliersList.length; i++) {
            if (suppliersList[i].Latitude !== null && suppliersList[i].Longitude !== null) {
                var pointB = [suppliersList[i].Latitude, suppliersList[i].Longitude];
                var distance = MarkerMDM.calculateDistance(pointA, pointB);
                if (distance < 100) {
                    var elList = suppliersList[i];
                    elList['distance'] = distance;
                    this.distanceItemList.push(elList);
                    this.distanceItemList.sort(function (a, b) {
                        return a.distance - b.distance;
                    });
                }
            }
        }
        ListElement.createMoreLi(this.distanceItemList);
        this.distanceItemList = [];
    }
}

function loadMapSuppliers() {
    $.ajax({
        type: "GET",
        url: '/api/suppliers',
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (value) {
            suppliersList = value;
            console.log(value);
            ListElement.createMoreLi(suppliersList);
            MapMDM.onInitFlag = false;
        },
        error: function () {
            alert("Error while inserting data");
        }
    });
}

function SPCButtonValidation(code) {
    if (code.match(/\d{2}-\d{3}/)) {
        return true;
    }
    document.getElementById("search__validation").style.display = "block";
    return false;
};
function onSPCButtonClick(e) {
    codeVal = document.getElementById("search").value;
    document.getElementById("search__validation").style.display = "none";
    if (SPCButtonValidation(codeVal)) {
        var api_url = GEOCODE_API.replace('#postcode', codeVal);
        $.ajax({
            type: "GET",
            url: api_url,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (value) {
                var firstResult = value[0];
                if (Mobile.check()) {
                    MobileListElement.create(customParseFloat(firstResult.lat, firstResult.lon));
                }
                else {
                    MapMDM.setView(firstResult.lat, firstResult.lon, 12);
                }
                },
            error: function () {
                alert("Error while inserting data");
            }
        });
    }
};
