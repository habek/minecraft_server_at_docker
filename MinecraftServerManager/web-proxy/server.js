'use strict';
var port = process.env.PORT || 1337;

//var http = require('http');

//http.createServer(function (req, res) {
//    res.writeHead(200, { 'Content-Type': 'text/plain' });
//    res.end('Hello World\n');
//}).listen(port);

const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');
const simpleRequestLogger = (proxyServer, options) => {
    proxyServer.on('proxyReq', (proxyReq, req, res) => {
        // console.log(`[HPM] [${req.method}] ${req.url}`); // outputs: [HPM] GET /users
    });
};
const app = express();

app.use('/api/',
    createProxyMiddleware({
        pathFilter: '/api',
        target: 'http://127.0.0.1:5054/',
        changeOrigin: false,
        ws: true,
        logLevel: 'debug',
        logger: console,
        // plugins:[simpleRequestLogger],
    }));
app.use('/',
    createProxyMiddleware({
        target: 'http://127.0.0.1:3000/',
        changeOrigin: false,
        logLevel: 'debug',
        logger: console,
        // plugins:[simpleRequestLogger],
    }));
console.info("Starting server at " + port);
app.listen(port);