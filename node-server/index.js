require('dotenv').config();
const express = require('express');
const app = express();

app.get('/services', async (req, res) => {
  const response = await fetch('https://api.render.com/v1/services', {
    headers: {
      'Authorization': `Bearer ${process.env.RENDER_API_KEY}`,
      'Accept': 'application/json'
    }
  });
  const data = await response.json();
  res.json(data);
});

app.listen(3000, () => console.log('Server running on port 3000'));