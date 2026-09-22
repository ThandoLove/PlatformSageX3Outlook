window.taskpaneScaler = (function () {
    function init(rootSelector) {
        try {
            var root = document.querySelector(rootSelector);
            if (!root) return;
            var scaleWrap = root.querySelector('.taskpane-scale-wrap');
            if (!scaleWrap) return;
            // Only apply scaling when running inside the Outlook taskpane (or explicit /taskpane route).
            // This avoids changing UI when viewing the app in a browser.
            var isOfficeHost = false;
            try {
                isOfficeHost = !!(window.Office && Office.context && Office.context.mailbox);
            } catch (e) { isOfficeHost = false; }

            var isTaskpaneRoute = window.location && window.location.pathname &&
                window.location.pathname.toLowerCase().indexOf('/taskpane') >= 0;

            if (!isOfficeHost && !isTaskpaneRoute) {
                return; // do not apply scaling in normal browser pages
            }

            // Ensure we only apply a transform and do not modify child styles or colors.
            scaleWrap.style.transformOrigin = 'top left';
            scaleWrap.style.willChange = 'transform';

            function update() {
                try {
                    // Temporarily clear transform to measure natural size
                    var prevTransform = scaleWrap.style.transform;
                    scaleWrap.style.transform = '';
                    var rect = scaleWrap.getBoundingClientRect();
                    var naturalWidth = rect.width || scaleWrap.scrollWidth || 1200;
                    var naturalHeight = rect.height || scaleWrap.scrollHeight || 800;

                    var containerWidth = root.clientWidth || naturalWidth;
                    var scale = Math.min(1, containerWidth / naturalWidth);

                    // Apply calculated scale via CSS transform only
                    scaleWrap.style.transform = 'scale(' + scale + ')';

                    // Set container height to the scaled content height to avoid double scrollbars
                    root.style.height = Math.max(naturalHeight * scale, root.clientHeight) + 'px';
                    root.style.overflowY = 'hidden';

                    // keep the computed transform; no need to restore prevTransform
                } catch (e) { console.warn('taskpaneScaler.update failed', e); }
            }

            update();
            window.addEventListener('resize', update);
        } catch (e) { console.warn('taskpaneScaler.init failed', e); }
    }

    return { init: init };
})();
