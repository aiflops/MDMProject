var mymap = L.map('profileMap').setView([51.643078, 19.609658], 7);

L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    maxZoom: 18,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1
}).addTo(mymap);

var geocoder = L.Control.Geocoder.nominatim();
var geocoderOptions = {
    geocoder: geocoder,
    position: 'topright',
    placeholder: 'Szukaj...',
    errorMessage: 'Nic nie znaleziono',
    iconLabel: 'Wyszukaj',
    defaultMarkGeocode: false
};

var geocoderControl = new L.Control.Geocoder(geocoderOptions);
var marker;
var popup = L.popup();

geocoderControl.on('markgeocode', function (result) {
    result = result.geocode || result;

    mymap.fitBounds(result.bbox);

    markPoint(result);
});
geocoderControl.addTo(mymap);

mymap.on('click', function (e) {
    geocoderControl.options.geocoder.reverse(e.latlng, mymap.options.crs.scale(mymap.getZoom()), function (results) {
        var result = results[0];
        if (result)
            markPoint(result);
    })
});

function markPoint(result) {
    if (marker) {
        mymap.removeLayer(marker);
    }
    marker = new L.Marker(result.center)
        .bindPopup(result.html || result.name)
        .addTo(mymap)
        .openPopup();

    updateInputs(result.center.lat, result.center.lng);
}

function onGeocodeFinished(result) {
    mymap.fitBounds(result.bbox);
    markPoint(result);
}

function updateInputs(latitude, longitude) {
    document.getElementById('Latitude').value = latitude;
    document.getElementById('Longitude').value = longitude;
}

//setup before functions
var typingTimer;                //timer identifier
var doneTypingInterval = 1000;  //time in ms (0.5 seconds)
var myInput = document.getElementById('myInput');

var addressElementIds = ['StreetName', 'HouseNumber', 'FlatNumber', 'PostalCode', 'City'];

/* for IE10 compatibility */
for (var i = 0; i < addressElementIds.length; i++) {
    var elementId = addressElementIds[i];
    var element = document.getElementById(elementId);
    element.addEventListener('keyup', onInputChange);
    element.addEventListener('paste', onInputChange);
}

function onInputChange(e) {
    console.log('Started waiting for ' + e.target.id);
    clearTimeout(typingTimer);

    typingTimer = setTimeout(doneTyping, doneTypingInterval);
}

//user is "finished typing," do something
function doneTyping() {
    var streetName = document.getElementById('StreetName').value;
    var houseNumber = document.getElementById('HouseNumber').value;
    var flatNumber = document.getElementById('FlatNumber').value;
    var postalCode = document.getElementById('PostalCode').value;
    var city = document.getElementById('City').value;

    // Pattern: ulica dom/miesz., kod, miasto
    // dla miejscowości bez nazw ulic: miasto dom/miesz., kod, miasto
    // wyszukuj dopiero gdy będzie uzupełniony kod i miasto

    var postCodeRegex = /\d{2}-\d{3}/g;

    if (!isEmptyOrSpaces(postalCode) && postalCode.match(postCodeRegex) && !isEmptyOrSpaces(city)) {
        var query = "";
        var isFullAddress = false;
        if (!isEmptyOrSpaces(streetName) && !isEmptyOrSpaces(houseNumber)) {
            query = streetName;
            query += " " + houseNumber;

            // Flat number doesn't work with geocode
            //if (!isEmptyOrSpaces(flatNumber)) {
            //    query += "/" + flatNumber;
            //}

            query += ", ";
            isFullAddress = true;
        }

        query += postalCode + " " + city;

        console.log('Lookup for \"' + query + "\"");
        geocoder.geocode(query, function (results) {
            var result = results[0];
            if (result) {
                onGeocodeFinished(result);
            }
            else if (isFullAddress) {
                var onlyPostCodeAndCity = postalCode + " " + city;
                geocoder.geocode(onlyPostCodeAndCity, function (innerResults) {

                    var innerResult = innerResults[0];
                    if (innerResult) {
                        onGeocodeFinished(innerResult);
                    }
                    else {
                        geocoder.geocode(postalCode, function (innerResults2) {
                            var innerResult2 = innerResults2[0];
                            if (innerResult2) {
                                onGeocodeFinished(innerResult2);
                            }
                            else {
                                console.log("Couldnt find coordinates for " + query);
                            }
                        }, window)
                    }

                }, window)
            }

        }, window);


    }
}

function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}