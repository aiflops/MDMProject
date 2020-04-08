
window.onload = loadMapSuppliers;

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

function loadMapSuppliers() {
    // todo get all supliers

}

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