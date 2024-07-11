import path from "path";
import size from "rollup-plugin-bundle-size";
import terser from "@rollup/plugin-terser";

function trim() {
    return {
        name: "trim",
        generateBundle(options, bundle) {
            if (options.file.match(/\.min\.js$/)) {
                const key = path.basename(options.file);
                bundle[key].code = bundle[key].code.trim();
            }
        }
    };
}

export default {
    input: "src/Ramstack.HtmxToolkit/Assets/htmx-toolkit.js",
    treeshake: "smallest",
    output: [{
        file: "src/Ramstack.HtmxToolkit/Assets/htmx-toolkit.js",
    }, {
        file: "src/Ramstack.HtmxToolkit/Assets/htmx-toolkit.min.js",
        plugins: [terser({
            output: {
                comments: false
            },
            compress: {
                passes: 5,
                ecma: 2020,
                drop_console: false,
                drop_debugger: true,
                pure_getters: true,
                arguments: true,
                unsafe_comps: true,
                unsafe_math: true,
                unsafe_methods: true
            }
        })]
    }],
    plugins: [trim(), size()]
};
