const TerserPlugin = require("terser-webpack-plugin");
const MergeIntoSingleFilePlugin = require('webpack-merge-and-include-globally');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const WebpackShellPluginNext = require('webpack-shell-plugin-next');
const path = require('path');
const fs = require('fs');

module.exports = [
{
    entry: {
        'libraries': [
        // css
            "./node_modules/@fortawesome/fontawesome-free/css/all.min.css",
            "./node_modules/bootstrap/dist/css/bootstrap.min.css",
            "./node_modules/bootstrap-table/dist/bootstrap-table.min.css",
            "./node_modules/bootstrap-select/dist/css/bootstrap-select.min.css",
            "./node_modules/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css",
            "./node_modules/izitoast/dist/css/iziToast.min.css",
            "./node_modules/vis/dist/vis.min.css",
            "./libs/bootstrap-reorder-rows/bootstrap-table-reorder-rows.css",
            "./libs/bootstrap-filter-control/bootstrap-table-filter-control.css",
            "./node_modules/@fullcalendar/core/main.css",
            "./node_modules/@fullcalendar/daygrid/main.css",
            "./node_modules/antd/dist/antd.min.css",
            "./libs/bootstrap-datetimepicker/bootstrap-datetimepicker.min.css"
        ],
        'app': [
            "./app/index.js",
            "./content/app.css"
        ]
    },
    module: {
        rules: [
            // {
            //     test: /\.js$/,
            //     exclude: /node_modules/,
            //     use: {
            //         loader: "uglify-es-loader"
            //     }
            // },
            {
                test: /\.css$/,
                use: [MiniCssExtractPlugin.loader, "css-loader"]
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                type: 'asset/resource',
                generator : {
                    filename : 'fonts/[name][ext][query]',
                }
            },
            {
                test: /\.(png|jpg)(\?v=\d+\.\d+\.\d+)?$/,
                type: 'asset/resource',
                generator : {
                    filename : 'images/[name][ext][query]',
                }
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].min.css"
        }),
        new MergeIntoSingleFilePlugin({
            files: {
                // legacy libraries, not modules
                "libraries.min.js": [
                    "node_modules/jquery/dist/jquery.min.js",
                    "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js",
                    "node_modules/tablednd/dist/jquery.tablednd.min.js",
                    "node_modules/bootstrap-table/dist/bootstrap-table.min.js",
                    "node_modules/bootstrap-table/dist/bootstrap-table-locale-all.min.js",

                    "node_modules/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.js",
                    "node_modules/izitoast/dist/js/iziToast.min.js",
                    "node_modules/vis/dist/vis.min.js",
                    //"node_modules/bootstrap-select/dist/js/bootstrap-select.min.js",
                    "libs/bootstrap-select/dist/js/bootstrap-select.min.js",
                    "libs/bootstrap-reorder-rows/bootstrap-table-reorder-rows.min.js",
                    "libs/bootstrap-filter-control/bootstrap-table-filter-control.js",
                    "node_modules/moment/min/moment-with-locales.min.js",
                    "libs/bootstrap-datetimepicker/bootstrap-datetimepicker.min.js",
                    "node_modules/react/umd/react.production.min.js",
                    "node_modules/react-dom/umd/react-dom.production.min.js",
                    "node_modules/antd/dist/antd-with-locales.min.js",
                    "node_modules/@fullcalendar/core/main.min.js",
                    "node_modules/@fullcalendar/daygrid/main.min.js",
                    "node_modules/@fullcalendar/interaction/main.min.js"
                ],
                '../libs/ckeditor/ckeditor-plugins.min.js': [
                    "libs/ckeditor/ckeditor.js",
                    "libs/ckeditor/plugins/axscontokens/plugin.js",
                    "libs/ckeditor/plugins/base64image/plugin.js",
                    "libs/ckeditor/plugins/justify/plugin.js",
                    "libs/ckeditor/plugins/imagebase/plugin.js",
                    "libs/ckeditor/plugins/font/plugin.js",
                    "libs/ckeditor/plugins/colorbutton/plugin.js",
                    "libs/ckeditor/plugins/colordialog/plugin.js"
                ]
            },
            transform: {
                'libraries.min.js': code => {
                    return require("uglify-js").minify(code).code;
                }
            }
        }),
        // new WebpackShellPluginNext({
        //     onBuildEnd: {
        //         scripts: [
        //             () => {
        //                 // fs.unlinkSync(path.join(path.resolve(__dirname, 'dist'), "libraries.min.js"));
        //                 // fs.unlinkSync(path.join(path.resolve(__dirname, 'dist'), "app.min.js"));
        //             }
        //         ]
        //     }
        // }),
    ],
    optimization: {
        minimize: true,
        minimizer: [
            new CssMinimizerPlugin(),
            new TerserPlugin({
                test: /\.js(\?.*)?$/i,
                parallel: true
            })
        ],
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].min.js'
    }
}
];