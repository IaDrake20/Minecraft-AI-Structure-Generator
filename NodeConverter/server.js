const express = require('express');
const path = require('path');

const app = express();

// Serve static files from the 'dist' directory
app.use(express.static(path.join(__dirname , 'web')));

// Catch-all route to serve 'index.html'
app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname, 'web' , 'index.html'));
});

// Start the server
const PORT = process.env.PORT || 4000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});