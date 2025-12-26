const API_BASE = 'http://localhost:8080/api'

// Auth API
export const auth = {
  register: (email, username, password, userRole) =>
    fetch(`${API_BASE}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, username, password, userRole }),
      credentials: 'include'
    }).then(r => r.json()),

  login: (email, username, password, userRole) =>
    fetch(`${API_BASE}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, username, password, userRole }),
      credentials: 'include'
    }).then(r => r.json()),

  logout: () =>
    fetch(`${API_BASE}/auth/logout`, {
      method: 'POST',
      credentials: 'include'
    }).then(r => r.json())
}

// Games API
export const games = {
  getAll: (minPrice, maxPrice, gameTitle, genres, page = 1, pageSize = 20) => {
    const params = new URLSearchParams()
    if (minPrice) params.append('minPrice', minPrice)
    if (maxPrice) params.append('maxPrice', maxPrice)
    if (gameTitle) params.append('gameTitle', gameTitle)
    if (genres && genres.length) genres.forEach(g => params.append('genres', g))
    params.append('page', page)
    params.append('pageSize', pageSize)
    
    return fetch(`${API_BASE}/games?${params}`, { credentials: 'include' })
      .then(r => r.json())
  },

  getById: (id) =>
    fetch(`${API_BASE}/games/${id}`, { credentials: 'include' })
      .then(r => r.json()),

  getMySeller: (page = 1, pageSize = 20) =>
    fetch(`${API_BASE}/games/my?page=${page}&pageSize=${pageSize}`, {
      credentials: 'include'
    }).then(r => r.json()),

  create: (formData) =>
    fetch(`${API_BASE}/games`, {
      method: 'POST',
      body: formData,
      credentials: 'include'
    }).then(r => r.json()),

  update: (id, formData) =>
    fetch(`${API_BASE}/games/${id}`, {
      method: 'PUT',
      body: formData,
      credentials: 'include'
    }).then(r => r.json()),

  delete: (id) =>
    fetch(`${API_BASE}/games/${id}`, {
      method: 'DELETE',
      credentials: 'include'
    }).then(r => r.json()),

  addKeys: (id, formData) =>
    fetch(`${API_BASE}/games/${id}/keys`, {
      method: 'POST',
      body: formData,
      credentials: 'include'
    }).then(r => r.json())
}

// Cart API
export const cart = {
  getCart: () =>
    fetch(`${API_BASE}/carts`, { credentials: 'include' })
      .then(r => r.json()),

  addItem: (gameId, quantity) =>
    fetch(`${API_BASE}/carts/items`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ gameId, quantity }),
      credentials: 'include'
    }).then(r => r.json()),

  removeItem: (gameId, quantity) =>
    fetch(`${API_BASE}/carts/items`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ gameId, quantity }),
      credentials: 'include'
    }).then(r => r.json())
}

// Orders API
export const orders = {
  getOrders: (page = 1, pageSize = 20) =>
    fetch(`${API_BASE}/orders?page=${page}&pageSize=${pageSize}`, {
      credentials: 'include'
    }).then(r => r.json()),

  createOrder: (orderItems) =>
    fetch(`${API_BASE}/orders`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ orderItems }),
      credentials: 'include'
    }).then(r => r.json())
}

// Genres API
export const genres = {
  getAll: () =>
    fetch(`${API_BASE}/genres`, { credentials: 'include' })
      .then(r => r.json())
}
