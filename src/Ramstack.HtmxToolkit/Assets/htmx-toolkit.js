document.body._r_htmx ??= (htmx => {
    addEventListener("htmx:afterOnLoad", e => {
        if (e.detail.boosted) {
            const html = new DOMParser().parseFromString(e.detail.xhr.responseText, "text/html");
            const meta = html.querySelector("meta[name='htmx-config']");

            meta && (htmx.config.antiForgery = JSON.parse(meta.content).antiForgery);
        }
    });

    addEventListener("htmx:configRequest", e => {
        if (!/^get$/i.test(e.detail.verb)) {
            const {
                headerName,
                formFieldName,
                requestToken
            } = htmx.config.antiForgery ?? {};

            if (requestToken && !e.detail.parameters[formFieldName]) {
                headerName
                    ? e.detail.headers[headerName] = requestToken
                    : e.detail.parameters[formFieldName] = requestToken;
            }
        }
    });

    return true;
})(htmx);
