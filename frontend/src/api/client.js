const PRODUCTION_API = 'https://magentrixdesk.onrender.com/api'
const LEGACY_DEAD_API = 'magentrix-desk-api.onrender.com'

function resolveApiBase() {
  const configured = (import.meta.env.VITE_API_BASE || '').replace(/\/$/, '')
  if (!configured) {
    return import.meta.env.PROD ? PRODUCTION_API : 'http://localhost:5080/api'
  }
  // Stale Vercel env still pointed at the retired Render hostname.
  if (configured.includes(LEGACY_DEAD_API)) {
    return PRODUCTION_API
  }
  return configured
}

const API_BASE = resolveApiBase()

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {})
    },
    ...options
  })

  if (response.status === 204) {
    return null
  }

  const contentType = response.headers.get('content-type') || ''
  const payload = contentType.includes('application/json')
    ? await response.json()
    : await response.text()

  if (!response.ok) {
    const message =
      (payload && payload.message) ||
      (payload && payload.title) ||
      `Request failed (${response.status})`
    const error = new Error(message)
    error.status = response.status
    error.payload = payload
    throw error
  }

  return payload
}

export const api = {
  getTickets(params = {}) {
    const query = new URLSearchParams()
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        query.set(key, value)
      }
    })
    const qs = query.toString()
    return request(`/tickets${qs ? `?${qs}` : ''}`)
  },
  getStats() {
    return request('/tickets/stats')
  },
  getTicket(id) {
    return request(`/tickets/${id}`)
  },
  createTicket(body) {
    return request('/tickets', { method: 'POST', body: JSON.stringify(body) })
  },
  updateTicket(id, body) {
    return request(`/tickets/${id}`, { method: 'PUT', body: JSON.stringify(body) })
  },
  deleteTicket(id) {
    return request(`/tickets/${id}`, { method: 'DELETE' })
  },
  addComment(id, body) {
    return request(`/tickets/${id}/comments`, { method: 'POST', body: JSON.stringify(body) })
  }
}

export const STATUS_LABELS = {
  0: 'Open',
  1: 'In Progress',
  2: 'Resolved',
  3: 'Closed'
}

export const PRIORITY_LABELS = {
  0: 'Low',
  1: 'Medium',
  2: 'High'
}

export const ASSIGNEES = ['Jordan Hale', 'Priya Nair', 'Sam Ortiz']
