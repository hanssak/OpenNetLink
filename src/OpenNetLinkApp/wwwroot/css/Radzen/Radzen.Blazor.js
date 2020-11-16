if (!Element.prototype.matches) {
    Element.prototype.matches = Element.prototype.msMatchesSelector ||
        Element.prototype.webkitMatchesSelector;
}

if (!Element.prototype.closest) {
    Element.prototype.closest = function (s) {
        var el = this;

        do {
            if (el.matches(s)) return el;
            el = el.parentElement || el.parentNode;
        } while (el !== null && el.nodeType === 1);
        return null;
    };
}

var resolveCallbacks = [];
var rejectCallbacks = [];

window.Radzen = {
    loadGoogleMaps: function (defaultView, apiKey, resolve, reject) {
        resolveCallbacks.push(resolve);
        rejectCallbacks.push(reject);

        if (defaultView['rz_map_init']) {
            return;
        }

        defaultView['rz_map_init'] = function () {
            for (var i = 0; i < resolveCallbacks.length; i++) {
                resolveCallbacks[i](defaultView.google);
            }
        };

        var document = defaultView.document;
        var script = document.createElement('script');

        script.src = 'https://maps.googleapis.com/maps/api/js?' + (apiKey ? 'key=' + apiKey + '&' : '') + 'callback=rz_map_init';

        script.async = true;
        script.defer = true;
        script.onerror = function (err) {
            for (var i = 0; i < rejectCallbacks.length; i++) {
                rejectCallbacks[i](err);
            }
        };

        document.body.appendChild(script);
    },
    createMap: function (wrapper, ref, id, apiKey, zoom, center, markers) {
        var api = function () {
            var defaultView = document.defaultView;

            return new Promise(function (resolve, reject) {
                if (defaultView.google && defaultView.google.maps) {
                    return resolve(defaultView.google);
                }

                Radzen.loadGoogleMaps(defaultView, apiKey, resolve, reject);
            });
        }

        api().then(function (google) {
            Radzen[id] = ref;
            Radzen[id].google = google;

            Radzen[id].instance = new google.maps.Map(wrapper, {
                center: center,
                zoom: zoom
            });

            Radzen[id].instance.addListener('click', function (e) {
                Radzen[id].invokeMethodAsync('RadzenGoogleMap.OnMapClick', { Position: { Lat: e.latLng.lat(), Lng: e.latLng.lng()} });
            });

            Radzen.updateMap(id, zoom, center, markers)
        });
    },
    updateMap: function (id, zoom, center, markers) {
        if (Radzen[id] && Radzen[id].instance) {

            if (Radzen[id].instance.markers && Radzen[id].instance.markers.length) {
                for (var i = 0; i < Radzen[id].instance.markers.length; i++) {
                    Radzen[id].instance.markers[i].setMap(null);
                }
            }

            Radzen[id].instance.markers = [];

            markers.forEach(function (m) {
                var marker = new this.google.maps.Marker({
                    position: m.position,
                    title: m.title,
                    label: m.label
                });

                marker.addListener('click', function (e) {
                    Radzen[id].invokeMethodAsync('RadzenGoogleMap.OnMarkerClick', { Title: marker.title, Label: marker.label, Position : marker.position});
                });

                marker.setMap(Radzen[id].instance);

                Radzen[id].instance.markers.push(marker);
            });

            Radzen[id].instance.setZoom(zoom);

            Radzen[id].instance.setCenter(center);
        }
    },
    destroyMap: function (id) {
        if (Radzen[id].instance) {
            delete Radzen[id].instance;
        }
    },
    createSlider: function (slider, parent, range, minHandle, maxHandle, min, max, value) {
        slider.mouseMoveHandler = function (e) {
            var offsetX = e.targetTouches && e.targetTouches[0] ? e.targetTouches[0].pageX - e.target.getBoundingClientRect().left : e.offsetX;
            var percent = (minHandle == e.target || maxHandle == e.target ? e.target.offsetLeft + offsetX : e.target.offsetLeft + offsetX) / parent.offsetWidth;
            var newValue = percent * max;
            var oldValue = range ? value[slider.isMin ? 0 : 1] : value;
            if (slider.canChange && newValue >= min && newValue <= max && newValue != oldValue) {
                slider.invokeMethodAsync('RadzenSlider.OnValueChange', newValue, !!slider.isMin);
            }
            e.preventDefault();
        }

        slider.mouseDownHandler = function (e) {
            if (minHandle == e.target || maxHandle == e.target) {
                slider.canChange = true;
                slider.isMin = minHandle == e.target;
            } else {
                var offsetX = e.targetTouches && e.targetTouches[0] ? e.targetTouches[0].pageX - e.target.getBoundingClientRect().left : e.offsetX;
                var percent = offsetX / parent.offsetWidth;
                var newValue = percent * max;
                var oldValue = range ? value[slider.isMin ? 0 : 1] : value;
                if (newValue >= min && newValue <= max && newValue != oldValue) {
                    slider.invokeMethodAsync('RadzenSlider.OnValueChange', newValue, !!slider.isMin);
                }
            }
        }

        slider.mouseUpHandler = function (e) {
            slider.canChange = false;
        }

        document.addEventListener("mousemove", slider.mouseMoveHandler);
        document.addEventListener("touchmove", slider.mouseMoveHandler, { passive: true });

        document.addEventListener("mouseup", slider.mouseUpHandler);
        document.addEventListener("touchend", slider.mouseUpHandler, { passive: true });

        parent.addEventListener("mousedown", slider.mouseDownHandler);
        parent.addEventListener("touchstart", slider.mouseDownHandler, { passive: true });
    },
    destroySlider: function (slider, parent) {
        if (slider.mouseMoveHandler) {
            document.removeEventListener('mousemove', slider.mouseMoveHandler);
            document.removeEventListener('touchmove', slider.mouseMoveHandler);
            delete slider.mouseMoveHandler;
        }
        if (slider.mouseUpHandler) {
            document.removeEventListener('mouseup', slider.mouseUpHandler);
            document.removeEventListener('touchend', slider.mouseUpHandler);
            delete slider.mouseUpHandler;
        }
        if (slider.mouseDownHandler) {
            parent.removeEventListener('mousedown', slider.mouseDownHandler);
            parent.removeEventListener('touchstart', slider.mouseDownHandler);
            delete slider.mouseDownHandler;
        }
    },
    focusElement: function (elementId) {
        var el = document.getElementById(elementId);
        if (el) {
            el.focus();
        }
    },
    focusListItem: function (input, ul, isDown, startIndex) {
        if (!input || !ul)
            return;

        var childNodes = ul.getElementsByTagName('LI');

        if (!childNodes || childNodes.length == 0)
            return;

        if (startIndex == undefined || startIndex == null) {
            startIndex = -1;
        }

        ul.nextSelectedIndex = startIndex;

        ul.prevSelectedIndex = ul.nextSelectedIndex;

        if (isDown) {
            if (ul.nextSelectedIndex < childNodes.length - 1) {
                ul.nextSelectedIndex++;
            }
        }
        else {
            if (ul.nextSelectedIndex > 0) {
                ul.nextSelectedIndex--;
            }
        }

        if (ul.prevSelectedIndex >= 0 && ul.prevSelectedIndex <= childNodes.length - 1) {
            childNodes[ul.prevSelectedIndex].classList.remove('ui-state-highlight');
        }

        if (ul.nextSelectedIndex >= 0 && ul.nextSelectedIndex <= childNodes.length - 1) {
            childNodes[ul.nextSelectedIndex].classList.add('ui-state-highlight');
            if (ul.parentNode.classList.contains('ui-autocomplete-panel')) {
                ul.parentNode.scrollTop = childNodes[ul.nextSelectedIndex].offsetTop;
            } else {
                ul.parentNode.scrollTop = childNodes[ul.nextSelectedIndex].offsetTop - ul.parentNode.offsetTop;
            }
        }

        return ul.nextSelectedIndex;
    },
    uploads: function (uploadComponent, id) {
        if (!Radzen.uploadComponents) {
            Radzen.uploadComponents = {};
        }
        Radzen.uploadComponents[id] = uploadComponent;
    },
    upload: function (fileInput, url, multiple) {
        var data = new FormData();
        var files = [];
        for (var i = 0; i < fileInput.files.length; i++) {
            var file = fileInput.files[i];
            data.append(multiple ? 'files' : 'file', file, file.name);
            files.push({ Name: file.name, Size: file.size });
        }
        var xhr = new XMLHttpRequest();
        xhr.upload.onprogress = function (e) {
            if (e.lengthComputable) {
                var uploadComponent = Radzen.uploadComponents && Radzen.uploadComponents[fileInput.id];
                if (uploadComponent) {
                    var progress = parseInt((e.loaded / e.total) * 100);
                    uploadComponent.invokeMethodAsync("RadzenUpload.OnProgress", progress, e.loaded, e.total, files);
                }
            }
        };
        xhr.onreadystatechange = function (e) {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                var status = xhr.status;
                var uploadComponent = Radzen.uploadComponents && Radzen.uploadComponents[fileInput.id];
                if (uploadComponent) {
                    if (status === 0 || (status >= 200 && status < 400)) {
                        uploadComponent.invokeMethodAsync("RadzenUpload.OnComplete", xhr.responseText);
                    } else {
                        uploadComponent.invokeMethodAsync("RadzenUpload.OnError", xhr.responseText);
                    }
                }
            }
        };
        xhr.open('POST', url, true);
        xhr.send(data);
    },
    getCookie: function (name) {
        var value = "; " + decodeURIComponent(document.cookie);
        var parts = value.split("; " + name + "=");
        if (parts.length == 2) return parts.pop().split(";").shift();
    },
    getCulture: function () {
        var cultureCookie = Radzen.getCookie(".AspNetCore.Culture");
        var uiCulture = cultureCookie ? cultureCookie.split('|').pop().split('=').pop() : null;
        return uiCulture || 'en-US';
    },
    numericOnPaste: function (e) {
        if (e.clipboardData) {
            var value = e.clipboardData.getData('text');

            if (value && /^-?\d*\.?\d*$/.test(value)) {
                return;
            }

            e.preventDefault();
        }
    },
    numericKeyPress: function (e) {
        if (e.metaKey || e.ctrlKey || e.keyCode == 9 || e.keyCode == 8 || e.keyCode == 13) {
            return;
        }

        var ch = String.fromCharCode(e.charCode);

        if (/^[-\d,.]$/.test(ch)) {
            return;
        }

        e.preventDefault();
    },
    openPopup: function (parent, id, syncWidth) {
        var popup = document.getElementById(id);
        var parentRect = parent.getBoundingClientRect();

        if (/Edge/.test(navigator.userAgent)) {
            var scrollLeft = document.body.scrollLeft
            var scrollTop = document.body.scrollTop;

        } else {
            var scrollLeft = document.documentElement.scrollLeft
            var scrollTop = document.documentElement.scrollTop;
        }

        var top = parentRect.bottom + scrollTop;
        var left = parentRect.left + scrollLeft;
        var width = parentRect.width;

        if (syncWidth) {
            popup.style.width = width + 'px';
        }

        popup.style.display = 'block';

        var rect = popup.getBoundingClientRect();

        if (top + rect.height > window.innerHeight && parentRect.top > rect.height) {
            top = parentRect.top - rect.height + scrollTop;
        }

        if (left + rect.width - scrollLeft > window.innerWidth && window.innerWidth > rect.width) {
            left = window.innerWidth - rect.width + scrollLeft;
        }

        popup.style.top = top + 'px';
        popup.style.left = left + 'px';
        popup.style.zIndex = 1000;

        Radzen[id] = function (e) {
            if (!parent.contains(e.target) && !popup.contains(e.target)) {
                Radzen.closePopup(id);
            }
        }

        document.body.appendChild(popup);
        document.addEventListener('click', Radzen[id]);
    },
    closePopup: function (id) {
        var popup = document.getElementById(id);
        if (popup) {
            popup.style.display = 'none';
        }
        document.removeEventListener('click', Radzen[id]);
    },
    togglePopup: function (parent, id, syncWidth) {
        var popup = document.getElementById(id);

        if (popup.style.display == 'block') {
            Radzen.closePopup(id);
        } else {
            Radzen.openPopup(parent, id, syncWidth);
        }
    },
    destroyPopup: function (id) {
        var popup = document.getElementById(id);
        if (popup) {
            popup.parentNode.removeChild(popup);
        }
        document.removeEventListener('click', Radzen[id]);
    },
    scrollDataGrid: function (e) {
        var scrollLeft = (e.target.scrollLeft ? '-' + e.target.scrollLeft : 0) + 'px';

        e.target.previousElementSibling.style.marginLeft = scrollLeft;

        if (e.target.nextElementSibling) {
            e.target.nextElementSibling.style.marginLeft = scrollLeft;
        }
    },
    openDialog: function () {
        if (document.documentElement.scrollHeight > document.documentElement.clientHeight) {
            document.body.classList.add('no-scroll');
        }
    },
    closeDialog: function () {
        document.body.classList.remove('no-scroll');
    },
    getInputValue: function (arg) {
        var input = (arg instanceof Element || arg instanceof HTMLDocument) ? arg : document.getElementById(arg);
        return input ? input.value : '';
    },
    setInputValue: function (arg, value) {
        var input = (arg instanceof Element || arg instanceof HTMLDocument) ? arg : document.getElementById(arg);
        if (input) {
            input.value = value;
        }
    },
    readFileAsBase64: function (fileInput, maxFileSize) {
        var readAsDataURL = function (fileInput) {
            return new Promise(function (resolve, reject) {
                var reader = new FileReader();
                reader.onerror = function () {
                    reader.abort();
                    reject("Error reading file.");
                };
                reader.addEventListener("load", function () {
                    resolve(reader.result);
                }, false);
                var file = fileInput.files[0];
                if (file.size <= maxFileSize) {
                    reader.readAsDataURL(file);
                } else {
                    reject("File too large.");
                }
            });
        };

        return readAsDataURL(fileInput);
    },
    closeMenuItems: function (event) {
        var menu = event.target.closest('.menu');

        if (!menu) {
            var targets = document.querySelectorAll('.navigation-item-wrapper-active');

            if (targets) {
                for (var i = 0; i < targets.length; i++) {
                    Radzen.toggleMenuItem(targets[i], false);
                }
            }
            document.removeEventListener('click', Radzen.closeMenuItems);
        }
    },
    closeOtherMenuItems: function (current) {
        var targets = document.querySelectorAll('.navigation-item-wrapper-active');
        if (targets) {
            for (var i = 0; i < targets.length; i++) {
                var target = targets[i];
                var item = target.closest('.navigation-item');

                if (!current || !item.contains(current)) {
                    Radzen.toggleMenuItem(target, false);
                }
            }
        }
    },
    toggleMenuItem: function (target, selected) {
        Radzen.closeOtherMenuItems(target);

        var item = target.closest(".navigation-item");

        if (selected === undefined) {
            selected = !item.classList.contains('navigation-item-active');
        }

        item.classList.toggle('navigation-item-active', selected);

        target.classList.toggle('navigation-item-wrapper-active', selected)

        var children = item.querySelector('.navigation-menu');

        if (children) {
            children.style.display = selected ? '' : 'none';
        } else if (selected) {
            Radzen.closeOtherMenuItems(null);
        }

        var icon = item.querySelector('.navigation-item-icon-children');

        if (icon) {
            var deg = selected ? '180deg' : 0;
            icon.style.transform = 'rotate(' + deg + ')';
        }

        document.removeEventListener('click', Radzen.closeMenuItems);
        document.addEventListener('click', Radzen.closeMenuItems);
    },
    destroyChart: function (ref) {
        if (ref.resizeHandler) {
            window.removeEventListener("resize", ref.resizeHandler);
            delete ref.resizeHandler;
        }
        if (ref.mouseMoveHandler) {
            ref.removeEventListener(ref.mouseMoveHandler);
            delete ref.mouseMoveHandler;
        }
    },
    createChart: function (ref, instance) {
        var rect = ref.getBoundingClientRect();

        ref.resizeHandler = function () {
            var rect = ref.getBoundingClientRect();

            instance.invokeMethodAsync("Resize", rect.width, rect.height);
        }

        ref.mouseMoveHandler = function (e) {
            var rect = ref.getBoundingClientRect();
            var x = e.clientX - rect.left;
            var y = e.clientY - rect.top;
            instance.invokeMethodAsync("MouseMove", x, y);
        };

        ref.addEventListener('mousemove', ref.mouseMoveHandler);

        window.addEventListener("resize", ref.resizeHandler);

        return { width: rect.width, height: rect.height };
    }
};