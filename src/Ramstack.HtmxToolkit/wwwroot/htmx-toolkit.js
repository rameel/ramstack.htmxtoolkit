document._r_htmx ||= ((document, htmx) => {
    const warn = message => console.warn(`ramstack.htmxtoolkit: ${message}`);

    if (htmx.defineExtension) {
        htmx.defineExtension("ramstack-morph", {
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

    // A JSON object cannot hold repeated names, so when an event name repeats, the server
    // sends all events of the header under "rs:event". Each event is prepared the way HTMX
    // prepares an HX-Trigger entry before passing it to htmx.trigger.
    const htmx_major = parseInt(htmx.version);

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
