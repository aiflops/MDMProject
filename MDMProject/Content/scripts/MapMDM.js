window.onload = loadMapSuppliers;
var suppliersList = [];
var suppliersListElHtml = document.getElementById('showSuppliers');
var mymap = L.map('mapID').setView([51.643078, 19.609658], 7);
var GEOCODE_API = "https:\//nominatim.openstreetmap.org/search?q=#postcode,Poland&accept-language=pl&format=json";
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    maxZoom: 18,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1
}).addTo(mymap);
function customParseFloat(lat, lon) {
    return [parseFloat(lat), parseFloat(lon)];
}
var MapClass = {
    setView: function (lat, lon, zoom) {
        mymap.setView(customParseFloat(lat,lon), zoom);
    },
    
    moveMap: function (e) {
        ListElement.clearAllLi();
        var bounds = this.getBounds();
        MapClass.checkIfSupplierOnMap(bounds);
        //_southWest _northEast
    },
    checkIfSupplierOnMap: function (bounds) {
        for (var i = 0; i < suppliersList.length; i++) {
            if (suppliersList[i].Address.Latitude !== null || suppliersList[i].Address.Longitude !== null) {
                var isContain = bounds.contains(customParseFloat(suppliersList[i].Address.Latitude, suppliersList[i].Address.Longitude));
                if (isContain)
                    ListElement.createOneLi(suppliersList[i]);
            }
                
        }
    }

};
mymap.on('moveend', MapClass.moveMap);
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
                console.log(value);
                var firstResult = value[0];
                MapClass.setView(firstResult.lat, firstResult.lon, 12);
            },
            error: function () {
                alert("Error while inserting data");
            }
        });
    }
};
var ListElement = {
    liListID:[],
    htmlEl : "<li class=\"list__item\" id=\"#Id\"> <div role = \"button\" class= \"entry #Type\" >" +
             "<div class=\"entry__wrapper p-1\"><div class=\"entry__icon\">"+
            "<img src=#Src alt=#Alt></div><h3 class=\"entry__text pl-1\">#Name</h3></div>" +
                    "<address class=\"entry__address p-1\">#Adress</address>"+
                        "<div class=\"entry__media p-1\">"+
                            "tel: <a href=\"tel:#Phone\" title=\"Zadzwoń: #Phone\">#Phone</a>"+
                            "<br><a href=\"mailto:#Email\"title=\"Wyślij e-mail: #Email\">#Email</a>"+
                            "</div></div></li>",

    createLi: function (item) {
        var tile = this.htmlEl;
        this.liListID.push('supplier-' + item.Id);
        tile = tile.replace('#Id', 'supplier-' + item.Id);
        var adress = item.Address.City + ", " + item.Address.StreetName + " " + item.Address.HouseNumber;
        if (item.Address.FlatNumber !== null) {
            adress = adress + " " + item.Address.FlatNumber;
        }
        tile = tile.replace("#Name", item.Name.split(" ")[0]).replace(/#Phone/gi, item.PhoneNumber).replace(/#Email/gi, item.Email).replace("#Adress", adress);
        if (item.OfferedHelp !== null && item.OfferedEquipment !== null) {
            tile = tile.replace('<img src=#Src alt=#Alt>', '<img src=\"/Content/images/mask.png\" alt=\"Drukuję w 3D\"><img src=\"/Content/images/3d.png\" alt=\"Oferuję Maskę"\>')
            tile = tile.replace(/#Type/gi, "multiple");
        }
        else if(item.OfferedHelp !== null) {
        tile = tile.replace(/#Src/gi, "/Content/images/3d.png").replace(/#Alt/gi, "Drukuję w 3D").replace(/#Type/gi, "print");
        MarkerMDM.setMarker(item.Address.Latitude, item.Address.Longitude, MarkerMDM.iconsPrinter, item.Id);
        }
        else if (item.OfferedEquipment !== null) {
        tile = tile.replace(/#Src/gi, "/Content/images/mask.png").replace(/#Alt/gi, "Oferuję Maskę").replace(/#Type/gi, "mask");
        MarkerMDM.setMarker(item.Address.Latitude, item.Address.Longitude, MarkerMDM.iconsMask, item.Id);
        }
        var liElement = document.createElement('li');
        liElement.innerHTML = tile;
        return liElement;
    },
    clearAllLi: function () {
        suppliersListElHtml.innerHTML = "";
    },
    createMoreLi: function (itemsList) {
        for (var i = 0; i < itemsList.length; i++) {
            if (itemsList[i].OfferedHelp !== null || itemsList[i].OfferedEquipment !== null)
                suppliersListElHtml.appendChild(this.createLi(itemsList[i]));
        }
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
}
var MarkerMDM = {
    markerList:[],
    iconsMask: L.icon({
        iconUrl: '/Content/images/mask.png',
        iconSize: [40, 40],
        iconAnchor: [22, 94],
        popupAnchor: [-3, -76]
    }),
    iconsPrinter : L.icon({
        iconUrl: '/Content/images/3d.png',
        iconSize: [40, 40],
        iconAnchor: [22, 94],
        popupAnchor: [-3, -76]
    }),
    onClick: function (e) {
        MapClass.setView(this.getLatLng().lat, this.getLatLng().lng, 14);
    },
    setMarker: function (lat, lon, icon, htmlId) {
    if (lat !== null && lon !== null) {
        var latD = parseFloat(lat);
        var lonD = parseFloat(lon);
        L.marker([latD, lonD], { icon: icon }).addTo(mymap).on('click', this.onClick);
        this.markerList[latD + "-" + lonD] = 'supplier-' + htmlId;
    }
    },
};

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
        },
        error: function () {
            alert("Error while inserting data");
        }
    });
}
