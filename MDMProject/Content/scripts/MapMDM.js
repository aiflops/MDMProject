
window.onload = loadMapSuppliers;

var suppliersList = [];

var mymap = L.map('mapID').setView([51.643078, 19.609658], 7);

L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    maxZoom: 18,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1
}).addTo(mymap);

var iconsMask = L.icon({
    iconUrl: '/Content/images/mask.png',
    iconSize: [40, 40],
    iconAnchor: [22, 94],
    popupAnchor: [-3, -76]
});
var iconsPrinter = L.icon({
    iconUrl: '/Content/images/3d.png',
    iconSize: [40, 40],
    iconAnchor: [22, 94],
    popupAnchor: [-3, -76]
});

L.marker([51.643078, 19.609658], { icon: iconsMask }).addTo(mymap);
L.marker([51.643, 20.60], { icon: iconsPrinter }).addTo(mymap);

var popup = L.popup();



function SPCButtonValidation(code) {
    if (code.match(/\d{2}-\d{3}/)) {
        return true;
    }
    document.getElementById("search_validation").style.display = "block";
    return false;
}

function onSPCButtonClick(e) {
    codeVal = document.getElementById("search").value;
    document.getElementById("search_validation").style.display = "none";
    if (SPCButtonValidation(codeVal)) {
        // todo ask api geolocalization 
        // change zoom map
        // hide supliers
        // hide markes

    }
}

function onMapClick(e) {
    popup
        .setLatLng(e.latlng)
        .setContent("You clicked the map at " + e.latlng.toString())
        .openOn(mymap);
}

mymap.on('click', onMapClick);

var ListElement = {

    htmlEl : "<li class=\"list__item\" id=\"#Id\"> <div role = \"button\" class= \"entry #Type\" >" +
             "<div class=\"entry__wrapper p-1\"><div class=\"entry__icon\">"+
            "<img src=#Src alt=#Alt></div><h3 class=\"entry__text pl-1\">#Name</h3></div>" +
                    "<address class=\"entry__address p-1\">#Adress</address>"+
                        "<div class=\"entry__media p-1\">"+
                            "tel: <a href=\"tel:#Phone\" title=\"Zadzwoń: #Phone\">#Phone</a>"+
                            "<br> e-mail:<a href=\"mailto:#Email\"title=\"Wyślij e-mail: #Email\">#Email</a>"+
                            "</div></div></li>",

    createLi: function (item) {
            var tile = this.htmlEl;
            tile = tile.replace('#Id', 'supplier-' + item.Id);
            tile = tile.replace("#Name", item.Name);
            var adress = item.Address.City + " " + item.Address.PostalCode + ", <br/>" + item.Address.StreetName + " " + item.Address.HouseNumber;
            if (item.Address.FlatNumber !== null) {
                adress = adress + " " + item.Address.FlatNumber;
            }
            tile = tile.replace("#Adress", adress);

            tile = tile.replace(/#Phone/gi, item.PhoneNumber);
            tile = tile.replace(/#Email/gi, item.Email);
        if (item.OfferedHelp !== null && item.OfferedEquipment !== null) {
            tile = tile.replace('<img src=#Src alt=#Alt>', '<img src=\"/Content/images/mask.png\" alt=\"Drukuję w 3D\"><img src=\"/Content/images/3d.png\" alt=\"Oferuję Maskę"\>')

            tile = tile.replace(/#Type/gi, "multiple");

            }
            else if(item.OfferedHelp !== null) {
            // 3d
                tile = tile.replace(/#Src/gi, "/Content/images/3d.png");
                tile = tile.replace(/#Alt/gi, "Drukuję w 3D");
                tile = tile.replace(/#Type/gi, "print");
            }
            else if (item.OfferedEquipment !== null) {
            //mask
                tile = tile.replace(/#Src/gi, "/Content/images/mask.png");
                tile = tile.replace(/#Alt/gi, "Oferuję Maskę");
                tile = tile.replace(/#Type/gi, "mask");
        }
        var liElement = document.createElement('li');
        liElement.innerHTML = tile;
        return liElement;

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
            var el = document.getElementById('showSuppliers');
            for (var i = 0; i < suppliersList.length; i++) {
                if (suppliersList[i].OfferedHelp !== null || suppliersList[i].OfferedEquipment !== null)
                    el.appendChild(ListElement.createLi(suppliersList[i]));
            }

        },
        error: function () {
            alert("Error while inserting data");
        }
    });

}
//#id X
//#Src
//#Alt

//#Name X

//#Adress X

//#Phone
//#Email

///api/suppliers - zwraca wszystkich dostawcow(masek lub druku)
//api/suppliers/17 - zwraca tylko gościa o id 17

//Id: "13"
//Name: "Krzysztof Kowalkiewicz"
//Email: "krzysztof@kowalkiewicz.pl"
//PhoneNumber: "+48 77 12 50 321"
//Address:
//City: "Opole"
//StreetName: "Krakowska"
//HouseNumber: "44"
//FlatNumber: null
//PostalCode: "45-075"
//Latitude: null
//Longitude: null
//__proto__: Object
//OfferedEquipment: Array(1)
//0: { Name: "Maska", Amount: 1, Comment: null }
//length: 1
//__proto__: Array(0)
//OfferedHelp: null
//AdditionalComment: "Tylko po 17:00!"
