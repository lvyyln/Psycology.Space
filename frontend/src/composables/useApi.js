import { useAuth } from './useAuth.js'

export function useApi() {
  const { token } = useAuth()

  async function request(url, options = {}) {
    const headers = { 'Content-Type': 'application/json', ...options.headers }
    if (token.value) {
      headers['Authorization'] = `Bearer ${token.value}`
    }
    const res = await fetch(url, { ...options, headers })
    if (res.status === 204) return null
    const data = await res.json()
    if (!res.ok) throw { status: res.status, data }
    return data
  }

  const get = (url) => request(url)
  const post = (url, body) => request(url, { method: 'POST', body: JSON.stringify(body) })
  const put = (url, body) => request(url, { method: 'PUT', body: JSON.stringify(body) })
  const del = (url) => request(url, { method: 'DELETE' })

  return { get, post, put, del }
}
