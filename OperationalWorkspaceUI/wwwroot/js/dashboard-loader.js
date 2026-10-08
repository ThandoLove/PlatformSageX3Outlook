// @ts-nocheck
// dashboard-loader.js
// Consolidates the inline scripts previously embedded in dashboard.html

(function () {
    try {
        // Ensure the router sees the desired route before Blazor starts
        try {
            window.history.replaceState({}, '', '/welcome');
        } catch (e) {
            // ignore
        }

        // Capture initial mailbox item and expose to Blazor via window['initialOfficeItem']
        function setInitialItem(item) {
            try {
                window['initialOfficeItem'] = {
                    subject: item.subject || '',
                    from: (item.from && item.from.emailAddress && item.from.emailAddress.address) || '',
                    body: ''
                };

                if (item.getBodyAsync) {
                    item.getBodyAsync("text", function (result) {
                        try {
                            if (result && result.status === window['Office'].AsyncResultStatus.Succeeded) {
                                window['initialOfficeItem'].body = result.value || '';
                            }
                        } catch (e) {
                            // ignore
                        }
                    });
                }
            } catch (e) {
                window['initialOfficeItem'] = window['initialOfficeItem'] || {};
            }
        }

        if (window['Office'] && window['Office'].onReady) {
            window['Office'].onReady().then(function () {
                try {
                    var item = window['Office'].context.mailbox.item;
                    setInitialItem(item);
                } catch (e) {
                    // ignore
                }
            });
        } else {
            window['initialOfficeItem'] = {};
        }

        // Blazor interactive script loader (try WebAssembly first, fallback to server script)
        function loadScript(src, onload, onerror) {
            var s = document.createElement('script');
            s.src = src;
            s.defer = true;
            if (onload) s.onload = onload;
            if (onerror) s.onerror = onerror;
            document.head.appendChild(s);
            return s;
        }

        loadScript('_framework/blazor.web.js', function () {
            console.log('Loaded blazor.web.js');
            loadScript('js/app.js', function () { console.log('Loaded js/app.js'); }, function () { console.warn('Failed to load js/app.js'); });
        }, function () {
            console.warn('blazor.web.js failed to load; attempting blazor.server.js');
            loadScript('_framework/blazor.server.js', function () {
                console.log('Loaded blazor.server.js');
                loadScript('js/app.js', function () { console.log('Loaded js/app.js'); }, function () { console.warn('Failed to load js/app.js'); });
            }, function () {
                console.error('Failed to load both blazor.web.js and blazor.server.js');
            });
        });

        // Diagnostic helper
        console.log('dashboard.html: checking Blazor and Office context');

        function showFallback(msg) {
            try {
                var app = document.getElementById('app');
                if (app) {
                    app.innerHTML = '<div style="padding:16px;color:#111;font-family:Segoe UI, Arial, sans-serif;line-height:1.4;">'
                        + '<h3>Operational Workspace (fallback)</h3>'
                        + '<p>' + msg + '</p>'
                        + '<p><small>Tip: open the app URL in your browser to complete the sign-in and debugging steps.</small></p>'
                        + '<p><a href="' + window.location.href + '" target="_blank">Open in browser</a></p>'
                        + '</div>';
                }
            } catch (e) { console.warn(e); }
        }

        try { console.log('Office present:', !!(window.Office && Office.context)); } catch (e) { console.warn(e); }

        var waited = 0;
        var interval = setInterval(function () {
            waited += 250;
            var blazorPresent = !!(window.Blazor || (window.DotNet && typeof window.DotNet.invokeMethodAsync === 'function'));
            if (blazorPresent) {
                console.log('Blazor runtime detected in dashboard.html');
                clearInterval(interval);
                return;
            }
            if (waited >= 2500) {
                clearInterval(interval);
                console.warn('Blazor runtime not detected - showing fallback.');
                showFallback('The Blazor runtime did not initialize in this host environment.');
            }
        }, 250);

    } catch (e) { console.warn('dashboard-loader failed', e); }
})();
