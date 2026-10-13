document._r_htmx ||= ((document, htmx) => {
    const warn = message => console.warn(`ramstack.htmxtoolkit: ${message}`);
    const htmx_major = parseInt(htmx.version);

    // The internal API of HTMX 1.x and 2.x. HTMX passes it to init when the extension is defined,
    // whether or not any element enables the extension with hx-ext.
    let api;

    if (htmx.defineExtension) {
        htmx.defineExtension("ramstack-swap", {
            init(internal_api) {
                api = internal_api;
            },
            isInlineSwap(swap_style) {
                return swap_style === "outerMorph" || swap_style === "outerSync";
            },
            handleSwap(swap_style, target, fragment) {
                if (swap_style === "textContent") {
                    target.textContent = fragment.textContent;
                    return [target];
                }

                let morph_style =
                    swap_style === "innerMorph" ? "innerHTML" :
                    swap_style === "outerMorph" ? "outerHTML" : null;

                if (morph_style) {
                    let idiomorph = globalThis.Idiomorph;
                    if (idiomorph?.morph) {
                        return idiomorph.morph(target, fragment.children, { morphStyle: morph_style });
                    }

                    warn(`Idiomorph is unavailable; falling back from ${swap_style} to ${morph_style}`);

                    let nodes = [...fragment.childNodes];
                    morph_style === "innerHTML"
                        ? target.replaceChildren(...nodes)
                        : target.replaceWith(...nodes);

                    return nodes;
                }

                if (swap_style === "outerSync") {
                    warn("outerSync requires HTMX 4.x; falling back to attribute sync and innerHTML");

                    let source = fragment.firstElementChild;
                    if (source) {
                        for (let attr of [...target.attributes]) {
                            source.hasAttribute(attr.name) || target.removeAttribute(attr.name);
                        }

                        for (let attr of source.attributes) {
                            target.setAttribute(attr.name, attr.value);
                        }

                        let nodes = source.childNodes;
                        target.replaceChildren(...nodes);

                        return nodes;
                    }

                }
            }
        });
    }

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
        val.requestToken && (antiforgery = val);
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

    // HTMX reads hx-request from the element and its ancestors; the nearest value wins.
    // Values computed with "js:" cannot be read without evaluating them, so they are skipped.
    const no_headers = elt => {
        let result = false;
        api.getClosestMatch(elt, e => {
            let value = api.getAttributeValue(e, "hx-request")?.trim();
            if (value && !/^(js|javascript):/.test(value)) {
                try {
                    let { noHeaders } = JSON.parse(value[0] === "{" ? value : `{${value}}`);
                    if (noHeaders != null) {
                        result = !!noHeaders;
                        return true;
                    }
                }
                catch {
                    // HTMX reports the invalid value itself.
                }
            }
        });

        return result;
    };

    // HTMX 4.x sends HX-Request-Type; HTMX 1.x and 2.x do not, so it is computed here
    // the same way: "full" when the response replaces the body or is filtered with a selector.
    // htmx:configRequest does not see the select of htmx.ajax and HX-Location requests;
    // htmx:beforeRequest does, and its request is open but not yet sent.
    listen("htmx:beforeRequest", e => {
        if (htmx_major < 4) {
            let elt = e.target;
            let { xhr, target, select, requestConfig } = e.detail;

            if (requestConfig.headers["HX-Request-Type"] == null && !no_headers(elt)) {
                select ||= api.getClosestAttributeValue(elt, "hx-select");
                let request_type = target === document.body || select && select !== "unset" ? "full" : "partial";
                xhr.setRequestHeader("HX-Request-Type", request_type);
            }
        }
    });

    // JSON.parse keeps only the last of repeated names, so when an event name repeats, the server
    // sends all events of the header under "rs:event". Each event is prepared the way HTMX
    // prepares an HX-Trigger entry before passing it to htmx.trigger.
    listen("rs:event", e => {
        let elt = e.target;

        // HTMX 1.x and 2.x wrap the array in { value }.
        // HTMX 4.x passes it as is.
        for (let { key, value: detail } of e.detail.value || e.detail) {
            let target = elt;

            if (htmx_major >= 4) {
                // HTMX 4.x passes any object, including an array, as is.
                // "target" applies to this event only.
                if (detail?.target) {
                    target = htmx.find(detail.target);
                }

                if (typeof detail !== "object") {
                    detail = { value: detail };
                }
            }
            else if (Object.prototype.toString.call(detail) === "[object Object]") {
                // HTMX 2.x applies "target" to this and all following events.
                // HTMX 1.x ignores it.
                if (htmx_major === 2 && detail.target !== undefined) {
                    elt = target = detail.target;
                }
            }
            else {
                detail = { value: detail };
            }

            htmx.trigger(target, key, detail);
        }
    });
    return true;
})(document, htmx);
