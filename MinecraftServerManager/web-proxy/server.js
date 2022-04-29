'use strict';
var port = process.env.PORT || 1337;

//var http = require('http');

//http.createServer(function (req, res) {
//    res.writeHead(200, { 'Content-Type': 'text/plain' });
//    res.end('Hello World\n');
//}).listen(port);

const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();

app.use('/api', createProxyMiddleware({ target: 'http://localhost:5054/', changeOrigin: false, ws: true }));
app.use('/', createProxyMiddleware({ target: 'http://localhost:3000/', changeOrigin: false }));
console.info("Starting server at " + port);
app.listen(port);