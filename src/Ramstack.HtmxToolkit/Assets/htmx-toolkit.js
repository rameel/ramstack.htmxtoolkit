document._r_htmx ||= ((document, htmx) => {
    const listen = (type, listener) => {
        document.addEventListener(type, listener);
    };

    const add_antiforgery = (method, headers, parameters) => {
        if (!/^get$/i.test(method)) {
            const {
                headerName,
                formFieldName,
                requestToken
            } = htmx.config.antiForgery ?? {};

            if (requestToken) {
                if (!parameters.has?.(formFieldName) && !parameters[formFieldName])
                {
                    if (headerName) {
                        headers[headerName] = requestToken;
                    }
                    else
                    {
                        parameters.set
                            ? parameters.set(formFieldName, requestToken)
                            : parameters[formFieldName] = requestToken;
                    }
                }
            }
        }
    };

    const update_antiforgery = content => {
        let html = new DOMParser().parseFromString(content || "", "text/html");
        let meta = html.querySelector("meta[name='htmx-config']");
        meta && (htmx.config.antiForgery = JSON.parse(meta.content).antiForgery);
    };

    listen("htmx:afterOnLoad", e => {
        let detail = e.detail;
        detail.boosted && update_antiforgery(detail.xhr.responseText);
    });

    listen("htmx:after:request", e => {
        let ctx = e.detail.ctx;
        ctx.boosted && update_antiforgery(ctx.text);
    });

    listen("htmx:configRequest", e => {
        let detail = e.detail;
        add_antiforgery(detail.verb, detail.headers, detail.parameters);
    });

    listen("htmx:config:request", e => {
        let request = e.detail.ctx.request;
        add_antiforgery(request.method, request.headers, request.body);
    });

    listen("rs:events", e => {
        for (let kvp of e.detail.value || e.detail) {
            htmx.trigger(e.target, kvp.key, kvp.value);
        }
    });
    return true;
})(document, htmx);
