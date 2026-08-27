document._r_htmx ||= ((document, htmx) => {
    const listen = (type, listener) => {
        document.addEventListener(type, listener);
    };

    const read_antiforgery = doc => {
        let data = doc.querySelector("meta[name='htmx-config']")?.dataset || {};
        return {
            headerName: data.antiforgeryHeaderName,
            formFieldName: data.antiforgeryFormFieldName,
            requestToken: data.antiforgeryRequestToken
        };
    };

    let antiforgery = read_antiforgery(document);

    const add_antiforgery = (method, headers, parameters) => {
        if (!/^get$/i.test(method)) {
            const {
                headerName,
                formFieldName,
                requestToken
            } = antiforgery;

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
        let doc = new DOMParser().parseFromString(content || "", "text/html");
        let val = read_antiforgery(doc);
        val && (antiforgery = val);
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
