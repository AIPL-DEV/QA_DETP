const { watch } = require('fs')
const path = require('path');
const { webpack, ProvidePlugin } = require('webpack')

module.exports = {
    entry: './src/index.js',
    mode: 'development',
    output: {
        filename: 'main.js',
        path: path.resolve(__dirname, 'wwwroot/dist'),
    },
    watch: true,
    watchOptions: {
        ignored: /node_modules/,
        poll: 1000
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader'],
            },
            {
                test: require.resolve("jquery"),
                loader: "expose-loader",
                options: {
                    // For `underscore` library, it can be `_.map map` or `_.map|map`
                    exposes: ["$", "jQuery"],
                    // To access please use `window.$` or `globalThis.$`
                },
            },
        ]
    },
    plugins: [
        new ProvidePlugin({
            $: "jquery",
            jQuery: "jquery"
        })
    ],
};