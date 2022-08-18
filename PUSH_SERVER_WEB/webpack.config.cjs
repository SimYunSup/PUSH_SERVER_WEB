const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const path = require("path");

const isDevelopment = process.env.NODE_ENV !== "production";

module.exports = {
    mode: isDevelopment ? "development" : "production",
    entry: [
        "./index.ts"
    ],
    plugins: [
        new MiniCssExtractPlugin()
    ],
    output: {
        path: path.resolve(__dirname, "wwwroot/build"),
        filename: "index.js",
        libraryTarget: 'umd',
        library: 'BlazorInsertedJS',
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                exclude: /node_modules/,
                use: "ts-loader",
            },
            {
                test: /.s[ac]ss$/i,
                exclude: /node_modules/,
                use: [
                    isDevelopment ? "style-loader" : MiniCssExtractPlugin.loader,
                    {
                        loader: "css-loader",
                        options: {
                            importLoaders: 2,
                        }
                    },
                    "sass-loader",
                    "postcss-loader"
                ]
            }
        ]
    },
    devtool: "inline-source-map",
    devServer: {
        static: {
            directory: path.resolve(__dirname, "wwwroot"),
        },
        port: 9000,
        historyApiFallback: true,
        hot: true,
        devMiddleware: {
            writeToDisk: true,
        },
    }
}