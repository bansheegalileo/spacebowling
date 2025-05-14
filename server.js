const https = require('https');
const fs = require('fs');
const express = require('express');
const compression = require('compression');
const path = require('path');

const app = express();
const port = 8080;

app.use(compression());

// Serve .br files with correct headers
app.use((req, res, next) => {
  if (req.url.endsWith('.br')) {
    res.setHeader('Content-Encoding', 'br');
    if (req.url.endsWith('.js.br')) {
      res.setHeader('Content-Type', 'application/javascript');
    } else if (req.url.endsWith('.wasm.br')) {
      res.setHeader('Content-Type', 'application/wasm');
    } else if (req.url.endsWith('.data.br')) {
      res.setHeader('Content-Type', 'application/octet-stream');
    }
  }
  next();
});

// Serve Unity WebGL files
app.use(express.static(path.join(__dirname, 'Builds')));

app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'Builds/index.html'));
});

// HTTPS options
const options = {
  key: fs.readFileSync('key.pem'),
  cert: fs.readFileSync('cert.pem'),
};

https.createServer(options, app).listen(port, () => {
  console.log(`HTTPS server running at https://localhost:${port}`);
});
