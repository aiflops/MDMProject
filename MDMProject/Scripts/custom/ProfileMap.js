$(document).ready(function () {
    var mymap,
        geocoder,
        marker;

    //#region Init calls
    function initializeMap() {
        mymap =
            L.map('profileMap')
                .on('click', onMapClick)
                .setView([51.643078, 19.609658], 7);

        initBaseLayer();
        initGeocoder();
        initGeocoderControl();
        loadInitialCoordinates();
        registerAddressFieldChangesHandlers();
    }
    //#endregion

    //#region Initialization
    function initBaseLayer() {
        L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
            maxZoom: 18,
            id: 'mapbox/streets-v11',
            tileSize: 512,
            zoomOffset: -1
        }).addTo(mymap);
    }

    function initGeocoder() {
        geocoder = L.Control.Geocoder.nominatim();
    }

    function initGeocoderControl() {
        var geocoderOptions = {
            geocoder: geocoder,
            position: 'topright',
            placeholder: 'Szukaj...',
            errorMessage: 'Nic nie znaleziono',
            iconLabel: 'Wyszukaj',
            defaultMarkGeocode: false
        };

        var geocoderControl = new L.Control.Geocoder(geocoderOptions);

        geocoderControl.on('markgeocode', onMarkGeocode);
        geocoderControl.addTo(mymap);
    }

    function loadInitialCoordinates() {
        setTimeout(function () {
            var lat = document.getElementById('Latitude').value;
            var lng = document.getElementById('Longitude').value;
            var zoom = 20;

            if (lat && lng) {
                // add a marker
                marker = L.marker([lat, lng], {}).addTo(mymap);
                // set the view
                mymap.setView([lat, lng], zoom);
            }
        }, 200);
    }

    //#endregion

    function onMarkGeocode(result) {
        result = result.geocode || result;

        mymap.fitBounds(result.bbox);

        markPoint(result);
    }

    function onMapClick(e) {
        geocoder.reverse(e.latlng, mymap.options.crs.scale(mymap.getZoom()), function (results) {
            var result = results[0];
            if (result)
                markPoint(result);
        });
    }

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

    //#region Address to point - listening on address field changes
    function registerAddressFieldChangesHandlers() {
        var addressElementIds = ['StreetName', 'HouseNumber', 'FlatNumber', 'PostalCode', 'City'];

        utils.arrayForEach(addressElementIds, function (elementId) {
            var element = document.getElementById(elementId);
            element.addEventListener('keyup', onInputChange);
            element.addEventListener('paste', onInputChange);
        });

        var typingTimer;                //timer identifier
        var doneTypingInterval = 1000;  //time in ms (1 second)

        function onInputChange(e) {
            console.log('Started waiting for ' + e.target.id);
            clearTimeout(typingTimer);

            typingTimer = setTimeout(doneTyping, doneTypingInterval);
        }
    }
    //#endregion

    //#region Address to point geocoding

    //user is "finished typing," do something
    function doneTyping() {
        // Get queries to perform
        var queriesToPerform = getQueriesToPerform();

        // Best match results collection
        var bestMatchCollection = [];

        // Perform lookup
        performLookup(queriesToPerform, bestMatchCollection);
    }

    function getQueriesToPerform() {
        var streetName = document.getElementById('StreetName').value;
        var houseNumber = document.getElementById('HouseNumber').value;
        var flatNumber = document.getElementById('FlatNumber').value; // Flat number doesn't work with geocode
        var postalCode = document.getElementById('PostalCode').value;
        var city = document.getElementById('City').value;

        // Pattern: ulica dom/miesz., kod, miasto
        // dla miejscowości bez nazw ulic: miasto dom/miesz., kod, miasto
        // wyszukuj dopiero gdy będzie uzupełniony kod i miasto

        var postCodeRegex = /\d{2}-\d{3}/g;

        var queriesToPerform = [];

        if (!utils.isEmptyOrSpaces(postalCode) && postalCode.match(postCodeRegex) && !utils.isEmptyOrSpaces(city)) {
            // Check in this order
            // 1. Street + house number
            // 2. Street
            // 3. Post code
            // 4. City

            // Street + house number, post code, city
            if (!utils.isEmptyOrSpaces(streetName) && !utils.isEmptyOrSpaces(houseNumber)) {
                queriesToPerform.push({
                    query: streetName + " " + houseNumber + ", " + postalCode + " " + city,
                    bestMatch: false
                });
            }

            // Street, post code, city
            if (!utils.isEmptyOrSpaces(streetName)) {
                queriesToPerform.push({
                    query: streetName + ", " + postalCode + " " + city,
                    bestMatch: false
                });
            }

            // Post code, city
            queriesToPerform.push({
                query: postalCode + " " + city,
                bestMatch: true
            });

            // Post code
            queriesToPerform.push({
                query: postalCode,
                bestMatch: true
            });

            // City
            queriesToPerform.push({
                query: city,
                bestMatch: true
            });

            // Now reverse to provide proper queue ordering
            queriesToPerform.reverse();
        }

        return queriesToPerform;
    }

    function performLookup(queriesToPerform, bestMatchCollection) {
        var queryToPerform = queriesToPerform.pop();

        if (queryToPerform) {
            console.log('Lookup for \"' + queryToPerform.query + "\"");
            geocoder.geocode(queryToPerform.query, function (results) {

                // If any results found
                if (results.length) {
                    // If should select only the first best match
                    if (!queryToPerform.bestMatch) {
                        // Put result to best match collection
                        bestMatchCollection.push(results);
                        selectBestMatch(bestMatchCollection);
                    }
                    else {
                        // If should check different scenarios - add result to set and perform next lookup
                        bestMatchCollection.push(results);
                        performLookup(queriesToPerform, bestMatchCollection);
                    }
                }
                else {
                    // If no results found - go to more general search
                    performLookup(queriesToPerform, bestMatchCollection);
                }

            }, window);
        }
        else {
            selectBestMatch(bestMatchCollection);
        }
    };

    function selectBestMatch(bestMatchCollection) {
        // If any match found
        if (bestMatchCollection.length) {

            // If there is only one exact match
            if (bestMatchCollection.length === 1) {

                // Take and display that match
                var bestMatch = bestMatchCollection[0];
                var bestResult = bestMatch[0];
                onGeocodeFinished(bestResult);
            }
            else {
                // Map all results to [result, area]
                var resultsByArea = utils.arrayMap(bestMatchCollection, function (results) {
                    return {
                        result: results[0],
                        area: calculateArea(results[0])
                    };
                });

                // Find the result with smallest area from all results
                var minResult = null;
                utils.arrayForEach(resultsByArea, function (item) {
                    if (minResult == null || item.area < minResult.area)
                        minResult = item;
                });

                // Display smallest area result
                onGeocodeFinished(minResult.result);
            }
        }
    }

    function calculateArea(result) {
        // Get bounding box for result
        var bbox = result.bbox;

        // Calculate area - width x height
        var width = Math.abs(bbox._southWest.lng - bbox._northEast.lng);
        var height = Math.abs(bbox._southWest.lat - bbox._northEast.lat);
        var area = width * height;
        return area;
    }
    //#endregion

    initializeMap();
});